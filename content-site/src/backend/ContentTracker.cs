using System;
using System.Collections.Generic;
using System.Text.Json;
using System.IO;

namespace content_site.src.backend
{
    public class ContentTracker
    {
        private class PageListJson {
            public List<Page> pages { get; set; }
        }
        
        private List<Page> pages;
        public string jsonLoc = "data/pages.json";
        
        public ContentTracker() {  
            saveRandomPages();
            var jsonData = JsonSerializer.Deserialize<PageListJson>(File.ReadAllText(jsonLoc));
            pages = jsonData.pages;
        }

        public List<Page> getLatestPages(int maxEntries = 5) {
            int max = (maxEntries > pages.Count) ? pages.Count : maxEntries;
            var r = new List<Page>();
            for (int i = 0; i < max; i++) {
                r.Add(pages[pages.Count - 1 - i]);
            }
            return r;
        }
        
        public List<Page> getProjects() {
            return pages.FindAll(delegate(Page p) { return p.type == "project"; });
        }

        public Page getProjectByName(string name) {
            return pages.Find(delegate(Page p) { return p.type == "project" && p.name == name;});
        }

        public List<Page> getPosts() {
            return pages.FindAll(delegate (Page p) { return p.type == "post"; });
        }

        public Page getPostByName(string name) {
            return pages.Find(delegate (Page p) { return p.type == "post" && p.name == name; });
        }

        public void saveRandomPages(){
            List<Page> pl = new List<Page>();

            string[] types = new string[] { "project", "post" };
            Random rnd = new Random();

            int pi = 0;
            int bi = 0;

            for (int i = 0; i < 10; i++) {
                int r = rnd.Next(0, 2);
                string t = types[r];

                string x;
                if (r == 0) {
                    pi += 1;
                    x = pi.ToString();
                }
                else if (r == 1) {
                    bi += 1;
                    x = bi.ToString(); 
                }
                else {
                    Console.WriteLine("fix ur code dumbass");
                    x = "";
                }

                pl.Add(new Page {
                    name = "test-" + t + "-" + x,
                    type = t,
                    view = "views/"+t+".html",
                    posted = DateTime.Now,
                    data = new Dictionary<string, string>() { { "index", i.ToString()} }
                });
            }

            var options = new JsonSerializerOptions() {
                WriteIndented = true
            };
            var d = JsonSerializer.Serialize(new Dictionary<string, List<Page>>() { { "pages", pl } }, options);
            File.WriteAllText(jsonLoc, d);
        }
    }
}
