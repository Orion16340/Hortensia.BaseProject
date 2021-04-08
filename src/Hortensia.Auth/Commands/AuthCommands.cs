using Hortensia.Auth.Managers;
using Hortensia.Core;
using Hortensia.Protocol.Custom;
using Hortensia.Synchronizer.Commands;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace Hortensia.Auth.Commands
{
    public class AuthCommands
    {
        [ConsoleCommand("account")]
        public static void CreateAccountCommand(string param, string username, string password, string role)
        {
            var accountManager = ServiceLocator.Provider.GetService<AccountManager>();
            var logger = ServiceLocator.Provider.GetService<ILogger>();

            var finalRole = Enum.Parse<RoleEnum>(role);
            var allRoles = Enum.GetValues<RoleEnum>().ToList();

            if (param == "create" || param == "new")
            {
                if (accountManager.UsernameExists(username))
                {
                    logger.LogWarning("username already exists..");
                    return;
                }
                
                else if (accountManager.NicknameExists(username))
                {
                    logger.LogWarning("username already exists..");
                    return;
                }

                else if (!allRoles.Contains(finalRole))
                {
                    logger.LogWarning($"role {finalRole} dont't exists in RoleEnum..");
                    return;
                }

                try
                {
                    accountManager.CreateAccount(username, password, finalRole);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex.Message);
                }
            }
        }
    }
}
