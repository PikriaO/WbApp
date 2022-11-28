using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;

namespace WbApp.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
       
        public ActionResult Operators()
        {
            return View("~/Views/User/Operators.cshtml");
        }
 
    }
}
