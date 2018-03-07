using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
    [Authorize(Policy = "AssemblyPolicy")]
    public class AssemblyController : BaseController
    {
        public AssemblyController(IHostingEnvironment env, IOptions<AgriOptions> ao) : base(env, ao)
        {

        }

        #region index

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        public class SearchRequest : JDataTableRequest
        {
            public string Name { get; set; }
        }

        public class SearchTableView
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        [HttpPost]
        public JsonResult Index(SearchRequest request)
        {
            using (var service = new AssemblyService(this._dbPath))
            {
                string SortColumn = "";
                switch (request.order.First().column)
                {
                    case 0:
                        SortColumn = "Id";
                        break;
                    case 1:
                        SortColumn = "Name";
                        break;
                    default:
                        SortColumn = "Id";
                        break;
                }

                SearchAssemblyParameter param = new SearchAssemblyParameter()
                {
                    Index = request.start,
                    PageSize = request.length,
                    Order = request.order.First().dir.ToLower() == "asc" ? SearchParameters.OrderBehavior.ASC : SearchParameters.OrderBehavior.DESC,
                    SortColumn = SortColumn,
                    Name = request.Name
                };

                SearchAssemblyResult result = service.Search(param);
                List<SearchTableView> ListTableView = new List<SearchTableView>();

                foreach (var i in result.ListData)
                {
                    ListTableView.Add(new SearchTableView()
                    {
                        Id = i.Id,
                        Name = i.Name
                    });
                }

                return Json(new JDataTableResponse<SearchTableView>(request.draw, ListTableView, result.totalCount));
            }

        }

        [ActionName("Assembly")]
        [HttpDelete]
        public JsonResult DelAssembly(int id)
        {
            try
            {
                using (var db = new AssemblyService(this._dbPath))
                {
                    db.Del(id);
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, msg = ex.Message });
            }
        }


        #endregion

        #region create

        [HttpGet]
        public IActionResult Create()
        {
            EditViewModel model = new EditViewModel();

            using (SensorDefinitionService service = new SensorDefinitionService(this._dbPath))
            {
                model.AllSensorDefinition = service.GetAll();
            }

            return View(model);
        }

        [ActionName("Assembly")]
        [HttpPost]
        public JsonResult CreateAssembly([FromBody] Assembly param)
        {
            try
            {
                //--總成名必須是數字、英文或是下底線
                if (!Regex.IsMatch(param.Name, @"^\w+$"))
                {
                    throw new Exception("總成名必須是數字、英文或是下底線!");
                }

                int id = 0;
                using (var service = new AssemblyService(this._dbPath))
                {
                    id = service.Add(param);
                }

                return Json(new { success = true, id = id });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, msg = ex.Message });
            }
        }

        #endregion

        #region edit

        public IActionResult Edit(int id)
        {
            EditViewModel model = new EditViewModel();

            using (var service = new AssemblyService(this._dbPath))
            {
                model.Data = service.Get(id);
            }

            using (var service = new SensorDefinitionService(this._dbPath))
            {
                model.AllSensorDefinition = service.GetAll();
            }

            return View(model);
        }

        [ActionName("Assembly")]
        [HttpPut]
        public JsonResult EditAssembly([FromBody] Assembly param)
        {
            try
            {
                //--總成名必須是數字、英文或是下底線
                if (!Regex.IsMatch(param.Name, @"^\w+$"))
                {
                    throw new Exception("總成名必須是數字、英文或是下底線!");
                }

                using (var service = new AssemblyService(this._dbPath))
                {
                    service.Update(param);
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, msg = ex.Message });
            }
        }

        public class EditViewModel
        {
            public Assembly Data { get; set; }
            public List<SensorDefinition> AllSensorDefinition { get; set; }
        }

        #endregion
    }
}