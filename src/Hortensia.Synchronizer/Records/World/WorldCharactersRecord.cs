using Hortensia.ORM.Attributes;
using Hortensia.ORM.Interfaces;
using System.Collections.Generic;

namespace Hortensia.Synchronizer.Records.World
{
    [Table("world_characters")]
    public class WorldCharactersRecord : IRecord
    {
        [Container]
        public static List<WorldCharactersRecord> WorldCharacters = new();

        [Primary]
        public int Id { get; set; }
        public long CharacterId { get; set; }
        public long AccountId { get; set; }

        [Update]
        public ushort ServerId { get; set; }
    }
}
