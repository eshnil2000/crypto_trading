using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace matching_engine
{
    public static class OrderSide
    {
        public const bool BUY = true;
        public const bool SELL = false;
    }

    public enum OrderTypes { Market, Limit, Stop }

    public class Order
    {
        public int Id { get; private set; }
        public int Type { get; private set; }
        public double Price { get; private set; }
        public bool Side { get; private set; }
        public double Quantity { get; private set; }
        public double Timestamp { get; private set; }
        public String TimeInForce { get; private set; }

        public Order(int id, String type, double price, bool side, double quantity, double timestamp)
        {

        }
    }

    class BidComparer : IComparer<Order>
    {
        // Higher prices first
        public int Compare(Order x, Order y)
        {
            var ret = y.Price.CompareTo(x.Price);
            if (ret == 0) { ret = x.Timestamp.CompareTo(y.Timestamp); }
            return ret;
        }
    }

    class AskComparer : IComparer<Order>
    {
        // Lower prices first
        public int Compare(Order x, Order y)
        {
            return x.Price.CompareTo(y.Price);
        }
    }

  
    public class OrderList<Order> : List<Order>
    {
        private IComparer<Order> Comparer;

        public OrderList(IComparer<Order> comparer)
        {
            Comparer = comparer;
        }

        public void SortedInsert(Order order)
        {
            if (this.Count == 0)
            {
                this.Add(order);
                return;
            }
            if (Comparer.Compare(this[this.Count - 1], order) <= 0)
            {
                this.Add(order);
                return;
            }
            if (Comparer.Compare(this[0], order) >= 0)
            {
                this.Insert(0, order);
                return;
            }
            int index = this.BinarySearch(order, Comparer);
            if (index < 0)
                index = ~index;
            this.Insert(index, order);
        }
    }

    public class OrderBook
    {
        private String Instrument;
        private Dictionary<int, Order> OrderIdMap;
        private OrderList<Order> Bids;
        private OrderList<Order> Asks;
        private OrderList<Order> Conditionals;

        public OrderBook(String instrument)
        {
            Instrument = instrument;
            Bids = new OrderList<Order>(new BidComparer());
            Asks = new OrderList<Order>(new AskComparer());
        }

        public void AddOrder(Order order)
        {
            if (order.Side == OrderSide.BUY)
            {
                Bids.SortedInsert(order);
            }
            else if (order.Side == OrderSide.SELL) 
            {
                Asks.SortedInsert(order);
            }
        }

        public void RemoveOrder(int orderId)
        {
            Order order = OrderIdMap[orderId];
            if (order.Side == OrderSide.BUY)
            {
                Bids.Remove(order);
            }
            else if (order.Side == OrderSide.SELL)
            {
                Asks.Remove(order);
            }
        }

    }

    public class CentralOrderBook
    {
        private Dictionary<String, OrderBook> OrderBooks;

        public OrderBook this[string symbol]
        {
            get { return OrderBooks[symbol]; }
        }

        public void AddInstrument(String symbol)
        {
            OrderBooks.Add(symbol, new Lokomotor.OrderBook(symbol));
        }

    }
}
