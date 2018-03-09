using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AgriSystemCore.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace AgriSystemCore.Controllers
{
    public abstract class BaseController : Controller
    {
        protected IHostingEnvironment _env;
        protected AgriOptions _ao;
        protected string _dbPath;

        public BaseController(IHostingEnvironment env, IOptions<AgriOptions> ao)
        {
            this._env = env;
            this._ao = ao.Value;
            this._dbPath = this._ao.LiteDB_Conn.Replace("{envPath}", this._env.ContentRootPath);
        }

    }

    /// <summary>
    /// Support JQuery DataTable plug-in for request
    /// </summary>
    public class JDataTableRequest
    {
        //--Client端在每次呼叫sAjaxSource時會產生一個特定的draw做為驗証碼，Server端必需在JSON中回傳相同的值做為認證 
        public int draw { get; set; }
        //--目前顯示的是第幾筆資料 
        public int start { get; set; }
        //--畫面上單次顯示的資料筆數
        public int length { get; set; }
        //--order
        public List<JDataTableOrderItem> order { get; set; }
    }

    public class JDataTableOrderItem
    {
        public int column { get; set; }
        public string dir { get; set; }
    }

    /// <summary>
    /// Support JQuery DataTable plug-in for repsponse
    /// </summary>
    public class JDataTableResponse<T>
    {
        //--Client送來的驗証碼
        public int draw { get; set; }

        //--未經過濾的資料總筆數
        public int recordsTotal { get; set; }

        //--經過過濾的資料總筆數 
        public int recordsFiltered { get; set; }

        //--回傳的資料 
        public List<T> data { get; set; }

        public JDataTableResponse(int draw, List<T> data, int total)
        {
            this.draw = draw;
            this.data = data;
            this.recordsTotal = total;
            this.recordsFiltered = total;
        }
    }
}

public static class SessionExtensions
{
    public static void Set<T>(this ISession session, string key, T value)
    {
        session.SetString(key, JsonConvert.SerializeObject(value));
    }

    public static T Get<T>(this ISession session, string key)
    {
        var value = session.GetString(key);
        return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
    }
}