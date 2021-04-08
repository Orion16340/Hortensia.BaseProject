using Hortensia.ORM.Attributes;
using Hortensia.ORM.Interfaces;
using Hortensia.Protocol.Custom;
using Hortensia.Synchronizer.Records.World;
using System;
using System.Collections.Generic;

namespace Hortensia.Synchronizer.Records.Accounts
{
    [Table("accounts")]
    public class AccountRecord : IRecord
    {
        [Container]
        public static List<AccountRecord> Accounts = new();

        [Primary]
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Nickname { get; set; }

        [Update]
        public RoleEnum Role { get; set; }

        public DateTime CreationTime { get; set; }

        [Update]
        public bool IsBanned { get; set; }
        [Update]
        public bool IsBannedForLife { get; set; }
        [Update]
        public DateTime BanEndTime { get; set; }

        [Update]
        public DateTime EndSubscriptionTime { get; set; }

        [Update]
        public string LastIP { get; set; }
        public string SecretQuestion { get; set; }
        public string SecretAnswer { get; set; }

        [Update]
        public int Tokens { get; set; }

        [Update]
        public sbyte CharacterSlots { get; set; }

        [Update]
        public bool IsConnected { get; set; }

        [Update]
        public string Ticket { get; set; }

        [Update]
        public string HardwareId { get; set; }

        [Update]
        public int LastConnectionWorld { get; set; }

        [NotMapped]
        public bool IsPremium => EndSubscriptionTime > DateTime.Now;

        [NotMapped]
        public bool HasRights => Role > RoleEnum.PLAYER;

        [NotMapped]
        public List<WorldCharactersRecord> WorldCharacters { get; set; }
    }
}
