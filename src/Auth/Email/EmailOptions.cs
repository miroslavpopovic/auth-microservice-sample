namespace Auth.Email
{
    public class EmailOptions
    {
        public string FromEmail { get; set; }
        public string FromName { get; set; }
        public SmtpOptions Smtp { get; set; }
    }
}
