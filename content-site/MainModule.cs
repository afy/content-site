using System;
using System.Collections.Generic;
using System.Text;
using Nancy;
using content_site.src.backend;
using content_site.src.models;

namespace content_site
{
    public class MainModule : NancyModule
    {
        public MainModule()
        {
            var projectTracker = new ProjectTracker();

            // main page; serve as "preview" or some sort of menu.
            Get("/", _ => {
                var model = new MainModel();
                return View["views/pages/main/main.html", model];
            });

            // about page; quick info.
            Get("/about", _ => {
                return "test - about page";
            });

            // a list-style page of posts, update, git commits, and stuff like that.
            Get("/posts", _ => {
                return "test - all posts tab";
            });

            // a list-style page of all projects, active listed on top.
            Get("/projects", _ => {
                var model = new ProjectListModel(projectTracker.getAllProjects());
                return View["views/pages/projects/main.html", model];
            });

            // detalied info on a specific project.
            Get("/projects/{name}/", param => {
                var p = projectTracker.getProjectFromName(param.name);
                if (p == null) {
                    return "test - no project with this name exists. (queried name="+param.name+")"; 
                }
                return View[p.view];
            });
        }
    }
}
