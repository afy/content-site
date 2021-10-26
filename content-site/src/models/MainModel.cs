using System;
using System.Collections.Generic;
using System.Text;

namespace content_site.src.models
{
    public class MainModel
    {
        public string uri = "";

        public MainModel() {
            uri = Program.getUri();
        } 
    }
}
