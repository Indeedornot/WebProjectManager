using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace shared.Models;

public class User {
    public int Id { get; set; }
    public string Name { get; set; }
    public Uri Avatar { get; set; }

    public IEnumerable<Project> Projects { get; set; }
}

public class UserDTO {
    public int Id { get; set; }

    public Uri Avatar { get; set; }

    public string Name { get; set; }

    public string Password { get; set; }

    public IEnumerable<Project> Projects { get; set; }
}