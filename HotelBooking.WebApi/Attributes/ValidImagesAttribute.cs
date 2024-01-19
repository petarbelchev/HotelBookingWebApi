using System.ComponentModel.DataAnnotations;
using static HotelBooking.Common.Constants.ValidationMessages;

namespace HotelBooking.WebApi.Attributes;

public class ValidImagesAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not IFormFileCollection images || !images.Any())
            return new ValidationResult(EmptyFileCollection);

        var allowedFileTypes = new string[] { "image/jpeg", "image/png" };
        var allowedFileSize = 50 * 1024; // 50 KB

        foreach (var image in images)
        {
            if (!allowedFileTypes.Contains(image.ContentType))
                return new ValidationResult(UnsupportedImageFileType);

            if (image.Length > allowedFileSize)
                return new ValidationResult(string.Format(UnsupportedImageFileSize, allowedFileSize / 1024));
        }

        return ValidationResult.Success;
    }
}
