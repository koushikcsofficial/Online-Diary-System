using DiarySystemWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DiarySystemWebApp.Controllers
{
    [Authorize]
    public class DiaryController : Controller
    {
        BusinessLogics businessLogics = new BusinessLogics();

        [HttpGet]
        public ActionResult Index()
        {
            if (Session["LoginEmail"] == null)
            {
                return RedirectToAction("Logout", "Authentication");
            }
            //return List of Diaries registered by the logged in user
            ViewData["Diaries"] = businessLogics.getAllDiaries(Session["LoginEmail"].ToString());
            return View();
        }

        [HttpGet]
        public ActionResult OfficialIndex()
        {
            if (Session["LoginEmail"] == null || Session["AccountType"].ToString() != "officialuser")
            {
                return RedirectToAction("Logout", "Authentication");
            }
            //return List of Diaries registered by the logged in user
            ViewData["Diaries"] = businessLogics.GetPendingDiaries(Session["LoginEmail"].ToString());
            return View();
        }

        [HttpGet]
        public ActionResult ShowDiary(Guid diary_Id)
        {
            if (Session["LoginEmail"] == null)
            {
                return RedirectToAction("Logout", "Authentication");
            }
            //return List of Diaries registered by the logged in user
            var data = businessLogics.getDiary(diary_Id);
            if (data != null)
            {
                ViewBag.Data = data;

                if (Session["AccountType"].ToString() == "user" && businessLogics.ifThisAccountRegisteredThisDiary(Session["LoginEmail"].ToString(), diary_Id))
                {
                    return View();
                }
                else if(Session["AccountType"].ToString() == "officialuser")
                {
                    return View();
                }
                else
                {
                    TempData["ErrorMsg"] = "You can't see this diary";
                    return RedirectToAction("Index", "Diary");
                }
            }
            else
            {
                TempData["ErrorMsg"] = "The diary you are looking for is invalid";
                return RedirectToAction("Index", "Diary");
            }
        }

        [HttpGet]
        public ActionResult CreateDiary()
        {
            if (Session["LoginEmail"] == null)
            {
                return RedirectToAction("Logout", "Authentication");
            }
            //if account id from session is valid then only diary can be registered
            if (businessLogics.getAccountId(Session["LoginEmail"].ToString()) != null)
            {
                return View();
            }
            else
            {
                TempData["ErrorMsg"] = "Error Occured while processing request";
                return RedirectToAction("Index", "Diary");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateDiary(string diary_subject, string diary_content)
        {
            // As get request validates the test cases. just post the details here
            int registerDiaryResult = businessLogics.registerDiary(Session["LoginEmail"].ToString().ToLower(), diary_subject, diary_content);
            if (registerDiaryResult == 1)
            {
                return RedirectToAction("Index", "Diary");
            }
            if (registerDiaryResult == 0)
            {
                ViewBag.ErrorMsg = "Mandetory fields can't be left empty.";
            }
            if (registerDiaryResult == 2)
            {
                ViewBag.ErrorMsg = "Internal error occured while registering the diary.";
            }
            return View();
        }

        [HttpGet]
        public ActionResult UpdateDiaryByUser(Guid diary_Id)
        {
            if (Session["LoginEmail"] == null)
            {
                return RedirectToAction("Logout", "Authentication");
            }
            //User can update the diary body (diary_subject, diary_content)

            //official can update the status of any diary (accept/reject)

            //if logged in user is official and registered diary by his own, then any official other than he can update the status.

            //if logged in user is official and registered diary by his own, then any official other than he can update the diary_body.
            var data = businessLogics.getDiary(diary_Id);
            if (data != null)
            {
                ViewBag.Data = data;

                if((Session["AccountType"].ToString()== "user" || Session["AccountType"].ToString() == "officialuser") && businessLogics.ifThisAccountRegisteredThisDiary(Session["LoginEmail"].ToString(),diary_Id) )
                {
                    if (data.Diary_IsAccepted == 1)
                    {
                        TempData["ErrorMsg"] = "Diary already accepted. Details can't be changed";
                        return RedirectToAction("Index", "Diary");
                    }else if(data.Diary_IsAccepted == 2)
                    {
                        return View("ChangeDiaryBody");
                    }
                    else
                    {
                        TempData["ErrorMsg"] = "Diary has been rejected. Details can't be changed";
                        return RedirectToAction("Index", "Diary");
                    }
                }

                if(Session["AccountType"].ToString() == "user" && !businessLogics.ifThisAccountRegisteredThisDiary(Session["LoginEmail"].ToString(), diary_Id))
                {
                    TempData["ErrorMsg"] = "You are trying to update an diary that doesn't belongs to you";
                    return RedirectToAction("Index", "Diary");
                }

                if(Session["AccountType"].ToString() == "officialuser" && !businessLogics.ifThisAccountRegisteredThisDiary(Session["LoginEmail"].ToString(), diary_Id))
                {
                    //Official person will get option in this page to accept or deny the diary
                    return View("SingleDiaryShowPage");
                }
            }
            else
            {
                TempData["ErrorMsg"] = "The diary you are looking for is invalid";
                return RedirectToAction("Index", "Diary");
            }
            return View("ErrorUpdatePage");
        }

        [HttpGet]
        public ActionResult UpdateDiaryByOfficial(Guid diary_Id,int? option)
        {
            if (Session["LoginEmail"] == null)
            {
                return RedirectToAction("Logout", "Authentication");
            }

            if (Session["AccountType"].ToString() == "officialuser" && !businessLogics.ifThisAccountRegisteredThisDiary(Session["LoginEmail"].ToString(), diary_Id))
            {
                if(option!=null && (option == 0 || option == 1 || option == 2))
                {
                    int resultUpdatingDiary = businessLogics.updateDiaryByOfficial(Session["LoginEmail"].ToString(), diary_Id, option.Value);
                    if (resultUpdatingDiary == 1)
                    {
                        return RedirectToAction("OfficialIndex", "Diary");
                    }
                    else
                    {
                        TempData["ErrorMsg"] = "Internal Error Occured.";
                        return RedirectToAction("Index", "Diary");
                    }
                }
                else
                {
                    TempData["ErrorMsg"] = "Updation faild. incorrect value passed";
                    return RedirectToAction("Index", "Diary");
                }
            }
            else
            {
                TempData["ErrorMsg"] = "You are unauthorized to update this detail.";
                return RedirectToAction("Index", "Diary");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateDiary(Guid diary_id, string diary_subject, string diary_content)
        {
            // As get request validates the test cases. just post the details here.
            if (!String.IsNullOrWhiteSpace(diary_subject) && !String.IsNullOrWhiteSpace(diary_content))
            {
                int resultUpdatingDiary = businessLogics.updateDiaryByUser(diary_id, diary_subject, diary_content);
                if (resultUpdatingDiary == 1)
                {
                    return RedirectToAction("Index", "Diary");
                }
                else
                {
                    ViewBag.ErrorMsg = "Internal Error Occured";
                }
            }
            else
            {
                ViewBag.ErrorMsg = "Mandetory fields can't be left empty.";
            }
            return View();
        }

        [HttpGet]
        public ActionResult DeleteDiary(Guid diary_Id)
        {
            if (Session["LoginEmail"] == null)
            {
                return RedirectToAction("Logout", "Authentication");
            }
            //if diary_Id is valid and belongs to the logged in user and not (viewed or accepted) then only Diary can be deleted
            var data = businessLogics.getDiary(diary_Id);
            if (data != null)
            {
                if (businessLogics.ifThisAccountRegisteredThisDiary(Session["LoginEmail"].ToString(), diary_Id) && data.Diary_IsAccepted == 2)
                {
                    int resultDeleteDiary = businessLogics.RemoveDiary(diary_Id);
                    if (resultDeleteDiary == 1)
                    {
                        return RedirectToAction("Index", "Diary");
                    }
                    else
                    {
                        TempData["ErrorMsg"] = "Internal error occured";
                        return RedirectToAction("Index", "Diary");
                    }
                }
                else
                {
                    TempData["ErrorMsg"] = "Diary can't be deleted";
                    return RedirectToAction("Index", "Diary");
                }
            }
            else
            {
                TempData["ErrorMsg"] = "Diary can't be found";
                return RedirectToAction("Index", "Diary");
            }
        }

    }
}