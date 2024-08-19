using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Puroguramu.Domains.modelsDomains;
using Puroguramu.Domains.Repositories;
using Puroguramu.Infrastructures.Data.models;

namespace Puroguramu.App.Pages.Account
{
    public class ChangePasswordModel : PageModel
    {
        private readonly IStudentRepository _studentRepository;

        public ChangePasswordModel(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }

        public StudentDto Student { get; set; }

        [BindProperty]
        public PasswordInputModel Input { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public class PasswordInputModel
        {
            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Mot de Passe Actuel")]
            public string CurrentPassword { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Nouveau Mot de Passe")]
            public string NewPassword { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Confirmez le Nouveau Mot de Passe")]
            [Compare("NewPassword", ErrorMessage = "Le nouveau mot de passe et la confirmation ne correspondent pas.")]
            public string ConfirmPassword { get; set; }
        }

        public async Task OnGetAsync()
        {
            Student = await _studentRepository.GetStudentProfileAsync(User);
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var success = await _studentRepository.ChangePasswordAsync(User, Input.CurrentPassword, Input.NewPassword);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, "Failed to change password.");
                return Page();
            }

            StatusMessage = "Votre mot de passe a été changé avec succès.";
            return RedirectToPage("~/Account/ChangePassword");
        }
    }
}
