namespace Movie.Settings
{
    public static class FileSettings
    {
        public const string ImagePath = "/assets/images/games";
        public static List<string> AllowedExtensions = new List<string>{ ".jpg", ".jpeg", ".png" };

        public const int MaxFileSizeInMB = 2;
        public const int MaxFileSizeInBytes = MaxFileSizeInMB * 1024 * 1024;
    }
}
