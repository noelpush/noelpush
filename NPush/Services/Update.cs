using System.Net.Http;


namespace NPush.Services
{
    class Update
    {
        private string version;

        public Update()
        {
            this.version = this.GetVersion();
        }

        private string GetVersion()
        {
            var url = "http://choco.ovh/npush/update.php";
            var response = new HttpClient().GetStringAsync(url);

            return response.Result;
        }

        public bool CheckVersion()
        {
            return this.version == Properties.Settings.Default.version;
        }

        public void DoUpdate()
        {
            var urlExe = "http://choco.ovh/npush/npush " + this.version + ".exe";
        }
    }
}
