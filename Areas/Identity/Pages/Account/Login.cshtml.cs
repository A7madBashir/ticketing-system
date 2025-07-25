using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TicketingSystem.Models.Common;
using TicketingSystem.Models.Identity;

namespace TicketingSystem.Areas.Identity.Pages.Account;

public class LoginModel(
    SignInManager<User> signInManager,
    UserManager<User> userManager
) : PageModel
{
    private readonly SignInManager<User> _signInManager = signInManager;
    private readonly UserManager<User> _userManager = userManager;

    [BindProperty]
    public InputModel Input { get; set; }

    public class InputModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }

    // This GET handler just displays the page
    public void OnGet() { }

    // 2. This POST handler processes the login attempt
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page(); // Return the page with validation errors
        }

        // Find the user by their email address
        var user = await _userManager.FindByEmailAsync(Input.Email);
        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return Page();
        }

        // ⚙️ KEY STEP: Check if the user has the "Admin" role
        var isAdmin = await _userManager.IsInRoleAsync(user, Roles.Admin);
        var IsAgent = await _userManager.IsInRoleAsync(user, Roles.Agent);
        if (!isAdmin && !IsAgent)
        {
            // As requested, return a 400 Bad Request if not an admin
            // For a better user experience, you could also use:
            // ModelState.AddModelError(string.Empty, "Access Denied. This login is for administrators only.");
            // return Page();
            return BadRequest("Access Denied. This login is for administrators only.");
        }

        // Attempt to sign in the user with the provided password
        var result = await _signInManager.PasswordSignInAsync(
            user,
            Input.Password,
            isPersistent: false,
            lockoutOnFailure: false
        );

        if (result.Succeeded)
        {
            // ✅ Success! Redirect to the dashboard or home page
            return LocalRedirect("/");
        }
        else
        {
            // If login fails, show an error
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return Page();
        }
    }
}
