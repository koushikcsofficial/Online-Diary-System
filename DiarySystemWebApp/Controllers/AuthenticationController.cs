using DiarySystemWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace DiarySystemWebApp.Controllers
{
    public class AuthenticationController : Controller
    {
        BusinessLogics businessLogics = new BusinessLogics();

        [HttpGet]
        public ActionResult Login()
        {
            if (User.Identity.IsAuthenticated && !String.IsNullOrEmpty(Session["LoginEmail"].ToString()))
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string email, string password)
        {
            var result = businessLogics.Login(email, password);
            if (result != null)
            {
                FormsAuthentication.SetAuthCookie(result.User_Email.ToString(), false);
                Session["LoginEmail"] = result.User_Email;
                Session["UserName"] = result.User_FirstName+" "+ result.User_LastName;
                if (result.Account_IsOfficial == 1)
                {
                    Session["AccountType"] = "officialuser";
                }
                else
                {
                    Session["AccountType"] = "user";
                }

                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.ErrorMsg = "No account found with the credential provided";
                return View();
            }
        }

        [HttpGet]
        public ActionResult Register()
        {
            if (User.Identity.IsAuthenticated && !String.IsNullOrEmpty(Session["LoginEmail"].ToString()))
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(string firstName, string lastName, string email, string password, string address, string mobile, int? secretCode)
        {
            //Except secretCode all other fields are mandetory

            //if secretCode matches with "7412020" then the account will be remarked as official account

            int registerUserResult = businessLogics.registerUser(firstName, lastName, email, password, address, mobile, secretCode);

            if (registerUserResult == 1)
            {
                //User Created successfully. Redirect to Login
                TempData["SuccessMsg"] = "Successfully created account. Please login here.";
                return RedirectToAction("Login");
            }
            if(registerUserResult == 0)
            {
                ViewBag.ErrorMsg = "Mandetory Fields can't be left empty.";
                return View();
            }
            if(registerUserResult == 2)
            {
                TempData["ErrorMsg"] = "Email id is already in use. Kindly login here.";
                return RedirectToAction("Login");
            }
            if (registerUserResult == 4)
            {
                ViewBag.ErrorMsg = "Improper user input given. Correct the input data and try again.";
                return View();
            }

            ViewBag.ErrorMsg = "Internal Error Occured. Try again!";
            return View();
        }

        [HttpGet]
        public ActionResult Logout()
        {
            if (User.Identity.IsAuthenticated)
            {
                FormsAuthentication.SignOut();
                Session.Abandon();
                return RedirectToAction("Login");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }
    }
}