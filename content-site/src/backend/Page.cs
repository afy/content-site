using System;
using System.Collections.Generic;
using System.Text;

namespace content_site.src.backend
{
    public class Page {
        public string name { get; set; }
        public string type { get; set; }
        public string view { get; set; }
        public DateTime posted { get; set; }
        public Dictionary<string, string> data { get; set; }
    }
}
