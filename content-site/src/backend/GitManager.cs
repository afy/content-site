using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Octokit;

namespace content_site.src.backend
{
    public class GitManager
    {
        private GitHubClient gitHubClient;
        private ContentTracker contentTracker; // passed through constructor
        public bool reloadRepos = false;
        public bool saveJsonOnload = false;
        
        public GitManager(ContentTracker ct) {
            gitHubClient = new GitHubClient(new ProductHeaderValue("content-site"));
            contentTracker = ct;
        }

        public void loadRepos(List<Page> pages) {
            if (reloadRepos)
            {
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

                var apiInfo = gitHubClient.GetLastApiInfo();
                var ratelimit = apiInfo?.RateLimit;
                Console.WriteLine("Done loading repos, " + ratelimit.Remaining + " requests remaining. (ratelimit " + ratelimit.Limit + "). Reset time: " + ratelimit.Reset + "(UTC)");
            }

            if (saveJsonOnload) {
                contentTracker.saveJson();
            }
        }

        public async Task<List<Commit>> getRepoCommits(string repoSrc)
        {
            var repo = await gitHubClient.Repository.Get("afy", repoSrc);
            var commits = await gitHubClient.Repository.Commit.GetAll(repo.Id);
            List<Commit> r = new List<Commit>();
            foreach (GitHubCommit c in commits)
            {
                var nc = new Commit();
                nc.author = c.Author.Login;
                nc.time = c.Commit.Author.Date.DateTime;
                nc.message = c.Commit.Message;
                nc.link = c.HtmlUrl;
                r.Add(nc);
            }
            return r;
        }
    }
}
