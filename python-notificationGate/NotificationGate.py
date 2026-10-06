import json
import os
import pika
import asyncio
from json import JSONDecodeError
import redis
import logging
from pathlib import Path
import geopandas as gpd
from shapely.geometry import Point

from confluent_kafka import Consumer

bootstrap_servers = os.getenv("KAFKA_BOOTSTRAP_SERVERS", "localhost:9092")
topic = os.getenv("KAFKA_CONSUMER_TOPIC", "rawData")

BASE_DIR = Path(__file__).resolve().parent
file_path = BASE_DIR / "regions.geojson"

logging.basicConfig(
    filename='logger.log',
    level=logging.INFO,
    format='%(asctime)s | %(levelname)s | %(message)s'
)

consumerConf = {
    'bootstrap.servers': bootstrap_servers,
    'group.id': 'RawData',
    'auto.offset.reset': 'earliest'
}

consumer = Consumer(consumerConf)
I = redis.Redis(host='localhost', port=6379, decode_responses=True)

connection = pika.BlockingConnection(pika.ConnectionParameters('localhost'))
channel = connection.channel()

running = True


async def raw_consumer(consumer, topics):
    logging.info("enter to raw consumer function")
    try:
        consumer.subscribe(topics)
        counter = 0
        while running:
            msg = consumer.poll(timeout=100)
            if msg is None:
                if counter == 0:
                    continue
                else:
                    print(f"no more message recived.total messages: {counter}")
                    logging.info(f"no more message recived.total messages: {counter}")
                    break

            else:
                counter += 1
                try:
                    raw_text = msg.value().decode("utf-8")
                    data_dict = json.loads(raw_text)
                    print(data_dict)
                    print("type of data_dict:", type(data_dict))
                    if not isinstance(data_dict, dict):
                        data_dict = json.loads(data_dict)
                        print("type of data_dict:", type(data_dict))

                    if not validate_warning(data_dict):
                        print("data recived was incorrect: skip to next message")
                        logging.warning("data recived was incorrect: skip to next message")
                        continue

                    if "alert_id" in data_dict:
                        if not I.set(f"alert:{data_dict["alert_id"]}", raw_text, nx=True):
                            logging.info(f"the data is already exists: {data_dict} skip to next message")
                            continue

                        print(f"first time of coming data: save it in redis cash:\n {data_dict}")
                        logging.info(f"first time of coming data: save it in redis cash:\n {data_dict}")

                    if len(data_dict) == 0:
                        print("warning data is empty: skip to next message")
                        continue

                    print(data_dict)
                    geographical_region = geographical_classification(data_dict)
                    print(f"geographical_classification:{geographical_region}\n")
                    data = json.dumps(data_dict)
                    print("before")
                    channel.queue_declare(queue=geographical_region, durable=True, arguments={'x-queue-type': 'quorum'})
                    print("middle")
                    channel.basic_publish(exchange='',
                                          routing_key=geographical_region,
                                          body=data)
                    print("after")
                    # await send_to_rabbit_queue_async(geographical_region, data)
                    print(f"send message to queue:{geographical_region}")
                    # logging.INFO

                except JSONDecodeError:
                    print("error occurred:", JSONDecodeError)
                except Exception as e:
                    print("error occurred:", e)
    finally:
        print("closing consumer")
        consumer.close()
        I.close()
        connection.close()


# async def send_to_rabbit_queue_async(stream_name,message,stream_retention=5000000000):
#      print("enter to rabbit")
#      async with Producer(
#             host="localhost",
#             username="guest",
#             password="guest",
#     ) as producer:
#         await producer.create_stream(stream_name, exists_ok=True, arguments={"MaxLengthBytes": stream_retention})
#         await producer.send(stream=stream_name, message=message)


def geographical_classification(data_dict):
    lon = float(data_dict["lon"])
    lat = float(data_dict["lat"])
    region = get_region_with_geopandas(file_path, lon, lat)
    # print("the region is:",region)
    return region


def validate_warning(data):
    if not isinstance(data, dict):
        print("invalid data type: data should be dict")

    category = ["alert_id", "source", "title", "content", "priority", "classification",
                "lat", "lon", "timestamp", "status"]

    for title in category:
        if title not in data:
            print(f"invalid title in data: missing: {title}")
            return False

    official_source = ["aman", "mossad", "pikud-haoref", "shabak"]
    classifications = ["UNCLASSIFIED", "RESTRICTED", "SECRET", "TOP_SECRET"]
    priorities = ["CRITICAL", "HIGH", "MEDIUM", "LOW"]
    source_type = data["source"]
    priority_type = data["priority"]
    classification_type = data["classification"]

    if priority_type not in priorities:
        print("invalid priority in data")
        return False

    if classification_type not in classifications:
        print("invalid classification in data")
        return False

    if source_type not in official_source:
        print("invalid source type in data")
        return False

    for value in category:
        val = data[value]
        if val is None or (isinstance(val, str) and val.strip() == ""):
            print("invalid category in data")
            return False

    if not (-90 <= float(data["lat"]) <= 90):
        print("invalid lat in data")
        return False

    if not (-180 <= float(data["lon"]) <= 180):
        print("invalid lon in data")
        return False
    return True


def get_region_with_geopandas(file_path, lon, lat):
    try:
        gdf = gpd.read_file(file_path)
        pt = Point(lon, lat)

        matched = gdf[gdf.geometry.contains(pt)]

        if not matched.empty:
            return matched.iloc[0]["region"]
        return "OVERSEAS"
    except FileNotFoundError:
        print(f"error: file in path {file_path} was not found")
    except FileExistsError:
        print(f"error: file in path {file_path} was not exists")


async def main():
    await raw_consumer(consumer, [topic])


if __name__ == "__main__":
    asyncio.run(main())