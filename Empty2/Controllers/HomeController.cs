using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Web;
using Microsoft.Owin.Security;


namespace Empty2.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(FormCollection collection)
        {
            var store = new UserStore<IdentityUser>();
            var manager = new UserManager<IdentityUser>(store);

            var user = new IdentityUser() { UserName = collection["UserName"] };
            var result = manager.Create(user, collection["Password"]);
            
            if (result.Succeeded)
            {
                var authManager = System.Web.HttpContext.Current.GetOwinContext().Authentication;
                var cIdentity = manager.CreateIdentity(user,DefaultAuthenticationTypes.ApplicationCookie);
                authManager.SignIn(new AuthenticationProperties(),cIdentity);

                ViewBag.Message = "Registration Successfull!";
            }
            else
            {
                ViewBag.Error = "Error in registration.";
            }

            return View("RegisterSuccess");
        }


        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(FormCollection collection)
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.authType = User.Identity.AuthenticationType;
                ViewBag.name = User.Identity.Name;
                ViewBag.auth = User.Identity.IsAuthenticated;
            }
            else
            {
                var store = new UserStore<IdentityUser>();
                var manager = new UserManager<IdentityUser>(store);

                var user = manager.Find(collection["UserName"], collection["Password"]);

                if (user != null)
                {
                    var authManager = System.Web.HttpContext.Current.GetOwinContext().Authentication;
                    var cIdentity = manager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);
                    authManager.SignIn(new AuthenticationProperties(), cIdentity);
                    ViewBag.authType = User.Identity.AuthenticationType;
                    ViewBag.name = User.Identity.Name;
                    ViewBag.auth = User.Identity.IsAuthenticated;
                    return View("PrivatePage");
                }
            }
            ViewBag.ErrorLogin = "Log In failed";
            return View();     
        }

        [Authorize]
        public ActionResult PrivatePage()
        {
            return View();
        }

        public ActionResult LogOut()
        {
            var authManager = System.Web.HttpContext.Current.GetOwinContext().Authentication;
            authManager.SignOut();
            return RedirectToAction("Index");
        }
    }
}