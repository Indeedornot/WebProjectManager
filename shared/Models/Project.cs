using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace shared.Models;
public class Project : BaseEntity {
    //here projects means a task, with an assignee, a due date, and a status
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime DueDate { get; set; }
    public string Status { get; set; }
    public IEnumerable<User> Assignees { get; set; }

    public static explicit operator ProjectDTO(Project project) {
        return new ProjectDTO {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            DueDate = project.DueDate,
            Status = project.Status,
            Assignees = project.Assignees.Select(u => (UserDTO)u)
        };
    }
}

public class ProjectDTO : BaseEntity {
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime DueDate { get; set; }
    public string Status { get; set; }
    public IEnumerable<UserDTO> Assignees { get; set; }
}