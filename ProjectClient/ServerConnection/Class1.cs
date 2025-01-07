using System;
using System.IO;
using System.Net.Sockets;

namespace ShoppingClient
{
    public class ServerConnection
    {
        private TcpClient _client;
        private StreamReader _reader;
        private StreamWriter _writer;

        public bool Connect(string hostname, int port, int accountNumber, string password, out string userName)
        {
            try
            {
                _client = new TcpClient();
                _client.Connect(hostname, port);

                NetworkStream stream = _client.GetStream();
                _reader = new StreamReader(stream);
                _writer = new StreamWriter(stream) { AutoFlush = true };

                string connectCommand = $"CONNECT:{accountNumber}:{password}";
                _writer.WriteLine(connectCommand);

                string response = _reader.ReadLine();
                if (response.StartsWith("CONNECTED"))
                {
                    userName = response.Split(':')[1];
                    return true;
                }
                else
                {
                    userName = null;
                    return false;
                }
            }
            catch
            {
                userName = null;
                return false;
            }
        }

        public string GetProducts()
        {
            _writer.WriteLine("GET_PRODUCTS");
            return _reader.ReadLine();
        }

        public string PurchaseProduct(string productName)
        {
            _writer.WriteLine($"PURCHASE:{productName}");
            return _reader.ReadLine();
        }

        public void Disconnect()
        {
            if (_client != null && _client.Connected)
            {
                _writer.WriteLine("DISCONNECT");
                _client.Close();
            }
        }
    }
}
