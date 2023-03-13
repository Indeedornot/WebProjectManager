using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using shared.Models;

namespace client.Pages.Users;

public class DisplayModel : PageModel {
    private int? id;
    public UserDTO User { get; set; }

    //if no id try current user
    public void OnGet(int? id) {
        this.id = id;
    }
}