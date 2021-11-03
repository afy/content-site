using System;
using Nancy.Hosting.Self;
using content_site.src.backend;

namespace content_site
{
    public class Program
    {
        private static Uri uri;
        public static ContentTracker contentTracker = new ContentTracker();

        static void Main(string[] args)
        {
            uri = new Uri("http://localhost:8999");
            
            using (var host = new NancyHost(uri)) {
                host.Start();
                Console.WriteLine("Now hosting");
                Console.ReadKey();
            }
        }

        // removes last char for proper model parsing
        public static string getUri() {
            string s = uri.ToString();
            return s.Remove(s.Length-1);
        }
    }
}
