using System;

namespace matching_engine
{
    public class Matcher
    {
        private string AlgorithmName;
        public Func<OrderBook, Order, Trade[]> MatchOrder;

        public Matcher(string algorithm)
        {
            AlgorithmName = algorithm;

            switch(algorithm)
            {
                case "PriceTime":
                    MatchOrder = PriceTimeMatchAlgo;
                    break;
            }
        }

        public void ProcessOrder(ref OrderBook orderBook, Order order)
        {
            Trades = MatchOrder(orderBook, order);
        }

        public Trade[] PriceTimeMatchAlgo(ref OrderBook orderBook, Order order)
        {
            double quantityFilled = 0;
            List<Trade> trades = new List<Trade>();
            List<Order> consumedOrders = new List<Order>();

            if (order.side == true && order.price >= orderBook.Asks[0])// BUY order
            {
                for (int i; i<orderBook.Asks.Length; i++)
                {
                    if (orderBook.Asks[i].price > order.price || quantityFilled == order.quantity)
                    {
                        // Price of ask is too high or order was filled
                        break;
                    }
                    else if (quantityFilled + orderBook.Asks[i] <= order.quantity)
                    {
                        quantityFilled += orderBook.Asks[i].quantity;
                        consumedOrders.Add(orderBook.Asks[i]);
                    }
                    else if (quantityFilled + orderBook.Asks[i].quantity > order.quantity)
                    {
                        double quantityLeft = order.quantity - quantityFilled;
                        quantityFilled += quantityLeft;
                        orderBook.Asks[i].quantity -= quantityLeft;
                    }
                }
            }
            else if (order.side == false && order.price <= orderBook.Bids[0])
            {
                for (int i; i<orderBook.Bids.Length; i++)
                {
                    if (orderBook.Bids[i].price < order.price || quantityFilled == order.quantity )
                    {
                        break;
                    }
                    else if (quantityFilled + orderBook.Bids[i].quantity <= order.quantity)
                    {
                        quantityFilled + orderBook.Bids[i].quantity;
                        consumedOrders.Add(orderBook.Bids[i]);
                    }
                    else if (quantityFilled + orderBook.Bids[0].quantity > order.quantity)
                    {
                        double quantityLeft = order.quantity - quantityFilled;
                        quantityFilled += quantityLeft;
                        orderBook.Bids[i].quantity -= volume;
                    }
                       
                }
            }
            else
            {
                orderBook.AddOrder(order);
            }

            // Remove consumed orders from book
            foreach (Order consumedOrder in consumedOrders)
            {
                Trade trade = new Trade(order.id,
                                        consumedOrder.id,
                                        consumedOrder.price,
                                        consumedOrder.volume);
                trades.Add(trade);
                orderBook.Remove(consumedOrder);
            }

            // Add leftover order volume as new order in book
            if (quantityFilled <= order.quantity)
            {
                double quantityLeft = order.quantity - quantityFilled;
                Order leftoverOrder = new Order(order.id,
                                                Order.OrderTypes.Limit,
                                                order.side,
                                                order.price,
                                                quantityLeft);
                orderBook.Add();
            }

            return trades;
        }

    }
}
