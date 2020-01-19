namespace Auth.Email
{
    public class EmailOptions
    {
        public string From { get; set; }
        public SmtpOptions Smtp { get; set; }
    }
}
