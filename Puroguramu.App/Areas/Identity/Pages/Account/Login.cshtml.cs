#nullable disable

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Puroguramu.Domains.Repositories;

namespace Puroguramu.App.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly IStudentRepository _studentRepository;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(IStudentRepository studentRepository, ILogger<LoginModel> logger)
        {
            _studentRepository = studentRepository;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<string> ExternalLogins { get; set; }
        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [RegularExpression(@"^[A-Za-z]\d{6}$", ErrorMessage = "Le matricule doit être une lettre suivie de six chiffres.")]
            public string Matricule { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");
            //await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            ExternalLogins = (await _studentRepository.GetExternalAuthenticationSchemesAsync()).ToList();
            ReturnUrl = returnUrl;
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = await _studentRepository.GetExternalAuthenticationSchemesAsync();

            if (ModelState.IsValid)
            {
                var result = await _studentRepository.AuthenticateAsync(Input.Matricule, Input.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("Utilisateur connecté.");
                    var role = await _studentRepository.GetUserRoleAsync(Input.Matricule);
                    if (role == "teacher")
                    {
                        return LocalRedirect("~/Dashboard/TeacherDashboard");
                    }
                    else if (role == "student")
                    {
                        return LocalRedirect("~/Dashboard/StudentDashboard");
                    }
                    return LocalRedirect(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("Compte utilisateur verrouillé.");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Tentative de connexion invalide.");
                    return Page();
                }
            }

            return Page();
        }
    }
}
