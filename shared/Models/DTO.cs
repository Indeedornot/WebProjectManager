using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace shared.Models;

public class ProjectDTO {
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime DueDate { get; set; }
    public string Status { get; set; }
    public IEnumerable<UserDTO> Assignees { get; set; }
}

public class ProjectUpdateDTO {
    public string? Name { get; set; }
    public string? Description { get; set; }
    public DateTime? DueDate { get; set; }
    public string? Status { get; set; }
}

public class UserDTO {
    public string Id { get; set; }
    public Uri Avatar { get; set; }
    public string Name { get; set; }

    public IEnumerable<int> Projects { get; set; }
}

public class ApplicationUserDTO {
    public string Id { get; set; }
    public string Name { get; set; }
    public Uri Avatar { get; set; }
}