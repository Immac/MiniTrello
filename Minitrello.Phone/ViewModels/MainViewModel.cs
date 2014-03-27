using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
//using System.Web.Script.Serialization;
using Minitrello.Phone.Models;
using Newtonsoft.Json;
using RestSharp;

namespace Minitrello.Phone.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private string _sampleProperty = "Sample Runtime Property Value";
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding
        /// </summary>
        /// <returns></returns>
        public string SampleProperty
        {
            get
            {
                return _sampleProperty;
            }
            set
            {
                if (value != _sampleProperty)
                {
                    _sampleProperty = value;
                    NotifyPropertyChanged("SampleProperty");
                }
            }
        }

        public ObservableCollection<ItemViewModel> Items { get; private set; }
        public ObservableCollection<LaneModel> LaneModels { get; private set; }
        public ObservableCollection<List<CardModel>> CardModels { get; private set; }
        const string BaseApiUrl = "http://mcminitrelloapi.apphb.com/";

      
        public MainViewModel()
        {
            Items = new ObservableCollection<ItemViewModel>();
            LaneModels = new ObservableCollection<LaneModel>();
            CardModels = new ObservableCollection<List<CardModel>>();
        }

        /// <summary>
        /// A collection for ItemViewModel objects.
        /// </summary>
        

        public bool IsLogedIn
        {
            get;
            private set;
        }

        /// <summary>
        /// Creates and adds a few ItemViewModel objects into the Items collection.
        /// </summary>
        public void Login(AccountLoginModel loginModel)
        {
            if (IsLogedIn == false)
            {
                Items.Add(new ItemViewModel()
                {
                    ID = "0",
                    LineOne = "Please Wait...",
                    LineTwo = "Please wait while the server is contacted.",
                    LineThree = null
                });
                var client = new RestClient(BaseApiUrl);
                var request = new RestRequest("/login", Method.POST);
                request.RequestFormat = DataFormat.Json;
                request.AddBody(loginModel);

                var asyncHandler = client.ExecuteAsync<AccountAuthenticationModel>(request, r =>
                {
                    if (r.ResponseStatus == ResponseStatus.Completed)
                    {
                        if (r.Data != null)
                        {
                            App.Token = r.Data.Token;
                            Items.Clear();
                            IsLogedIn = true;

                        }
                    }
                });
            }
        }

        public void Boards()
        {
            Items.Clear();
            Items.Add(new ItemViewModel()
            {
                ID = "0",
                LineOne = "Please Wait...",
                LineTwo = "Please wait while the server is contacted.",
                LineThree = null
            });
            var client = new RestClient(BaseApiUrl);
            var request = new RestRequest("/boards", Method.GET);
            request.RequestFormat = DataFormat.Json;
            
            var asyncHandler = client.ExecuteAsync<GetBoardsModel>(request, r =>
            {
                if (r.ResponseStatus == ResponseStatus.Completed)
                {
                    if (r.Data != null)
                    {
                        var id = 0;
                        var boards = r.Data.Boards;
                        Items.Clear();
                        foreach (var board in boards)
                        {
                            Items.Add(new ItemViewModel
                            {
                                ID = (id++).ToString(),
                                LineOne = board.Title,
                                LineTwo = board.Id.ToString(),
                                LineThree = board.MemberAccounts.ToString()
                            }
                                );
                        }
                        IsLogedIn = true;
                    }
                }
            });
        }
        private void webClient_DownloadCatalogCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                Items.Clear();
                if (e.Result != null)
                {
                    
                    var boards = JsonConvert.DeserializeObject<GetBoardsModel>(e.Result);
                    int id = 0;
                    foreach (BoardModel board in boards.Boards)
                    {
                        Items.Add(new ItemViewModel()
                        {
                            ID = (id++).ToString(),
                            //LineOne = board.Title,
                            LineTwo = "Got here"
                            //LineThree = board.Description.Replace("\n", " ")
                        });
                    }
                    IsLogedIn = true;
                   
                }
            }
            catch (Exception ex)
            {
                Items.Add(new ItemViewModel()
                {
                    ID = "0",
                    LineOne = "An Error Occurred",
                    LineTwo = String.Format("The following exception occured: {0}", ex.Message),
                    LineThree = String.Format("Additional inner exception information: {0}", ex.InnerException.Message)
                });
            }
        }

        public void LoadBoards()
        {
            Items.Clear();
            Items.Add(new ItemViewModel
            {
                ID = "0",
                LineOne = "Please Wait...",
                LineTwo = "Please wait while the server is contacted.",
                LineThree = null
            });
            var client = new RestClient(BaseApiUrl);
            var request = new RestRequest("/boards/" + App.Token , Method.GET);
            request.RequestFormat = DataFormat.Json;
            request.OnBeforeDeserialization = resp => { resp.ContentType = "application/json"; }; //HUH?
            var asyncHandler = client.ExecuteAsync<GetBoardsModel>(request, r =>
            {
                if (r.ResponseStatus == ResponseStatus.Completed)
                {
                    if (r.Data != null)
                    {
                        var jsonData = r.Content;
                        var allBoardsModel = JsonConvert.DeserializeObject<GetBoardsModel>(jsonData);
                        var id = 0;
                        var boards = allBoardsModel.Boards; 
                        Items.Clear();
                        foreach (var board in boards)
                        {
                            Items.Add(new ItemViewModel
                            {
                                ID = (id++).ToString(),
                                LineOne = board.Title,
                                LineTwo = board.Id.ToString(),
                                Lanes = board.Lanes
                                
                            }
                                );
                        }
                        if (id == 0)
                        {
                            Items.Add(new ItemViewModel
                            {
                                ID = "0",
                                 LineOne = "Nothing here."
                            });
                        }

                    }
                }
            });
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void LoadDetails(int id)
        {
            LaneModels.Clear();
            CardModels.Clear();
            var Name = Items[id].LineOne;
            var lanes = Items[id].Lanes;
            if (lanes != null)
            {
                foreach (var lane in lanes)
                {
                    LaneModels.Add(lane);
                    CardModels.Add(lane.Cards);
                }
            }

        }
                
        }
    }




/*var request = HttpWebRequest.Create(new Uri("http://mcminitrelloapi.apphb.com/boards/123166140046156128196152082048140224004251126035"));
                request.Method = "GET";
                request.Headers["Content-Type"] = "application/json";*/
//WebResponse response = request.BeginGetResponse(Callback, new object());
/*
    Items.Clear();
                
var webClient = new WebClient();
webClient.Headers = request.
webClient.Headers["Get"] = "application/json";
webClient.DownloadStringCompleted += webClient_DownloadCatalogCompleted;
webClient.DownloadStringAsync(new Uri(BaseApiUrl)); 
*/