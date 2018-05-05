using System;
using System.Collections.Generic;
using System.Linq;

namespace MatchingEngine
{
   
    public class Matcher
    {
        public Matcher()
        {
        }

        public List<Trade> ProcessOrder(OrderBook orderBook, Order order)
        {
            return PriceTimeMatchAlgo(orderBook, order);
        }

        private List<Trade> MatchOrder(OrderBook orderBook, Order order)
        {
            if (order.Side == OrderSide.BUY)
            {
                return MatchBid(orderBook, order);
            } 
            return MatchAsk(orderBook, order);
        }
        
        private Trade FillOrders(Order initiator, Order counterparty, decimal amount)
        {
            if (counterparty.Quantity <= amount)
            {
                // Bid size is smaller than or equal to the size of the ask
                // The bid is completely consumed by the ask
                return new Trade(initiator.Id, counterparty.Id, counterparty.Price, counterparty.Quantity);
            }
            else
            {
                // Only a part of the counterparty order was consumed
                decimal partialFill = counterparty.Quantity - amount;
                counterparty.Quantity -= partialFill;
                return new Trade(initiator.Id, counterparty.Id, counterparty.Price, partialFill);
            }
        }

        private List<Trade> MatchBid(OrderBook orderBook, Order bid)
        {
            var trades = new List<Trade>();
            decimal quantityFilled = 0;
            
            for (int i = 0; i < orderBook.Asks.Count; i++)
            {
                if (orderBook.Asks[i].Price > bid.Price || quantityFilled == bid.Quantity)
                {
                    // Price of ask is too high or order was filled
                    break;
                }
                else
                {
                    var trade = FillOrders(bid, orderBook.Asks[i], bid.Quantity - quantityFilled);
                    trades.Add(trade);
                    quantityFilled += trade.Quantity;
                }
            }

            return trades;
        }
        
        private List<Trade> MatchAsk(OrderBook orderBook, Order ask)
        {
            // Dict with order id as key and quantity filled as associated value 
            var trades = new List<Trade>();
            decimal quantityFilled = 0;
            
            for (int i = 0; i<orderBook.Bids.Count; i++)
            {
                if (orderBook.Bids[i].Price < ask.Price || quantityFilled == ask.Quantity )
                {
                    // Either the bid price has become too low or the order was filled
                    // The matching process is complete
                    break;
                }
                else
                {
                    var trade = FillOrders(ask, orderBook.Bids[i], ask.Quantity - quantityFilled);
                    trades.Add(trade);
                    quantityFilled += trade.Quantity;
                }
            }

            return trades;
        }

        public List<Trade> PriceTimeMatchAlgo(OrderBook orderBook, Order order)
        {
            List<Trade> trades = new List<Trade>();
            
            if (orderBook.IsEmpty(!order.Side))
            {
                // No matches possible if the other side is empty, so simply add the order
                orderBook.Add(order);
            }
            else
            {
                if ((order.Side == OrderSide.BUY && order.Price >= orderBook.Asks[0].Price) ||
                    (order.Side == OrderSide.SELL && order.Price <= orderBook.Bids[0].Price))
                {
                    trades = MatchOrder(orderBook, order);
                    var quantityFilled = trades.Sum(t => t.Quantity);
                    
                    // Remove all consumed orders
                    orderBook.RemoveAll(o => o.Quantity == 0);
                    
                    // Add leftover order volume as new order in book
                    if (quantityFilled < order.Quantity)
                    {
                        decimal quantityLeft = order.Quantity - quantityFilled;
                        Order leftoverOrder = new Order(order.Id,
                            order.Type,
                            order.Price,
                            order.Side,
                            quantityLeft, 
                            order.Timestamp);
                        orderBook.Add(leftoverOrder);
                    }
                }
                else
                {
                    orderBook.Add(order);
                }  
            }
            return trades;
        }

    }
}
