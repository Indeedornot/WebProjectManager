using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace shared.Models {
    public class Project {
        //here projects means a task, with an assignee, a due date, and a status
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public string Status { get; set; }
        public IEnumerable<User> Assignees { get; set; }
    }

    public class ProjectDTO {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public string Status { get; set; }
        public IEnumerable<User> Assignees { get; set; }

        public static explicit operator Project(ProjectDTO projectDTO) {
            return new Project {
                Name = projectDTO.Name,
                Description = projectDTO.Description,
                DueDate = projectDTO.DueDate,
                Status = projectDTO.Status,
                Assignees = projectDTO.Assignees
            };
        }
    }
}