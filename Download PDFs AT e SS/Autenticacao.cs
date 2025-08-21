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
            
            driver.FindElement(By.XPath("//*[@id=\"radix-:ra:-trigger-N\"]")).Click();

            driver.FindElement(By.Name("username")).SendKeys(empresa.NIF);
            driver.FindElement(By.Name("password")).SendKeys(empresa.PasswordAT);
            Thread.Sleep(500);
            driver.FindElement(By.XPath("/html/body/div/div/main/div[2]/div[2]/div[1]/div[3]/form/button")).Click();
        }

        internal static void AutenticarFundosCompensacao(IWebDriver driver, Empresa empresa)
        {
            driver.Navigate().GoToUrl("https://www.fundoscompensacao.pt/sso/login?service=https%3A%2F%2Fwww.fundoscompensacao.pt%2Ffc%2Fcaslogin");
            Thread.Sleep(500);
            driver.FindElement(By.Id("toogleAuth")).Click();
            driver.FindElement(By.Id("username")).SendKeys(empresa.NISS);
            driver.FindElement(By.Id("password")).SendKeys(empresa.PasswordSS);
            Thread.Sleep(500);
            driver.FindElement(By.Id("submitBtn")).Click();

            Thread.Sleep(500);
            //if continuarBtn exists
            var continuarBtnArray = driver.FindElements(By.Id("continuarBtn"));
            if (continuarBtnArray.Count > 0)
            {
                continuarBtnArray[0].Click();
            }
        }

        internal static void AutenticarSS(IWebDriver driver, Empresa empresa)
        {
            driver.Navigate().GoToUrl("https://www.seg-social.pt/sso/login?service=https%3A%2F%2Fwww.seg-social.pt%2Fptss%2Fcaslogin");
            Thread.Sleep(500);
            driver.FindElement(By.Id("toogleAuth")).Click();
            driver.FindElement(By.Id("username")).SendKeys(empresa.NISS);
            driver.FindElement(By.Id("password")).SendKeys(empresa.PasswordSS);
            Thread.Sleep(500);
            driver.FindElement(By.Id("submitBtn")).Click();

            Thread.Sleep(500);
            //if continuarBtn exists
            var continuarBtnArray = driver.FindElements(By.Id("continuarBtn"));
            if (continuarBtnArray.Count > 0)
            {
                continuarBtnArray[0].Click();
            }
        }
    }
}
