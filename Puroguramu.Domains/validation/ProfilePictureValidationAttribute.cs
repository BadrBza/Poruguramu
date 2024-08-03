
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace Puroguramu.Domains.Validation
{
    public class ProfilePictureValidationAttribute : ValidationAttribute
    {
        public int MaxFileSize { get; set; }
        public string[] AllowedExtensions { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var file = value as IFormFile;

            if (file == null)
            {
                return ValidationResult.Success;
            }

            if (file.Length > MaxFileSize)
            {
                return new ValidationResult($"La taille du fichier ne doit pas dépasser {MaxFileSize / 1024 / 1024} Mo.");
            }

            var extension = Path.GetExtension(file.FileName);
            if (Array.IndexOf(AllowedExtensions, extension.ToLower()) < 0)
            {
                return new ValidationResult("Le format du fichier n'est pas valide. Les formats autorisés sont : " + string.Join(", ", AllowedExtensions));
            }

            return ValidationResult.Success;
        }
    }
}
