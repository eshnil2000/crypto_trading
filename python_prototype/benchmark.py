import socket
import json
import threading
import random
from timeit import default_timer as timer

import engine as m_engine
import server as m_server


def direct_benchmark(nr_of_orders, midpoint=100):
    engine = m_engine.MatchingEngine()

    orderbook_size = len(engine.orderbook)
    orders = []
    for i in range(nr_of_orders):
        side = random.choice(["buy", "sell"])
        price = random.gauss(midpoint, 5)
        quantity = random.expovariate(0.05)
        orders.append(m_engine.Order("Null", "limit", side, price, quantity))

    start = timer()
    for order in orders:
        engine.process(order)
    end = timer()
    t = end - start
    print('{0} orders processed over {1:.2f} seconds,'.format(nr_of_orders, t))
    print("at an average speed of {0:.0f} orders/second or {1:.2f} microseconds/order,".format((nr_of_orders/t), (t/nr_of_orders) * 1000 * 1000))
    print('resulting in {0} new orders in the book and {1} trades.'.format( len(engine.orderbook)-orderbook_size, len(engine.trades)))

def socket_benchmark(nr_of_orders, midpoint=100):
    engine = m_engine.MatchingEngine()
    server = m_server.MatchingEngineServer(("localhost", 8080), engine)

    t = threading.Thread(target=server.serve_forever)
    t.daemon = True
    t.start()

    orderbook_size = len(engine.orderbook)
    orders = []
    for i in range(nr_of_orders):
        side = random.choice(["buy", "sell"])
        price = random.gauss(midpoint, 5)
        quantity = random.expovariate(0.05)
        orders.append({'id': 0, 'type': 'limit', 'side': side, 'price': price, 'quantity': quantity})

    start = timer()
    sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    sock.connect(("localhost", 8080))
    for order in orders:
        serialized_order = json.dumps(order).encode('utf-8')
        sock.sendall(serialized_order)
        acknowledge = str(sock.recv(1024), "utf-8")
    end = timer()
    t = end - start
    print('{0} orders processed over {1:.2f} seconds,'.format(nr_of_orders, t))
    print("at an average speed of {0:.0f} orders/second or {1:.2f} microseconds/order,".format((nr_of_orders/t), (t/nr_of_orders) * 1000 * 1000))
    print('resulting in {0} new orders in the book and {1} trades.'.format( len(engine.orderbook)-orderbook_size, len(engine.trades)))

socket_benchmark(100000)
    

