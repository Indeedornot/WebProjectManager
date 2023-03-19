using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace shared.Models;

public class Entities : BaseEntity {
    //here projects means a task, with an assignee, a due date, and a status
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime DueDate { get; set; }
    public string Status { get; set; }
    public IEnumerable<ProjectUser> Assignees { get; set; }
}

public class ProjectUser : BaseEntity {
    public int ProjectId { get; set; }
    public int UserId { get; set; }
}