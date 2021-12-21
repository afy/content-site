using System;
using System.Collections.Generic;
using System.Text;

namespace content_site.src.backend
{
    public class ProjectData {
        public List<string> gallery { get; set; }
        public string githubSrc { get; set; }

        // set after load
        public bool githubLoaded { get; set; } 
        public List<Commit> commits { get; set; }
        public Commit latestCommit { get; set; }
        public string gitFailedReason { get; set; }
    }

    public class Page {
        public string name { get; set; }
        public string type { get; set; }
        public string view { get; set; }
        public string text { get; set; }
        public DateTime posted { get; set; }
        public Dictionary<string, string> data { get; set; }
        public ProjectData projectData { get; set; } // optional; set only for projects
    }
}
