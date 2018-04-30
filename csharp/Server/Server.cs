using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Concurrent;

namespace Server
{

    public interface IServer
    {
        void Start();
        void Stop();
        Order GetOrder();
        void PushTrades(Trade[] trades);
    }

    public abstract class BaseServer: IServer
    {
        protected BlockingCollection<Order> OrderQueue = new BlockingCollection<Order>();

        public abstract void Start();
        public abstract void Stop();

        public Order GetOrder()
        {
            return OrderQueue.Take();
        }

        public abstract void PushTrades(Trade[] trades);
    }

    public class TCPServer : BaseServer
    {
        private bool running = false;

        private TcpListener TcpServer = new TcpListener(IPAddress.Any, 5050); // Change port later based on command line args
        private List<TcpClient> Clients = new List<TcpClient>();
        private static readonly Object lockObj = new Object();

        public TCPServer()
        {
        }

       protected void AcceptConnections(TcpListener listener)
        {
            while (running)
            {
                // Accept connection
                TcpClient tcpClient = listener.AcceptTcpClient();

                Thread clientThread = new Thread(() => HandleClient(tcpClient));
                clientThread.Start();
            }
        }

        public void HandleClient(TcpClient tcpClient)
        {
            if (OrderProtocol.Handshake(tcpClient))
            {
                Console.WriteLine("Accepted new client.");

                lock (lockObj)
                {
                    Clients.Add(tcpClient);
                }
                int clientId = Clients.IndexOf(tcpClient);

                while (running)
                {
                    Order order = OrderProtocol.ReadOrder(tcpClient);
                    OrderQueue.Add(order);
                }
            }
        }

        public override void PushTrades(Trade[] trades)
        {
            throw new NotImplementedException();
        }

        public override void Start()
        {
            Thread connectionsThread = new Thread(() => AcceptConnections(TcpServer));
            connectionsThread.Start();
        }

        public override void Stop() { }

        public void Run()
        {

        }


    }
}
