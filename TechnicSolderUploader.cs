using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers; 
using System.Net.Http.Headers; 

namespace TechnicSolderPackager
{
    internal class TechnicSolderUploader
    {

        ChromeDriver driver;
        public HttpClient client = new HttpClient();
        string IP;

        public TechnicSolderUploader(string ip)
        {
            IP = ip;
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            ChromeOptions options = new();
            options.AddArgument("--headless");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-dev-shm-usage");
            driver = new(options);
        }

        public bool RegisterMod(string mod)
        {
            string modName = mod.Split("-")[0];
         
            driver.Navigate().GoToUrl($"http://{IP}/mod/create");
            driver.FindElement(By.Name("name")).SendKeys(modName); 
            driver.FindElement(By.Name("pretty_name")).SendKeys(modName);
            driver.FindElement(By.ClassName("btn-success")).Click();
            var alert = driver.FindElements(By.ClassName("alert"));
            if (alert.Count()  != 0 ){
                Console.WriteLine("{0}", alert);
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool Login(string email, string password)
        {
            driver.Navigate().GoToUrl($"http://{IP}/login");
            driver.FindElement(By.Name("email")).SendKeys(email);
            driver.FindElement(By.Name("password")).SendKeys(password);
            driver.FindElement(By.Name("login")).Click();
            return driver.FindElements(By.ClassName("notice")).Count == 0 ? true : false ;
        }

        public bool DoesModExist(string modName)
        { 
            return client.GetAsync($"http://{IP}/api/mod/{modName}").Result.IsSuccessStatusCode;
        }

        public bool DoesModVersionExist(string modName, string version)
        {
            var modRequest = client.GetAsync($"http://{IP}/api/mod/{modName}/{version}").Result;
            if (modRequest.IsSuccessStatusCode)
            {
                var response = client.GetStringAsync($"http://{IP}/api/mod/{modName}/{version}").Result;
                if (response.Equals("{\"error\":\"Mod version does not exist\"}")) return false;
            }
            return true;
        }


        public IEnumerable<string> GetVersions(string slug)
        {
            if (DoesModExist(slug))
            {
                var response = client.GetStringAsync($"http://{IP}/api/mod/{slug}").Result;
                JArray versionsAsArray = JObject.Parse(response)["versions"] as JArray;
                return versionsAsArray.ToObject<List<string>>();
            }
            return new List<string>();
        }

        public int GetModID(string modName)
        {
            if (DoesModExist(modName))
            {
                var response =  client.GetStringAsync($"http://{IP}/api/mod/{modName}").Result; 
                return int.Parse((string)JObject.Parse(response)["id"]);
            }
            else
            {
                return -1; 
            }
        }
        
        public void AddVersion(int modID, string version)
        {
            driver.Navigate().GoToUrl($"http://{IP}/mod/view/{modID}"); 
            driver.FindElement(By.Name("add-version")).SendKeys(version);
            var button= driver.FindElement(By.ClassName("btn-success"));
            new WebDriverWait(driver, TimeSpan.FromSeconds(3))
                .Until(ExpectedConditions.ElementToBeClickable(By.XPath("//button[contains(.,'Add Version')]")))
                .Click();
        }
    }
}
