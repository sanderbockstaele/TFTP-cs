namespace TFTP_cs
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
        public TFTP(string host, string filepath)
        {
            hostUri ??= new Uri(host);
        }
    }
    }
}