using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace client.Pages.Crud;
public class Projects : PageModel {
    private readonly ILogger<Projects> _logger;

    public Projects(ILogger<Projects> logger) {
        _logger = logger;
    }

    public void OnGet() {
    }
}