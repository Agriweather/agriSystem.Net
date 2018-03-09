using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgriSystemCore.Controllers
{
    [Authorize(Policy = "DocumentPolicy")]
    public class DocumentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}