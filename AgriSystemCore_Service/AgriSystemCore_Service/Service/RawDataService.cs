using AgriSystemCore_Service.Domain;
using AgriSystemCore_Service.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

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

                //--這邊其實寫的不好，應該降低使用 FindAll 才對
                var temp = db.GetCollection<RawData>(DatabaseName.RawData).FindAll().AsQueryable();

                if (!string.IsNullOrWhiteSpace(param.Name))
                {
                    var colAssembly = db.GetCollection<Assembly>(DatabaseName.Assembly);

                    var targetAssembly = colAssembly.FindOne(x => x.Name == param.Name);

                    if (!colAssembly.Exists(x => x.Name == param.Name))
                    {
                        throw new Exception("此Assembly name不存在！");
                    }

                    var colSensorDefinition = db.GetCollection<SensorDefinition>(DatabaseName.SensorDefinition);

                    List<SensorDefinition> listSensorDefinition = new List<SensorDefinition>();
                    foreach (int i in targetAssembly.Sensors)
                    {
                        if (colSensorDefinition.Exists(x => x.Id == i))
                        {
                            listSensorDefinition.Add(colSensorDefinition.FindById(i));
                        }
                    }

                    temp = temp.Where(x => x.Name == param.Name && check(x, listSensorDefinition));
                }

                if (param.Start != null)
                {
                    temp = temp.Where(x=>x.CreateDatetime >= param.Start);
                }

                if (param.End != null)
                {
                    DateTime t = param.End.Value.AddDays(1).AddSeconds(-1);
                    temp = temp.Where(x=>x.CreateDatetime <= t);
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

        public List<RawData> ExportData(SearchRawDataParameter param)
        {
            try
            {
                SearchRawDataResult result = new SearchRawDataResult();

                //--這邊其實寫的不好，應該降低使用 FindAll 才對
                var temp = db.GetCollection<RawData>(DatabaseName.RawData).FindAll().AsQueryable();

                if (!string.IsNullOrWhiteSpace(param.Name))
                {
                    var colAssembly = db.GetCollection<Assembly>(DatabaseName.Assembly);

                    var targetAssembly = colAssembly.FindOne(x => x.Name == param.Name);

                    if (!colAssembly.Exists(x => x.Name == param.Name))
                    {
                        throw new Exception("此Assembly name不存在！");
                    }

                    var colSensorDefinition = db.GetCollection<SensorDefinition>(DatabaseName.SensorDefinition);

                    List<SensorDefinition> listSensorDefinition = new List<SensorDefinition>();
                    foreach (int i in targetAssembly.Sensors)
                    {
                        if (colSensorDefinition.Exists(x => x.Id == i))
                        {
                            listSensorDefinition.Add(colSensorDefinition.FindById(i));
                        }
                    }

                    temp = temp.Where(x => x.Name == param.Name && check(x, listSensorDefinition));
                }

                if (param.Start != null)
                {
                    temp = temp.Where(x => x.CreateDatetime >= param.Start);
                }

                if (param.End != null)
                {
                    DateTime t = param.End.Value.AddDays(1).AddSeconds(-1);
                    temp = temp.Where(x => x.CreateDatetime <= t);
                }

                return temp.ToList();
            }
            catch
            {
                throw;
            }
        }

        public bool check(RawData rd, List<SensorDefinition> sensors)
        {
            int count = 0;
            bool check = true;
            foreach (var d in rd.Data)
            {
                if (count < sensors.Count() && !string.IsNullOrWhiteSpace(sensors[count].Regex))
                {
                    if (!Regex.IsMatch(d, sensors[count].Regex))
                    {
                        //--任一不合就跳出
                        check = false;
                        break;
                    }
                }

                count++;
            }

            return check;
        }

    }
}