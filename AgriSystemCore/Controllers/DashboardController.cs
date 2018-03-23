using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AgriSystemCore.Models;
using AgriSystemCore_Service.Domain;
using AgriSystemCore_Service.Service;
using AgriSystemCore_Service.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace AgriSystemCore.Controllers
{
    [Authorize(Policy = "DashboardPolicy")]
    public class DashboardController :  BaseController
    {
        public DashboardController(IHostingEnvironment env, IOptions<AgriOptions> ao) : base(env, ao)
        {

        }

        public IActionResult Index()
        {
            return View();
        }
    }
}