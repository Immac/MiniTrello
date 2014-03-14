using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Http;
using System.Web.Script.Serialization;
using AttributeRouting.Web.Http;
using AutoMapper;
using MiniTrello.Api.Controllers.Helpers;
using MiniTrello.Api.CustomExceptions;
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
            var card = _mappingEngine.Map<CardCreateModel, Card>(model);
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
        public LaneModel CreateLane([FromBody]LaneCreateModel model,string token)
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
            var editedBoard = _readOnlyRepository.First<Board>(board1 => board1.Lanes.Any(lane1 => lane1.Id == lane.Id));
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
            
            var newLane = _mappingEngine.Map<LaneCreateModel,Lane>(model);
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
            var card    = _readOnlyRepository.First<Card>(card1 => card1.Id == model.CardId);
            var newLane = _readOnlyRepository.GetById<Lane>(model.DestinationId);
            var oldLane = _readOnlyRepository.First<Lane>(lane => lane.Cards.Contains(card));
            var board   = _readOnlyRepository.First<Board>(board1 => board1.Lanes.Contains(newLane) && board1.Lanes.Contains(oldLane));
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

        [DELETE("boards/deletelane/{token}")]
        public BoardModel DeleteLane([FromBody] LaneDeleteModel model, string token)
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
            Lane lane = _readOnlyRepository.First<Lane>(lane1 => lane1.Id == model.LaneId);
            if (lane == null)
            {
                return new BoardModel
                {
                    ErrorCode = 1,
                    ErrorMessage = ErrorStrings.LaneDoesNotExist
                };
            }      
            Security.IsThisAccountMemberOfThisBoard(editedBoard, accountFromSession);
            lane.IsArchived = model.IsArchived;
            editedBoard.Log = editedBoard.Log + accountFromSession.FirstName + " DeleteLane " + lane.Id + " ";
            _writeOnlyRepository.Update(lane);
            return _mappingEngine.Map<Board, BoardModel>(editedBoard);
        }
        
        [DELETE("boards/delete/{token}")]
        public GetBoardsModel DeleteBoard([FromBody] BoardDeleteModel model, string token)
        {
            var session = Security.VerifiySession(token, _readOnlyRepository);
            if (session == null)
            {
                return new GetBoardsModel
                {
                    ErrorCode = 1,
                    ErrorMessage = ErrorStrings.SessionDoesNotExistOnThisServer
                };
            }
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
            var editedBoard = _readOnlyRepository.First<Board>(board1 => board1.Lanes.Any(lane1 => lane1.Id == lane.Id));
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

            editedBoard.IsArchived = model.IsArchived;
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

        [DELETE("boards/deletecard/{token}")]
        public LaneModel DeleteCard([FromBody] CardDeleteModel model, string token)
        {
            Session session = Security.VerifiySession(token, _readOnlyRepository);
            Security.IsTokenExpired(session);
            Account myAccount = Security.GetAccountFromSession(session, _readOnlyRepository);
            Card card = _readOnlyRepository.First<Card>(card1 => card1.Id == model.CardId);
            if (card == null) throw new BadRequestException("The card you are trying to reach does not exist in this server");
            Lane myLane = _readOnlyRepository.First<Lane>(lane => lane.Cards.Contains(card));
            Board myBoard = _readOnlyRepository.First<Board>(board => board.Lanes.Contains(myLane));
            myBoard.Log = myBoard.Log + myAccount.FirstName + " deleteCard " + card.Id.ToString(CultureInfo.InvariantCulture) + " ";
            _writeOnlyRepository.Update(myBoard);
            _writeOnlyRepository.Archive(card);
            return _mappingEngine.Map<Lane, LaneModel>(myLane);
        }

        
       
        [PUT("boards/addmember/{accessToken}")]
        public BoardModel AddMember([FromBody]AddMemberBoardModel model,string accessToken)
        {
            Account memberToAdd = _readOnlyRepository.GetById<Account>(model.MemberID);
            Board board = _readOnlyRepository.GetById<Board>(model.BoardID);
            Session session = Security.VerifiySession(accessToken,_readOnlyRepository);
            Security.IsTokenExpired(session);
            Account myAccount = Security.GetAccountFromSession(session,_readOnlyRepository);
            board.Log = board.Log + myAccount.FirstName + " addMember " + memberToAdd.FirstName + " ";
            board.AddMemberAccount((memberToAdd));
            Board updatedBoard = _writeOnlyRepository.Update(board);
            BoardModel boardModel = _mappingEngine.Map<Board, BoardModel>(updatedBoard);
            
            return boardModel;
        }
       
        [POST("boards/create/{token}")]
        public GetBoardsModel CreateBoard([FromBody]BoardCreateModel model, string token)
        {
            Session session = Security.VerifiySession(token,_readOnlyRepository);
            Security.IsTokenExpired(session);
            Board newBoard = _mappingEngine.Map<BoardCreateModel, Board>(model);
            Account myAccount =
                Security.GetAccountFromSession(session, _readOnlyRepository);
            myAccount.AddBoard(newBoard);
            newBoard.Log = newBoard.Log + myAccount.FirstName + " create ";
            _writeOnlyRepository.Update(myAccount);

            myAccount = Security.GetAccountFromSession(session, _readOnlyRepository);

            GetBoardsModel myModel = new GetBoardsModel();
            foreach (var board in myAccount.Boards)
            {
                BoardModel myBoardModel = _mappingEngine.Map<Board, BoardModel>(board);
                myModel.AddBoard(myBoardModel);
            }
            return myModel;
        }

        [GET("boards/{boardId}/{token}")]
        public BoardModel GetBoard(long boardId,string token)
        {
            Session session = Security.VerifiySession(token, _readOnlyRepository);
            Security.IsTokenExpired(session);
            Account myAccount = Security.GetAccountFromSession(session, _readOnlyRepository);
            Board myBoard = _readOnlyRepository.GetById<Board>(boardId);
            Security.IsThisAccountMemberOfThisBoard(myBoard,myAccount);
            BoardModel boardModel = _mappingEngine.Map<Board,BoardModel>(myBoard);
            
           

            return boardModel;
        }

        [GET("boards/{boardId}/log/{token}")]
        public BoardLogModel GetBoardLog(long boardId, string token)
        {
            Session session = Security.VerifiySession(token, _readOnlyRepository);
            Security.IsTokenExpired(session);
            Account myAccount = Security.GetAccountFromSession(session, _readOnlyRepository);
            Board myBoard = _readOnlyRepository.GetById<Board>(boardId);
            Security.IsThisAccountMemberOfThisBoard(myBoard, myAccount);
            BoardLogModel boardModel = _mappingEngine.Map<Board, BoardLogModel>(myBoard);
            

            return boardModel;
        }

        [GET("boards/members/{boardId}/{token}")]
        public MembersModel GetBoardMembers(long boardId, string token)
        {
            Session session = Security.VerifiySession(token, _readOnlyRepository);
            Security.IsTokenExpired(session);
            Account myAccount = Security.GetAccountFromSession(session, _readOnlyRepository);
            Board myBoard = _readOnlyRepository.GetById<Board>(boardId);
            Security.IsThisAccountMemberOfThisBoard(myBoard, myAccount);
            List<Account> myMemberList = myBoard.MemberAccounts.ToList();
            List<string> myMemberNamesList = myMemberList.Select(account => (account.FirstName) + " " + (account.LastName)).ToList();
            JavaScriptSerializer oSerializer = new JavaScriptSerializer();
            MembersModel myModel = new MembersModel 
                {MembersList = oSerializer.Serialize(myMemberNamesList)};
            return myModel;
        }
    }

    public class BoardLogModel
    {
        public string Log { set; get; }
    }

    public class CardMoveModel
    {
        public long DestinationId { set; get; }
        public long CardId { set; get; }
    }

    public class MembersModel
    {
        public string MembersList { set; get; }
    }
}