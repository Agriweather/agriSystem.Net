using AgriSystemCore_Service.Domain;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgriSystemCore_Service.Service
{
    public class AuthorizationService :BaseService
    {
        public AuthorizationService(string dbConn) : base(dbConn)
        {

        }

        /// <summary>
        /// 確認登入資訊正確，並提供系統 Manager 資訊
        /// </summary>
        /// <param name="name">使用者帳號</param>
        /// <param name="password">使用者密碼</param>
        /// <param name="manager">Manager 物件</param>
        /// <returns></returns>
        public bool Login(string name, string password, out Manager manager)
        {
            try
            {
                bool result = false;

                var col = db.GetCollection<Manager>(DatabaseName.Manager);
                if (col.Exists(x => x.Name == name && x.Password == password))
                {
                    manager = col.FindOne(x => x.Name == name);
                    result = true;
                }
                else
                {
                    manager = new Manager();
                }

                return result;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 如果在登入頁面發現系統 DB不存在時，會自動重新建一個 DB
        /// </summary>
        /// <param name="json_Menu">系統功能設定，位置在 [host]/Data/FunctionMenu.json</param>
        public void CreateAdmin(string json_Menu, string json_DefaultSensor)
        {
            try
            {
                //--必須是 Manager 的 collection 不存在才允許 Init
                if (!db.CollectionExists(DatabaseName.Manager))
                {
                    List<Menu> menu = JsonConvert.DeserializeObject<List<Menu>>(json_Menu);

                    Manager admin = new Manager();
                    admin.Name = "Admin";
                    admin.Password = "admin";
                    admin.Auth = menu;

                    var col = db.GetCollection<Manager>(DatabaseName.Manager);
                    col.EnsureIndex(x => x.Name);
                    col.Insert(admin);

                    if (!db.CollectionExists(DatabaseName.SensorDefinition))
                    {
                        List<SensorDefinition> sensors = JsonConvert.DeserializeObject<List<SensorDefinition>>(json_DefaultSensor);

                        var colSensor = db.GetCollection<SensorDefinition>(DatabaseName.SensorDefinition);
                        foreach (var i in sensors)
                        {
                            if (string.IsNullOrWhiteSpace(i.Note))
                            {
                                i.Note = i.Name;
                            }
                            i.IsDefaultDefinition = true;

                            colSensor.Insert(i);
                        }

                    }
                }

            }
            catch
            {
                throw;
            }
        }
    }
}
