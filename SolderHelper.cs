using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers; 
using System.Net.Http.Headers; 

namespace TechnicSolderPackager
{
    internal class SolderHelper
    {

        ChromeDriver driver;
        public HttpClient client = new HttpClient();
        string IP;

        public SolderHelper(string ip)
        {
            IP = ip;
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            ChromeOptions options = new();
            options.AddArgument("--headless");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-dev-shm-usage");
            driver = new(options);
        }

        public bool CreateMod(string mod)
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

        public void CreateModpackVersion(string version, string minecraftVersion, string modapckNumber)
        {
            driver.Navigate().GoToUrl($"http://{IP}/modpack/add-build/{modapckNumber}");
            driver.FindElement(By.Id("version")).SendKeys(version);
            new SelectElement(driver.FindElement(By.Name("minecraft")))
                .SelectByValue(minecraftVersion);
            new SelectElement(driver.FindElement(By.Name("clone"))).Options.Last().Click();
              //  .SelectByText(version); 

            var button = driver.FindElement(By.ClassName("btn-success"));
            button.Click();
        }

        public string GetModpackIndex(string modpackSlug)
        {
            driver.Navigate().GoToUrl($"http://{IP}/modpack/list");
            driver.FindElement(By.XPath("//*[@id=\"dataTables_filter\"]/label/input")).SendKeys(modpackSlug);

            string href =  driver.FindElement(By.XPath("//*[@id=\"dataTables\"]/tbody/tr/td[7]/a[1]")).GetAttribute("href");
            string[] splitHref = href.Split('/');
            string index = splitHref.Last();
            return index;
        }


        public string GetModpackVersionIndex(string modpackIndex, string version)
        {
            driver.Navigate().GoToUrl($"http://{IP}/modpack/view/{modpackIndex}");
            driver.FindElement(By.XPath("//*[@id=\"dataTables_filter\"]/label/input")).SendKeys(version);
            string index = driver.FindElement(By.XPath("//*[@id=\"dataTables\"]/tbody/tr/td[1]")).Text;
            return index;
        }


        public void RemoveModFromModpack(string modSlug, string modVersion, string modpack, string modpackVersion)
        {             string modpackIndex = GetModpackIndex(modpack);
            string modpackVersionIndex = GetModpackVersionIndex(modpackIndex, modpackVersion);
            driver.Navigate().GoToUrl($"http://{IP}/modpack/build/{modpackVersionIndex}");
            driver.FindElement(By.XPath("//*[@id=\"mod-list_filter\"]/label/input"))
                .SendKeys(modSlug+" "+ modVersion);
            var button = driver.FindElements(By.XPath("//*[@id=\"mod-list\"]/tbody/tr/td[3]/form/button"));
            if(button.Count > 0)
            {
                button.First().Click();
                Console.WriteLine($"Removed {driver.FindElement(By.XPath("//*[@id=\"mod-list\"]/tbody/tr/td[1]")).Text}");
                if (WasActionSuccessful())
                {
                    Console.WriteLine($"[SUCCESS] Mod: {modSlug}, version: {modVersion} was removed!");
                }
                else
                {
                    Console.WriteLine($"[FAIL] Mod: {modSlug}, version: {modVersion} failed to be removed!");
                }
            }
            else
            {
                Console.WriteLine($"Could not find {modSlug} to remove! Maybe it is not present already.");
            }


                
        }

        public void ChangeVersionOfMod(string modSlug, string modVersion, string modpack, string modpackVersion)
        {
            string modpackIndex = GetModpackIndex(modpack);
            string modpackVersionIndex = GetModpackVersionIndex(modpackIndex, modpackVersion);
            driver.Navigate().GoToUrl($"http://{IP}/modpack/build/{modpackVersionIndex}");
            driver.FindElement(By.XPath("//*[@id=\"mod-list_filter\"]/label/input")).SendKeys(modSlug+" "+modVersion);
            new SelectElement(driver.FindElement(By.Name("version")))
                .SelectByText(modVersion);
            driver.FindElement(By.XPath("//*[@id=\"mod-list\"]/tbody/tr[1]/td[2]/form/div/span/button")).Click();
            if (WasActionSuccessful())
            {
                Console.WriteLine($"[SUCCESS] Mod: {modSlug}, version: {modVersion} was changed!");
            }
            else
            {
                Console.WriteLine($"[FAIL] Mod: {modSlug}, version: {modVersion} failed to be changed!");
            }
        }

        public void AddModToModpack(string modSlug, string modVersion, string modpack, string modpackVersion)
        {
            string modpackIndex = GetModpackIndex(modpack);
            string modpackVersionIndex = GetModpackVersionIndex(modpackIndex, modpackVersion);
            driver.Navigate().GoToUrl($"http://{IP}/modpack/build/{modpackVersionIndex}");
            
            driver.FindElement(By.XPath("//*[@id=\"mod-list-add\"]/td[1]/div/div")).Click();
            ClickOnDivSelectElement(By.XPath("/html/body/div[2]/div"), modSlug, false);

            driver.FindElement(By.XPath("//*[@id=\"mod-list-add\"]/td[2]/div")).Click();
            ClickOnDivSelectElement(By.XPath("/html/body/div[3]/div"), modVersion, false);
           
            driver.FindElement(By.XPath("//*[@id=\"mod-list-add\"]/td[3]/button")).Click();

            if (WasActionSuccessful())
            {
                Console.WriteLine($"[SUCCESS] Mod: {modSlug}, version: {modVersion} was added!");
            }
            else
            { 
                Console.WriteLine($"[FAIL] Mod: {modSlug}, version: {modVersion} failed to be added!");
            }
        }
         
        private bool WasActionSuccessful()
        {
            Thread.Sleep(1000);
            return driver.FindElements(By.ClassName("alert-success")).Count > 0;
        }

        public void ClickOnDivSelectElement(By by, string elementToClick, bool text)
        { 
            foreach (IWebElement element in driver.FindElement(by).FindElements(By.TagName("div"))) 
            {

                 if (element.GetAttribute("data-value").Equals(elementToClick))
                        element.Click();
         

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

        public IEnumerable<string> GetModVersions(string slug)
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
        
        public void AddModVersion(int modID, string version)
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
