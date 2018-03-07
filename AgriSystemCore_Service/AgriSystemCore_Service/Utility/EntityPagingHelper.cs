using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AgriSystemCore_Service.Utility
{
    public static class EntityPagingHelper<T>
    {
        public static IQueryable<T> Paging(IQueryable<T> _ListSource, int? _Length, int? _Index)
        {
            try
            {
                if (_Length != null && _Index != null)
                {
                    return _ListSource.Skip((int)_Index).Take((int)_Length);
                }
                else
                {
                    return _ListSource;
                }
            }
            catch
            {
                throw;
            }
        }

        public static List<T> Paging(List<T> _ListSource, int? _Length, int? _Index)
        {
            try
            {
                if (_Length != null && _Index != null)
                {
                    return _ListSource.Skip((int)_Index).Take((int)_Length).ToList<T>();
                }
                else
                {
                    return _ListSource;
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
