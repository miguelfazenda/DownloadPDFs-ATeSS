using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Download_PDFs_AT_e_SS
{
    class Autenticacao
    {
        internal static void AutenticarAT(IWebDriver driver, Empresa empresa)
        {
            driver.Navigate().GoToUrl("https://www.acesso.gov.pt/v2/loginForm?partID=PFAP&path=/geral/dashboard");
            Thread.Sleep(500);
            driver.FindElement(By.Id("username")).SendKeys(empresa.NIF);
            driver.FindElement(By.Id("password-nif")).SendKeys(empresa.PasswordAT);
            Thread.Sleep(500);
            driver.FindElement(By.Id("sbmtLogin")).Click();
        }

        internal static void AutenticarFundosCompensacao(IWebDriver driver, Empresa empresa)
        {
            driver.Navigate().GoToUrl("https://www.fundoscompensacao.pt/sso/login");
            Thread.Sleep(500);
            driver.FindElement(By.Id("username")).SendKeys(empresa.NISS);
            driver.FindElement(By.Id("password")).SendKeys(empresa.PasswordSS);
            Thread.Sleep(500);
            driver.FindElement(By.XPath("//*[@id=\"credentials\"]/div[5]/input")).Click();
        }

        internal static void AutenticarSS(IWebDriver driver, Empresa empresa)
        {
            driver.Navigate().GoToUrl("https://app.seg-social.pt/sso/login");
            Thread.Sleep(500);
            driver.FindElement(By.Id("username")).SendKeys(empresa.NISS);
            driver.FindElement(By.Id("password")).SendKeys(empresa.PasswordSS);
            Thread.Sleep(500);
            driver.FindElement(By.XPath("//*[@id=\"credentials\"]/div[5]/input")).Click();
        }
    }
}
