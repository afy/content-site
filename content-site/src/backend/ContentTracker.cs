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
        public bool githubTrackerEnabled = false;
        public GitHubClient gitHubClient;
        
        public ContentTracker() {
            gitHubClient = new GitHubClient(new ProductHeaderValue("content-site"));

            savePages();
            var jsonData = JsonSerializer.Deserialize<PageListJson>(File.ReadAllText(jsonLoc));
            pages = jsonData.pages;

            if (githubTrackerEnabled)
            {
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
        public void savePages(){
            List<Page> pl = new List<Page>();
            pl.Add(new Page() {
                name = "content-site",
                type = "project",
                view = "views/project.html",
                info = "A simple project for tracking/displaying projects and updates",
                banner = "~/images/i1.png",
                posted = new DateTime(2021, 10, 26),
                gallery = new List<string>() { "~/images/i1.png", "~/images/i2.png" },
                updates = new List<Update>() {
                    new Update() {
                        title = "New parameters",
                        posted = new DateTime(2022, 2, 5, 16, 37, 00),
                        text = "Added various data to projects to in an effort to style the site a bit. Also added the ability to update the project (though they dont show up in latest list just yet). Beyond simple backend additions I also want to start migrating old/local projects both to the site and to my github, as well as make the github tracking for the site a bit more robust. I also want to touch on some parts of the site (more commit info in tracker, gallery viewer, etc.)"
                    }
                },
                projectData = new ProjectData() {   
                    githubSrc = "content-site"
                }
            });
            pl.Add(new Page() {
                name = "Styling the website",
                type = "post",
                view = "views/post.html",
                info = "Staring at autogenerated palettes and standard fonts is getting to me, so I have started tidying up the site and doing some basic styling to make it look a bit better. It's not perfect yet (not even close) but it's nice to see some progress given that this project has taken the backseat for a while now. More to come soon.",
                posted = new DateTime(2022, 2, 5, 16, 40, 00)
            });

            var options = new JsonSerializerOptions() {
                WriteIndented = true
            };
            var d = JsonSerializer.Serialize(new Dictionary<string, List<Page>>() { { "pages", pl } }, options);
            File.WriteAllText(jsonLoc, d);
        }
    }
}
