using System;
using System.Collections.Generic;
using System.Text;
using content_site.src.backend;

namespace content_site.src.models
{
    public class MainModel
    {
        public string uri = "";
        public List<Page> latestPages;

        public MainModel() {
            latestPages = Program.contentTracker.getLatestPages(10);
        } 
    }
}
