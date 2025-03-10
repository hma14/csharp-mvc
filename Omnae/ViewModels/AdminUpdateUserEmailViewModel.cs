using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Omnae.ViewModels
{
    public class AdminUpdateUserEmailViewModel
    {
        public string Id { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "New Email")]
        public string Email { get; set; }
    }
}