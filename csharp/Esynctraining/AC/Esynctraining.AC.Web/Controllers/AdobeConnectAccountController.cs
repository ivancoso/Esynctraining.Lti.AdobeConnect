namespace eSyncTraining.Web.Controllers
{
    using System.Web.Mvc;

    using Esynctraining.AC.Provider.DataObjects;
    using eSyncTraining.Web.Models;

    public class AdobeConnectAccountController : AdobeConnectedControllerBase
    {
        //
        // GET: /Account/Login
        public ActionResult Login(string returnUrl)
        {
            this.ViewBag.ReturnUrl = returnUrl;
            return this.View();
        }

        //
        // POST: /Account/Login

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(AdobeConnectLoginModel model, string returnUrl)
        {
            if (this.ModelState.IsValid)
            {
                var loginResult = this.AdobeConnect.Login(new UserCredentials(model.Login, model.Password));
                
                if (loginResult.Success)
                {
                    if (loginResult.Status != null)
                    {
                        this.CreateSessionCookie(loginResult.Status.SessionInfo);                        
                    }

                    if (loginResult.User != null)
                    {
                        Database.Write(loginResult.User);
                    }

                    return this.RedirectToLocal(returnUrl);
                }
            }

            // If we got this far, something failed, redisplay form
            this.ModelState.AddModelError("", "The user name or password provided is incorrect.");
            return this.View(model);
        }

        //
        // POST: /Account/LogOff

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            this.AdobeConnect.Logout();

            this.DeleteSessionCookie();

            return this.RedirectToAction("Index", "Home");
        }
    }
}
