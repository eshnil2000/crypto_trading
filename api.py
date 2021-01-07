#curl -d '{"side":"buy", "price": "100", "quantity":"1000"}' -H "Content-Type: application/json" -X POST http://localhost:5000/order/new
#curl -d '{"side":"sell", "price": "102", "quantity":"100"}' -H "Content-Type: application/json" -X POST http://localhost:5000/order/new
#curl -X GET http://localhost:5000/orderbook

import hashlib
import json
from time import time
from urllib.parse import urlparse
from uuid import uuid4

import requests
from flask import Flask, jsonify, request,render_template, request
from flask_cors import CORS

import flask_apscheduler 
from flask_apscheduler import APScheduler

import engine
import numpy as np
import random
import kafka
   
me=engine.MatchingEngine()
# Instantiate the Orderbook with random orders
# nr_of_orders=100
# midpoint=100

# orders = []
# for i in range(nr_of_orders):
#     #print(i)
#     side = random.choice(["buy", "sell"])
#     print(side + str(i))
#     price = random.gauss(midpoint, 5)
#     quantity = random.expovariate(0.05)
#     #orders.append({'id': 0, 'type': 'limit', 'side': side, 'price': price, 'quantity': quantity})
#     orders.append(engine.Order(0,'limit',side,price,quantity))
    

# for order in orders:
#     me.process(order)
# END Instantiate the Orderbook with random orders

app = Flask(__name__)
CORS(app)

# Generate a globally unique address for this node
node_identifier = str(uuid4()).replace('-', '')

@app.route("/")
def hello():
    return render_template('index.html')

@app.route('/orders/random', methods=['POST'])
def random_order():
    global me
    values = request.get_json()
    print(values)
    # Check that the required fields are in the POST'ed data
    required = ['number', 'midpoint']
    if not all(k in values for k in required):
        return 'Missing values', 400
    
    orders = []
    
    me.orderbook.bids*= 0
    me.orderbook.asks*= 0

    for i in range(int(values["number"])):
    #for i in range(100):
        #print(i)
        side = random.choice(["buy", "sell"])
        print(side + str(i))
        price = random.gauss(int(values["midpoint"]), 5)
        #price = random.gauss(8000, 50)
        quantity = random.expovariate(0.05)
        #orders.append({'id': 0, 'type': 'limit', 'side': side, 'price': price, 'quantity': quantity})
        orders.append(engine.Order(0,'limit',side,price,quantity))
        
    for order in orders:
        me.process(order)

    response = {'message': f'Generated {values["number"]} Orders with midpoint {values["midpoint"]} '}
    return jsonify(response), 201

@app.route('/order/new', methods=['POST'])
def new_order():
    global me
    values = request.get_json()
    print(values)
    # Check that the required fields are in the POST'ed data
    required = ['side', 'price', 'quantity']
    if not all(k in values for k in required):
        return 'Missing values', 400
    order=engine.Order(0,'limit',values["side"],int(values["price"]),int(values["quantity"]))
    me.process(order)

    print("no of bids" + str(len(me.orderbook.bids)))
    print("no of asks"+ str(len(me.orderbook.asks)))
    response = {'message': f'Received {values["side"]} Order at $ {values["price"]} for {values["quantity"]} '}
    return jsonify(response), 201

@app.route('/order/new-kafka', methods=['POST'])
def new_order_kafka():
    print('new-kafka')
    TRANSACTIONS_TOPIC = os.environ.get('TRANSACTIONS_TOPIC')
    KAFKA_BROKER_URL1 = os.environ.get('KAFKA_BROKER_URL1')
    KAFKA_BROKER_URL2 = os.environ.get('KAFKA_BROKER_URL2')
    KAFKA_BROKER_URL3 = os.environ.get('KAFKA_BROKER_URL3')
    bootstrap_servers = [KAFKA_BROKER_URL1,KAFKA_BROKER_URL2,KAFKA_BROKER_URL3]
    producer = KafkaProducer(
            bootstrap_servers=bootstrap_servers,
            # Encode all values as JSON
            value_serializer=lambda value: json.dumps(value).encode(),
        )
    
    values = request.get_json()
    print(values)
    # Check that the required fields are in the POST'ed data
    required = ['side', 'price', 'quantity']
    if not all(k in values for k in required):
        return 'Missing values', 400
    producer.send(TRANSACTIONS_TOPIC, value=value)
    
    response = {'message': f'Received {values["side"]} Order at $ {values["price"]} for {values["quantity"]} '}
    return jsonify(response), 201

@app.route('/orderbook', methods=['GET'])
def full_book():
    bid_price=[order.price for order in me.orderbook.bids] 
    bid_quantity=[order.quantity for order in me.orderbook.bids] 
    
    ask_price=[order.price for order in me.orderbook.asks] 
    ask_quantity=[order.quantity for order in me.orderbook.asks] 
    #take cumulative sums for printing
    bid_quantity=np.cumsum(bid_quantity).tolist()
    ask_quantity=np.cumsum(ask_quantity).tolist()
    
    #ask_quantity=[np.cumsum(ask_quantity)]
    #print(bid_quantity)
    #print(np.cumsum(bid_quantity))
    response = {
        'No_bids': str(len(me.orderbook.bids)),
        'bid_price': bid_price,
        'bid_quantity': bid_quantity,
        'No_asks': str(len(me.orderbook.asks)),
        'ask_price': ask_price,
        'ask_quantity': ask_quantity,

    }
    print(response)
    #return jsonify(response), 200
    return jsonify({'status':'OK','answer':json.dumps({"orderbook":response})}),200



if __name__ == '__main__':
    from argparse import ArgumentParser

    parser = ArgumentParser()
    parser.add_argument('-p', '--port', default=5000, type=int, help='port to listen on')
    args = parser.parse_args()
    port = args.port

    #app.config.from_object(Config())
    app.config['SCHEDULER_TIMEZONE']='utc'
    app.config['JSONIFY_PRETTYPRINT_REGULAR'] = True
    scheduler = APScheduler()
    scheduler.init_app(app)
    scheduler.start()

    app.run(host='0.0.0.0', port=port,use_reloader=True, use_debugger=True, use_evalex=True)