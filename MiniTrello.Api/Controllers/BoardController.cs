using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Http;
using AttributeRouting.Web.Http;
using AutoMapper;
using MiniTrello.Api.Controllers.Helpers;
using MiniTrello.Domain.DataObjects;
using MiniTrello.Domain.Entities;
using MiniTrello.Domain.Services;

namespace MiniTrello.Api.Controllers
{
    public class BoardController : ApiController
    {
        readonly IReadOnlyRepository _readOnlyRepository;
        readonly IWriteOnlyRepository _writeOnlyRepository;
        readonly IMappingEngine _mappingEngine;

        public BoardController(IReadOnlyRepository readOnlyRepository, IWriteOnlyRepository writeOnlyRepository,
            IMappingEngine mappingEngine)
        {
            _readOnlyRepository = readOnlyRepository;
            _writeOnlyRepository = writeOnlyRepository;
            _mappingEngine = mappingEngine;
        }
        
        [POST("boards/rename/{token}")]
        public BoardModel RenameBoard([FromBody] BoardChangeTitleModel boardRenameModel, string token)
        {
            var session = Security.VerifiySession(token, _readOnlyRepository);
            if (session == null)
            {
                return new BoardModel
                {
                    ErrorCode = 1,
                    ErrorMessage = ErrorStrings.SessionDoesNotExistOnThisServer
                };
            }
            if (Security.IsTokenExpired(session))
            {
                return new BoardModel
                {
                    ErrorCode = 1,
                    ErrorMessage = ErrorStrings.SessionHasExpired
                };
            }
            var accountFromSession = Security.GetAccountFromSession(session, _readOnlyRepository);
            if (accountFromSession == null)
            {
                return new BoardModel
                {
                    ErrorCode = 1,
                    ErrorMessage = ErrorStrings.AccountDoesNotExist
                };
            }
            var editedBoardModel = _readOnlyRepository.First<Board>(board => board.Id == boardRenameModel.Id);
            if (!Security.IsThisAccountAdminOfThisBoard(editedBoardModel, accountFromSession))
            {
                return new BoardModel
                {
                    ErrorCode = 1,
                    ErrorMessage = ErrorStrings.NotEnoughPriviledges
                };
            }
            editedBoardModel.Title = boardRenameModel.Title;
            editedBoardModel.Log = editedBoardModel.Log + accountFromSession.FirstName + " RenameBoard " + boardRenameModel.Title + " ";
            _writeOnlyRepository.Update(editedBoardModel);
            var returnBoardModel = _mappingEngine.Map<Board, BoardModel>(editedBoardModel);
            return returnBoardModel;
        }
        
        [POST("boards/createcard/{token}")]
        public CardModel CreateCard([FromBody]CardCreateModel model,string token)
        {
            var session = Security.VerifiySession(token, _readOnlyRepository);
            if (session == null)
            {
                return new CardModel
                {
                    ErrorCode = 1,
                    ErrorMessage = ErrorStrings.SessionDoesNotExistOnThisServer
                };
            }
            if (Security.IsTokenExpired(session))
            {
                return new CardModel
                {
                    ErrorCode = 1,
                    ErrorMessage = ErrorStrings.SessionHasExpired
                };
            }
            var accountFromSession = Security.GetAccountFromSession(session, _readOnlyRepository);
            if (accountFromSession == null)
            {
                return new CardModel
                {
                    ErrorCode = 1,
                    ErrorMessage = ErrorStrings.AccountDoesNotExist
                };
            }
            var lane = _readOnlyRepository.GetById<Lane>(model.LaneId);
            if (lane == null)
            {
                return new CardModel
                {
                    ErrorCode = 1,
                    ErrorMessage = ErrorStrings.LaneDoesNotExist
                };
            }
            var editedBoard = _readOnlyRepository.First<Board>(board1 => board1.Lanes.Any(lane1 => lane1.Id == lane.Id));
            if (editedBoard == null)
            {
                return new CardModel
                {
                    ErrorCode = 1,
                    ErrorMessage = ErrorStrings.BoardDoesNotExist
                };
            }
            var accountShell = _mappingEngine.Map<Account,AccountShell>(accountFromSession);
            editedBoard.AddMemberAccount(accountShell);
            var card = _mappingEngine.Map<CardCreateModel, Card>(model);
            card = _writeOnlyRepository.Create(card);
            if (card == null)
            {
                return new CardModel
                {
                    ErrorCode = 1,
                    ErrorMessage = ErrorStrings.CardDoesNotExist
                };
            }
            if (!Security.IsThisAccountMemberOfThisBoard(editedBoard, accountFromSession))
            {
                return new CardModel
                {
                    ErrorCode = 1,
                    ErrorMessage = ErrorStrings.NotEnoughPriviledges
                };
            }
            lane.AddCard(card);
            editedBoard.Log = editedBoard.Log + accountFromSession.FirstName + " CreateCArd " + card.Id + " ";
            _writeOnlyRepository.Update(editedBoard);
            return _mappingEngine.Map<Card, CardModel>(card);
        }

