using System.Collections.Generic;
using DomainDrivenDatabaseDeployer;
using FizzWare.NBuilder;
using MiniTrello.Domain.Entities;
using NHibernate;

namespace MiniTrello.DatabaseDeployer
{
    public class AccountSeeder : IDataSeeder
    {
        readonly ISession _session;

        public AccountSeeder(ISession session)
        {
            _session = session;
        }

        public void Seed()
        {
            IList<Account> accountList = Builder<Account>.CreateListOfSize(10).Build();
            foreach (Account account in accountList)
            {
                _session.Save(account);
                IList<Board> boards = Builder<Board>.CreateListOfSize(2).Build();
                IList<Organization> organizations = Builder<Organization>.CreateListOfSize(2).Build();
                
                
                foreach (Board board in boards)
                {
                    IList<Lane> lanes = Builder<Lane>.CreateListOfSize(2).Build();
                    foreach (var lane in lanes)
                    {
                        IList<Card> cards = Builder<Card>.CreateListOfSize(4).Build();
                        foreach (var card in cards)
                        {
                            _session.Save(card);
                            lane.AddCard(card);
                        }
                        _session.Save(lane);
                        board.AddLane(lane);    
                    }
                  
                
                    _session.Save(board);
                }
                foreach (var organization in organizations)
                {
                    _session.Save(organization);
                    
                }
                

                account.AddOrganization(organizations[0]);
                account.AddOrganization(organizations[1]);

                account.AddBoard(boards[0]);
                account.AddBoard(boards[1]);
                _session.Update(account);
            }
            
        }
    }
}