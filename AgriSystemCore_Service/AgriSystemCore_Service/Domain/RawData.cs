using AgriSystemCore_Service.Utility;
using System;
using System.Collections.Generic;

namespace AgriSystemCore_Service.Domain
{
    /// <summary>
    /// RawData 指的是從「總成(Assembly)」所收到的原始資料
    /// </summary>
    public class RawData
    {
        /// <summary>
        /// 流水號，由 LiteDB 自動產生
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// 總成名稱，注意！這邊的總成名稱，必須和系統內設定的一樣才能由API或後台圖示查找
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 資料收集時間(Server Time)，這邊一律使用 Server 時間，避免各總成間還要較正時間
        /// </summary>
        public DateTime CreateDatetime { get; set; }
        /// <summary>
        /// CreateDatetime format 成 yyyy-MM-dd HH:mm:ss
        /// </summary>
        public string CD { get; set; }
        /// <summary>
        /// 收進到的資料
        /// </summary>
        public List<string> Data { get; set; }
    }

    public class SearchRawDataParameter : SearchParameters
    {
        public string Name { get; set; }
    }

    public class SearchRawDataResult : SearchResult
    {
        public List<RawData> ListData { get; set; }
    }
}
