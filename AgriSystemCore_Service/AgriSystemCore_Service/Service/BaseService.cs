using LiteDB;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgriSystemCore_Service.Service
{
    public abstract class BaseService : IDisposable
    {
        protected LiteDatabase db;

        public BaseService(string dbConn)
        {
            this.db = new LiteDatabase(dbConn);
        }

        public virtual void Dispose()
        {
            try
            {
                this.db.Dispose();
            }
            catch
            {
                throw;
            }
        }

        public static class DatabaseName
        {
            public const string Manager = "Manager";
            public const string RawData = "RawData";
            public const string Assembly = "Assembly";
            public const string SensorDefinition = "SensorDefinition";
        }
    }
}
