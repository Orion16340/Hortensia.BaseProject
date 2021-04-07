using Hortensia.ORM.Attributes;
using Hortensia.ORM.Interfaces;
using Hortensia.Protocol.Custom;
using System;
using System.Collections.Generic;

namespace Hortensia.Synchronizer.Records
{
    [Table("accounts")]
    public class Account : IRecord
    {
        [Container]
        public static List<Account> Accounts = new();

        [Primary]
        public int Id { get; }

        public string Username { get; set; }
        public string Password { get; set; }

        [Update]
        public RoleEnum Role { get; set; }

        [Update]
        public DateTime SubscriptionEndDate { get; set; }

        [Update]
        public DateTime BanEndDate { get; set; }

        [NotMapped]
        public bool IsBanned => BanEndDate > DateTime.Now;

        [NotMapped]
        public bool IsPremium => SubscriptionEndDate > DateTime.Now;

        [NotMapped]
        public bool HasRights => Role > RoleEnum.PLAYER;
    }
}
