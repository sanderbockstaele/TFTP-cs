using System.Data;
using System.Data.Common;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TFTP_cs
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            if (args.Length == 0 )
            {
                // Start as host
                TFTP tFTP = new TFTP();

            }
            else
            {
                // start as client
                TFTP tftp = new TFTP(args[0], args[1]);

                // preforms the test handshake
                tftp.sendTest();

                // kills the socket
                tftp.killSocket();
            }

            
        }
    }
    class TFTP
    {
        Socket? socket = null;

        /// <summary>
        /// The constructor for a client
        /// </summary>
        /// <param name="hostIpAdress"></param>
        /// <param name="filepath"></param>
        public TFTP(string hostIpAdress, string filepath)
        {
            // Reprisents the host. It contains the IP adress and the post number
            IPEndPoint ipEndPoint = new( new IPAddress(Int64.Parse(hostIpAdress)), 22);
            
            // builds a socket to connect to the host
            socket = new( ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // if not assigned to an empty variable the compiler moans about it being a async function
            _ = connect(ipEndPoint);
        }

        /// <summary>
        /// 
        /// </summary>
        public TFTP()
        {
            // Get the IPAdress by searching the dns for localhost 
            IPAddress ipAddress = Dns.GetHostEntry("localhost").AddressList[0];
            // Construct the IpEndpoint
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, 11000);
            // construct the socket
            socket = new( ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // the socket should listen on port 26 for packets
            socket.Bind(ipEndPoint);
            socket.Listen(26);
        }
        /// <summary>
        /// Connect the socket to the host
        /// </summary>
        /// <param name="endPoint">the adress of the host</param>
        /// <returns></returns>
        async Task connect(IPEndPoint endPoint)
        {
            await socket.ConnectAsync(endPoint);
        }

        public async void sendTest()
        {
            // The message to be sent
            string testMessage = "TEST<|EOM|>";
            // Convert the message to a array of raw bytes
            var message = Encoding.UTF8.GetBytes(testMessage);
            // send the message over the network
            _ = await socket.SendAsync(message, SocketFlags.None);

            // the buffer for the raw bytes
            var inputBuffer = new byte[1024];
            // recieve the data and send it the the buffer
            var recieve = await socket.ReceiveAsync(inputBuffer, SocketFlags.None);
            // convert the raw bytes to a string
            var response = Encoding.UTF8.GetString(inputBuffer, 0, recieve);
            // check if the response is a acknowledgement
            if (response == "<|ACK|>")
            {
                Console.WriteLine("socket recieved acknowledgment");
            }
            
        }

        public async void recieve()
        {
            var handler = await socket.AcceptAsync();
            while(true)
            {
                // Receive message.
                var buffer = new byte[1_024];
                var received = await handler.ReceiveAsync(buffer, SocketFlags.None);
                var response = Encoding.UTF8.GetString(buffer, 0, received);

                var eom = "<|EOM|>";
                if (response.IndexOf(eom) > -1 /* is end of message */)
                {
                    Console.WriteLine(
                        $"Socket server received message: \"{response.Replace(eom, "")}\"");

                    var ackMessage = "<|ACK|>";
                    var echoBytes = Encoding.UTF8.GetBytes(ackMessage);
                    await handler.SendAsync(echoBytes, 0);
                    Console.WriteLine(
                        $"Socket server sent acknowledgment: \"{ackMessage}\"");

                    break;
                }
            }
        }
        /// <summary>
        /// This shuts down the socket on both directions
        /// </summary>
         public void killSocket()
        {
            socket.Shutdown(SocketShutdown.Both);
        }
    }
}