using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace MatchingEngine.tests
{
    [TestFixture]
    public class MatchingTests
    {

        private OrderBook orderBook;
        private Matcher matcher;

        [SetUp]
        public void SetUp()
        {
            matcher = new Matcher();
            orderBook = new OrderBook("stuff");
        }

        [Test]
        public void OrderAddedIfNoMatch()
        {
            // both bids and asks are empty, simply add new order
            Order bid = new Order(0, 0, 3.35, OrderSide.BUY, 10m, 1.32);
            Order ask = new Order(0, 0, 3.45, OrderSide.SELL, 10m, 1.32);
            var trades = matcher.ProcessOrder(orderBook, bid);
            trades.AddRange(matcher.ProcessOrder(orderBook, ask));
            Assert.IsEmpty(trades);
        }

        [Test]
        public void TestProcessOrderNoCrossing()
        {
            Order bid1 = new Order(0, 0, 3.45, OrderSide.BUY, 10m, 1.32);
            Order bid2 = new Order(0, 0, 3.35, OrderSide.BUY, 10m, 1.32);
            Order ask1 = new Order(0, 0, 3.55, OrderSide.SELL, 10m, 1.32);
            Order ask2 = new Order(0, 0, 3.65, OrderSide.SELL, 10m, 1.32);
            
            var trades = matcher.ProcessOrder(orderBook, bid1);
            trades.AddRange(matcher.ProcessOrder(orderBook, bid2));
            trades.AddRange(matcher.ProcessOrder(orderBook, ask1));
            trades.AddRange(matcher.ProcessOrder(orderBook, ask2));
            
            Assert.That(trades.Count, Is.EqualTo(0));

        }

        [Test]
        public void TestSingleOrderCompleteFill()
        {
            Order bid = new Order(0, 0, 3.45, OrderSide.BUY, 10m, 1.32);
            Order ask = new Order(0, 0, 3.45, OrderSide.SELL, 10m, 1.32);
            
            var trades = matcher.ProcessOrder(orderBook, bid);
            trades.AddRange(matcher.ProcessOrder(orderBook, ask));
            var trade = trades[0];
            
            Assert.That(trades.Count, Is.EqualTo(1));
            Assert.That(trade.Price, Is.EqualTo(3.45));
            Assert.That(trade.Quantity, Is.EqualTo(10));
        }
        
        [Test]
        public void TestSingleOrderPartialFill()
        {
            Order bid = new Order(0, 0, 3.45, OrderSide.BUY, 10m, 1.32);
            Order ask = new Order(0, 0, 3.45, OrderSide.SELL, 5m, 1.32);
            
            var trades = matcher.ProcessOrder(orderBook, ask);
            trades.AddRange(matcher.ProcessOrder(orderBook, bid));
            var trade = trades[0];
            
            Assert.That(trades.Count, Is.EqualTo(1));
            Assert.That(trade.Price, Is.EqualTo(3.45));
            Assert.That(trade.Quantity, Is.EqualTo(5m));

            Order leftOverBid = orderBook.Bids.Find(order => order.Id == bid.Id);
            
            Assert.That(leftOverBid.Quantity, Is.EqualTo(5m));
        }

        [Test]
        public void TestMultipleOrders()
        {
            Order bid = new Order(0, 0, 3.45, OrderSide.BUY, 10m, 1.32);
            Order ask = new Order(0, 0, 3.35, OrderSide.SELL, 5m, 1.32);
            
            var trades = matcher.ProcessOrder(orderBook, bid);
            trades.AddRange(matcher.ProcessOrder(orderBook, ask));
            
            Order bid2 = new Order(0, 0, 3.75, OrderSide.BUY, 10m, 3.32);
            trades.AddRange(matcher.ProcessOrder(orderBook, bid2));
 
            Order ask2 = new Order(0, 0, 2.75, OrderSide.SELL, 10m, 1.32);
            trades.AddRange(matcher.ProcessOrder(orderBook, ask2));

            Assert.That(trades.Count, Is.EqualTo(2));
            Assert.That(trades[1].Price, Is.EqualTo(3.75));
        }

        [Test]
        public void TestManyOrdersFromFile()
        {
            
        }
        
    }
}