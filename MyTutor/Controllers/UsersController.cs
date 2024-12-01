using log4net;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using MyTutor.Models;
using MyTutor.Usefull;
using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MyTutor.Controllers
{
    public class UsersController : Controller
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(UsersController));
        MyTutorDBEntities dbContext = new MyTutorDBEntities();
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        public UsersController()
        {

        }
        public UsersController(ApplicationUserManager manager)
        {
            UserManager = manager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        public ActionResult Signin()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Default", "Home");
            return View();
        }
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Signin(Signin signinModel)
        {
            try
            {
                if (!string.IsNullOrEmpty(signinModel.Email) && string.IsNullOrEmpty(signinModel.Password))
                {
                    return RedirectToAction("Signin");
                }
                if (ModelState.IsValid)
                {
                    var userRec = dbContext.MyTutor_Registration.Where(x => x.Email.ToLower() == signinModel.Email.ToLower()
                                  && x.Password == signinModel.Password)
                                  .FirstOrDefault();

                    if (userRec != null)
                    {
                        if (!(bool)userRec.IsValidated)
                        {
                            ViewBag.ErrorMsg = "Please activate your account first.";
                            return View();
                        }
                        userRec.IsActive = true;
                        dbContext.SaveChanges();

                        var authManager = HttpContext.GetOwinContext().Authentication;
                        var appUserDetails = new ApplicationUser() { UserName = userRec.Email, Email = userRec.Email };

                        string passwordGen = signinModel.Email.Split('@')[0];
                        var user = UserManager.Find(signinModel.Email, passwordGen);
                        if (user != null)
                        {
                            var ident = UserManager.CreateIdentity(user,
                                DefaultAuthenticationTypes.ApplicationCookie);
                            ident.AddClaims(new[] {
                                    new Claim("Id",userRec.Id.ToString())
                                });
                            authManager.SignIn(
                                new AuthenticationProperties() { IsPersistent = true },
                                ident);
                            return RedirectToAction("MyProfile", "Profile");
                        }
                        HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                        ViewBag.ErrorMsg = "Some technical error!";
                        return View();
                    }
                    else
                    {
                        ViewBag.ErrorMsg = "Login failed: Invalid email or password.";
                        return View();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return View();
        }

        [HttpGet]
        public ActionResult Logout()
        {
            try
            {
                HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                int uId = Convert.ToInt32(((ClaimsIdentity)User.Identity).FindFirstValue("Id"));
                var userRec = dbContext.MyTutor_Registration.Where(x => x.Id == uId).FirstOrDefault();
                userRec.IsActive = false;
                dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return RedirectToAction("Signin");
        }
        [Authorize]
        public ActionResult EditProfile()
        {
            var userProfile = new UserProfile();
            try
            {
                var userId = Convert.ToInt32(((ClaimsIdentity)User.Identity).FindFirstValue("Id"));
                var userData = dbContext.MyTutor_Profile.Where(x => x.TutorId == userId).FirstOrDefault();
                if (userData != null)
                {
                    userProfile.HighestEducation = userData.HighestEducation;
                    userProfile.Medium = userData.Medium;
                    userProfile.Subjects = userData.Subjects;
                    userProfile.Experience = userData.Experience;
                    userProfile.ClassFrom = userData.ClassFrom;
                    userProfile.ClassTo = userData.ClassTo;
                    userProfile.Description = userData.Description;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return View(userProfile);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProfile(UserProfile userModel)
        {
            try
            {
                var userId = Convert.ToInt32(((ClaimsIdentity)User.Identity).FindFirstValue("Id"));
                var userReg = dbContext.MyTutor_Registration.Where(x => x.Id == userId).FirstOrDefault();
                string uniqueFileName = UploadedFile(userModel);
                if (uniqueFileName != null)
                {
                    FileInfo file = new FileInfo(Server.MapPath("~/Images/" + userReg.ProfilePicPath));
                    if (file.Exists)
                    {
                        file.Delete();
                    }
                    userReg.ProfilePicPath = uniqueFileName;
                }
                var userProfile = new UserProfile();
                var userData = dbContext.MyTutor_Profile.Where(x => x.TutorId == userId).FirstOrDefault();
                if (userData != null)
                {
                    userData.HighestEducation = userModel.HighestEducation;
                    userData.Medium = userModel.Medium;
                    userData.Subjects = userModel.Subjects;
                    userData.Experience = userModel.Experience;
                    userData.ClassFrom = userModel.ClassFrom;
                    userData.ClassTo = userModel.ClassTo;
                    userData.Description = userModel.Description;
                }
                else
                {
                    var saveData = new MyTutor_Profile()
                    {
                        TutorId = userId,
                        HighestEducation = userModel.HighestEducation,
                        Medium = userModel.Medium,
                        Subjects = userModel.Subjects,
                        Experience = userModel.Experience,
                        ClassFrom = userModel.ClassFrom,
                        ClassTo = userModel.ClassTo,
                        Description = userModel.Description
                    };
                    dbContext.MyTutor_Profile.Add(saveData);
                }
                dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return RedirectToAction("MyProfile", "Profile");
        }

        private string UploadedFile(UserProfile model)
        {
            string uniqueFileName = null;
            if (model.ProfileImage != null)
            {
                string ext = System.IO.Path.GetExtension(model.ProfileImage.FileName).ToLower();
                if (model.ProfileImage.ContentLength > 4000000)
                {
                    return uniqueFileName = "101";
                }
                if (ext.ToUpper().Trim() != ".JPG" && ext.ToUpper() != ".PNG" && ext.ToUpper() != ".GIF" && ext.ToUpper() != ".JPEG")
                {
                    return uniqueFileName = "102";
                }
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ProfileImage.FileName;
                string path = Path.Combine(Server.MapPath("~/Images"), Path.GetFileName(uniqueFileName));
                model.ProfileImage.SaveAs(path);
            }
            return uniqueFileName;
        }

        [HttpGet]
        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ChangePassword(ChangePassword changepasswordModel)
        {
            try
            {
                int uId = Convert.ToInt32(((ClaimsIdentity)User.Identity).FindFirstValue("Id"));
                var userData = dbContext.MyTutor_Registration.Where(x => x.Id == uId).FirstOrDefault();
                userData.Password = changepasswordModel.NewPassword;
                dbContext.SaveChanges();
                ViewBag.ErrorMsg = "Password change successfully!";
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return View(changepasswordModel);
        }

        [HttpGet]
        public ActionResult ForgetPassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ForgetPassword(ForgetPassword forgetPasswordModel)
        {
            try
            {
                var dummyPassword = CommonMethods.CreatePassword(10);
                var userData = dbContext.MyTutor_Registration.Where(x => x.Email.ToLower() == forgetPasswordModel.Email.ToLower()).FirstOrDefault();
                if (userData != null)
                {
                    userData.Password = dummyPassword;
                    dbContext.SaveChanges();
                }
                else
                {
                    ViewBag.ErrorMsg = "Email id is not registered with us.";
                    return View();
                }

                string body = string.Empty;
                using (StreamReader reader = new StreamReader(Server.MapPath("~/Templates/ChangePassword.txt")))
                {
                    body = reader.ReadToEnd();
                }
                body = body.Replace("{userPassword}", dummyPassword.ToString());
                body = body.Replace("{UserName}", forgetPasswordModel.Email.ToString());
                bool IsSendEmail = SendEmails.EmailSend(forgetPasswordModel.Email.ToString(), "Change Password", body, true);
                if (IsSendEmail)
                    ViewBag.ErrorMsg = "Password has been sent to your registered email id.";
                else
                    ViewBag.ErrorMsg = "OOPs! Something went wrong, please try after sometime.";
                return View();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return View();
            }
        }

        [HttpGet]
        public ActionResult VerifyLink()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult VerifyLink(ForgetPassword forgetPasswordModel)
        {
            try
            {
                var tutorData = dbContext.MyTutor_Registration.Where(x => x.Email.ToLower() == forgetPasswordModel.Email.ToLower()).FirstOrDefault();
                if (tutorData != null)
                {
                    if ((bool)tutorData.IsValidated)
                    {
                        ViewBag.Msg = "Your account is already verified.";
                        return View(forgetPasswordModel);
                    }
                    else
                    {
                        string body = string.Empty;
                        using (StreamReader reader = new StreamReader(Server.MapPath("~/Templates/AccountActivation.txt")))
                        {
                            body = reader.ReadToEnd();
                        }
                        body = body.Replace("{ConfirmationLink}", "http://maitutors.com//Register/ValidateMe?id=" + tutorData.Id + "&code=" + tutorData.VerificationCode);
                        body = body.Replace("{UserName}", tutorData.Email);
                        bool IsSendEmail = SendEmails.EmailSend(tutorData.Email, "Confirm your account", body, true);
                        if (IsSendEmail)
                            ViewBag.Msg = "Congratulations! A verification link has been sent to your registered email id, please verify it.";
                        else
                            ViewBag.Msg = "OOPs! Something went wrong. Very soon you will get a verificaion link in mail.";
                        return View(forgetPasswordModel);
                    }
                }
                else
                {
                    ViewBag.Msg = "Email id is not registered with us.";
                    return View(forgetPasswordModel);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return View();
        }
    }
}
