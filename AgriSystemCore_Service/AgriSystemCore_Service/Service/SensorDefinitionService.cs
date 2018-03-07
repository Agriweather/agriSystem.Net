using AgriSystemCore_Service.Domain;
using AgriSystemCore_Service.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AgriSystemCore_Service.Service
{
    public class SensorDefinitionService: BaseService
    {
        public SensorDefinitionService(string dbConn) : base(dbConn)
        {

        }

        public int Add(SensorDefinition param)
        {
            try
            {
                int result = 0;

                var col = db.GetCollection<SensorDefinition>(DatabaseName.SensorDefinition);

                if (col.Exists(x => x.Name == param.Name))
                {
                    throw new Exception("感測器名稱重覆！");
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
                //--如果刪除時，發現 Assembly 有用到，幫助使用者刪除
                var colAssembly = db.GetCollection<Assembly>(DatabaseName.Assembly);
                foreach (var i in colAssembly.FindAll().ToList())
                {
                    if (i.Sensors.Any(x => x == id))
                    {
                        i.Sensors.RemoveAll(x=>x == id);
                    }

                    colAssembly.Update(i);
                }

                var col = db.GetCollection<SensorDefinition>(DatabaseName.SensorDefinition);
                col.Delete(x => x.Id == id);
            }
            catch
            {
                throw;
            }
        }

        public SensorDefinition Get(int id)
        {
            try
            {
                var col = db.GetCollection<SensorDefinition>(DatabaseName.SensorDefinition);
                return col.FindById(id);
            }
            catch
            {
                throw;
            }
        }

        public SearchSensorDefinitionResult Search(SearchSensorDefinitionParameter param)
        {
            try
            {
                SearchSensorDefinitionResult result = new SearchSensorDefinitionResult();

                var temp = db.GetCollection<SensorDefinition>(DatabaseName.SensorDefinition).FindAll().AsQueryable();

                if (!string.IsNullOrWhiteSpace(param.Name))
                {
                    temp = temp.Where(x => x.Name.Contains(param.Name));
                }

                result.totalCount = temp.Count();

                if (param.SortColumn != null)
                {
                    temp = param.Order == SearchParameters.OrderBehavior.ASC ? EntityOrderHelper.OrderBy(temp, param.SortColumn) : EntityOrderHelper.OrderByDescending(temp, param.SortColumn);
                }

                temp = EntityPagingHelper<SensorDefinition>.Paging(temp, param.PageSize, param.Index);

                result.ListData = temp.ToList();

                return result;
            }
            catch
            {
                throw;
            }
        }

        public void Update(SensorDefinition param)
        {
            try
            {
                var col = db.GetCollection<SensorDefinition>(DatabaseName.SensorDefinition);
                if (col.Exists(x => x.Name == param.Name && x.Id != param.Id))
                {
                    throw new Exception("感測器名稱重覆！");
                }

                var target = col.FindById(param.Id);
                target.Name = param.Name;
                target.Regex = param.Regex;
                target.Note = param.Note;
                target.CssClass = param.CssClass;

                col.Update(target);

            }
            catch
            {
                throw;
            }
        }

        public List<SensorDefinition> GetAll()
        {
            try
            {
                var col = db.GetCollection<SensorDefinition>(DatabaseName.SensorDefinition);
                return col.FindAll().ToList();
            }
            catch
            {
                throw;
            }
        }
    }
}
