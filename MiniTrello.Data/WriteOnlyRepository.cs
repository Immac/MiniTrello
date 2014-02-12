using System;
using MiniTrello.Domain.Entities;
using MiniTrello.Domain.Services;
using NHibernate;

namespace MiniTrello.Data
{
    public class WriteOnlyRepository : IWriteOnlyRepository
    {
        private readonly ISession _session;

        public WriteOnlyRepository(ISession session)
        {
            _session = session;
        }

        public T Create<T>(T itemToCreate) where T : class, IEntity
        {
            _session.Save(itemToCreate);
            return itemToCreate;
        }

        public T Update<T>(T itemToUpdate) where T : class, IEntity
        {
            _session.Update(itemToUpdate);
            return itemToUpdate;
        }

        public T Archive<T>(T itemToArchive) where T : class,IEntity
        {
            itemToArchive.IsArchived = true;
            return Update(itemToArchive);
        }

    }
}