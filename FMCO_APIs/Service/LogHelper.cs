namespace ProductManagment_APIs.Service
{
    public class LogHelper
    {
        private readonly IConfiguration _configuration;

        public LogHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Log(string message)
        {
            WriteLog("INFO", message);
        }

        public void Exception(Exception ex)
        {
            WriteLog("ERROR", ex.Message);
        }

        private void WriteLog(string type, string message)
        {
            if (!Convert.ToBoolean(_configuration["LogSettings:LogEnable"]))
                return;

            string log = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss},{type},{message}";

            bool isCsv = Convert.ToBoolean(_configuration["LogSettings:IsLogCSV"]);

            if (isCsv)
            {
                string folder = "Logs";

                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                string file = Path.Combine(folder, $"Log_{DateTime.Now:yyyyMMdd}.csv");

                if (!File.Exists(file))
                {
                    File.AppendAllText(file, "DateTime,Type,Message" + Environment.NewLine);
                }

                File.AppendAllText(file, log + Environment.NewLine);
            }
            else
            {
                Console.WriteLine(log);
            }
        }
    }
}