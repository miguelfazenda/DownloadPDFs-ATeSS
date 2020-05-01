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

        public static List<IWebDriver> driversAbertos = new List<IWebDriver>();

        //Cria a instacia do driver(chrome)
        public static void CriarDriver()
        {
            ChromeDriverService chromeDriverService = ChromeDriverService.CreateDefaultService();
            chromeDriverService.HideCommandPromptWindow = true;

            driver = new ChromeDriver(chromeDriverService);
            driversAbertos.Add(driver);
        }

        internal static void AbrePedidoCertidao(string codigoCertidaoPermanente)
        {
            Browser.driver.Navigate().GoToUrl("https://eportugal.gov.pt/RegistoOnline/Services/CertidaoPermanente/consultaCertidao.aspx?id=" + codigoCertidaoPermanente);
        }

        internal static void AbrePortalDasFinancas(Empresa empresa)
        {
            Autenticacao.AutenticarAT(driver, empresa);
        }

        internal static void AbreEFatura(Empresa empresa)
        {
            Autenticacao.AutenticarAT(driver, empresa);
            driver.Navigate().GoToUrl("https://faturas.portaldasfinancas.gov.pt/");
        }

        internal static void AbreSegurancaSocial(Empresa empresa)
        {
            Autenticacao.AutenticarSS(driver, empresa);
            driver.Navigate().GoToUrl("https://app.seg-social.pt/ptss/ptss/home");
        }

        internal static void AbreFundosDeCompensacao(Empresa empresa)
        {
            Autenticacao.AutenticarFundosCompensacao(driver, empresa);
            driver.Navigate().GoToUrl("https://www.fundoscompensacao.pt/fc/gfct/home");
        }

        internal static void FechaDriversAbertos()
        {
            foreach(IWebDriver d in driversAbertos)
            {
                d.Quit();
            }
        }
    }
}
