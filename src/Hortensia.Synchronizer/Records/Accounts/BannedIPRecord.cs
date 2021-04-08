using Hortensia.ORM.Attributes;
using Hortensia.ORM.Interfaces;
using System.Collections.Generic;

namespace Hortensia.Synchronizer.Records.Accounts
{
    [Table("banned_ips")]
    public class BannedIPRecord : IRecord
    {
        [Container]
        public static List<BannedIPRecord> BannedIPS = new();

        [Primary]
        public int Id { get; set; }

        [Update]
        public string IP { get; set; }
    }
}
