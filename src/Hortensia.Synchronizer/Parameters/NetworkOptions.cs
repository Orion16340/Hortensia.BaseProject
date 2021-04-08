namespace Hortensia.Synchronizer.Parameters
{
    public interface INetworkOptions
    {
        int Backlog { get; set; }
        byte[] Buffer { get; set; }
        int BufferEndPosition { get; set; }
        int BufferLength { get; set; }
        string IP { get; set; }
        int MaxConcurrentConnections { get; set; }
        int MaxConnectionsPairIP { get; set; }
        int Port { get; set; }
    }

    public class NetworkOptions : INetworkOptions
    {
        public NetworkOptions()
        {
            Buffer = new byte[BufferLength];
        }

        public string IP { get; set; }
        public int Port { get; set; }
        public int Backlog { get; set; }

        public int MaxConcurrentConnections { get; set; }
        public int MaxConnectionsPairIP { get; set; }

        public byte[] Buffer { get; set; }
        public int BufferLength { get; set; }
        public int BufferEndPosition { get; set; }
    }
}
