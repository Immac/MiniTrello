using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Script.Serialization;
using AttributeRouting.Web.Http;
using AutoMapper;
using MiniTrello.Api.Controllers.Helpers;
using MiniTrello.Api.CustomExceptions;
using MiniTrello.Api.Models;
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
        public HttpResponseMessage RenameBoard([FromBody] BoardChangeTitleModel boardRenameModel, string token)
        {
            Session session = Security.VerifiySession(token, _readOnlyRepository);
            Security.IsTokenExpired(session);
            var myAccount =
                Security.GetAccountFromSession(session, _readOnlyRepository);
            Board editedBoard = _readOnlyRepository.First<Board>(board => board.Id == boardRenameModel.Id);
            Security.IsThisAccountAdminOfThisBoard(editedBoard, myAccount);
            editedBoard.Title = boardRenameModel.Title;
            _writeOnlyRepository.Update(editedBoard);
            return new HttpResponseMessage(HttpStatusCode.OK);
        }
        
        [POST("boards/createcard/{token}")]
        public HttpResponseMessage CreateCard([FromBody]CardCreateModel model,string token)
        {
            Session session = Security.VerifiySession(token, _readOnlyRepository);
            Security.IsTokenExpired(session);
            var account = Security.GetAccountFromSession(session, _readOnlyRepository);
            var lane = _readOnlyRepository.GetById<Lane>(model.LaneId);
            var editedBoard = _readOnlyRepository.First<Board>(board1 => board1.Lanes.Any(lane1 => lane1.Id == lane.Id));
            var card = _mappingEngine.Map<CardCreateModel, Card>(model);
            Security.IsThisAccountMemberOfThisBoard(editedBoard, account);
            lane.AddCard(card);
            _writeOnlyRepository.Update(editedBoard);

        return new HttpResponseMessage(HttpStatusCode.OK);
        }

        [POST("boards/createlane/{token}")]
        public HttpResponseMessage CreateLane([FromBody]LaneCreateModel model,string token)
        {
            Session session = Security.VerifiySession(token, _readOnlyRepository);
            Security.IsTokenExpired(session);
            var myAccount = Security.GetAccountFromSession(session, _readOnlyRepository);
            var editedBoard = _readOnlyRepository.GetById<Board>(model.BoardId);
            Security.IsThisAccountMemberOfThisBoard(editedBoard,myAccount);
            Lane newLane = _mappingEngine.Map<LaneCreateModel,Lane>(model);
            editedBoard.AddLane(newLane);
            _writeOnlyRepository.Update(editedBoard);
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        [POST("boards/movecard/{token}")]
        public HttpResponseMessage MoveCard([FromBody] CardMoveModel model, string token)
        {
            Session session = Security.VerifiySession(token, _readOnlyRepository);
            Security.IsTokenExpired(session);
            Account myAccount = Security.GetAccountFromSession(session, _readOnlyRepository);
            Card card = _readOnlyRepository.First<Card>(card1 => card1.Id == model.CardId);
            Lane newLane = _readOnlyRepository.GetById<Lane>(model.DestinationId);
            Lane oldLane = _readOnlyRepository.First<Lane>(lane => lane.Cards.Contains(card));
            Board board =
                _readOnlyRepository.First<Board>(board1 => board1.Lanes.Contains(newLane) && board1.Lanes.Contains(oldLane));
            if (board == null) throw new BadRequestException("You can't transfer cards over boards");

            newLane.AddCard(card);
            oldLane.RemoveCard(card);
            _writeOnlyRepository.Update(myAccount);
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        [DELETE("boards/deletelane/{token}")]
        public HttpResponseMessage DeleteLane([FromBody] LaneDeleteModel model, string token)
        {
            Session session = Security.VerifiySession(token, _readOnlyRepository);
            Security.IsTokenExpired(session);
            Account myAccount = Security.GetAccountFromSession(session, _readOnlyRepository);
            Lane lane = _readOnlyRepository.First<Lane>(lane1 => lane1.Id == model.LaneId);
            if (lane == null) throw new BadRequestException("Lane Id does not match any existing lanes");
            Board editedBoard = _readOnlyRepository.First<Board>(board1 => board1.Lanes.Contains(lane));
            if (editedBoard == null) throw new BadRequestException("This lane is not owned by any board");
            Security.IsThisAccountMemberOfThisBoard(editedBoard, myAccount);
            lane.IsArchived = model.IsArchived;
            _writeOnlyRepository.Update(lane);
            return new HttpResponseMessage(HttpStatusCode.OK);
        }
        
        //Can restore files too by setting "IsArchived to false"
        [DELETE("boards/delete/{token}")]
        public HttpResponseMessage DeleteBoard([FromBody] BoardDeleteModel model, string token)
        {
            Session session = Security.VerifiySession(token, _readOnlyRepository);
            Security.IsTokenExpired(session);
            Account myAccount =
                Security.GetAccountFromSession(session, _readOnlyRepository);
            Board editedBoard = _readOnlyRepository.First<Board>(board => board.Id == model.Id);
            if (editedBoard == null) throw new BadRequestException("The board you are trying to reach does not exist in this server");

            Security.IsThisAccountAdminOfThisBoard(editedBoard, myAccount);
            editedBoard.IsArchived = model.IsArchived;
            _writeOnlyRepository.Update(editedBoard);
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        [DELETE("boards/deleteCard/{token}")]
        public HttpResponseMessage DeleteCard([FromBody] CardDeleteModel model, string token)
        {
            Session session = Security.VerifiySession(token, _readOnlyRepository);
            Security.IsTokenExpired(session);
            Account myAccount = Security.GetAccountFromSession(session, _readOnlyRepository);
            Card card = _readOnlyRepository.First<Card>(card1 => card1.Id == model.CardId);
            if (card == null) throw new BadRequestException("The card you are trying to reach does not exist in this server");
            _writeOnlyRepository.Archive(card);
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        
       
        [PUT("boards/addmember/{accessToken}")]
        public BoardModel AddMember([FromBody]AddMemberBoardModel model,string accessToken)
        {
            Account memberToAdd = _readOnlyRepository.GetById<Account>(model.MemberID);
            Board board = _readOnlyRepository.GetById<Board>(model.BoardID);
            board.AddMemberAccount((memberToAdd));
            Board updatedBoard = _writeOnlyRepository.Update(board);
            BoardModel boardModel = _mappingEngine.Map<Board, BoardModel>(updatedBoard);
            return boardModel;
        }
       
        [POST("boards/create/{token}")]
        public HttpResponseMessage CreateBoard([FromBody]BoardCreateModel model, string token)
        {
            Session session = Security.VerifiySession(token,_readOnlyRepository);
            Security.IsTokenExpired(session);
            Board newBoard = _mappingEngine.Map<BoardCreateModel, Board>(model);
            Account myAccount =
                Security.GetAccountFromSession(session, _readOnlyRepository);
            myAccount.AddBoard(newBoard);
            _writeOnlyRepository.Update(myAccount);
            return new HttpResponseMessage(HttpStatusCode.OK);   
            
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