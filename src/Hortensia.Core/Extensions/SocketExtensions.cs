using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Hortensia.Core.Extensions
{
    public static class SocketExtensions
    {
        public static Task<Socket> AcceptAsync(this Socket s) =>
            Task.Factory.FromAsync(s.BeginAccept, s.EndAccept, s);

        public static Task<int> ReceiveAsync(this Socket s, byte[] buffer, int offset, int size, SocketFlags sf = SocketFlags.None) =>
            Task.Factory.FromAsync(s.BeginReceive(buffer, offset, size, sf, null, s), s.EndReceive);

        public static Task<int> SendAsync(this Socket s, byte[] buffer, int offset, int size, SocketFlags sf = SocketFlags.None) =>
            Task.Factory.FromAsync(s.BeginSend(buffer, offset, size, sf, null, s), s.EndSend);

        public static Task ConnectAsync(this Socket s, EndPoint endPoint) =>
            Task.Factory.FromAsync(s.BeginConnect(endPoint, null, s), s.EndConnect);
    }
}
