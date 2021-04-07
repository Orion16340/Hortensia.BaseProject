using Hortensia.Core.Threads.Timers;
using System;

namespace Hortensia.ORM.Configuration
{
    public class DatabaseConfiguration
    {
        private readonly string host, database, user, password = "<unknown>";

        public string Host
        {
            get => host;
            init => host = (value ?? throw new ArgumentNullException(nameof(host)));
        }
        public string Database
        {
            get => database;
            init => database = value ?? throw new ArgumentNullException(nameof(database));
        }
        public string User
        {
            get => user;
            init => user = (value ?? throw new ArgumentNullException(nameof(user)));
        }
        public string Password
        {
            get => password;
            init => password = (value ?? throw new ArgumentNullException(nameof(password)));
        }

        public TimerConfigurationEntry SaveConfiguration { get; set; }

        public override string ToString()
            => string.Format("Server={0};UserId={1};Password={2};Database={3}", Host, User, Password, Database);
    }
}
