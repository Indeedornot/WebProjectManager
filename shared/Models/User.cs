using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace shared.Models;

public class User {
    public int Id { get; set; }
    public string Name { get; set; }
    public Uri Avatar { get; set; }
    public string Password { get; set; }

    public IEnumerable<Project> Projects { get; set; }

    public static explicit operator UserDTO(User user) {
        return new UserDTO {
            Id = user.Id,
            Name = user.Name,
            Avatar = user.Avatar,
            Projects = user.Projects.Select(p => p.Id)
        };
    }
}

public class UserDTO {
    public int Id { get; set; }
    public Uri Avatar { get; set; }
    public string Name { get; set; }

    public IEnumerable<int> Projects { get; set; }
}