        [POST("boards/createlane/{token}")]
        public LaneModel CreateLane([FromBody]LaneCreateModel laneCreateModelmodel,string token)
        {
            var session = Security.VerifiySession(token, _readOnlyRepository);
            if (session == null)
            {
                return new LaneModel
                {
                    ErrorCode = 1,
                    ErrorMessage = ErrorStrings.SessionDoesNotExistOnThisServer
                };
            }
            if (Security.IsTokenExpired(session))
            {
                return new LaneModel
                {
                    ErrorCode = 1,
                    ErrorMessage = ErrorStrings.SessionHasExpired
                };
            }
            var accountFromSession = Security.GetAccountFromSession(session, _readOnlyRepository);
            if (accountFromSession == null)
            {
                return new LaneModel
                {
                    ErrorCode = 1,
                    ErrorMessage = ErrorStrings.AccountDoesNotExist
                };
            }
            var editedBoard = _readOnlyRepository.First<Board>(board => board.Id == laneCreateModelmodel.BoardId);
            if (editedBoard == null)
            {
                return new LaneModel
                {
                    ErrorCode = 1,
                    ErrorMessage = ErrorStrings.BoardDoesNotExist
                };
            }
            if (!Security.IsThisAccountMemberOfThisBoard(editedBoard, accountFromSession))
            {
                return new LaneModel
                {
                    ErrorCode = 1,
                    ErrorMessage = ErrorStrings.NotEnoughPriviledges
                };
            }
            var newLane = _mappingEngine.Map<LaneCreateModel,Lane>(laneCreateModelmodel);
            newLane = _writeOnlyRepository.Create(newLane);
            if (newLane == null)
            {
                return new LaneModel
                {
                    ErrorCode = 1,
                    ErrorMessage = ErrorStrings.CouldNotCreateItem
                };
            }

            editedBoard.AddLane(newLane);
            editedBoard.Log = editedBoard.Log + accountFromSession.FirstName + " CreateLane " + newLane.Id + " ";
            _writeOnlyRepository.Update(editedBoard);
            return _mappingEngine.Map<Lane, LaneModel>(newLane);
        }

