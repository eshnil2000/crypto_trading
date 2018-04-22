using System;
using System.Collections.Generic;
using System.Threading;

namespace matching_engine
{
	public class Engine
	{
        private OrderBook OrderBook;
        private Matcher Matcher;
        private IServer Server;
        private Dictionary<String, Object> Config;
        private bool Running = true;

        public Engine() { }

        public void LoadConfiguration()
        {
            Matcher = new Matcher(Config["algorithm"]);

            switch(Config["server_type"])
            {
                case "TCP":
                    Server = new TCPServer();
                    break;
            }

        }

        public void Run()
        {
            LoadConfiguration();
            Server.Start();
            // Loop over orderQueue and write resulting trades to relevant clients
            while (Running)
            {
                Order order = Server.GetOrder();
                Trade[] trades = Matcher.ProcessOrder(orderBook, order);
                Server.PushTrades(trades);
            }
            Server.Stop();
        }

        static void Main(string[] args)
        {
            Engine engine = new Engine();
            engine.Run();
        }
    }
}
