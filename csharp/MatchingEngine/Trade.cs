using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchingEngine
{
    public class Trade
    {
        private int InitiatorId { get; }
        private int CounterpartyId { get; }
        public double Price { get; }
        public decimal Quantity { get; }

        public Trade(int initiatorId, int counterpartyId, double price, decimal quantity)
        {
            InitiatorId = initiatorId;
            CounterpartyId = counterpartyId;
            Price = price;
            Quantity = quantity;
        }
        
        public override string ToString()
        {
            return "Trade: " + Quantity + " units @ " + Price;
        }  
    }
}
