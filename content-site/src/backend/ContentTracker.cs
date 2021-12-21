using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.Json;
using System.IO;
using Octokit;

namespace content_site.src.backend
{
    public class ContentTracker
    {
        private class PageListJson {
            public List<Page> pages { get; set; }
        }
        
        private List<Page> pages;
        public string jsonLoc = "data/pages.json";
        public bool githubTrackerEnabled = true;
        public GitHubClient gitHubClient;
        
        public ContentTracker() {
            gitHubClient = new GitHubClient(new ProductHeaderValue("content-site"));

            saveRandomPages();
            var jsonData = JsonSerializer.Deserialize<PageListJson>(File.ReadAllText(jsonLoc));
            pages = jsonData.pages;

            if (githubTrackerEnabled)
            {
                // today is just not my day
                /*var apiInfo = gitHubClient.GetLastApiInfo().RateLimit;
                Console.WriteLine("GitHub tracking enabled \nLimit: " + apiInfo.Limit.ToString() + "\nRemaining: " +
                    apiInfo.Remaining.ToString() + "\nReset: " + apiInfo.Reset.ToString());*/

                // https://stackoverflow.com/questions/45585356/how-to-get-list-of-files-for-every-commit-with-octokit-and-c-sharp   <--- god bless

                foreach (var p in pages)
                {
                    if (p.type == "project" && p.projectData.githubSrc != null)
                    {
                        p.projectData.githubLoaded = true;
                        Task<List<Commit>> commitList = getRepoCommits(p.projectData.githubSrc);
                        commitList.Wait(10000);

                        p.projectData.commits = new List<Commit>();
                        foreach (Commit c in commitList.Result)
                        {
                            p.projectData.commits.Add(c);
                        }

                        p.projectData.latestCommit = p.projectData.commits[0];
                    }
                    else
                    {
                        if (p.projectData != null)
                        {
                            p.projectData.githubLoaded = false;
                            p.projectData.gitFailedReason = "No git source set for this project.";
                        }
                    }
                }
            }
            else {
                foreach (var p in pages) {
                    if (p.type == "project") {
                        p.projectData.githubLoaded = false;
                        p.projectData.gitFailedReason = "Git tracking is disabled for all projects.";
                    }
                }
            }
        }
       

        public async Task<List<Commit>> getRepoCommits(string repoSrc) {
            var repo = await gitHubClient.Repository.Get("afy", repoSrc);
            var commits = await gitHubClient.Repository.Commit.GetAll(repo.Id);
            List<Commit> r = new List<Commit>();
            foreach (GitHubCommit c in commits) {
                var nc = new Commit();
                nc.author = c.Author.Login;
                nc.time = c.Commit.Author.Date.DateTime;
                nc.message = c.Commit.Message;
                nc.link = c.HtmlUrl;
                r.Add(nc);
            }
            return r;
        }

        public List<Page> getLatestPages(int maxEntries = 5)
        {
            int max = (maxEntries > pages.Count) ? pages.Count : maxEntries;
            var r = new List<Page>();
            for (int i = 0; i < max; i++)
            {
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

        public int getNumberOfPages() {
            return pages.Count;
        }

        // debug function / test page layout
        // ideally pages shouldn't be written on load (obviously)
        // but it isn't vitally important to fix right away 
        public void saveRandomPages(){
            List<Page> pl = new List<Page>();

            string[] types = new string[] { "project", "post" };
            Random rnd = new Random();

            int pi = 0;
            int bi = 0;
            
            for (int i = 0; i < 10; i++) {
                int r = rnd.Next(0, 2);
                string t = types[r];
                var _d = new Dictionary<string, string>() { { "index", i.ToString() } };
                Page p = new Page {
                    type = t,
                    view = "views/" + t + ".html",
                    posted = DateTime.Now,
                    data = _d
                };


                string x;
                if (r == 0) { // project
                    pi += 1;
                    x = pi.ToString();
                    p.text = "test project number " + x;
                    p.projectData = new ProjectData() { gallery = new List<string>() {} };
                }
                else if (r == 1) { // post
                    bi += 1;
                    x = bi.ToString();
                    p.text = "test project number " + x;
                }
                else {
                    Console.WriteLine("fix ur code dumbass");
                    x = "";
                }
                
                p.name = "test-" + t + "-" + x;
                pl.Add(p);
            }

            pl.Add(new Page() {
                name = "content-site",
                type = "project",
                view = "views/project.html",
                text = "A simple project for tracking/displaying projects and updates",
                posted = DateTime.Now,
                projectData = new ProjectData() {
                    gallery = new List<string>() { "i1.png", "i2.png" },
                    githubSrc = "content-site"
                }
            });

            var options = new JsonSerializerOptions() {
                WriteIndented = true
            };
            var d = JsonSerializer.Serialize(new Dictionary<string, List<Page>>() { { "pages", pl } }, options);
            File.WriteAllText(jsonLoc, d);
        }
    }
}
