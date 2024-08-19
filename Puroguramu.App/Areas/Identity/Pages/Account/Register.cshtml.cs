#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Puroguramu.Domains.Repositories;
using Puroguramu.Domains.modelsDomains;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Puroguramu.Domains.Validation;

namespace Puroguramu.App.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly IStudentRepository _studentRepository;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public RegisterModel(IStudentRepository studentRepository, ILogger<RegisterModel> logger, IWebHostEnvironment webHostEnvironment)
        {
            _studentRepository = studentRepository;
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }
        public IList<string> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "L'adresse e-mail n'est pas valide.")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "Le {0} doit être au moins {2} et au plus {1} caractères.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Mot de passe")]
            [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&.]).{6,}$", ErrorMessage = "Le mot de passe doit contenir au moins 6 caractères, dont au moins une lettre majuscule, une lettre minuscule, un chiffre et un caractère spécial.")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirmer le mot de passe")]
            [Compare("Password", ErrorMessage = "Le mot de passe et la confirmation ne correspondent pas.")]
            public string ConfirmPassword { get; set; }

            [Required]
            [RegularExpression(@"^[A-Za-z]\d{6}$", ErrorMessage = "Le matricule doit être une lettre suivie de six chiffres.")]
            [Display(Name = "Matricule")]
            public string Matricule { get; set; }

            [Required]
            [Display(Name = "Prénom")]
            public string FirstName { get; set; }

            [Required]
            [Display(Name = "Nom")]
            public string LastName { get; set; }

            [Display(Name = "Groupe de laboratoire")]
            public string LabGroup { get; set; }

            [Display(Name = "Photo de profil")]
            [ProfilePictureValidation(MaxFileSize = 1 * 1024 * 1024, AllowedExtensions = new[] { ".jpg", ".jpeg", ".png" })]
            public IFormFile ProfilePicture { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = await _studentRepository.GetExternalAuthenticationSchemesAsync();
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = await _studentRepository.GetExternalAuthenticationSchemesAsync();

            if (ModelState.IsValid)
            {
                var emailExists = await _studentRepository.IsEmailInUseAsync(Input.Email);
                var matriculeExists = await _studentRepository.IsMatriculeInUseAsync(Input.Matricule);

                if (emailExists)
                {
                    ModelState.AddModelError("Input.Email", "L'adresse e-mail est déjà utilisée.");
                }

                if (matriculeExists)
                {
                    ModelState.AddModelError("Input.Matricule", "Le matricule est déjà utilisé.");
                }

                if (!emailExists && !matriculeExists)
                {
                    var student = new StudentDto
                    {
                        Matricule = Input.Matricule,
                        FirstName = Input.FirstName,
                        LastName = Input.LastName,
                        Email = Input.Email,
                        LabGroup = Input.LabGroup,
                        ProfilePicture = null
                    };

                    if (Input.ProfilePicture != null)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await Input.ProfilePicture.CopyToAsync(memoryStream);
                            student.ProfilePicture = memoryStream.ToArray();
                        }
                    }
                    else
                    {
                        var defaultProfilePicturePath = Path.Combine(_webHostEnvironment.WebRootPath, "ressources", "basic.jpg");
                        student.ProfilePicture = await System.IO.File.ReadAllBytesAsync(defaultProfilePicturePath);
                    }

                    var role = await _studentRepository.RegisterStudentAsync(student, Input.Password);

                    var userId = await _studentRepository.GetUserIdByEmailAsync(Input.Email);
                    await _studentRepository.SignInAsync(userId, isPersistent: false);

                    if (role == "teacher")
                    {
                        return LocalRedirect("~/Dashboard/TeacherDashboard");
                    }
                    else if (role == "student")
                    {
                        return LocalRedirect("~/Dashboard/StudentDashboard");
                    }

                    Console.WriteLine("url de redirection : " + returnUrl);
                    return LocalRedirect(returnUrl);
                }
            }

            return Page();
        }
    }
}
