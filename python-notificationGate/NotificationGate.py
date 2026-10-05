import json
import sys
import os
import redis.client
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

running = True
def raw_consumer(consumer, topics):
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
                raw_text = msg.value().decode("utf-8")
                data_dict = json.loads(raw_text)
                if not validate_warning(data_dict):
                    print("data recived was incorrect. skip to next message")
                    logging.warning("data recived was incorrect. skip to next message")
                    continue
                if "alert_id" in data_dict:
                    alert_json = json.dumps(data_dict)
                    if not I.set(f"alert:{counter}", alert_json):
                        logging.info(f"the data is already exists: {data_dict}")
                        continue
                    print(f"first time of coming data, save it in redis cash: {data_dict}")
                    logging.info(f"first time of coming data, save it in redis cash: {data_dict}")

                if len(data_dict) == 0:
                    continue

                print(data_dict)
    finally:
        print("closing consumer")
        consumer.close()
        I.close()

def validate_warning(data):
    # data = json.loads(raw_text)[0]
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

    if not (-90 <= data["lat"] <= 90):
        print("invalid lat in data")
        return False

    if not (-180 <= data["lon"] <= 180):
        print("invalid lon in data")
        return False
    return True

def get_region_with_geopandas(file_path: str, lon: float, lat: float) -> str:
    # 1. טעינת קובץ ה-GeoJSON ל-GeoDataFrame
    gdf = gpd.read_file(file_path)

    # 2. יצירת נקודה מתאימה
    pt = Point(lon, lat)

    # 3. סינון השורות שהפוליגון שלהן מכיל את הנקודה
    matched = gdf[gdf.geometry.contains(pt)]

    # 4. החזרת שם האזור אם נמצאה התאמה, אחרת OVERSEAS
    if not matched.empty:
        return matched.iloc[0]["region"]
    return "OVERSEAS"


# --- דוגמת שימוש ---
region = get_region_with_geopandas("regions.geojson", 34.800, 32.100)
print(region)


if __name__ == "__main__":
    raw_consumer(consumer, [topic])