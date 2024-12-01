using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace MyTutor.Models
{
    public class CityLst
    {
        public int CityId { get; set; }
        public string CityName { get; set; }
    }
    public class AreaLst
    {
        public int AreaId { get; set; }
        public string AreaName { get; set; }
    }
    public class Signin
    {
        [Required(ErrorMessage = "Email is required.")]
        //[EmailAddress(ErrorMessage = "Invalid Email Address")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Please enter a valid e-mail adress")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is required.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,15}$", ErrorMessage = "Password must be between 6 to 20 characters and contain one uppercase letter, one lowercase letter, one digit and one special character.")]
        public string Password { get; set; }
    }
    public class UserProfile
    {
        public int Id { get; set; }
        public int TutorId { get; set; }
        public string HighestEducation { get; set; }
        public string Medium { get; set; }
        public string ClassFrom { get; set; }
        public string ClassTo { get; set; }
        public string Subjects { get; set; }
        public string Experience { get; set; }
        public string ResumePath { get; set; }
        public string Description { get; set; }
        [DataType(DataType.Upload)]
        [Display(Name = "Profile Picture")]
        public HttpPostedFileBase ProfileImage { get; set; }
    }

    public class SearchFilter
    {
        [Display(Name = "State")]
        public int StateId { get; set; }

        [Display(Name = "City")]
        public int CityId { get; set; }

        [Display(Name = "Area")]
        public int AreaId { get; set; }
    }

    public class ChangePassword
    {
        [Display(Name = "New Password")]
        [Required(ErrorMessage = "New Password is required.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,15}$", ErrorMessage = "Password must be between 6 to 20 characters and contain one uppercase letter, one lowercase letter, one digit and one special character.")]
        public string NewPassword { get; set; }
        [Compare("NewPassword", ErrorMessage = "Password doesn't match.")]
        [Display(Name = "Confirm Password")]
        [Required(ErrorMessage = "Confirm Password is required.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,15}$", ErrorMessage = "Password must be between 6 to 20 characters and contain one uppercase letter, one lowercase letter, one digit and one special character.")]
        public string ConfirmPassword { get; set; }
    }

    public class ForgetPassword
    {
        [Required(ErrorMessage = "Email is required.")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Please enter a valid e-mail adress")]
        public string Email { get; set; }
    }

    public class ContactUs
    {
        [Required(ErrorMessage = "Email is required.")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Please enter a valid e-mail adress")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; }
    }

    public class GetContactDetails
    {
        [Required]
        public string Name { get; set; }
        [Display(Name = "Mobile Number")]
        [Required(ErrorMessage = "Mobile Number is required.")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid phone number")]
        public String MobileNumber { get; set; }
        public string Description { get; set; }
    }
}
