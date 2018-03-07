using AgriSystemCore_Service.Utility;
using System;
using System.Collections.Generic;

namespace AgriSystemCore_Service.Domain
{
    public class Manager
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public List<Menu> Auth { get; set; }
    }

    public class SearchManagerParameter : SearchParameters
    {
        public string Name { get; set; }
    }

    public class SearchManagerResult : SearchResult
    {
        public List<Manager> ListData { get; set; }
    }
}
