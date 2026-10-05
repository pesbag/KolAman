import json
import sys
import os

from confluent_kafka import Consumer, KafkaException, KafkaError
bootstrap_servers = os.getenv("KAFKA_BOOTSTRAP_SERVERS", "localhost:9092")
# topic=os.getenv("KAFKA_CONSUMER_TOPIC")

consumerConf = {
    'bootstrap.servers': bootstrap_servers,
    'group.id': 'RawData',
    'auto.offset.reset': 'earliest'
}

consumer = Consumer(consumerConf)

running=True
def raw_consumer(consumer,topics):
    try:
        consumer.subscribe(topics)
        counter=0
        while running:
            msg=consumer.poll(timeout=1000)
            if msg is None:
                if counter == 0:
                    continue
                else:
                    print(f"no more message recived.total messages: {counter}")
                    break

            if msg.error():
                if msg.error().code() == KafkaError._PARTITION_EOF:
                    sys.stderr.write('%% %s %d reached end at offset %d\n' %
                                     (msg.topic(), msg.partition(), msg.offset()))
                elif msg.error():
                    raise KafkaException(msg.error())
            else:
                counter+=1
                raw_text = msg.value().decode("utf-8")
                data_dict=json.loads(raw_text)
                if len(data_dict) == 0:
                    continue
                # dict_record=data_dict.to_dict(orient='records')[0]
                print(data_dict)
    finally:
        print("closing consumer")
        consumer.close()

if __name__=="__main__":
    raw_consumer(consumer, ["rawData"])