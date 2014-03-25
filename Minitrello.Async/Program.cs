using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Net;
namespace Minitrello.Async
{
    class Program
    {
        static void Main(string[] args)
        {
        }
    }


    public class MiniTrelloAsync
    {
        static HttpWebRequest initRequest(string resource, HttpMethod method)
        {
            var request = HttpWebRequest.Create("http://localhost:1416/");
            return null;
        }
    }

}
