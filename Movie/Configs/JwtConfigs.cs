namespace Movie.Configs
{
    public class JwtConfigs
    {
        public string SecretKey { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int Expire { get; set; }
    }

}
