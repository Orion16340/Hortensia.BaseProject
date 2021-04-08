using System;
using System.Net;

namespace Hortensia.Framing.Network
{
    public interface INetworkClient : IDisposable
    {
        string IP { get; }
        IPEndPoint EndPoint { get; }
        bool Connected { get; }
    }
}