        [POST("boards/movecard/{token}")]
        public BoardModel MoveCard([FromBody] CardMoveModel model, string token)
        {
            var session = Security.VerifiySession(token, _readOnlyRepository);
            if (session == null)
            {
                return new BoardModel
                {
                    ErrorCode = 1,
                    ErrorMessage = ErrorStrings.SessionDoesNotExistOnThisServer
                };
            }
            if (Security.IsTokenExpired(session))
            {
                return new BoardModel
                {
                    ErrorCode = 1,
                    ErrorMessage = ErrorStrings.SessionHasExpired
                };
            }
            var accountFromSession = Security.GetAccountFromSession(session, _readOnlyRepository);
            if (accountFromSession == null)
            {
                return new BoardModel
                {
                    ErrorCode = 1,
                    ErrorMessage = ErrorStrings.AccountDoesNotExist
                };
            }
            var card    = _readOnlyRepository.First<Card>(card1 => card1.Id == model.CardId);
            var newLane = _readOnlyRepository.GetById<Lane>(model.DestinationId);
            var oldLane = _readOnlyRepository.First<Lane>(lane => lane.Cards.Contains(card));
            var board   = _readOnlyRepository.First<Board>(board1 => board1.Lanes.Contains(newLane) && board1.Lanes.Contains(oldLane));
            if (!Security.IsThisAccountMemberOfThisBoard(board, accountFromSession))
            {
                return new BoardModel
                {
                    ErrorCode = 1,
                    ErrorMessage = ErrorStrings.NotEnoughPriviledges
                };
            }
            if (board == null)
            {
                return new BoardModel
                {
                    ErrorCode = 1,
                    ErrorMessage = "Transfering cards to other boards is not allowed"
                };
            }

            newLane.AddCard(card);
            oldLane.RemoveCard(card);
            board.Log = board.Log + accountFromSession.FirstName + " MoveCard " + card.Id + " to lane " +
                        newLane.Id.ToString(CultureInfo.InvariantCulture) + " ";
            _writeOnlyRepository.Update(accountFromSession);
            return _mappingEngine.Map<Board, BoardModel>(board);
        }
        [AcceptVerbs("DELETE")]
        [DELETE("boards/deletelane/{id}/{token}")]
        public BoardModel DeleteLane(long id, string token)
        {
            var session = Security.VerifiySession(token, _readOnlyRepository);
            if (session == null)
            {
                return new BoardModel
                {
                    ErrorCode = 1,
                    ErrorMessage = ErrorStrings.SessionDoesNotExistOnThisServer
                };
            }
            if (Security.IsTokenExpired(session))
            {
                return new BoardModel
                {
                    ErrorCode = 1,
                    ErrorMessage = ErrorStrings.SessionHasExpired
                };
            }
            var accountFromSession = Security.GetAccountFromSession(session, _readOnlyRepository);
            if (accountFromSession == null)
            {
                return new BoardModel
                {
                    ErrorCode = 1,
                    ErrorMessage = ErrorStrings.AccountDoesNotExist
                };
            }


            var lane = _readOnlyRepository.GetById<Lane>(id);
            if (lane == null)
            {
                return new BoardModel
                {
                    ErrorCode = 1,
                    ErrorMessage = ErrorStrings.LaneDoesNotExist
                };
            }
            var editedBoard = _readOnlyRepository.First<Board>(board1 => board1.Lanes.Any(lane1 => lane1.Id == lane.Id));
            if (editedBoard == null)
            {
                return new BoardModel
                {
                    ErrorCode = 1,
                    ErrorMessage = ErrorStrings.BoardDoesNotExist
                };
            }
            if (!Security.IsThisAccountMemberOfThisBoard(editedBoard, accountFromSession))
            {
                return new BoardModel
                {
                    ErrorCode = 1,
                    ErrorMessage = ErrorStrings.NotEnoughPriviledges
                };
            }
            Security.IsThisAccountMemberOfThisBoard(editedBoard, accountFromSession);
            _writeOnlyRepository.Archive(lane);
            editedBoard.Log = editedBoard.Log + accountFromSession.FirstName + " DeleteLane " + lane.Id + " ";
            _writeOnlyRepository.Update(lane);
            return _mappingEngine.Map<Board, BoardModel>(editedBoard);
        }
        
