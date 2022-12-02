namespace Settings
{
    public class MONGODBSETTINGS
    {
        public string HOST { get; set; }
        public int PORT { get; set; }
        public string USER { get; set; }
        public string PASSWORD { get; set; }
        public string DB {get; set;}

        public string ConnectionString
        {
            get
            {
                var cs = $"mongodb://{USER}:{PASSWORD}@{HOST}:{PORT}/{DB}";
                Console.WriteLine(cs);
                return cs;
            }
        }
    }
}