using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;

namespace ShoppingServer
{
    public class ClientHandler
    {
        private readonly TcpClient _client;
        private readonly Dictionary<int, Tuple<string, string>> _accounts; // Account number, (username, password)
        private readonly Dictionary<string, int> _products;
        private readonly Dictionary<int, List<Tuple<string, int>>> _orders;

        public ClientHandler(TcpClient client, Dictionary<int, Tuple<string, string>> accounts, Dictionary<string, int> products, Dictionary<int, List<Tuple<string, int>>> orders)
        {
            _client = client;
            _accounts = accounts;
            _products = products;
            _orders = orders;
        }

        public void HandleClient()
        {
            NetworkStream stream = _client.GetStream();
            StreamReader reader = new StreamReader(stream);
            StreamWriter writer = new StreamWriter(stream) { AutoFlush = true };

            int accountNumber = 0;
            bool isConnected = false;

            try
            {
                while (true)
                {
                    string command = reader.ReadLine();
                    if (string.IsNullOrEmpty(command))
                        break;

                    Console.WriteLine("Received: " + command);

                    string response = string.Empty;

                    if (command.StartsWith("CONNECT"))
                    {
                        var parts = command.Split(':');
                        accountNumber = int.Parse(parts[1]);
                        string inputPassword = parts[2];

                        if (_accounts.ContainsKey(accountNumber) && _accounts[accountNumber].Item2 == inputPassword)
                        {
                            isConnected = true;
                            response = "CONNECTED:" + _accounts[accountNumber].Item1;
                        }
                        else
                        {
                            response = "CONNECT_ERROR";
                        }
                    }
                    else if (command == "GET_PRODUCTS" && isConnected)
                    {
                        response = "PRODUCTS:";
                        foreach (var product in _products)
                        {
                            response += product.Key + "," + product.Value + "|";
                        }
                        response = response.TrimEnd('|');
                    }
                    else if (command == "GET_ORDERS" && isConnected)
                    {
                        response = "ORDERS:";
                        if (_orders.ContainsKey(accountNumber))
                        {
                            foreach (var order in _orders[accountNumber])
                            {
                                response += order.Item1 + "," + order.Item2 + "," + _accounts[accountNumber].Item1 + "|";
                            }
                        }
                        response = response.TrimEnd('|');
                    }
                    else if (command.StartsWith("PURCHASE") && isConnected)
                    {
                        string productName = command.Split(':')[1];
                        if (_products.ContainsKey(productName) && _products[productName] > 0)
                        {
                            if (!_orders.ContainsKey(accountNumber))
                            {
                                _orders[accountNumber] = new List<Tuple<string, int>>();
                            }
                            _orders[accountNumber].Add(new Tuple<string, int>(productName, 1));
                            _products[productName] -= 1;
                            response = "PURCHASE_SUCCESS";
                        }
                        else
                        {
                            response = "NOT_AVAILABLE";
                        }
                    }
                    else if (command == "DISCONNECT" && isConnected)
                    {
                        response = "DISCONNECTED";
                        writer.WriteLine(response);
                        Console.WriteLine("Sent: " + response);
                        break;
                    }
                    else
                    {
                        response = "NOT_CONNECTED";
                    }

                    // Print the response to the console before sending it to the client
                    Console.WriteLine("Sent: " + response);
                    writer.WriteLine(response);
                }
            }
            finally
            {
                _client.Close();
            }
        }
    }
}
