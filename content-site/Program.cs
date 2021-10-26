using System;
using Nancy.Hosting.Self;

namespace content_site
{
    class Program
    {
        private static Uri uri;

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
