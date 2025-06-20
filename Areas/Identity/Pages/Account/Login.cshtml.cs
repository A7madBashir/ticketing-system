using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TicketingSystem.Areas.Identity.Pages.Account;

public class LoginModel : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; }

    public class InputModel
    {
        // [Required] // Add validation attributes in a real application
        // [EmailAddress]
        public string Email { get; set; }

        // [Required]
        // [DataType(DataType.Password)]
        public string Password { get; set; }
    }

    public void OnGet()
    {
        // This method handles the initial GET request to display the login page.
        // No specific logic needed here for this simple example.
    }
}
