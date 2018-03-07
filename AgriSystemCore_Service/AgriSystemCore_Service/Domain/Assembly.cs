using AgriSystemCore_Service.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AgriSystemCore_Service.Domain
{
    public class Assembly
    {
        public int Id { get; set; }
        /// <summary>
        /// 總成(Assembly)名稱，一個「總成」包含很多實際意義上的硬體 Sensor
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 總成的備註
        /// </summary>
        public string Note { get; set; }
        /// <summary>
        /// 總成所包含的 Sensor
        /// </summary>
        public List<int> Sensors { get; set; }
    }

    public class SensorDefinition
    {
        public int Id { get; set; }
        /// <summary>
        /// 定義名稱，ex.深層溼度、空氣溫度、電壓…
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 正則式，可空白，若空白就是不驗證
        /// </summary>
        public string Regex { get; set; }
        /// <summary>
        /// 使用者自行備註
        /// </summary>
        public string Note { get; set; }
        /// <summary>
        /// 可自定在後台使用時的 icon
        /// </summary>
        public string CssClass { get; set; }
        /// <summary>
        /// 是否為系統內建的數值
        /// </summary>
        public bool IsDefaultDefinition { get; set; }
    }

    public class SearchSensorDefinitionParameter : SearchParameters
    {
        public string Name { get; set; }
    }

    public class SearchSensorDefinitionResult : SearchResult
    {
        public List<SensorDefinition> ListData { get; set; }
    }

    public class SearchAssemblyParameter : SearchParameters
    {
        public string Name { get; set; }
    }

    public class SearchAssemblyResult : SearchResult
    {
        public List<Assembly> ListData { get; set; }
    }
}