namespace Angelo.Common.Messaging
{
    public class SmtpOptions
    {
        public string Server { get; set; } = string.Empty;
        public int Port { get; set; } = 25;

        public bool RequireSsl { get; set; } = false;
        public bool RequireAuthentication { get; set; } = false;

        public string DefaultReplyToAddress { get; set; }
        public string DefaultFromAddress { get; set; }

        public SmtpCredentials Credentials { get; set; }

        public class SmtpCredentials
        {
            public string Username { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }
    }
}