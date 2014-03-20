using System.Net.Sockets;

namespace MiniTrello.Domain.DataObjects
{
    public class LaneCreateModel
    {
        public long BoardId { set; get; }
        public string Name { set; get; }
    }
}