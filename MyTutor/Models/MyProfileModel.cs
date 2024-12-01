using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyTutor.Models
{
    public class MyProfileModel
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public String MobileNumber { get; set; }
        public string Address { get; set; }
        public string EmailId { get; set; }
        public string ProfilePicPath { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Area { get; set; }
        public string HighestEducation { get; set; }
        public string Medium { get; set; }
        public string ClassFrom { get; set; }
        public string ClassTo { get; set; }
        public string Subjects { get; set; }
        public string Experience { get; set; }
        public string ResumePath { get; set; }
        public string Description { get; set; }
        public bool? IsAccountSuspended { get; set; }
    }
}
