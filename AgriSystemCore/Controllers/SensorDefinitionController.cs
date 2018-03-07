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

namespace AgriSystemCore.Controllers
{
    [Authorize(Policy = "SensorDefinitionPolicy")]
    public class SensorDefinitionController : BaseController
    {
        public SensorDefinitionController(IHostingEnvironment env, IOptions<AgriOptions> ao) : base(env, ao)
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
            public string Regex { get; set; }
            public string Note { get; set; }
            public bool IsDefaultDefinition { get; set; }
        }

        [HttpPost]
        public JsonResult Index(SearchRequest request)
        {
            using (var service = new SensorDefinitionService(this._dbPath))
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
                    case 2:
                        SortColumn = "Id"; //Regex
                        break;
                    default:
                        SortColumn = "Id";
                        break;
                }

                SearchSensorDefinitionParameter param = new SearchSensorDefinitionParameter()
                {
                    Index = request.start,
                    PageSize = request.length,
                    Order = request.order.First().dir.ToLower() == "asc" ? SearchParameters.OrderBehavior.ASC : SearchParameters.OrderBehavior.DESC,
                    SortColumn = SortColumn,
                    Name = request.Name
                };

                SearchSensorDefinitionResult result = service.Search(param);
                List<SearchTableView> ListTableView = new List<SearchTableView>();

                foreach (var i in result.ListData)
                {
                    ListTableView.Add(new SearchTableView()
                    {
                        Id = i.Id,
                        Name = i.Name,
                        Regex = i.Regex,
                        Note = i.Note,
                        IsDefaultDefinition = i.IsDefaultDefinition
                    });
                }

                return Json(new JDataTableResponse<SearchTableView>(request.draw, ListTableView, result.totalCount));
            }

        }

        [ActionName("SensorDefinition")]
        [HttpDelete]
        public JsonResult DelSensorDefinition(int id)
        {
            try
            {
                using (var db = new SensorDefinitionService(this._dbPath))
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
            return View();
        }

        [ActionName("SensorDefinition")]
        [HttpPost]
        public JsonResult CreateSensorDefinition([FromBody] SensorDefinition param)
        {
            try
            {
                int id = 0;
                using (var service = new SensorDefinitionService(this._dbPath))
                {
                    param.IsDefaultDefinition = false;
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
            SensorDefinition model = new SensorDefinition();
            using (var service = new SensorDefinitionService(this._dbPath))
            {
                model = service.Get(id);
            }

            return View(model);
        }

        [ActionName("SensorDefinition")]
        [HttpPut]
        public JsonResult EditSensorDefinition([FromBody] SensorDefinition param)
        {
            try
            {
                using (var service = new SensorDefinitionService(this._dbPath))
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

        #endregion
    }
}