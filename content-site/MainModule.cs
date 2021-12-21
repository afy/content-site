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
            // main page; serve as "preview" or some sort of menu.
            Get("/", _ => {
                var model = new MainModel();
                return View["views/main.html", model];
            });

            // about page; quick info.
            Get("/about", _ => {
                return "test - about page";
            });

            // a list-style page of posts, update, git commits, and stuff like that.
            Get("/post", _ => {
                var model = new PageListModel(Program.contentTracker.getPosts());
                return View["views/postlist.html", model];
            });

            Get("/post/{name}", param => {
                Page p = Program.contentTracker.getPostByName(param.name);
                if (p == null) {
                    return "error page - no post of that name exists";
                }
                return View[p.view, p];
            });

            // a list-style page of all projects, active listed on top.
            Get("/project", _ => {
                var model = new PageListModel(Program.contentTracker.getProjects());
                return View["views/projectlist.html", model];
            });
            
            // detalied info on a specific project.
            Get("/project/{name}/", param => {
                Page p = Program.contentTracker.getProjectByName(param.name);
                if (p == null) {
                    return "error page - no project of that name exists";
                }
                return View[p.view, p];       
            });
        }
    }
}
