using log4net;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using MyTutor.Models;
using MyTutor.Usefull;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MyTutor.Controllers
{
    public class RegisterController : Controller
    {
        MyTutorDBEntities dbContext = new MyTutorDBEntities();
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        private static readonly ILog Log = LogManager.GetLogger(typeof(RegisterController));

        public RegisterController()
        {

        }
        public RegisterController(ApplicationUserManager manager)
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
        public ActionResult Register()
        {
            var model = new RegisterModel();
            if (TempData["msg"] != null)
            {
                ViewBag.ErrorMsg = TempData["msg"].ToString();
            }
            var stateList = dbContext.MyTutor_StateList.ToList();
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

            return View(model);
        }
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var verificationCode = CommonMethods.RandomString(10);
                    if (dbContext.MyTutor_Registration.Where(x => x.Email.ToLower() == model.EmailId.ToLower()).Count() == 0)
                    {
                        string uniqueFileName = UploadedFile(model);

                        if (uniqueFileName != null)
                        {
                            if (uniqueFileName == "101" || uniqueFileName == "102")
                            {
                                var stateList = dbContext.MyTutor_StateList.ToList();
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
                                if (uniqueFileName == "101")
                                    ViewBag.ErrorMsg = "Image is too big.";
                                else if (uniqueFileName == "102")
                                    ViewBag.ErrorMsg = "Please choose only .jpg, .png and .gif image types!";
                                return View(model);
                            }
                            var reg = new MyTutor_Registration()
                            {
                                FullName = model.FullName,
                                Email = model.EmailId,
                                MobileNumber = model.MobileNumber,
                                Address = model.Address,
                                StateId = model.StateId,
                                CityId = model.CityId,
                                AreaId = model.AreaId,
                                Password = model.Password,
                                ProfilePicPath = uniqueFileName,
                                DateTime = DateTime.Now,
                                IsValidated = false,
                                IsActive = false,
                                IsAccountSuspended = true,
                                VerificationCode = verificationCode
                            };
                            dbContext.MyTutor_Registration.Add(reg);
                            dbContext.SaveChanges();

                            var authManager = HttpContext.GetOwinContext().Authentication;
                            var appUserDetails = new ApplicationUser() { UserName = model.EmailId, Email = model.EmailId };
                            string passwordGen = model.EmailId.Split('@')[0];
                            var result = await UserManager.CreateAsync(appUserDetails, passwordGen);
                            if (result.Succeeded)
                            {
                                await SignInManager.SignInAsync(appUserDetails, isPersistent: false, rememberBrowser: false);
                            }

                            string body = string.Empty;
                            using (StreamReader reader = new StreamReader(Server.MapPath("~/Templates/AccountActivation.txt")))
                            {
                                body = reader.ReadToEnd();
                            }
                            body = body.Replace("{ConfirmationLink}", "http://maitutors.com//Register/ValidateMe?id=" + reg.Id + "&code=" + verificationCode);
                            body = body.Replace("{UserName}", model.EmailId.ToString());
                            bool IsSendEmail = SendEmails.EmailSend(model.EmailId.ToString(), "Confirm your account", body, true);
                            if (IsSendEmail)
                                TempData["msg"] = "Congratulations! A verification link has been sent to your registered email id, please verify it.";
                            else
                                TempData["msg"] = "OOPs! Something went wrong. Very soon you will get a verificaion link in mail.";
                            HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                        }
                        else
                        {
                            var stateList = dbContext.MyTutor_StateList.ToList();
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
                            ViewBag.ErrorMsg = "Please select profile pic.";
                            return View(model);
                        }
                    }
                    else
                    {
                        var stateList = dbContext.MyTutor_StateList.ToList();
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
                        ViewBag.ErrorMsg = "Email Id is already exists!";
                        return View(model);
                    }
                }
                else
                {
                    var stateList = dbContext.MyTutor_StateList.ToList();
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
                    ViewBag.ErrorMsg = "Model is not validated.";
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return RedirectToAction("Register");
        }

        private string UploadedFile(RegisterModel model)
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

        [HttpPost]
        public JsonResult CityList(int StateId)
        {
            var cityDetail = new List<CityLst>();
            var cityListData = dbContext.MyTutor_CityList.Where(x => x.StateId == StateId).OrderBy(x => x.CityName).ToList();
            foreach (var item in cityListData)
            {
                var cities = new CityLst()
                {
                    CityId = item.CityId,
                    CityName = item.CityName
                };
                cityDetail.Add(cities);
            }
            return Json(cityDetail, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult AreaList(int CityId)
        {
            var areaDetail = new List<AreaLst>();
            var areaListData = dbContext.MyTutor_AreaList.Where(x => x.CityId == CityId).OrderBy(x => x.AreaName).ToList();
            foreach (var item in areaListData)
            {
                var areas = new AreaLst()
                {
                    AreaId = item.AreaId,
                    AreaName = item.AreaName
                };
                areaDetail.Add(areas);
            }
            return Json(areaDetail, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ValidateMe(string id, string code)
        {
            int uId = Convert.ToInt32(id);
            var validate = dbContext.MyTutor_Registration.Where(x => x.Id == uId
                           && x.VerificationCode == code).FirstOrDefault();
            if (validate != null)
            {
                validate.IsValidated = true;
                validate.IsAccountSuspended = false;
                dbContext.SaveChanges();
                ViewBag.Msg = "You have successfully validated. Signin and enjoy!";
            }
            else
            {
                ViewBag.Msg = "We are not able to validate you.";
            }
            return View();
        }
    }
}