        [AcceptVerbs("DELETE")]
        [DELETE("boards/{id}/{token}")]
        public GetBoardsModel DeleteBoard(long id, string token)
        {
            var session = Security.VerifiySession(token, _readOnlyRepository);
            if (session == null)
                return new GetBoardsModel
                {
                    ErrorCode = 1,
                    ErrorMessage = ErrorStrings.SessionDoesNotExistOnThisServer
                };
            if (Security.IsTokenExpired(session))
            {
                return new GetBoardsModel
                {
                    ErrorCode = 1,
                    ErrorMessage = ErrorStrings.SessionHasExpired
                };
            }
            var accountFromSession = Security.GetAccountFromSession(session, _readOnlyRepository);
            if (accountFromSession == null)
            {
                return new GetBoardsModel
                {
                    ErrorCode = 1,
                    ErrorMessage = ErrorStrings.AccountDoesNotExist
                };
            }
            var editedBoard = _readOnlyRepository.GetById<Board>(id);
            if (editedBoard == null)
            {
                return new GetBoardsModel
                {
                    ErrorCode = 1,
                    ErrorMessage = ErrorStrings.BoardDoesNotExist
                };
            }

            if (!Security.IsThisAccountAdminOfThisBoard(editedBoard, accountFromSession))
            {
                return new GetBoardsModel
                {
                    ErrorCode = 1,
                    ErrorMessage = ErrorStrings.NotEnoughPriviledges
                };
            }

            editedBoard.IsArchived = true;
            editedBoard.Log = editedBoard.Log + accountFromSession.FirstName + " deleteBoard " + editedBoard.Id.ToString(CultureInfo.InvariantCulture) + " ";
            _writeOnlyRepository.Update(editedBoard);
            var boardsModel = new GetBoardsModel();
            foreach (var board in accountFromSession.Boards)
            {
                var myBoardModel = _mappingEngine.Map<Board, BoardModel>(board);
                boardsModel.AddBoard(myBoardModel);
            }
            return boardsModel;
        }
        [AcceptVerbs("DELETE")]
        [DELETE("boards/deletecard/{id}/{token}")]
        public LaneModel DeleteCard(long id,string token)
        {
            var session = Security.VerifiySession(token, _readOnlyRepository);
            if (session == null)
            {
                return new LaneModel
                {
                    ErrorCode = 1,
                    ErrorMessage = ErrorStrings.SessionDoesNotExistOnThisServer
                };
            }
            if (Security.IsTokenExpired(session))
            {
                return new LaneModel
                {
                    ErrorCode = 1,
                    ErrorMessage = ErrorStrings.SessionHasExpired
                };
            }
            var accountFromSession = Security.GetAccountFromSession(session, _readOnlyRepository);
            if (accountFromSession == null)
            {
                return new LaneModel
                {
                    ErrorCode = 1,
                    ErrorMessage = ErrorStrings.AccountDoesNotExist
                };
            }
            var card = _readOnlyRepository.First<Card>(card1 => card1.Id == id);
            if (card == null)
            {
                return new LaneModel
                {
                    ErrorCode = 1,
                    ErrorMessage = ErrorStrings.CardDoesNotExist
                }; 
            }
            var myLane = _readOnlyRepository.First<Lane>(lane => lane.Cards.Contains(card));
            if (myLane == null)
            {
                return new LaneModel
                {
                    ErrorCode = 1,
                    ErrorMessage = ErrorStrings.LaneDoesNotExist
                };
            }
            var myBoard = _readOnlyRepository.First<Board>(board => board.Lanes.Contains(myLane));
            if (myBoard == null)
            {
                return new LaneModel
                {
                    ErrorCode = 1,
                    ErrorMessage = ErrorStrings.BoardDoesNotExist
                };
            }
            myBoard.Log = myBoard.Log + accountFromSession.FirstName + " deleteCard " + card.Id.ToString(CultureInfo.InvariantCulture) + " ";
            _writeOnlyRepository.Update(myBoard);
            _writeOnlyRepository.Archive(card);
            return _mappingEngine.Map<Lane, LaneModel>(myLane);
        }

        
        [AcceptVerbs("PUT")]
        [PUT("boards/addmember/{accessToken}")]
        public BoardModel AddMember([FromBody]AddMemberBoardModel model,string accessToken)
        {
            var memberToAdd = FindCorrespondingAccount(model.MemberEmail);
            if (memberToAdd == null)
            {
                return new BoardModel
                {
                    ErrorCode = 1,
                    ErrorMessage = ErrorStrings.AccountDoesNotExist
                };
            }
            var board = _readOnlyRepository.GetById<Board>(model.BoardID);
            if (board == null)
            {
                return new BoardModel
                {
                    ErrorCode = 1,
                    ErrorMessage = ErrorStrings.BoardDoesNotExist
                };
            }
            var session = Security.VerifiySession(accessToken,_readOnlyRepository);
            if (session == null)
            {
                return new BoardModel
                {
                    ErrorCode = 1,
                    ErrorMessage = ErrorStrings.SessionDoesNotExistOnThisServer
                };
            }
            if (Security.IsTokenExpired(session))
            {
                return new BoardModel
                {
                    ErrorCode = 1,
                    ErrorMessage = ErrorStrings.SessionHasExpired
                };
            }
            var myAccount = Security.GetAccountFromSession(session,_readOnlyRepository);
            if (myAccount == null)
            {
                return new BoardModel
                {
                    ErrorCode = 1,
                    ErrorMessage = ErrorStrings.AccountDoesNotExist
                };
            }
            board.Log = board.Log + myAccount.FirstName + " addMember " + memberToAdd.FirstName + " ";
            var accountShell = _mappingEngine.Map<Account, AccountShell>(memberToAdd);
            board.AddMemberAccount(accountShell);
            _writeOnlyRepository.Update(memberToAdd);
            var updatedBoard = _writeOnlyRepository.Update(board);
             
            return _mappingEngine.Map<Board, BoardModel>(updatedBoard);
        }
       
