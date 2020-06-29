using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyProject.Models
{
    public class ChangePasswordModel
    {
        public string OldPassword { get; set; }

        [RegularExpression("[a-b]", ErrorMessage = "Should have alphabets only")]
        public string NewPassword { get; set; }

        [Compare("NewPassword", ErrorMessage = "New Password & Confirm New Password didnt match")]
        public string ConfirmNewPassword { get; set; }

    }
}