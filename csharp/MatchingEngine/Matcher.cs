using System;
using System.Collections.Generic;

namespace MatchingEngine
{
    public class Matcher
    {
        private string AlgorithmName;
        public Func<OrderBook, Order, List<Trade>> MatchOrder;

        public Matcher(string algorithm)
        {
            AlgorithmName = algorithm;

            switch(algorithm)
            {
                case "PriceTime":
                default:
                    MatchOrder = PriceTimeMatchAlgo;
                    break;
            }
        }

        public List<Trade> ProcessOrder(ref OrderBook orderBook, Order order)
        {
            return MatchOrder(orderBook, order);
        }

        public List<Trade> PriceTimeMatchAlgo(OrderBook orderBook, Order order)
        {
            decimal quantityFilled = 0;
            List<Trade> trades = new List<Trade>();
            List<Order> consumedOrders = new List<Order>();

            if (order.Side == OrderSide.BUY && order.Price >= orderBook.Asks[0].Price)
            {
                for (int i = 0; i < orderBook.Asks.Count; i++)
                {
                    if (orderBook.Asks[i].Price > order.Price || quantityFilled == order.Quantity)
                    {
                        // Price of ask is too high or order was filled
                        break;
                    }
                    else if (quantityFilled + orderBook.Asks[i].Quantity <= order.Quantity)
                    {
                        quantityFilled += orderBook.Asks[i].Quantity;
                        consumedOrders.Add(orderBook.Asks[i]);
                    }
                    else if (quantityFilled + orderBook.Asks[i].Quantity > order.Quantity)
                    {
                        decimal quantityLeft = order.Quantity - quantityFilled;
                        quantityFilled += quantityLeft;
                        orderBook.Asks[i].Quantity -= quantityLeft;
                    }
                }
            }
            else if (order.Side == OrderSide.SELL && order.Price <= orderBook.Bids[0].Price)
            {
                for (int i = 0; i<orderBook.Bids.Count; i++)
                {
                    if (orderBook.Bids[i].Price < order.Price || quantityFilled == order.Quantity )
                    {
                        break;
                    }
                    else if (quantityFilled + orderBook.Bids[i].Quantity <= order.Quantity)
                    {
                        quantityFilled += orderBook.Bids[i].Quantity;
                        consumedOrders.Add(orderBook.Bids[i]);
                    }
                    else if (quantityFilled + orderBook.Bids[0].Quantity > order.Quantity)
                    {
                        decimal quantityLeft = order.Quantity - quantityFilled;
                        quantityFilled += quantityLeft;
                        orderBook.Bids[i].Quantity -= quantityLeft;
                    }
                       
                }
            }
            else
            {
                orderBook.Add(order);
            }

            // Remove consumed orders from book
            foreach (Order consumedOrder in consumedOrders)
            {
                Trade trade = new Trade(order.Id,
                                        consumedOrder.Id,
                                        consumedOrder.Price,
                                        consumedOrder.Quantity);
                trades.Add(trade);
                orderBook.Remove(consumedOrder);
            }

            // Add leftover order volume as new order in book
            if (quantityFilled <= order.Quantity)
            {
                decimal quantityLeft = order.Quantity - quantityFilled;
                Order leftoverOrder = new Order(order.Id,
                                                Order.OrderTypes.Limit,
                                                order.Side,
                                                order.Price,
                                                quantityLeft);
                orderBook.Add(leftoverOrder);
            }

            return trades;
        }

    }
}
