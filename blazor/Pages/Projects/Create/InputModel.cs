using blazor.Validators;

using shared.Models;

using System.ComponentModel.DataAnnotations;

namespace blazor.Pages.Projects.Create;

public class InputModel {
    [Required] public string Name { get; set; } = string.Empty;
    [Required] public string Description { get; set; } = string.Empty;
    [LaterThanNow] public DateTime DueDate { get; set; } = DateTime.Now;
    [Required] public string Status { get; set; } = string.Empty;
    [MinLength(1)] public IEnumerable<string> Assignees { get; set; } = Array.Empty<string>();
}