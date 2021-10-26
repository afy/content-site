using System;
using System.Collections.Generic;
using System.Text;

namespace content_site.src.backend
{
    public class ProjectTracker
    {
        // temporary solution; replace with db loading later
        private Dictionary<string, Project> loadedProjects = new Dictionary<string, Project>();

        public ProjectTracker() {
            var p1 = new Project("test-project", "views/custom/test-project/main.html");
            loadedProjects.Add(p1.name, p1);

            var p2 = new Project("test-project-2", "views/custom/test-project-2/main.html");
            loadedProjects.Add(p2.name, p2);
        }
        
        public List<Project> getAllProjects() {
            var r = new List<Project>();
            foreach (var p in loadedProjects.Values) {
                r.Add(p);
            }
            return r;
        }

        public Project getProjectFromName(string n) {
            if (loadedProjects.ContainsKey(n)) {
                return loadedProjects[n];
            }
            else {
                return null;
            }
        }
    }
}
