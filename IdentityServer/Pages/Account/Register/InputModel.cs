﻿using System.ComponentModel.DataAnnotations;

namespace IdentityServer.Pages.Account.Register;

public class InputModel
{
    [Required] public string Username { get; set; }

    [Required] public string Password { get; set; }

    [Required][Url] public string Avatar { get; set; }

    public bool RememberLogin { get; set; }

    public string ReturnUrl { get; set; }

    public string Button { get; set; }
}
