using System;
using System.Collections.Generic;
using System.Text;

namespace content_site.src.backend
{
    public class Commit {
        public string author { get; set; }
        public string message { get; set; }
        public string link { get; set; }
        public DateTime time { get; set; }
    }
}