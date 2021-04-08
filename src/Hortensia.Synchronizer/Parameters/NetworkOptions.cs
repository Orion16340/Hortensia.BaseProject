namespace Hortensia.Synchronizer.Parameters
{
    public class NetworkOptions
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
