﻿using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Web.Http;
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
        
        [POST("/boards/rename/{token}")]
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

        [DELETE("boards/deletelane/{token}")]
        public HttpResponseMessage DeleteLane([FromBody] LaneDeleteModel model, string token)
        {
            Session session = Security.VerifiySession(token, _readOnlyRepository);
            Security.IsTokenExpired(session);
            var myAccount = Security.GetAccountFromSession(session, _readOnlyRepository);
            var lane = _readOnlyRepository.First<Lane>(lane1 => lane1.Id == model.LaneId);
            if (lane == null) throw new BadRequestException("Lane Id does not match any existing lanes");
            var editedBoard = _readOnlyRepository.First<Board>(board1 => board1.Lanes.Contains(lane));
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
            Security.IsThisAccountAdminOfThisBoard(editedBoard, myAccount);
            editedBoard.IsArchived = model.IsArchived;
            _writeOnlyRepository.Update(editedBoard);
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

        [POST("/boards/create/{token}")]
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

    }
}