using shared.Models;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace shared.Api;
public static class Routes {
    public static class Project {
        public const string GetProjects = "Project";
        public static string GetProject(int id) => $"Project/{id}";
        public const string CreateProject = "Project";
    }
}

public abstract class IProjectApi {
    public abstract Task<Project?> GetProject(int id);
    public abstract Task<IEnumerable<Project>> GetProjects();
    public abstract Task<Project> CreateProject(ProjectDTO project);
}

public class ProjectApi : IProjectApi {
    private readonly HttpClient client;
    public ProjectApi(HttpClient client) {
        this.client = client;
    }

    public override async Task<Project?> GetProject(int id) {
        string url = Routes.Project.GetProject(id);

        return await client.GetFromJsonAsync<Project>(url);
    }

    public override async Task<IEnumerable<Project>> GetProjects() {
        string url = Routes.Project.GetProjects;

        var projects = await client.GetFromJsonAsync<IEnumerable<Project>>(url);
        if (projects == null) {
            throw new HttpRequestException("Could not get projects");
        }

        return projects;
    }

    public override async Task<Project> CreateProject(ProjectDTO newProject) {
        var response = await client.PostAsJsonAsync("Project", newProject);
        response.EnsureSuccessStatusCode();

        var project = await response.Content.ReadFromJsonAsync<Project>();
        if (project == null) {
            throw new HttpRequestException("Could not create project");
        }

        return project;
    }
}