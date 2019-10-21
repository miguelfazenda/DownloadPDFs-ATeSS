using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Download_PDFs_AT_e_SS
{
    class Browser
    {
        public static IWebDriver driver;

        //Cria a instacia do driver(chrome)
        public static void CriarDriver()
        {
            ChromeDriverService chromeDriverService = ChromeDriverService.CreateDefaultService();
            chromeDriverService.HideCommandPromptWindow = true;

            driver = new ChromeDriver(chromeDriverService);
        }

        internal static void AbrePedidoCertidao(string codigoCertidaoPermanente)
        {
            Browser.driver.Navigate().GoToUrl("https://eportugal.gov.pt/RegistoOnline/Services/CertidaoPermanente/consultaCertidao.aspx?id=" + codigoCertidaoPermanente);
        }
    }
}
