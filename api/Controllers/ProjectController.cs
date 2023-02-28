using api.Database;

using Microsoft.AspNetCore.Mvc;

using shared.Models;

namespace api.Controllers;
[ApiController]
[Route("[controller]")]
public class ProjectController : ControllerBase {
    // [HttpGet(Name = "GetProject")]
    // public IEnumerable<Project> GetProject(int id) {
    //     return new Project();
    // }

    private readonly DataContext db;
    public ProjectController(DataContext db) {
        this.db = db;
    }

    [HttpGet(Name = "GetProjects")]
    public IEnumerable<Project> GetProjects() {
        return db.Projects.ToList();
    }

    [HttpGet("{id}", Name = "GetProject")]
    public Project? GetProject(int id) {
        return db.Projects.Find(id);
    }

    [HttpPost("CreateProject")]
    public IActionResult CreateProject(ProjectDTO project) {
        db.Projects.Add((Project)project);
        db.SaveChanges();
        return CreatedAtRoute("GetProject", new { id = project.Id }, project);
    }
}
