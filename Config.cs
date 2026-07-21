namespace PalworldManager
{
    public class Config
    {
        public string ServerIP { get; set; } = "";
        public string ServerPort { get; set; } = "";
        public string ServerPassword { get; set; } = "";
        public string ServerRESTPort { get; set; } = "";

        public string ServerPath { get; set; } = "";
        public string ServerParams { get; set; } = "";

        public bool Debugging { get; set; } = false;

        public bool LogToFile { get; set; } = false;

        public int WaitForPlayerTimeout { get; set; } = 300; // in seconds

        public Config()
        {
        }

        public void Save()
        {
            var configJson = System.Text.Json.JsonSerializer.Serialize(this, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText("config.json", configJson);
        }

        public void Load()
        {
            if (!File.Exists("config.json"))
            {
                Save();
                throw new Exception("config.json not found, created a new one. Please fill it out and restart the program.");
            }
            var configJson = System.IO.File.ReadAllText("config.json");
            var config = System.Text.Json.JsonSerializer.Deserialize<Config>(configJson);
            if (config == null)
            {
                throw new Exception("Failed to load config.json");
            }
            ServerIP = config.ServerIP;
            ServerPort = config.ServerPort;
            ServerPassword = config.ServerPassword;
            ServerRESTPort = config.ServerRESTPort;
            ServerPath = config.ServerPath;
            ServerParams = config.ServerParams;
            Debugging = config.Debugging;
            LogToFile = config.LogToFile;
            WaitForPlayerTimeout = config.WaitForPlayerTimeout;
        }
    }
}