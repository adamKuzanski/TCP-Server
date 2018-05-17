using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerTCP
{
    // przerob get message aby byla uniwersalnie wywolywansa 

    class Server
    {
        public string Message { get; private set; } = "";
        public string MessageToBeSent { get; private set; } = "";
        public int MaxLengthOfMessage { get; set; } = 10;
        private readonly int _port = 13000;
        private readonly IPAddress _localAddress;       
        private TcpListener _theServer;
        private TcpClient _client;
        private NetworkStream _stream = null;

        public Server()
        {
            this._localAddress = IPAddress.Parse(("127.0.0.1"));
            this._theServer = new TcpListener(_localAddress, _port);         
        }

        public Server(int port, string localAdress)
        {
            this._port = port;
            this._localAddress = IPAddress.Parse(localAdress);
            this._theServer = new TcpListener(_localAddress, _port);
        }

        

        // this function connect server with localAddress and comunicates using port
        public void StartServer()
        {
            this._theServer = new TcpListener(_localAddress, _port);
            _theServer.Start();
            Console.WriteLine("Start server done!");
        }

        // this function waits untill the client connect with the server
        public bool Connect()
        {
            //tu byl while(stream != null) ale chyba zbedny
            Console.WriteLine("Waitig for connection...");
            this._client = _theServer.AcceptTcpClient();
            Console.WriteLine("Connected!");
            this._stream = _client.GetStream();
            return true;
        }

        //this function gets message from the stream untill sign is met
        public string GetMessageSign(string sign = "#")
        {
            var buffor = new byte[256];
            var offsetListen = 0;
            var checkNumberOfSigns = 0;
            var flagConnectionAvailable = true;
            var flagEndOfMessageReached = true;

            do
            {
                if (this._stream.DataAvailable)
                {
                    if (this._stream.Read(buffor, offsetListen++, 1) == 0)
                    {
                        throw new ApplicationException("Server disconnected before the expected amount of data"); //?
                    }

                    //jeżeli nie spotkaliśmy naszego znaku końca wiadomosci to modyfikujemy Message
                    if (Encoding.ASCII.GetString(new[] { buffor[offsetListen - 1] }) != sign)
                    {
                        this.Message += Encoding.ASCII.GetString(new[] { buffor[offsetListen - 1] });
                    }
                    else
                    {
                        flagEndOfMessageReached = false;
                    }
                }
                else
                {
                    flagConnectionAvailable = false;
                }
                checkNumberOfSigns++;
            } while (flagEndOfMessageReached && checkNumberOfSigns < MaxLengthOfMessage);
            return flagConnectionAvailable ? this.Message : null;
        }



        //this function modifies data send in message
        public string WorkOnData()
        {
            var newMessage = this.Message;
            newMessage += "jest boski";
            this.MessageToBeSent = newMessage;
            return newMessage;
        }

        //this funcrion sends message back to the client and returns true if everything goes wright 
        public bool SendMessage(string messageFromUser = "")
        {
            var message = "";
            message = messageFromUser == "" ? this.MessageToBeSent : messageFromUser;

            try
            {
                var sendyByte = Encoding.ASCII.GetBytes(message);
                _stream.Write(sendyByte, 0, sendyByte.Length);
                Console.WriteLine("I sent this {0}", message);
                _client.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine("madafuka");
                return false;
            }
            finally
            {
                _theServer.Stop();
            }
            return true;
        }

        public string GetMessageLength()
        {
            var buffor = new byte[MaxLengthOfMessage];
            var flagMessageAvailable = true;
            var remaining = this.MaxLengthOfMessage ;
            var offset = 0;

            while (remaining > 0 && flagMessageAvailable)
            {
                if (_stream.DataAvailable)
                {
                    var nbOfBytes = _stream.Read(buffor, offset, remaining);
                    if (nbOfBytes == 0)
                    {
                        throw new ApplicationException("Server disconnected befor the expected amount of data");
                    }
                    offset += nbOfBytes;
                    remaining -= nbOfBytes;
                }
                else
                {
                    flagMessageAvailable = false;
                }
            }

            Message = Encoding.ASCII.GetString(buffor, 0, MaxLengthOfMessage);
            return Message;
        }





    }
}






// NOT IMPORTANAT
/*
message = GetMessageLength (stream, message);  //GetMessageSign(stream, message, sign);
        if (ProceedData(stream, message))
        {
            Console.WriteLine("DONE");
        }
        else
        {
            Console.WriteLine("NOT SO DONE");
        }             
        client.Close();
    }

}*/

/*

        private static string GetMessageSign (NetworkStream stream, string message, string sign = "#")
        {
            var buffor = new byte[256];
            var offsetListen = 0;
            var checkNumberOfSigns = 0;
            var flagConnectionAvailable = true;
            var flagEndOfMessageReached = true;

            do
            {
                if (stream.DataAvailable)
                {
                    if (stream.Read(buffor, offsetListen++, 1) == 0)
                    {
                        throw new ApplicationException("Server disconnected before the expected amount of data");
                    }

                    if (Encoding.ASCII.GetString (new[] {buffor [offsetListen - 1]}) != sign)
                    {
                        message += Encoding.ASCII.GetString (new[] { buffor[offsetListen - 1] });
                    }
                    else
                    {
                        flagEndOfMessageReached = false;
                    }
                }
                else
                {
                    flagConnectionAvailable = false;
                }
                checkNumberOfSigns++;
            } while (flagEndOfMessageReached && checkNumberOfSigns < MaxLengthOfMessage);

            return flagConnectionAvailable ? message : null;
        }

        
        private static string GetMessageLength (NetworkStream stream, string message)
        {
            var buffor = new byte[256];
            var flagMessageAvailable = true;
            var remaining = MaxLengthOfMessage;
            var offset = 0;

            while (remaining > 0 && flagMessageAvailable)
            {
                if (stream.DataAvailable)
                {
                    var nbOfBytes = stream.Read(buffor, offset, remaining);
                    if (nbOfBytes == 0)
                    {
                        throw new ApplicationException("Server disconnected befor the expected amount of data");
                    }
                    offset += nbOfBytes;
                    remaining -= nbOfBytes;
                }
                else
                {
                    flagMessageAvailable = false;
                }
            }

            message = Encoding.ASCII.GetString(buffor, 0, MaxLengthOfMessage);
            return message;
        }
        


        private static bool ProceedData(NetworkStream stream, string message)
        {
            if (message != null)
            {
                var answerOfServer = message;
                answerOfServer += "chuj";
                var sendyByte = Encoding.ASCII.GetBytes(answerOfServer);
                stream.Write(sendyByte, 0, sendyByte.Length);
                Console.WriteLine("I sent this {0}", answerOfServer);
                return true;
            }
            else
            {
                Console.WriteLine("Could't send data ");
                return false;
            }
        }     
    }
}*/
