using System;
using System.Collections.Generic;
using System.Text.Json;
using System.IO;

namespace content_site.src.backend
{
    public class ContentTracker
    {
        private class PageListJson
        {
            public List<Page> pages { get; set; }
        }

        public GitManager gitTracker;

        private List<Page> pages;
        public string jsonLoc = "data/pages.json";

        public ContentTracker()
        {
            gitTracker = new GitManager(this);
            var jsonData = JsonSerializer.Deserialize<PageListJson>(File.ReadAllText(jsonLoc));
            pages = jsonData.pages;
            gitTracker.loadRepos(pages);
        }

        public void saveJson() {
            var options = new JsonSerializerOptions()
            {
                WriteIndented = true
            };
            var d = JsonSerializer.Serialize(new Dictionary<string, List<Page>>() { { "pages", pages } }, options);
            File.WriteAllText(jsonLoc, d);
        }

        public List<Page> getLatestPages(int maxEntries = 5)
        {
            int max = (maxEntries > pages.Count) ? pages.Count : maxEntries;
            var r = new List<Page>();
            for (int i = 0; i < max; i++)
            {
                r.Add(pages[i]);
            }
            return r;
        }
        public List<Page> getProjects() {
            return pages.FindAll(delegate (Page p) { return p.type == "project"; });
        }
        public List<Page> getPosts() {
            return pages.FindAll(delegate (Page p) { return p.type == "post"; });
        }
        public Page getProjectByName(string name)
        {
            return pages.Find(delegate (Page p) { return p.type == "project" && p.name == name; });
        }
        public Page getPostByName(string name) {
            return pages.Find(delegate (Page p) { return p.type == "post" && p.name == name; });
        }
        public int getNumberOfPages() {
            return pages.Count;
        }
    }
}
