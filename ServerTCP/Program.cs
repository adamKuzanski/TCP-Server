using System;

namespace ServerTCP
{
    class Program
    {
        static void Main(string[] args)
        {
            var flag = true;
            do
            {
                var tcp = new Server();               
                tcp.StartServer();
                tcp.Connect();
                var receivedMessage = tcp.GetMessageLength();
                var newMessage = tcp.WorkOnData();
                flag = tcp.SendMessage();
                Console.WriteLine("Hello Pikra");
            } while (flag);
            Console.WriteLine("Dupa");

        }
    }
}



/*
 * NetworkStream networkStream = _client.GetStream();
    byte[] bytesFrom = new byte[_client.ReceiveBufferSize];
    networkStream.Read(bytesFrom, 0, (int)_client.ReceiveBufferSize);
    string dataFromServer = System.Text.Encoding.ASCII.GetString(bytesFrom);

    */