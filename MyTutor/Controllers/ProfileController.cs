using log4net;
using Microsoft.AspNet.Identity;
using MyTutor.Models;
using MyTutor.Usefull;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MyTutor.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(HomeController));
        MyTutorDBEntities dbContext = new MyTutorDBEntities();
        [HttpGet]
        public ActionResult MyProfile()
        {
            if (TempData["Activation"] != null)
            {
                ViewBag.ErrorMsg = TempData["Activation"].ToString();
            }
            var myProfileData = new MyProfileModel();
            try
            {
                var userId = Convert.ToInt32(((ClaimsIdentity)User.Identity).FindFirstValue("Id"));
                string str = System.Environment.CurrentDirectory;
                var myProfile = dbContext.MyTutor_Registration.Where(x => x.Id == userId).FirstOrDefault();
                var userProfileInfo = dbContext.MyTutor_Profile.Where(x => x.TutorId == userId).FirstOrDefault();
                if (myProfile != null)
                {
                    myProfileData = new MyProfileModel();
                    myProfileData.Id = myProfile.Id;
                    myProfileData.FullName = myProfile.FullName;
                    myProfileData.MobileNumber = myProfile.MobileNumber;
                    myProfileData.Address = myProfile.Address;
                    myProfileData.State = dbContext.MyTutor_StateList.Where(x => x.StateId == myProfile.StateId).FirstOrDefault().States;
                    myProfileData.City = dbContext.MyTutor_CityList.Where(x => x.CityId == myProfile.CityId).FirstOrDefault().CityName;
                    myProfileData.Area = dbContext.MyTutor_AreaList.Where(x => x.AreaId == myProfile.AreaId).FirstOrDefault().AreaName;
                    myProfileData.EmailId = myProfile.Email;
                    myProfileData.ProfilePicPath = myProfile.ProfilePicPath;
                    myProfileData.IsAccountSuspended = myProfile.IsAccountSuspended;
                    if (userProfileInfo != null)
                    {
                        myProfileData.HighestEducation = userProfileInfo.HighestEducation;
                        myProfileData.Medium = userProfileInfo.Medium;
                        myProfileData.ClassFrom = userProfileInfo.ClassFrom;
                        myProfileData.ClassTo = userProfileInfo.ClassTo;
                        myProfileData.Experience = userProfileInfo.Experience;
                        myProfileData.Subjects = userProfileInfo.Subjects;
                        myProfileData.Description = userProfileInfo.Description;
                    }
                    else if (userProfileInfo == null)
                    {
                        ViewBag.Msg = "Please complete your profile details by clicking on 'Edit Profile'.";
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return View(myProfileData);
        }
        [HttpGet]
        public ActionResult ProfileDeActivate()
        {
            try
            {
                var userId = Convert.ToInt32(((ClaimsIdentity)User.Identity).FindFirstValue("Id"));
                var updateProfile = dbContext.MyTutor_Registration.Where(x => x.Id == userId).FirstOrDefault();
                if (updateProfile != null)
                {
                    updateProfile.IsAccountSuspended = true;
                    dbContext.SaveChanges();
                    TempData["Activation"] = "Your account is deactivated. It will not show to anyone.";
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return RedirectToAction("MyProfile");
        }
        [HttpGet]
        public ActionResult ProfileActivate()
        {
            try
            {
                var userId = Convert.ToInt32(((ClaimsIdentity)User.Identity).FindFirstValue("Id"));
                var updateProfile = dbContext.MyTutor_Registration.Where(x => x.Id == userId).FirstOrDefault();
                if (updateProfile != null)
                {
                    updateProfile.IsAccountSuspended = false;
                    dbContext.SaveChanges();
                    TempData["Activation"] = "Your account is activated.";
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return RedirectToAction("MyProfile");
        }
    }
}
