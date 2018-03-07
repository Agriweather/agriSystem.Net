using System;
using System.Collections.Generic;

namespace AgriSystemCore_Service.Domain
{
    public class Menu
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public List<MenuItem> Child { get; set; }
    }

    public class MenuItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
        public string Uri { get; set; }
    }
}
