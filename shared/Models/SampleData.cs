using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace shared.Models;

public static class SampleData {
    public static UserDTO User { get; set; } = new() {
        Id = "1",
        Name = "John Doe",
        Avatar = new Uri("https://gravatar.com/avatar/3ea0e9faaf1dace16ab0eb65acca0acb?s=400&d=retro&r=x"),
        Projects = new List<int> { 1, 2, 3 }
    };

    public static UserDTO User2 { get; set; } = new() {
        Id = "2",
        Name = "Jane Doe",
        Avatar = new Uri("https://gravatar.com/avatar/b8844f132ae281031d514c4e5b85ab7d?s=400&d=retro&r=x"),
        Projects = new List<int> { 1, 2 }
    };

    public static UserDTO User3 { get; set; } = new() {
        Id = "3",
        Name = "John Smith",
        Avatar = new Uri("https://gravatar.com/avatar/63764c76af6f38a20018b69c64ba7a26?s=400&d=retro&r=x"),
        Projects = new List<int> { 2, 3 }
    };

    public static IEnumerable<ProjectDTO> Projects { get; set; } = new List<ProjectDTO> {
        new() {
            Id = 1,
            Name = "Project 1",
            Description = "This is a sample project",
            DueDate = DateTime.Now.AddDays(7),
            Status = "In Progress",
            Assignees = new List<UserDTO> { User, User2 }
        },
        new() {
            Id = 2,
            Name = "Project 2",
            Description = "This is a sample project",
            DueDate = DateTime.Now.AddDays(7),
            Status = "In Progress",
            Assignees = new List<UserDTO> { User, User2, User3 }
        },
        new() {
            Id = 3,
            Name = "Project 3",
            Description = "This is a sample project",
            DueDate = DateTime.Now.AddDays(7),
            Status = "In Progress",
            Assignees = new List<UserDTO> { User, User3 }
        }
    };
}