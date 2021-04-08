using Hortensia.ORM.Attributes;
using Hortensia.ORM.Interfaces;
using System.Collections.Generic;

namespace Hortensia.Synchronizer.Records.World
{
    [Table("Worlds")]
    public class WorldRecord : IRecord
    {
        [Container]
        public static List<WorldRecord> Worlds = new();

        [Primary]
        public int Id { get; set; }
        public string Name { get; set; }
        public ushort Port { get; set; }
        public string Address { get; set; }
        /*public GameServerTypeEnum Type { get; set; }
        public bool RequireSubscription { get; set; }
        public RoleEnum RequireRole { get; set; }
        public ServerCompletionEnum Completion { get; set; }*/

        [Update]
        public bool ServerSelectable { get; set; }
        public int CharsCapacity { get; set; }

        [Update]
        public int CharactersCount { get; set; }

        /*[Update]
        public ServerStatusEnum Status { get; set; }

        [NotMapped]
        public bool Connected => Status == ServerStatusEnum.ONLINE;*/

        public override string ToString()
            => $"{Name}({Id})";
    }
}
