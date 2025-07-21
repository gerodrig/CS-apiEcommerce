using System;
using Microsoft.AspNetCore.Identity;

namespace cs_apiEcommerce.Models;

public class ApplicationUser : IdentityUser
{
    public string? Name { get; set; }
}
