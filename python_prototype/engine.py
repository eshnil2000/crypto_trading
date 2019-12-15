from collections import deque
from sortedcontainers import SortedList
import threading


class Order:

    def __init__(self, id, order_type, side, price, quantity):
        self.id = id
        self.type = order_type
        self.side = side.lower()
        self.price = price
        self.quantity = quantity

    def __str__(self):
        return "[" + str(self.price) + " for " + str(self.quantity) + " shares]"

class Trade:

    def __init__(self, buyer, seller, price, quantity):
        self.buy_order_id = buyer
        self.sell_order_id = seller
        self.price = price
        self.quantity = quantity

    def show(self):
        print("[", self.price, self.quantity, "]")

class OrderBook:

    def __init__(self, bids=[], asks=[]):
        self.bids = SortedList(bids, key = lambda order: -order.price)
        self.asks = SortedList(asks, key = lambda order: order.price)

    def __len__(self):
        return len(self.bids) + len(self.asks)

    def best_bid(self):
        if len(self.bids) > 0:
            return self.bids[0].price
        else:
            return 0

    def best_ask(self):
        if len(self.asks) > 0:
            return self.asks[0].price
        else:
            return 0

    def add(self, order):
        if order.side == 'buy':
            index = self.bids.bisect_right(order)
            #self.bids.insert(index, order)
            self.bids.add(order)
        elif order.side == 'sell':
            index = self.asks.bisect_right(order)
            #self.asks.insert(index, order)
            self.asks.add( order)
    def remove(self, order):
        if order.side == 'buy':
            self.bids.remove(order)
        elif order.side == 'sell':
            self.asks.remove(order)

class MatchingEngine:

    def __init__(self):
        self.queue = deque()
        self.orderbook = OrderBook()
        self.trades = deque()

    def process(self, order):
        if order.type == "limit":
                self.match_limit_order(order)

    def get_trades(self):
        trades = list(self.trades)
        return trades

    def match_limit_order(self, order):
        if order.side == 'buy' and order.price >= self.orderbook.best_ask():
            # Buy order crossed the spread
            filled = 0
            consumed_asks = []
            for i in range(len(self.orderbook.asks)):
                ask = self.orderbook.asks[i]

                if ask.price > order.price:
                    break # Price of ask is too high, stop filling order
                elif filled == order.quantity:
                    break # Order was filled

                if filled + ask.quantity <= order.quantity: # order not yet filled, ask will be consumed whole
                    filled += ask.quantity
                    trade = Trade(order.id, ask.id, ask.price, ask.quantity)
                    self.trades.append(trade)
                    consumed_asks.append(ask)
                elif filled + ask.quantity > order.quantity: # order is filled, ask will be consumed partially
                    volume = order.quantity-filled
                    filled += volume
                    trade = Trade(order.id, ask.id, ask.price, volume)
                    self.trades.append(trade)
                    ask.quantity -= volume

            for ask in consumed_asks:
                self.orderbook.remove(ask)

            if filled < order.quantity:
                self.orderbook.add(Order(order.id, "limit", order.side, order.price, order.quantity-filled))

        elif order.side == 'sell' and order.price <= self.orderbook.best_bid():
            # Sell order crossed the spread
            filled = 0
            consumed_bids = []
            for i in range(len(self.orderbook.bids)):
                bid = self.orderbook.bids[i]

                if bid.price < order.price:
                    break # Price of bid is too low, stop filling order
                if filled == order.quantity:
                    break # Order was filled

                if filled + bid.quantity <= order.quantity: # order not yet filled, bid will be consumed whole
                    filled += bid.quantity
                    trade = Trade(order.id, bid.id, bid.price, bid.quantity)
                    self.trades.append(trade)
                    consumed_bids.append(bid)
                elif filled + bid.quantity > order.quantity: # order is filled, bid will be consumed partially
                    volume = order.quantity-filled
                    filled += volume
                    trade = Trade(order.id, bid.id, bid.price, volume)
                    self.trades.append(trade)
                    bid.quantity -= volume

            for bid in consumed_bids:
                self.orderbook.remove(bid)

            if filled < order.quantity:
                self.orderbook.add(Order(order.id, "limit", order.side, order.price, order.quantity-filled))

        else:
            # Order did not cross the spread, place in order book
            self.orderbook.add(order)

    def cancel_order(self, cancel):
        pass