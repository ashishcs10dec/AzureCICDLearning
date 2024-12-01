using log4net;
using MyTutor.Models;
using MyTutor.Usefull;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MyTutor.Controllers
{
    public class HomeController : Controller
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(HomeController));
        MyTutorDBEntities dbContext = new MyTutorDBEntities();

        public ActionResult Default()
        {
            Log.Info("Welcome to MaiTutors!");
            return View();
        }
        public ActionResult About()
        {
            //Log.Debug("Hi I am log4net Debug Level");
            //Log.Info("Hi I am log4net Info Level");
            //Log.Warn("Hi I am log4net Warn Level");

            ViewData["Message"] = "Your application description page.";
            return View();
        }

        public ActionResult Contact()
        {
            if (TempData["msg"] != null)
            {
                ViewBag.ErrorMsg = TempData["msg"].ToString();
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Contact(ContactUs contactUsModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var contactUs = new MyTutor_ContactUs()
                    {
                        EmailId = contactUsModel.Email,
                        Description = contactUsModel.Description,
                        Date = DateTime.Now,
                        IsAnswered = false
                    };
                    dbContext.MyTutor_ContactUs.Add(contactUs);
                    dbContext.SaveChanges();
                    string body = string.Empty;
                    using (StreamReader reader = new StreamReader(Server.MapPath("~/Templates/EnquiryMail.txt")))
                    {
                        body = reader.ReadToEnd();
                    }
                    body = body.Replace("{FromEmail}", contactUsModel.Email);
                    body = body.Replace("{Description}", contactUsModel.Description);
                    bool IsSendEmail = SendEmails.EmailSend("support@maitutors.com", "New Enquiry Mail", body, true);
                    TempData["msg"] = "Thanks for contacting us!";
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return RedirectToAction("Contact");
        }

        [HttpGet]
        public ActionResult AllProfile(int pageNumber = 0)
        {
            var profileList = new List<MyProfileModel>();
            var searchData = new SearchFilter();
            try
            {
                int pageSize = 20;
                var numberOfRecordToSkip = pageNumber * pageSize;
                var stateList = dbContext.MyTutor_StateList.OrderBy(x => x.States).ToList();
                List<SelectListItem> list = new List<SelectListItem>();
                foreach (var item in stateList)
                {
                    list.Add(new SelectListItem()
                    {
                        Text = item.States.ToString(),
                        Value = item.StateId.ToString()
                    });
                }
                ViewBag.StateList = list;

                List<SelectListItem> listCity = new List<SelectListItem>();
                listCity.Add(new SelectListItem()
                {
                    Text = "Select City"
                });
                ViewBag.CityList = listCity;

                List<SelectListItem> listArea = new List<SelectListItem>();
                listArea.Add(new SelectListItem()
                {
                    Text = "Select Area"
                });
                ViewBag.AreaList = listArea;

                var allRegisteredUsers = dbContext.MyTutor_Registration.Where(x => x.IsValidated == true && x.IsAccountSuspended == false).OrderByDescending(x => x.Id).Skip(numberOfRecordToSkip).Take(pageSize).ToList();

                foreach (var userData in allRegisteredUsers)
                {
                    var allProfileData = dbContext.MyTutor_Profile.Where(x => x.TutorId == userData.Id).FirstOrDefault();
                    if (allProfileData != null)
                    {
                        var tempProfileData = new MyProfileModel()
                        {
                            Id = userData.Id,
                            FullName = userData.FullName,
                            State = dbContext.MyTutor_StateList.Where(x => x.StateId == userData.StateId).FirstOrDefault().States,
                            City = dbContext.MyTutor_CityList.Where(x => x.CityId == userData.CityId).FirstOrDefault().CityName,
                            Area = dbContext.MyTutor_AreaList.Where(x => x.AreaId == userData.AreaId).FirstOrDefault().AreaName,
                            ClassFrom = allProfileData.ClassFrom,
                            ClassTo = allProfileData.ClassTo,
                            HighestEducation = allProfileData.HighestEducation,
                            Subjects = allProfileData.Subjects,
                            Experience = allProfileData.Experience,
                            MobileNumber = userData.MobileNumber,
                            ProfilePicPath = userData.ProfilePicPath,
                            EmailId = userData.Email
                        };
                        profileList.Add(tempProfileData);
                    }
                    else
                    {
                        var tempProfileData = new MyProfileModel()
                        {
                            Id = userData.Id,
                            FullName = userData.FullName,
                            State = dbContext.MyTutor_StateList.Where(x => x.StateId == userData.StateId).FirstOrDefault().States,
                            City = dbContext.MyTutor_CityList.Where(x => x.CityId == userData.CityId).FirstOrDefault().CityName,
                            Area = dbContext.MyTutor_AreaList.Where(x => x.AreaId == userData.AreaId).FirstOrDefault().AreaName,
                            ClassFrom = "",
                            ClassTo = "",
                            HighestEducation = "",
                            Subjects = "",
                            Experience = "",
                            MobileNumber = userData.MobileNumber,
                            ProfilePicPath = userData.ProfilePicPath,
                            EmailId = userData.Email
                        };
                        profileList.Add(tempProfileData);
                    }
                }
                var CustomTuple = new Tuple<List<MyProfileModel>, SearchFilter>(profileList, searchData);
                return View(CustomTuple);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                var CustomTuple = new Tuple<List<MyProfileModel>, SearchFilter>(profileList, searchData);
                return View(CustomTuple);
            }
        }
        [HttpPost]
        public ActionResult AllProfile(int pageNumber = 0, string StateId = "0", string CityId = "0", string AreaId = "0")
        {
            try
            {
                var searchModel = new SearchFilter()
                {
                    StateId = StateId == "" ? 0 : Convert.ToInt32(StateId),
                    CityId = CityId == "Select City" ? 0 : Convert.ToInt32(CityId),
                    AreaId = AreaId == "Select Area" ? 0 : Convert.ToInt32(AreaId)
                };
                int pageSize = 20;
                var numberOfRecordToSkip = pageNumber * pageSize;
                var allRegisteredUsers = new List<MyTutor_Registration>();
                if (searchModel.StateId == 0 && searchModel.CityId == 0 && searchModel.AreaId == 0)
                {
                    allRegisteredUsers = dbContext.MyTutor_Registration.Where(x => x.IsValidated == true && x.IsAccountSuspended == false).OrderByDescending(x => x.Id).Skip(numberOfRecordToSkip).Take(pageSize).ToList();
                }
                else
                {
                    if (searchModel.StateId != 0 && searchModel.CityId == 0 && searchModel.AreaId == 0)
                    {
                        allRegisteredUsers = dbContext.MyTutor_Registration.Where(x => x.StateId == searchModel.StateId && x.IsValidated == true && x.IsAccountSuspended == false)
                                             .OrderByDescending(x => x.Id)
                                             .Skip(numberOfRecordToSkip).Take(pageSize).ToList();
                    }
                    else if (searchModel.StateId != 0 && searchModel.CityId != 0 && searchModel.AreaId == 0)
                    {
                        allRegisteredUsers = dbContext.MyTutor_Registration.Where(x => x.CityId == searchModel.CityId && x.IsValidated == true && x.IsAccountSuspended == false)
                                             .OrderByDescending(x => x.Id)
                                             .Skip(numberOfRecordToSkip).Take(pageSize).ToList();
                    }
                    else if (searchModel.StateId != 0 && searchModel.CityId != 0 && searchModel.AreaId != 0)
                    {
                        allRegisteredUsers = dbContext.MyTutor_Registration.Where(x => x.AreaId == searchModel.AreaId && x.IsValidated == true && x.IsAccountSuspended == false)
                                             .OrderByDescending(x => x.Id)
                                             .Skip(numberOfRecordToSkip).Take(pageSize).ToList();
                    }
                }
                var profileList = new List<MyProfileModel>();
                foreach (var userData in allRegisteredUsers)
                {
                    var allProfileData = dbContext.MyTutor_Profile.Where(x => x.TutorId == userData.Id).FirstOrDefault();
                    if (allProfileData != null)
                    {
                        var tempProfileData = new MyProfileModel()
                        {
                            Id = userData.Id,
                            FullName = userData.FullName,
                            State = dbContext.MyTutor_StateList.Where(x => x.StateId == userData.StateId).FirstOrDefault().States,
                            City = dbContext.MyTutor_CityList.Where(x => x.CityId == userData.CityId).FirstOrDefault().CityName,
                            Area = dbContext.MyTutor_AreaList.Where(x => x.AreaId == userData.AreaId).FirstOrDefault().AreaName,
                            ClassFrom = allProfileData.ClassFrom,
                            ClassTo = allProfileData.ClassTo,
                            HighestEducation = allProfileData.HighestEducation,
                            Subjects = allProfileData.Subjects,
                            Experience = allProfileData.Experience,
                            MobileNumber = userData.MobileNumber,
                            ProfilePicPath = userData.ProfilePicPath,
                            EmailId = userData.Email
                        };
                        profileList.Add(tempProfileData);
                    }
                    else
                    {
                        var tempProfileData = new MyProfileModel()
                        {
                            Id = userData.Id,
                            FullName = userData.FullName,
                            State = dbContext.MyTutor_StateList.Where(x => x.StateId == userData.StateId).FirstOrDefault().States,
                            City = dbContext.MyTutor_CityList.Where(x => x.CityId == userData.CityId).FirstOrDefault().CityName,
                            Area = dbContext.MyTutor_AreaList.Where(x => x.AreaId == userData.AreaId).FirstOrDefault().AreaName,
                            ClassFrom = "",
                            ClassTo = "",
                            HighestEducation = "",
                            Subjects = "",
                            Experience = "",
                            MobileNumber = userData.MobileNumber,
                            ProfilePicPath = userData.ProfilePicPath,
                            EmailId = userData.Email
                        };
                        profileList.Add(tempProfileData);
                    }
                }
                var searchData = new SearchFilter();
                var CustomTuple = new Tuple<List<MyProfileModel>, SearchFilter>(profileList, searchData);
                return PartialView("~/Views/Home/_PartialUsers.cshtml", CustomTuple);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                var profileList = new List<MyProfileModel>();
                var searchData = new SearchFilter();
                var CustomTuple = new Tuple<List<MyProfileModel>, SearchFilter>(profileList, searchData);
                return PartialView("~/Views/Home/_PartialUsers.cshtml", CustomTuple);
            }
            //string viewContent = ConvertViewToString("~/Views/Home/_PartialUsers.cshtml", CustomTuple);
            //return Json(new { PartialView = viewContent });
        }

        private string ConvertViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (StringWriter writer = new StringWriter())
            {
                ViewEngineResult vResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                ViewContext vContext = new ViewContext(this.ControllerContext, vResult.View, ViewData, new TempDataDictionary(), writer);
                vResult.View.Render(vContext, writer);
                return writer.ToString();
            }
        }
        [HttpGet]
        public ActionResult UserProfile(int uId)
        {
            if (TempData["Msg"] != null)
            {
                ViewBag.Msg = TempData["Msg"].ToString();
            }
            var getContactModel = new GetContactDetails();
            var userId = uId;
            TempData["uId"] = uId;
            string str = System.Environment.CurrentDirectory;
            var myProfile = dbContext.MyTutor_Registration.Where(x => x.Id == userId).FirstOrDefault();
            var userProfileInfo = dbContext.MyTutor_Profile.Where(x => x.TutorId == userId).FirstOrDefault();
            var myProfileData = new MyProfileModel();
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
            }
            var multipleModel = new Tuple<MyProfileModel, GetContactDetails>(myProfileData, getContactModel);
            return View(multipleModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserProfile([Bind(Prefix = "Item2")] GetContactDetails getContactModel)
        {
            int uId = Convert.ToInt32(TempData["uId"].ToString());
            try
            {
                var tutorEmail = dbContext.MyTutor_Registration.Where(x => x.Id == uId).FirstOrDefault().Email;
                string body = string.Empty;
                using (StreamReader reader = new StreamReader(Server.MapPath("~/Templates/ContactDetails.txt")))
                {
                    body = reader.ReadToEnd();
                }
                body = body.Replace("{Name}", getContactModel.Name);
                body = body.Replace("{MobileNo}", getContactModel.MobileNumber);
                body = body.Replace("{Description}", getContactModel.Description);
                bool IsSendEmail = SendEmails.EmailSend(tutorEmail, "Confirm your account", body, true);
                var contactInfo = new MyTutor_GetContactInformation()
                {
                    Name = getContactModel.Name,
                    MobileNo = getContactModel.MobileNumber,
                    Descriptions = getContactModel.Description,
                    Date = DateTime.Now,
                    TutorId = uId,
                    IsEmailSend = IsSendEmail
                };
                dbContext.MyTutor_GetContactInformation.Add(contactInfo);
                dbContext.SaveChanges();
                TempData["Msg"] = "Tutor will connect you very soon.";
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return RedirectToAction("UserProfile", new { uId = uId });
        }
    }
}
