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
    [Authorize(Policy = "RawDataPolicy")]
    public class RawDataController : BaseController
    {
        public RawDataController(IHostingEnvironment env, IOptions<AgriOptions> ao) : base(env, ao)
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
            public long Id { get; set; }
            public string Name { get; set; }
            public string CreateDatetime { get; set; }
            public string Data { get; set; }
        }

        [HttpPost]
        public JsonResult Index(SearchRequest request)
        {
            using (var service = new RawDataService(this._dbPath))
            {
                string SortColumn = "";
                switch (request.order.First().column)
                {
                    case 0:
                        SortColumn = "Id"; //--其實是按鈕
                        break;
                    case 1:
                        SortColumn = "Id"; 
                        break;
                    case 2:
                        SortColumn = "Name";
                        break;
                    case 3:
                        SortColumn = "CreateDatetime";
                        break;
                    case 4:
                        SortColumn = "Id"; //--其實是Data
                        break;
                    default:
                        SortColumn = "Id";
                        break;
                }

                SearchRawDataParameter param = new SearchRawDataParameter()
                {
                    Index = request.start,
                    PageSize = request.length,
                    Order = request.order.First().dir.ToLower() == "asc" ? SearchParameters.OrderBehavior.ASC : SearchParameters.OrderBehavior.DESC,
                    SortColumn = SortColumn,
                    Name = request.Name
                };

                SearchRawDataResult result = service.Search(param);
                List<SearchTableView> ListTableView = new List<SearchTableView>();

                foreach (var i in result.ListData)
                {

                    string data = string.Join(",", i.Data);

                    ListTableView.Add(new SearchTableView()
                    {
                        Id = i.Id,
                        Name = i.Name,
                        CreateDatetime = i.CreateDatetime.ToString("yyyy-MM-dd HH:mm:ss"),
                        Data = data
                    });
                }

                return Json(new JDataTableResponse<SearchTableView>(request.draw, ListTableView, result.totalCount));
            }

        }

        [ActionName("RawData")]
        [HttpDelete]
        public JsonResult DelRawData(int id)
        {
            try
            {
                using (var db = new RawDataService(this._dbPath))
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

        [ActionName("RawData")]
        [HttpPost]
        public JsonResult CreateRawData([FromBody] RawData param)
        {
            try
            {
                using (var service = new RawDataService(this._dbPath))
                {
                    service.Add(param);
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