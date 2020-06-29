using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace MyProject.Models
{
    public class LoginViewModel
    {
        [Required]
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$",
            ErrorMessage = " Invalid e-mail adress")]
        public string EmailId { get; set; }
        
        [Required]
        [RegularExpression("^(?=.*[0-9]+.*)(?=.*[a-zA-Z]+.*)[0-9a-zA-Z]{5,}$",
            ErrorMessage = "Password must contain at least one letter, at least one number, and be longer than five charaters.")]
        public string Password { get; set; }

        [Required]
        public int RoleId { get; set; }
        
        public List<SelectListItem> RoleName { get; set; }

    }
}