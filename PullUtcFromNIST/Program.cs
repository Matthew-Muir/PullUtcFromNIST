using System;
using System.Globalization;
using System.IO;
using System.Net.Sockets;

namespace PullUtcFromNIST
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(GetNISTdtUTC());
            
        }
        public static string GetNISTdtUTC()
        {
            //List of servers to ping time, sorted by nearest to furthest.
            string[] serverList = new string[]
            {
                "utcnist.colorado.edu", // University of Colorado, Boulder
                "time-c-b.nist.gov", // Boulder, Colorado
                "time-c-wwv.nist.gov", // Fort Collins, Colorado
                "time-c-g.nist.gov" // Gaithersburg, Maryland
            };
            //Loops through serverList looking for a reponsive server to get time from. If a server isn't found the loop
            //exits and returns the local machines UTC time.
            foreach (var server in serverList)
            {
                try
                {
                    var client = new TcpClient();
                    if (!client.ConnectAsync(server, 13).Wait(250))
                    {
                        // connection failure
                    }
                    using (var streamReader = new StreamReader(client.GetStream()))
                    {
                        var response = streamReader.ReadToEnd();
                        var utcDateTimeString = response.Substring(7, 17);
                        var utcHold = DateTime.ParseExact(utcDateTimeString, "yy-MM-dd HH:mm:ss", CultureInfo.CurrentCulture);
                        DateTime utcDateTime = DateTime.SpecifyKind(utcHold, DateTimeKind.Utc);
                        Console.WriteLine(server);
                        return utcDateTime.ToString("yyyy-MM-ddTHH:mm:ss");
                        
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Broken");
                }
            }
            return DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss");
        }
    }
}
