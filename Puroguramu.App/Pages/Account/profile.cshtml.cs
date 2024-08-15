using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Puroguramu.Domains.modelsDomains;
using Puroguramu.Domains.Repositories;
using Puroguramu.Domains.Validation;
using Puroguramu.Infrastructures.Data.models;

namespace Puroguramu.App.Pages.Account
{
    public class Profile : PageModel
    {
        private readonly IStudentRepository _studentRepository;

        [BindProperty]
        public StudentDto Student { get; set; }

        [BindProperty]
        [Display(Name = "Photo de Profil")]
        [ProfilePictureValidation(MaxFileSize = 1 * 1024 * 1024, AllowedExtensions = new[] { ".jpg", ".jpeg", ".png" })]
        public IFormFile ProfilePictureFile { get; set; }

        public Profile(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var studentDto = await _studentRepository.GetStudentProfileAsync(User);
            if (studentDto == null)
                return NotFound($"Unable to load user.");

            Student = studentDto;

            return Page();
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostAsync()
        {

            var success = await _studentRepository.UpdateStudentProfileAsync(User, Student, ProfilePictureFile);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, "Failed to update profile.");
                return Page();
            }

            return RedirectToPage();
        }
    }
}
