using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace MatchingEngine.tests
{
    [TestFixture]
    public class OrderTests{}

    [TestFixture]
    public class OrderCompareTests
    {
        private IComparer<Order> bidComparer;
        private IComparer<Order> askComparer;

        [SetUp]
        public void TestSetup()
        {
            bidComparer = new BidComparer();
            askComparer = new AskComparer();
        }

        [Test]
        public void BidOrdersPriceTimeComparedCorrectly()
        {
            long time = DateTimeOffset.Now.ToUnixTimeSeconds();
            Order highBid = new Order(0, 0, 3.45, OrderSide.BUY, 10m, time);
            Order lowBid = new Order(0, 0, 3.35, OrderSide.BUY, 10m, time);
            Order lateBid = new Order(0, 0, 3.35, OrderSide.BUY, 10m, time+1);
            
            // highBid is "smaller" than lowBid, so should return -1
            int result = bidComparer.Compare(highBid, lowBid);
            Assert.That(result, Is.EqualTo(-1));
            
            // lowBid is "larger" than highBid, so should return 1
            result = bidComparer.Compare(lowBid, highBid);
            Assert.That(result, Is.EqualTo(1));
            
            // lowBid arrived earlier than lateBid, so should return -1
            result = bidComparer.Compare(lowBid, lateBid);
            Assert.That(result, Is.EqualTo(-1));
            
            // lateBid arrived later than lowBid, so should return 1
            result = bidComparer.Compare(lateBid, lowBid);
            Assert.That(result, Is.EqualTo(1));
            
            // Comparing an order against itself should obviously return 0/equal
            result = bidComparer.Compare(lowBid, lowBid);
            Assert.That(result, Is.EqualTo(0));
        }
        
        [Test]
        public void AskOrdersPriceTimeComparedCorrectly()
        {
            long time = DateTimeOffset.Now.ToUnixTimeSeconds();
            Order highAsk = new Order(0, 0, 3.45, OrderSide.SELL, 10m, time);
            Order lowAsk = new Order(0, 0, 3.35, OrderSide.SELL, 10m, time);
            Order lateAsk = new Order(0, 0, 3.35, OrderSide.SELL, 10m, time+1);
            
            // highAsk is "larger" than lowAsk, so should return 1
            int result = askComparer.Compare(highAsk, lowAsk);
            Assert.That(result, Is.EqualTo(1));
            
            // lowAsk is "smaller" than highAsk, so should return -1
            result = askComparer.Compare(lowAsk, highAsk);
            Assert.That(result, Is.EqualTo(-1));
            
            // lowAsk arrived earlier than lateAsk, so should return -1
            result = askComparer.Compare(lowAsk, lateAsk);
            Assert.That(result, Is.EqualTo(-1));
            
            // lateAsk arrived later than lowAsk, so should return 1
            result = askComparer.Compare(lateAsk, lowAsk);
            Assert.That(result, Is.EqualTo(1));
            
            // Comparing an order against itself should obviously return 0/equal
            result = askComparer.Compare(highAsk, highAsk);
            Assert.That(result, Is.EqualTo(0));
        }


        [TestFixture]
        public class OrderListTests
        {

            [Test]
            public void AsksAreCorrectlySorted()
            {
                OrderList<Order> orderList = new OrderList<Order>(new AskComparer());
                long time = DateTimeOffset.Now.ToUnixTimeSeconds();
                Order highAsk = new Order(0, 0, 3.45, OrderSide.SELL, 10m, time);
                Order middleAsk = new Order(0, 0, 3.40, OrderSide.SELL, 10m, time);
                Order lowAsk = new Order(0, 0, 3.35, OrderSide.SELL, 10m, time);
                Order lateAsk = new Order(0, 0, 3.35, OrderSide.SELL, 10m, time-1);
                
                orderList.SortedInsert(lowAsk);
                orderList.SortedInsert(highAsk);
                orderList.SortedInsert(middleAsk);
                
                Assert.That(orderList.IndexOf(lowAsk), Is.EqualTo(0));
                Assert.That(orderList.IndexOf(middleAsk), Is.EqualTo(1));
                Assert.That(orderList.IndexOf(highAsk), Is.EqualTo(2));

                orderList.SortedInsert(lateAsk);
                
                Assert.That(orderList.IndexOf(lateAsk), Is.EqualTo(0));
            }
            
            [Test]
            public void BidsAreCorrectlySorted()
            {
                OrderList<Order> orderList = new OrderList<Order>(new BidComparer());
                long time = DateTimeOffset.Now.ToUnixTimeSeconds();
                Order highBid = new Order(0, 0, 3.45, OrderSide.BUY, 10m, time);
                Order middleBid = new Order(0, 0, 3.40, OrderSide.BUY, 10m, time);
                Order lowBid = new Order(0, 0, 3.35, OrderSide.BUY, 10m, time);
                Order earlyBid = new Order(0, 0, 3.35, OrderSide.BUY, 10m, time-1);
                
                orderList.SortedInsert(lowBid);
                orderList.SortedInsert(highBid);
                orderList.SortedInsert(middleBid);
                
                Assert.That(orderList.IndexOf(highBid), Is.EqualTo(0));
                Assert.That(orderList.IndexOf(middleBid), Is.EqualTo(1));
                Assert.That(orderList.IndexOf(lowBid), Is.EqualTo(2));
                
                orderList.SortedInsert(earlyBid);
                
                Assert.That(orderList.IndexOf(earlyBid), Is.EqualTo(2));
            }
        }

        [TestFixture]
        public class OrderBookTest{}
    }
}