using System.Collections.Generic;
using System.Configuration;
using MiniTrello.Domain.DataObjects;
using RestSharp;

namespace MiniTrello.ApiWrapper
{
    public class MiniTrelloSdk
    {
        private static RestRequest InitRequest(string resource, Method method,object payload)
        {
            var request = new RestRequest(resource, method);
            request.AddHeader("Content-Type", "application/json");
            if (payload != null) request.AddBody(payload);

            return request;
        }

        //Account Controls
        public static AccountAuthenticationModel Login(AccountLoginModel loginModel)
        {   
                var client = new RestClient(BaseUrl);
            
                var request = InitRequest("/login", Method.POST, loginModel);
                IRestResponse<AccountAuthenticationModel> response = client.Execute<AccountAuthenticationModel>(request);
                ConfigurationManager.AppSettings["accessToken"] = response.Data.Token;
                return response.Data;
        }
        public static RegisterConfirmationModel Register(AccountRegisterModel registerModel)
        {
                var client = new RestClient(BaseUrl);
                var request = InitRequest("/register", Method.POST, registerModel);
                IRestResponse<RegisterConfirmationModel> response = client.Execute<RegisterConfirmationModel>(request);
                return response.Data;
        }

        public static GetBoardsModel GetBoards()
        {
            var token = ConfigurationManager.AppSettings.Get("accessToken");
            var client = new RestClient(BaseUrl);
            var request = InitRequest("/boards/" + token, Method.GET,null);
            IRestResponse<GetBoardsModel> response = client.Execute<GetBoardsModel>(request);
            return response.Data;
        }

        //Board Controls
        public static BoardModel RenameBoard(BoardChangeTitleModel changeTitleModel)
        {
            var token = ConfigurationManager.AppSettings.Get("accessToken");
            var client = new RestClient(BaseUrl);
            var request = InitRequest("/boards/rename/" + token, Method.POST, changeTitleModel);
            var response = client.Execute<BoardModel>(request);
            return response.Data;
        }

        public static LaneModel CreateLane(LaneCreateModel laneCreateModel)
        {
            var token = ConfigurationManager.AppSettings.Get("accessToken");
            var client = new RestClient(BaseUrl);
            var request = InitRequest("/boards/createLane/" + token, Method.POST, laneCreateModel);
            var response = client.Execute<LaneModel>(request);
            return response.Data;
        }

        public static BoardModel MoveCard(CardMoveModel cardMoveModel)
        {
            var token = ConfigurationManager.AppSettings.Get("accessToken");
            var client = new RestClient(BaseUrl);
            var request = InitRequest("boards/deleteLane/" + token, Method.DELETE, cardMoveModel);
            var response = client.Execute<BoardModel>(request);
            return response.Data;
        }
        public static BoardModel DeleteLane(LaneDeleteModel laneDeleteModel)
        {
            var token = ConfigurationManager.AppSettings.Get("accessToken");
            var client = new RestClient(BaseUrl);
            var request = InitRequest("boards/deletelane/" + token, Method.POST, laneDeleteModel);
            var response = client.Execute<BoardModel>(request);
            return response.Data;
        }

        public static GetBoardsModel DeleteBoard(BoardDeleteModel boardDeleteModel)
        {
            var token = ConfigurationManager.AppSettings.Get("accessToken");
            var client = new RestClient(BaseUrl);
            var request = InitRequest("boards/delete" + token, Method.DELETE, boardDeleteModel);
            var response = client.Execute<GetBoardsModel>(request);
            return response.Data;
        }

        public static LaneModel DeleteCard(CardDeleteModel cardDeleteModel)
        {
            var token = ConfigurationManager.AppSettings.Get("accessToken");
            var client = new RestClient(BaseUrl);
            var request = InitRequest("boards/deletecard/" + token, Method.DELETE, cardDeleteModel);
            var response = client.Execute<LaneModel>(request);
            return response.Data;
        }

        public static BoardModel AddMember(AddMemberBoardModel addMemberBoardModel)
        {
            var token = ConfigurationManager.AppSettings.Get("accessToken");
            var client = new RestClient(BaseUrl);
            var request = InitRequest("boards/addmember/" + token, Method.PUT, addMemberBoardModel);
            var response = client.Execute<BoardModel>(request);
            return response.Data;
        }

        public static GetBoardsModel CreateBoard(BoardCreateModel boardCreateModel)
        {
            var token = ConfigurationManager.AppSettings.Get("accessToken");
            var client = new RestClient(BaseUrl);
            var request = InitRequest("boards/create/" + token, Method.POST, boardCreateModel);
            var response = client.Execute<GetBoardsModel>(request);
            return response.Data;
        }

        public static GetBoardsModel GetBoard(long boardId)
        {
            var token = ConfigurationManager.AppSettings.Get("accessToken");
            var client = new RestClient(BaseUrl);
            var request = InitRequest("boards/" + boardId + "/" + token, Method.GET, null);
            var response = client.Execute<GetBoardsModel>(request);
            return response.Data;
        }

        public static GetBoardsModel GetBoardMembers(long boardId)
        {
            var token = ConfigurationManager.AppSettings.Get("accessToken");
            var client = new RestClient(BaseUrl);
            var request = InitRequest("boards/members/" + boardId + "/" + token, Method.GET, null);
            var response = client.Execute<GetBoardsModel>(request);
            return response.Data;
        }

        
        

        private static string BaseUrl
        {
            get { return ConfigurationManager.AppSettings["baseUrl"]; }
        }

        public static List<OrganizationModel> GetOrganization()
        {

            return null;
        } 

        
    }
}
