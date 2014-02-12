using System;
using System.Web;
using System.Web.Caching;

namespace MiniTrello.Infrastructure
{
    public class AspNetCacheProvider : ICacheProvider
    {
        public T GetObject<T>(string key)
        {
            return (T) HttpContext.Current.Cache.Get(key);
        }

        public void SetObject<T>(string key, DateTime expiration, T obj)
        {
            HttpContext.Current.Cache.Insert(key, obj, null, expiration,
                Cache.NoSlidingExpiration);
        }
    }
}