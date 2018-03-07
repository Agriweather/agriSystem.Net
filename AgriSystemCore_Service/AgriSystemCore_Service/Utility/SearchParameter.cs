using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AgriSystemCore_Service.Utility
{
    /// <summary>
    /// 常用的 SearchParameters
    /// </summary>
    public abstract class SearchParameters
    {
        /// <summary>
        /// 排序方式
        /// </summary>
        public enum OrderBehavior { DESC = 0, ASC };

        private int? pageSize;

        /// <summary>
        /// 每一頁幾筆
        /// </summary>
        public int PageSize
        {
            get
            {
                if (pageSize == null)
                {
                    return int.MaxValue;
                }
                else
                {
                    return pageSize.Value;
                }
            }
            set
            {
                pageSize = value;
            }
        }

        /// <summary>
        /// 起始 index
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// 排序方式
        /// </summary>
        public OrderBehavior Order { get; set; }

        /// <summary>
        /// 排序的欄位
        /// </summary>
        public string SortColumn { get; set; }

        /// <summary>
        /// 總筆數
        /// </summary>
        public int Total { get; set; }
    }
}
