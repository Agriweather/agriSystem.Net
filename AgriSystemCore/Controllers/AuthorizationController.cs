using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AgriSystemCore.Models;
using AgriSystemCore_Service.Domain;
using AgriSystemCore_Service.Service;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AgriSystemCore.Controllers
{
    public class AuthorizationController : BaseController
    {
        public AuthorizationController(IHostingEnvironment env, IOptions<AgriOptions> ao) : base(env, ao)
        {

        }

        public IActionResult Index()
        {
            //--在登入頁面時，就先確認目前的DB是否存在，如果不存在就新增並Init好
            using (AuthorizationService service = new AuthorizationService(this._dbPath))
            {
                string functionMenu = System.IO.File.ReadAllText(this._env.ContentRootPath + "/Data/FunctionMenu.json");
                string defaultSensor = System.IO.File.ReadAllText(this._env.ContentRootPath + "/Data/DefaultSensor.json");

                service.CreateAdmin(functionMenu, defaultSensor);
            }

            HttpContext.SignOutAsync();

            return View();
        }

        [HttpPost]
        public async Task<JsonResult> Login(string name, string password, string returnUrl)
        {
            string msg = "";

            try
            {
                using (AuthorizationService service = new AuthorizationService(this._dbPath))
                {
                    Manager manager;

                    if (service.Login(name, password, out manager))
                    {
                        List<Claim> claims = new List<Claim>();
                        claims.Add(new Claim(ClaimTypes.Name, name));

                        foreach (var i in manager.Auth)
                        {
                            if (i.Child != null && i.Child.Count() > 0)
                            {
                                foreach (var c in i.Child)
                                {
                                    claims.Add(new Claim(ClaimTypes.Role, c.Role));
                                }
                            }
                        }

                        ClaimsIdentity identity = new ClaimsIdentity(claims, "cookie");

                        ClaimsPrincipal principal = new ClaimsPrincipal(identity);

                        await HttpContext.SignInAsync(
                            scheme: "AgriSecurityScheme",
                            principal: principal);

                        HttpContext.Session.Set<Manager>("ManagerInfo", manager);

                        string url = Url.Action("Index", "Default");

                        if (!string.IsNullOrWhiteSpace(returnUrl))
                        {
                            url = WebUtility.UrlDecode(returnUrl);
                        }

                        return Json(new { success = true, url });
                    }
                    else
                    {
                        msg = "帳號或密碼錯誤！";
                        throw new Exception(msg);
                    }
                }

            }
            catch (Exception ex)
            {
                return Json(new { success = false, msg = string.IsNullOrWhiteSpace(msg) ? "系統錯誤！" : msg });
            }
        }

        [Authorize]
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [Authorize]
        public JsonResult ChangePassword(string oldPwd, string newPwd)
        {
            try
            {
                Manager managerInfo = SessionExtensions.Get<Manager>(HttpContext.Session, "ManagerInfo");

                if (managerInfo.Password != oldPwd)
                {
                    throw new Exception("舊密碼錯誤！");
                }

                if (string.IsNullOrWhiteSpace(newPwd))
                {
                    throw new Exception("新密碼空白！");
                }

                using (var service = new ManagerService(this._dbPath))
                {
                    managerInfo.Password = newPwd.Trim();
                    service.Update(managerInfo);
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, msg = ex.Message });
            }
        }
    }
}