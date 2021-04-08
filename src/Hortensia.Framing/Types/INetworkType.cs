using Hortensia.Framing.IO;

namespace Hortensia.Framing.Types
{
    public interface INetworkType
    {
        short TypeId { get; }
        void Serialize(ICustomDataWriter writer);
        void Deserialize(ICustomDataReader reader);
    }
}
