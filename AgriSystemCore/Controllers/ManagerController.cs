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
    [Authorize(Policy = "ManagerPolicy")]
    public class ManagerController : BaseController
    {
        public ManagerController(IHostingEnvironment env, IOptions<AgriOptions> ao) : base(env, ao)
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
            using (var service = new ManagerService(this._dbPath))
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

                SearchManagerParameter param = new SearchManagerParameter()
                {
                    Index = request.start,
                    PageSize = request.length,
                    Order = request.order.First().dir.ToLower() == "asc" ? SearchParameters.OrderBehavior.ASC : SearchParameters.OrderBehavior.DESC,
                    SortColumn = SortColumn,
                    Name = request.Name
                };

                SearchManagerResult result = service.Search(param);
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

        [ActionName("Manager")]
        [HttpDelete]
        public JsonResult DelManager(int id)
        {
            try
            {
                using (var db = new ManagerService(this._dbPath))
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

        [ActionName("Manager")]
        [HttpPost]
        public JsonResult CreateManager([FromBody] Manager param)
        {
            try
            {
                int id = 0;
                using (var service = new ManagerService(this._dbPath))
                {
                    param.Password = "";
                    param.Auth = new List<Menu>();

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

            using (var service = new ManagerService(this._dbPath))
            {
                model.Data = service.Get(id);
            }

            string json_Menu = System.IO.File.ReadAllText(this._env.ContentRootPath + "/Data/FunctionMenu.json");
            model.AllMenu = JsonConvert.DeserializeObject<List<Menu>>(json_Menu);

            return View(model);
        }

        [ActionName("Manager")]
        [HttpPut]
        public JsonResult EditManager([FromBody] Manager param)
        {
            try
            {
                using (var service = new ManagerService(this._dbPath))
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

        public class EditViewModel
        {
            public Manager Data { get; set; }
            public List<Menu> AllMenu { get; set; }
        }
    }


}