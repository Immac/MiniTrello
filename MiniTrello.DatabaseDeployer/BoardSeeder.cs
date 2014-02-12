using System.Collections.Generic;
using DomainDrivenDatabaseDeployer;
using FizzWare.NBuilder;
using MiTrello.Domain.Entities;
using NHibernate;

namespace MiniTrello.DatabaseDeployer
{
    class BoardSeeder : IDataSeeder
    {
        readonly ISession _session;

        public BoardSeeder(ISession session)
        {
            _session = session;
        }

        public void Seed()
        {
            //IList<Board> boardList = Builder<Board>.CreateListOfSize(10).Build();
            //foreach (Board board in boardList)
            //{
            //    _session.Save(board);
            //}
        }
    }
}