using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchingEngine
{
    public class Trade
    {
        public int BuyOrderId { get; }
        public int SellOrderId { get; }
        public double Price { get; }
        public decimal Quantity { get; }

        public Trade(int buyOrderId, int sellOrderId, double price, decimal quantity)
        {
            BuyOrderId = buyOrderId;
            SellOrderId = sellOrderId;
            Price = price;
            Quantity = quantity;
        }
    }
}
