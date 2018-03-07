using AgriSystemCore_Service.Domain;
using AgriSystemCore_Service.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AgriSystemCore_Service.Service
{
    public class RawDataService : BaseService
    {
        public RawDataService(string dbConn) : base(dbConn)
        {

        }

        public void Add(RawData param)
        {
            try
            {
                var col = db.GetCollection<RawData>(DatabaseName.RawData);
                param.CreateDatetime = DateTime.Now;
                param.CD = param.CreateDatetime.ToString("yyyy-MM-dd HH:mm:ss");
                col.Insert(param);
            }
            catch
            {
                throw;
            }
        }

        public void Del(int id)
        {
            try
            {
                var col = db.GetCollection<RawData>(DatabaseName.RawData);
                col.Delete(x => x.Id == id);
            }
            catch
            {
                throw;
            }
        }

        public void Update(RawData param)
        {
            try
            {
                var col = db.GetCollection<RawData>(DatabaseName.RawData);
                var target = col.FindById(param.Id);
                target.Name = param.Name;
                target.Data = param.Data;

                col.Update(target);

            }
            catch
            {
                throw;
            }
        }

        public RawData Get(int id)
        {
            try
            {
                var col = db.GetCollection<RawData>(DatabaseName.RawData);
                return col.FindById(id);
            }
            catch
            {
                throw;
            }
        }

        public SearchRawDataResult Search(SearchRawDataParameter param)
        {
            try
            {
                SearchRawDataResult result = new SearchRawDataResult();

                var temp = db.GetCollection<RawData>(DatabaseName.RawData).FindAll().AsQueryable();

                if (!string.IsNullOrWhiteSpace(param.Name))
                {
                    temp = temp.Where(x => x.Name.Contains(param.Name));
                }

                result.totalCount = temp.Count();

                if (param.SortColumn != null)
                {
                    temp = param.Order == SearchParameters.OrderBehavior.ASC ? EntityOrderHelper.OrderBy(temp, param.SortColumn) : EntityOrderHelper.OrderByDescending(temp, param.SortColumn);
                }

                temp = EntityPagingHelper<RawData>.Paging(temp, param.PageSize, param.Index);

                result.ListData = temp.ToList();

                return result;
            }
            catch
            {
                throw;
            }
        }

    }
}