using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ShoppingServer
{
    public class Program
    {
        private static readonly int SERVER_PORT = 55055;
        private static Dictionary<int, Tuple<string, string>> accounts = new Dictionary<int, Tuple<string, string>>
        {
            { 1, new Tuple<string, string>("Alice", "1") },
            { 2, new Tuple<string, string>("Bob", "2") },
            { 3, new Tuple<string, string>("Charlie", "3") }
        };

        private static Dictionary<string, int> products = new Dictionary<string, int>();
        private static Dictionary<int, List<Tuple<string, int>>> orders = new Dictionary<int, List<Tuple<string, int>>>();

        public static void Main(string[] args)
        {
            InitializeProducts();

            TcpListener listener = new TcpListener(IPAddress.Any, SERVER_PORT);
            listener.Start();
            Console.WriteLine("Server started on port " + SERVER_PORT);

            try
            {
                while (true)
                {
                    TcpClient client = listener.AcceptTcpClient();
                    ClientHandler handler = new ClientHandler(client, accounts, products, orders);
                    Thread clientThread = new Thread(handler.HandleClient);
                    clientThread.Start();
                }
            }
            finally
            {
                listener.Stop();
            }
        }

        private static void InitializeProducts()
        {
            Random rand = new Random();
            products["Apple"] = rand.Next(1, 4);
            products["Orange"] = rand.Next(1, 4);
            products["Banana"] = rand.Next(1, 4);
            products["Grapes"] = rand.Next(1, 4);
            products["Mango"] = rand.Next(1, 4);
        }
    }
}