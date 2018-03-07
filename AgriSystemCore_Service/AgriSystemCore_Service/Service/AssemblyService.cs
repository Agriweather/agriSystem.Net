using AgriSystemCore_Service.Domain;
using AgriSystemCore_Service.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AgriSystemCore_Service.Service
{
    public class AssemblyService : BaseService
    {
        public AssemblyService(string dbConn) : base(dbConn)
        {

        }

        public int Add(Assembly param)
        {
            try
            {
                int result = 0;

                var col = db.GetCollection<Assembly>(DatabaseName.Assembly);

                if (col.Exists(x => x.Name == param.Name))
                {
                    throw new Exception("總成名稱重覆！");
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
                var col = db.GetCollection<Assembly>(DatabaseName.Assembly);
                col.Delete(x => x.Id == id);
            }
            catch
            {
                throw;
            }
        }

        public Assembly Get(int id)
        {
            try
            {
                var col = db.GetCollection<Assembly>(DatabaseName.Assembly);
                return col.FindById(id);
            }
            catch
            {
                throw;
            }
        }

        public List<Assembly> GetAll()
        {
            try
            {
                var col = db.GetCollection<Assembly>(DatabaseName.Assembly);
                return col.FindAll().ToList();
            }
            catch
            {
                throw;
            }
        }

        public void Update(Assembly param)
        {
            try
            {
                var col = db.GetCollection<Assembly>(DatabaseName.Assembly);
                if (col.Exists(x => x.Name == param.Name && x.Id != param.Id))
                {
                    throw new Exception("總成名稱重覆！");
                }

                var target = col.FindById(param.Id);
                target.Name = param.Name;
                target.Note = param.Note;
                target.Sensors = param.Sensors;

                col.Update(target);

            }
            catch
            {
                throw;
            }
        }

        public SearchAssemblyResult Search(SearchAssemblyParameter param)
        {
            try
            {
                SearchAssemblyResult result = new SearchAssemblyResult();

                var temp = db.GetCollection<Assembly>(DatabaseName.Assembly).FindAll().AsQueryable();

                if (!string.IsNullOrWhiteSpace(param.Name))
                {
                    temp = temp.Where(x => x.Name.Contains(param.Name));
                }

                result.totalCount = temp.Count();

                if (param.SortColumn != null)
                {
                    temp = param.Order == SearchParameters.OrderBehavior.ASC ? EntityOrderHelper.OrderBy(temp, param.SortColumn) : EntityOrderHelper.OrderByDescending(temp, param.SortColumn);
                }

                temp = EntityPagingHelper<Assembly>.Paging(temp, param.PageSize, param.Index);

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