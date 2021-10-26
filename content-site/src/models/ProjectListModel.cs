using System;
using System.Collections.Generic;
using System.Text;
using content_site.src.backend;

namespace content_site.src.models
{
    public class ProjectListModel
    {
        public List<Project> projects { get; }
        public string uri = "";
        
        public ProjectListModel(List<Project> l) {
            projects = l;
            uri = Program.getUri();
        }
    }
}
