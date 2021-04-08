using Hortensia.Auth.Network;
using Hortensia.Core;
using Hortensia.Core.Extensions;
using Hortensia.Core.Threads;
using Hortensia.ORM;
using Hortensia.ORM.Tables.IO;
using Hortensia.Protocol.Custom;
using Hortensia.Synchronizer.Records.Accounts;
using Hortensia.Synchronizer.Records.World;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Hortensia.Auth.Managers
{
    public class AccountManager
    {
        public List<AccountRecord> Accounts => AccountRecord.Accounts;
        private AsyncRandom _random { get; set; }
        private readonly ILogger _logger;

        public AccountManager(ILogger logger)
        {
            _random = new();
            _logger = logger;
        }

        public void CreateAccount(string username, string password, RoleEnum role)
        {
            var account = new AccountRecord
            {
                Id = Accounts.DynamicPop(),
                Username = username,
                Password = password,
                Nickname = username,
                Role = role,
                BanEndTime = DateTime.Now,
                CreationTime = DateTime.Now,
                EndSubscriptionTime = DateTime.Now,
                CharacterSlots = 0,
                HardwareId = string.Empty,
                IsBanned = false, 
                IsBannedForLife = false,
                IsConnected = false, 
                LastConnectionWorld = 0,
                LastIP = string.Empty,
                SecretAnswer = "Oui",
                SecretQuestion = "Supprimer ce personnage ?",
                Ticket = string.Empty,
                Tokens = 0
            };

            account.AddInstantElement();

            _logger.LogInformation($"Account => Username = {account.Username} | Password = {account.Password} | Role = {account.Role}\n");
        }

        public AccountRecord GetAccount(int id)
        {
            return DatabaseReader.ReadFirst<AccountRecord>("Id", id.ToString());
        }

        public AccountRecord GetAccount(string username)
        {
            return DatabaseReader.ReadFirst<AccountRecord>("Username", username);
        }

        public bool UsernameExists(string username)
        {
            return DatabaseReader.ReadFirst<AccountRecord>("Username", username) != null;
        }

        public bool NicknameExists(string nickname)
        {
            return DatabaseReader.ReadFirst<AccountRecord>("Nickname", nickname) != null;
        }

        public void AddBanTime(AccountRecord account, int days, int hours, int min, int sec)
        {
            account.BanEndTime.Add(new(days, hours, min, sec));
            account.UpdateInstantElement();
        }

        public void AddSubscriptionTime(AccountRecord account, int days, int hours, int min, int sec)
        {
            account.EndSubscriptionTime.Add(new(days, hours, min, sec));
            account.UpdateInstantElement();
        }

        public sbyte GetCharactersCountByWorld(AuthClient client, WorldRecord world)
            => (sbyte)client.Account.WorldCharacters.Count(x => x.ServerId == world.Id);
    }
}