        [POST("boards/create/{token}")]
        public GetBoardsModel CreateBoard([FromBody]BoardCreateModel model, string token)
        {
            var session = Security.VerifiySession(token,_readOnlyRepository);
            if (session == null)
            {
                return new GetBoardsModel
                {
                    ErrorMessage = ErrorStrings.SessionDoesNotExistOnThisServer
                };
            }
            if (Security.IsTokenExpired(session))
            {
                return new GetBoardsModel
                {
                    ErrorMessage = ErrorStrings.SessionHasExpired,
                    ErrorCode = 1
                };
            }
            var newBoard = _mappingEngine.Map<BoardCreateModel, Board>(model);
            initBoard(newBoard);
            var accountFromSession = Security.GetAccountFromSession(session, _readOnlyRepository);
            if (accountFromSession == null)
            {
                return new GetBoardsModel
                {
                    ErrorMessage = ErrorStrings.AccountDoesNotExist,
                    ErrorCode = 1
                };
            }
            accountFromSession.AddBoard(newBoard);
            newBoard.Log = newBoard.Log + accountFromSession.FirstName + " create ";
            _writeOnlyRepository.Update(accountFromSession);
            accountFromSession = Security.GetAccountFromSession(session, _readOnlyRepository);
            var boardsModel = new GetBoardsModel();
            foreach (var board in accountFromSession.Boards)
            {
                var boardModel = _mappingEngine.Map<Board, BoardModel>(board);
                boardsModel.AddBoard(boardModel);
            }
            return boardsModel;
        }

        private void initBoard(Board newBoard)
        {
            var newLane = new Lane
            {
                Name = "To Do:"
            };
            newLane = _writeOnlyRepository.Create(newLane);
            newBoard.AddLane(newLane);
            var newLane2 = new Lane
            {
                Name = "Doing:"
            };
            newLane2 = _writeOnlyRepository.Create(newLane2);
            newBoard.AddLane(newLane2);
            
            var newLane3 = new Lane
            {
                Name = "Done:"
            };
            newLane3 = _writeOnlyRepository.Create(newLane3);
            newBoard.AddLane(newLane3);

        }

        [GET("boards/{boardId}/{token}")] 
        public BoardModel GetBoard(long boardId,string token)
        {
            var session = Security.VerifiySession(token, _readOnlyRepository);
            if (session == null)
            {
                return new BoardModel
                {
                    ErrorCode = 1,
                    ErrorMessage = ErrorStrings.SessionDoesNotExistOnThisServer
                };
            }
            if (Security.IsTokenExpired(session))
            {
                return new BoardModel
                {
                    ErrorCode = 1,
                    ErrorMessage = ErrorStrings.SessionHasExpired
                };
            }
            var myAccount = Security.GetAccountFromSession(session, _readOnlyRepository);
            if (myAccount == null)
            {
                return new BoardModel
                {
                    ErrorCode = 1,
                    ErrorMessage = ErrorStrings.AccountDoesNotExist
                };
            }
            var myBoard = _readOnlyRepository.GetById<Board>(boardId);
            if (myBoard == null)
            {
                return new BoardModel
                {
                    ErrorCode = 1,
                    ErrorMessage = ErrorStrings.BoardDoesNotExist
                };
            }
            if (!Security.IsThisAccountMemberOfThisBoard(myBoard, myAccount))
            {
                return new BoardModel
                {
                    ErrorCode = 1,
                    ErrorMessage = ErrorStrings.NotEnoughPriviledges
                };
            }
            return _mappingEngine.Map<Board,BoardModel>(myBoard);
        }

