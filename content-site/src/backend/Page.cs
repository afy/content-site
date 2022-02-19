using System;
using System.Collections.Generic;
using System.Text;

namespace content_site.src.backend
{
    public class Update {
        public DateTime posted { get; set; }
        public string title { get; set; }
        public string text { get; set; }
    }

    public class ProjectData {
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
        public string info { get; set; } // header value; old .text for posts, redirect for updates
        public string banner { get; set; }
        public DateTime posted { get; set; }
        public List<Update> updates { get; set; }
        public List<string> gallery { get; set; }
        public ProjectData projectData { get; set; } // optional; set only for projects
        public bool isRedirect { get; set; }
    }
}
