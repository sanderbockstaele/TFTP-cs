using System.Net.Sockets;

﻿namespace TFTP_cs
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
        }
    }
    class TFTP
    {
        Uri? hostUri = null;
        Uri? filePath = null;
        Socket? socket = null;

        public TFTP(string hostIpAdress, string filepath)
        {
            hostUri ??= new Uri(hostIpAdress);
            filePath ??= new Uri(filepath);
            socket ??= new Socket(SocketType.Stream, ProtocolType.Udp);
        }
    }
}