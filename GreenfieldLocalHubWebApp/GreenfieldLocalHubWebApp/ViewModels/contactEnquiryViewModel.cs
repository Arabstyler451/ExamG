using System.ComponentModel.DataAnnotations;

namespace GreenfieldLocalHubWebApp.ViewModels
{
    public class contactEnquiryViewModel
    {
        [Required(ErrorMessage = "First name is required.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please select a subject.")]
        public string Subject { get; set; }

        public int? ProducerId { get; set; } // optional

        [Required(ErrorMessage = "Please enter a message.")]
        [MinLength(10, ErrorMessage = "Message must be at least 10 characters.")]
        public string Message { get; set; }
    }
}