        [GET("boards/{boardId}/log/{token}")]
        public BoardLogModel GetBoardLog(long boardId, string token)
        {
            var session = Security.VerifiySession(token, _readOnlyRepository);
            if (session == null)
            {
                return new BoardLogModel
                {
                    ErrorCode = 1,
                    ErrorMessage = ErrorStrings.SessionDoesNotExistOnThisServer
                };
            }
            if (Security.IsTokenExpired(session))
            {
                return new BoardLogModel
                {
                    ErrorCode = 1,
                    ErrorMessage = ErrorStrings.SessionHasExpired
                };
            }
            var myAccount = Security.GetAccountFromSession(session, _readOnlyRepository);
            if (myAccount == null) 
            {
                return new BoardLogModel
                {
                    ErrorCode = 1,
                    ErrorMessage = ErrorStrings.AccountDoesNotExist
                };
            }
            var myBoard = _readOnlyRepository.GetById<Board>(boardId);
            if (myBoard == null)
            {
                return new BoardLogModel
                {
                    ErrorCode = 1,
                    ErrorMessage = ErrorStrings.BoardDoesNotExist
                };
            }
            if (!Security.IsThisAccountMemberOfThisBoard(myBoard, myAccount))
            {
                return new BoardLogModel
                {
                    ErrorCode = 1,
                    ErrorMessage = ErrorStrings.NotEnoughPriviledges
                };
            }
            return _mappingEngine.Map<Board, BoardLogModel>(myBoard);
        }

        [GET("boards/members/{boardId}/{token}")]
        public GetMembersModel GetBoardMembers(long boardId, string token)
        {
            var session = Security.VerifiySession(token, _readOnlyRepository);
            if (session == null)
            {
                return new GetMembersModel
                {
                    ErrorCode = 1,
                    ErrorMessage = ErrorStrings.SessionDoesNotExistOnThisServer
                };
            }
            if (Security.IsTokenExpired(session))
            {
                return new GetMembersModel
                {
                    ErrorMessage = ErrorStrings.SessionHasExpired,
                    ErrorCode = 1
                };
            }
            var myAccount = Security.GetAccountFromSession(session, _readOnlyRepository);
            if (myAccount == null)
            {
                return new GetMembersModel
                {
                    ErrorMessage = ErrorStrings.AccountDoesNotExist,
                    ErrorCode = 1
                };
            }
            var myBoard = _readOnlyRepository.GetById<Board>(boardId);
            if (myBoard == null)
            {
                return new GetMembersModel
                {
                    ErrorMessage = ErrorStrings.BoardDoesNotExist,
                    ErrorCode = 1
                };
            }
            if (Security.IsThisAccountMemberOfThisBoard(myBoard, myAccount))
            {
                return new GetMembersModel
                {
                    ErrorMessage = ErrorStrings.NotEnoughPriviledges,
                    ErrorCode = 1
                };
            }
            
            var myMemberList = myBoard.MemberAccounts.ToList();
            var membersModel = new GetMembersModel();
            foreach (var member in myMemberList)
            {
                var memberModel = _mappingEngine.Map<AccountShell, MemberModel>(member);
                membersModel.AddMember(memberModel);
            }            
            return membersModel;
        }
        private Account FindCorrespondingAccount(AccountLoginModel model)
        {
            var myAES = new SimpleAES();
            var account = _readOnlyRepository.First<Account>(
                account1 => account1.Email == model.Email);
            if (account == null) return null;
            var accountPassword = myAES.DecryptString(account.Password);
            return accountPassword != model.Password ? null : account;
        }
        private Account FindCorrespondingAccount(string email)
        {
            var account = _readOnlyRepository.First<Account>(
                account1 => account1.Email == email);
            return account;
        }
    }

}
