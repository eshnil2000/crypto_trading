import engine
import numpy as np
import random
me=engine.MatchingEngine()

# order = engine.Order(0,'limit','buy',80,200)
# me.process(order)
# order = engine.Order(0,'limit','buy',90,13)
# me.process(order)
# order = engine.Order(0,'limit','buy',100,130)
# me.process(order)
# order = engine.Order(0,'limit','sell',160,130)
# me.process(order)
# order = engine.Order(0,'limit','sell',155,1000)
# me.process(order)
# order = engine.Order(0,'limit','sell',151,100)
# me.process(order)
# order = engine.Order(0,'limit','sell',150,105)
# me.process(order)

nr_of_orders=200
midpoint=100

orders = []
for i in range(nr_of_orders):
	side = random.choice(["buy", "sell"])
	price = random.gauss(midpoint, 5)
	quantity = random.expovariate(0.05)
	#orders.append({'id': 0, 'type': 'limit', 'side': side, 'price': price, 'quantity': quantity})
	orders.append(engine.Order(0,'limit',side,price,quantity))
	

for order in orders:
	me.process(order)

for bid in me.orderbook.bids:
	print(bid)

for ask in me.orderbook.asks:
	print(ask)

bid_x=[order.price for order in me.orderbook.bids] 
bid_y=[order.quantity for order in me.orderbook.bids] 

ask_x=[order.price for order in me.orderbook.asks] 
ask_y=[order.quantity for order in me.orderbook.asks] 


import matplotlib
import matplotlib.pyplot as plt
fig = plt.figure(figsize=(10,5))
ax = fig.add_subplot(111)

ax.step(bid_x, np.cumsum(bid_y), color='green')

ax.step(ask_x, np.cumsum(ask_y), color='red')
plt.show()
#plt.show(block=False)
