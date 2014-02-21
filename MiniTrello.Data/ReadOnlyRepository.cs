using System;
using System.Linq;
using System.Linq.Expressions;
using MiniTrello.Domain.Entities;
using MiniTrello.Domain.Services;
using NHibernate;
using NHibernate.Linq;

namespace MiniTrello.Data
{
    public class ReadOnlyRepository : IReadOnlyRepository
    {
        private readonly ISession _session;

        public ReadOnlyRepository(ISession session)
        {
            _session = session;
        }

        public T First<T>(Expression<Func<T, bool>> query) where T : class, IEntity
        {
            T firstOrDefault = _session.Query<T>().Where(x=>x.IsArchived == false).FirstOrDefault(query);
            return firstOrDefault;
        }

        public T First<T>(Expression<Func<T, bool>> query,bool seeArchived) where T : class, IEntity
        {
            if (seeArchived)
            {
                T firstOrDefault = _session.Query<T>().FirstOrDefault(query);
                return firstOrDefault;
            }
            return First<T>(query);
        }

        public T GetById<T>(long id) where T : class, IEntity
        {
            T item = _session.Get<T>(id);
            if (item.IsArchived) return null;
            return item;
        } 

        public T GetById<T>(long id,bool seeArchived) where T : class, IEntity
        {
            
            T item = _session.Get<T>(id);
            if (item.IsArchived && !seeArchived) return null;
            return item;
        }

        public IQueryable<T> Query<T>(Expression<Func<T, bool>> expression) where T : class, IEntity
        {
            return _session.Query<T>().Where(x=>x.IsArchived == false).Where(expression);
        }
        public IQueryable<T> Query<T>(Expression<Func<T, bool>> expression,bool seeArchived) where T : class, IEntity
        {
            if(seeArchived)
            return _session.Query<T>().Where(expression);
            return Query(expression);
        }
    }
}