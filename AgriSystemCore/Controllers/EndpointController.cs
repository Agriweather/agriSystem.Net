using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AgriSystemCore.Models;
using AgriSystemCore_Service.Domain;
using AgriSystemCore_Service.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using static AgriSystemCore_Service.Service.EndpointService;

namespace AgriSystemCore.Controllers
{
    public class EndpointController : BaseController
    {
        public EndpointController(IHostingEnvironment env, IOptions<AgriOptions> ao) : base(env, ao)
        {

        }

        [HttpGet]
        public IActionResult TestCase()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Index(RawData request)
        {
            try
            {
                using (var service = new RawDataService(this._dbPath))
                {
                    service.Add(request);
                }

                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }
        }

        [Authorize]
        [HttpPost]
        public JsonResult GetAllAssembly()
        {
            try
            {
                List<Assembly> data = new List<Assembly>();

                using (AssemblyService service = new AssemblyService(this._dbPath))
                {
                    data = service.GetAll();
                }

                return Json(new { success = true, data });
            }
            catch
            {
                return Json(new { success = false });
            }
        }
        [Authorize]
        [HttpPost]
        public JsonResult Monitor(string assembly, int limit = 10)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(assembly))
                {
                    throw new Exception("Assembly is null or white space!!");
                }

                EndpointDataitem data = new EndpointDataitem();

                using (EndpointService service = new EndpointService(this._dbPath))
                {
                    data = service.Monitor(assembly, limit);
                }

                return Json(new { success = true, data });
            }
            catch
            {
                return Json(new { success = false });
            }
        }
        [Authorize]
        [HttpPost]
        public JsonResult History(string assembly, DateTime date, int day = 1, int frequency = 3600)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(assembly))
                {
                    throw new Exception("Assembly is null or white space!!");
                }

                EndpointDataitem data = new EndpointDataitem();

                using (EndpointService service = new EndpointService(this._dbPath))
                {
                    data = service.History(assembly, date, day, frequency);
                }

                return Json(new { success = true, data });
            }
            catch
            {
                return Json(new { success = false });
            }
        }
    }
}