using System.Net.Http;
using System.Web.Http;
using AttributeRouting.Web.Http;
using AutoMapper;
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
        public HttpResponseMessage RenameBoard([FromBody] ChangeBoardTitleModel model, long id)
        {
            Board board = _mappingEngine.Map<ChangeBoardTitleModel, Board>(model);
            return null;
        }*/

        [PUT("boards/addmember/{accessToken}")]
        public BoardModel AddMember([FromBody]AddMemberBoardModel model,string accessToken)
        {
            //validar seguridad
            var memberToAdd = _readOnlyRepository.GetById<Account>(model.MemberID);
            var board = _readOnlyRepository.GetById<Board>(model.BoardID);
            
            board.AddMember((memberToAdd));
            var updatedBoard = _writeOnlyRepository.Update(board);
            var boardModel = _mappingEngine.Map<Board, BoardModel>(updatedBoard);
            return boardModel;
        }
    }

    public class AddMemberBoardModel
    {
        public long MemberID { get; set; }
        public long BoardID { get; set; }
    }

    public class BoardModel
    {
        public string Title { get; set; }
    }
}