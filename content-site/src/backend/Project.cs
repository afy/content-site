using System;
using System.Collections.Generic;
using System.Text;

namespace content_site.src.backend
{
    public class Project
    {
        public string name { get; }
        public string view { get; }

        // temporary initialization via variables (for testing)
        public Project(string _n, string _v) {
            name = _n;
            view = _v;
        }
    }
}
