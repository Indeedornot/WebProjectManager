using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

using shared.Models;

namespace client.Pages.Projects;

public class Display : PageModel {
    private readonly ILogger<Display> _logger;

    public Display(ILogger<Display> logger) {
        _logger = logger;
    }

    private int? id;
    public UserDTO User { get; set; }

    public void OnGet(int? id) {
        this.id = id;
    }
}