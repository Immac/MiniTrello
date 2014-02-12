using System;

namespace MiniTrello.Infrastructure
{
    public interface ICacheProvider
    {
        T GetObject<T>(string key);
        void SetObject<T>(string key, DateTime expiration, T obj);
    }
}