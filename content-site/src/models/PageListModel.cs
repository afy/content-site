using System;
using System.Collections.Generic;
using System.Text;
using content_site.src.backend;

namespace content_site.src.models
{
    public class PageListModel
    {
        public string uri = "";
        public List<Page> pages;
        
        public PageListModel(List<Page> p) {
            uri = Program.getUri();
            pages = p;
        }
    }
}
