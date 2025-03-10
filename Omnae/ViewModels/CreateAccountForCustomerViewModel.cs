using System.ComponentModel.DataAnnotations;
using Omnae.Models;

namespace Omnae.ViewModels
{
    public class CreateAccountForCustomerViewModel
    {
        [Required]
        public RegisterViewModel Resgister { get; set; }
        public ContinueRegistrationViewModel ContinueRegistration { get; set; }
    }
}