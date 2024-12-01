using System;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace MyTutor.Models
{
    public class RegisterModel
    {
        public int Id { get; set; }
        [Display(Name = "Full Name")]
        [Required(ErrorMessage = "Full Name is required.")]
        public string FullName { get; set; }

        [Display(Name = "Mobile Number")]
        [Required(ErrorMessage = "Mobile Number is required.")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid phone number")]
        public String MobileNumber { get; set; }

        [Display(Name = "Address")]
        [Required(ErrorMessage = "Address is required.")]
        public string Address { get; set; }
        
        [Display(Name = "Password")]
        [Required(ErrorMessage = "Password is required.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,15}$", ErrorMessage = "Password must be between 6 to 20 characters and contain one uppercase letter, one lowercase letter, one digit and one special character.")]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Password doesn't match.")]
        [Display(Name = "Confirm Password")]
        [Required(ErrorMessage = "Confirm Password is required.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,15}$", ErrorMessage = "Password must be between 6 to 20 characters and contain one uppercase letter, one lowercase letter, one digit and one special character.")]
        public string ConfirmPassword { get; set; }

        [Display(Name ="Email")]
        [Required(ErrorMessage = "Email is required.")]
        [DataType(DataType.EmailAddress)]
        [MaxLength(50)]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Please enter a valid e-mail adress")]
        public string EmailId { get; set; }

        [DataType(DataType.Upload)]
        [Required(ErrorMessage = "Please choose profile image")]
        [Display(Name = "Profile Picture")]
        //[RegularExpression(@"([a-zA-Z0-9\s_\\.\-:])+(.png|.jpg|.gif)$")]
        public HttpPostedFileBase ProfileImage { get; set; }

        [Display(Name = "State")]
        [Required(ErrorMessage = "State is required.")]
        public int StateId { get; set; }

        [Display(Name = "City")]
        [Required(ErrorMessage = "City is required.")]
        public int CityId { get; set; }

        [Display(Name = "Area")]
        [Required(ErrorMessage = "Area is required.")]
        public int AreaId { get; set; }
    }
}
