using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchingEngine
{
    public static class OrderSide
    {
        public const bool BUY = true;
        public const bool SELL = false;
    }
    
    public enum OrderTypes { Market, Limit, Stop }

    public class Order
    {
        public int Id { get; }
        public int Type { get;  }
        public double Price { get; set; }
        public bool Side { get;  set; }
        public decimal Quantity { get; set; }
        public double Timestamp { get; set; }
        public String TimeInForce { get; set; }

        public Order(int id, int type, double price, bool side, decimal quantity, double timestamp)
        {
            Id = id;
            Type = type;
            Price = price;
            Side = side;
            Quantity = quantity;
            Timestamp = timestamp;
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
            var ret = x.Price.CompareTo(y.Price);
            if (ret == 0) { ret = x.Timestamp.CompareTo(y.Timestamp); }
            return ret;            
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
            if (Count == 0)
            {
                // List is empty so the order can just be appended
                Add(order);
            }
            else if (Comparer.Compare(this[Count - 1], order) <= 0)
            {
                // Order is smaller than the last order, so it can also just be appended
                Add(order);
            }
            else if (Comparer.Compare(this[0], order) >= 0)
            {
                // Order is larger than the first order, so it can be inserted at the top
                Insert(0, order);
            }
            else
            {
                int index = BinarySearch(order, Comparer);
                if (index < 0)
                    index = ~index;
                Insert(index, order);
            }
            
        }
    }

    public class OrderBook
    {
        private string Instrument { get; }
        private Dictionary<int, Order> OrderIdMap;
        public OrderList<Order> Bids { get; }
        public OrderList<Order> Asks { get; }
        private OrderList<Order> Conditionals;

        public OrderBook(string instrument)
        {
            Instrument = instrument;
            Bids = new OrderList<Order>(new BidComparer());
            Asks = new OrderList<Order>(new AskComparer());
        }

        public void Add(Order order)
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

        public void Remove(Order order)
        {
            if (order.Side == OrderSide.BUY)
            {
                Bids.Remove(order);
            }
            else if (order.Side == OrderSide.SELL)
            {
                Asks.Remove(order);
            }
        }

        public void Remove(int orderId)
        {
            Order order = OrderIdMap[orderId];
            Remove(order);
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
            OrderBooks.Add(symbol, new OrderBook(symbol));
        }

    }
}
