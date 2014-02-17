using System.Linq;
using System.Net;
using System.Net.Http;
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
        

        /*[POST("board/rename/{id}")]
        public HttpResponseMessage RenameBoard([FromBody] BoardChangeTitleModel model, long id)
        {
            Board board = _mappingEngine.Map<BoardChangeTitleModel, Board>(model);
            return null;
        }*/
        [POST("/boards/rename/{token}")]
        public HttpResponseMessage RenameBoard([FromBody] BoardChangeTitleModel boardRenameModel, string token)
        {
            Session session = _readOnlyRepository.First<Session>(session1 => session1.Token == token);
            if (session.SessionAccount != null)
            {
                Security.IsTokenExpired(session);
                Account myAccount =
                    _readOnlyRepository.First<Account>(account1 => account1.Email == session.SessionAccount.Email);
                Board editedBoard = _readOnlyRepository.First<Board>(board => board.Id == boardRenameModel.Id);
                if (!isThisAccountAdminOfThisBoard(editedBoard, myAccount)) throw new BadRequestException("You do not posses Administrative priviledges on this board");
                editedBoard.Title = boardRenameModel.Title;
                _writeOnlyRepository.Update(editedBoard);
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            throw new BadRequestException("Session you are trying to reach does not exist in this server");
        }

        private bool isThisAccountAdminOfThisBoard(Board board, Account account)
        {
            return board.AdminAccounts.Any(adminAccount => adminAccount.Email == account.Email);
        }
        [PUT("boards/addmember/{accessToken}")]
        public BoardModel AddMember([FromBody]AddMemberBoardModel model,string accessToken)
        {
            Account memberToAdd = _readOnlyRepository.GetById<Account>(model.MemberID);
            Board board = _readOnlyRepository.GetById<Board>(model.BoardID);
            
            board.AddMember((memberToAdd));
            Board updatedBoard = _writeOnlyRepository.Update(board);
            BoardModel boardModel = _mappingEngine.Map<Board, BoardModel>(updatedBoard);
            return boardModel;
        }

        [POST("boards/create/{token}")]
        public HttpResponseMessage AddBoard([FromBody] BoardCreateModel model, string token)
        {
            //Session session = _readOnlyRepository.First(token);
            return null;
        }
    }
}