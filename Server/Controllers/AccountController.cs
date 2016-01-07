using System;
using System.Web.Mvc;
using System.Web.Security;
using Utilities.MVC;

namespace Server {

	public class User {

		public string Name { get; set; }
		public string Password { get; set; }

	}

	public class AccountController : BaseController {

		private readonly IAuthenticationConfiguration _configuration;

		public AccountController(IAuthenticationConfiguration configuration) {
			this._configuration = configuration;
		}

		[Trace]
		[ActionName("Login")]
		[HttpGet]
		public ActionResult Login() {
			return View();
		}

		[Trace]
		[ActionName("Login")]
		[HttpPost]
		public ActionResult Login(User model, string returnUrl) {
			bool userValid = model.Name != null && model.Name.Equals(this._configuration.UserId, StringComparison.InvariantCultureIgnoreCase) && model.Password != null && model.Password.Equals(this._configuration.Password, StringComparison.InvariantCulture);
			if(userValid) {
				FormsAuthentication.SetAuthCookie(model.Name, false);
				if(Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/") && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\")) {
					return Redirect(returnUrl);
				} 
				return RedirectToAction("Index", "Home");
			} 
			ModelState.AddModelError("", "The user name or password provided is incorrect.");
			// If we got this far, something failed, redisplay form
			return View();
		}

		[Trace]
		[ActionName("Logoff")]
		public ActionResult LogOff() {
			FormsAuthentication.SignOut();
			return RedirectToAction("Index", "Home");
		}

	}

}