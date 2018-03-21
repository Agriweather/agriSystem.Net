using AgriSystemCore_Service.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;

namespace AgriSystemCore_Service.Service
{
    public class EndpointService : BaseService
    {
        public EndpointService(string dbConn) : base(dbConn)
        {

        }

        public EndpointDataitem Monitor(string assembly, int limit)
        {
            try
            {
                if (limit > 100)
                {
                    limit = 100;
                }
                else if (limit < 0)
                {
                    limit = 1;
                }

                EndpointDataitem result = new EndpointDataitem();

                //--先找出指定的 assembly
                var colAssembly = db.GetCollection<Assembly>(DatabaseName.Assembly);

                if (!colAssembly.Exists(x => x.Name == assembly))
                {
                    throw new Exception("此Assembly name不存在！");
                }

                var targetAssembly = colAssembly.FindOne(x => x.Name == assembly);

                result.Id = targetAssembly.Id;
                result.Name = targetAssembly.Name;
                result.Note = targetAssembly.Note;
                result.Sensors = new List<SensorDefinition>();
                result.Value = new List<RawData>();

                var colSensorDefinition = db.GetCollection<SensorDefinition>(DatabaseName.SensorDefinition);
                foreach (int i in targetAssembly.Sensors)
                {
                    if (colSensorDefinition.Exists(x => x.Id == i))
                    {
                        var csd = colSensorDefinition.FindById(i);
                        if (string.IsNullOrWhiteSpace(csd.CssClass))
                        {
                            csd.CssClass = "wi wi-na";
                        }

                        result.Sensors.Add(csd);
                    }
                }

                var colRawData = db.GetCollection<RawData>(DatabaseName.RawData);

                foreach (var i in colRawData.Find(x => x.Name == assembly && check(x, result.Sensors)).OrderByDescending(x => x.Id).Take(limit))
                {
                    result.Value.Add(i);
                }

                return result;
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

        public EndpointDataitem History(string assembly, DateTime date, int day, int frequency)
        {
            try
            {
                EndpointDataitem result = new EndpointDataitem();

                //--先找出指定的 assembly
                var colAssembly = db.GetCollection<Assembly>(DatabaseName.Assembly);

                if (!colAssembly.Exists(x => x.Name == assembly))
                {
                    throw new Exception("此Assembly name不存在！");
                }

                var targetAssembly = colAssembly.FindOne(x => x.Name == assembly);

                result.Id = targetAssembly.Id;
                result.Name = targetAssembly.Name;
                result.Note = targetAssembly.Note;
                result.Sensors = new List<SensorDefinition>();
                result.Value = new List<RawData>();

                var colSensorDefinition = db.GetCollection<SensorDefinition>(DatabaseName.SensorDefinition);
                foreach (int i in targetAssembly.Sensors)
                {
                    if (colSensorDefinition.Exists(x => x.Id == i))
                    {
                        result.Sensors.Add(colSensorDefinition.FindById(i));
                    }
                }

                var colRawData = db.GetCollection<RawData>(DatabaseName.RawData);

                DateTime start = date;
                DateTime end = start.AddDays(day).AddSeconds(-1);

                DateTime pointer = start;
                var rawData = colRawData.Find(x => x.Name == assembly && check(x, result.Sensors)  && x.CreateDatetime >= start && x.CreateDatetime <= end).OrderBy(x => x.Id);

                foreach (var i in rawData)
                {
                    if (i.CreateDatetime >= pointer)
                    {
                        pointer = i.CreateDatetime.AddSeconds(frequency);

                        result.Value.Add(i);
                    }
                }

                if (pointer >= end)
                {
                    TimeSpan ts = pointer - end;
                    if (ts.Seconds > Math.Ceiling((double)frequency / 2))
                    {
                        result.Value.Add(rawData.Last());
                    }
                }

                return result;
            }
            catch
            {
                throw;
            }
        }

        public class EndpointDataitem
        {
            /// <summary>
            /// assembly id
            /// </summary>
            public int Id { get; set; }
            /// <summary>
            /// assembly name
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// assembly Note
            /// </summary>
            public string Note { get; set; }

            public List<SensorDefinition> Sensors { get; set; }

            public List<RawData> Value { get; set; }
        }

    }
}
