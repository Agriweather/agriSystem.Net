using AgriSystemCore_Service.Domain;
using AgriSystemCore_Service.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AgriSystemCore_Service.Service
{
    public class ManagerService : BaseService
    {
        public ManagerService(string dbConn) : base(dbConn)
        {

        }

        public int Add(Manager param)
        {
            try
            {
                int result = 0;

                var col = db.GetCollection<Manager>(DatabaseName.Manager);

                if (col.Exists(x => x.Name == param.Name))
                {
                    throw new Exception("帳號重覆！");
                }

                col.Insert(param);
                result = col.FindOne(x => x.Name == param.Name).Id;

                return result;
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
                var col = db.GetCollection<Manager>(DatabaseName.Manager);
                col.Delete(x => x.Id == id);
            }
            catch
            {
                throw;
            }
        }

        public Manager Get(int id)
        {
            try
            {
                var col = db.GetCollection<Manager>(DatabaseName.Manager);
                return col.FindById(id);
            }
            catch
            {
                throw;
            }
        }

        public SearchManagerResult Search(SearchManagerParameter param)
        {
            try
            {
                SearchManagerResult result = new SearchManagerResult();

                var temp = db.GetCollection<Manager>(DatabaseName.Manager).FindAll().AsQueryable();

                if (!string.IsNullOrWhiteSpace(param.Name))
                {
                    temp = temp.Where(x => x.Name.Contains(param.Name));
                }

                result.totalCount = temp.Count();

                if (param.SortColumn != null)
                {
                    temp = param.Order == SearchParameters.OrderBehavior.ASC ? EntityOrderHelper.OrderBy(temp, param.SortColumn) : EntityOrderHelper.OrderByDescending(temp, param.SortColumn);
                }

                temp = EntityPagingHelper<Manager>.Paging(temp, param.PageSize, param.Index);

                result.ListData = temp.ToList();

                return result;
            }
            catch
            {
                throw;
            }
        }

        public void Update(Manager param)
        {
            try
            {
                var col = db.GetCollection<Manager>(DatabaseName.Manager);
                if (col.Exists(x => x.Name == param.Name && x.Id != param.Id))
                {
                    throw new Exception("帳號重覆！");
                }

                var target = col.FindById(param.Id);
                target.Name = param.Name;

                if (!string.IsNullOrWhiteSpace(param.Password))
                {
                    target.Password = param.Password;
                }

                target.Auth = param.Auth;

                col.Update(target);

            }
            catch
            {
                throw;
            }
        }

    }
}
