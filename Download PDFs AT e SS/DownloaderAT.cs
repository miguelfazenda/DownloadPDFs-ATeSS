using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Download_PDFs_AT_e_SS
{
    partial class Downloader
    {
        const string XPATH_MODELO22_BOTAO_OBTER = "/html/body/div/main/div/div[2]/div/section/div/consultar-comprovativo-app/div[3]/consultar-comprovativo-tabela/div/div[2]/div/div/table/tbody/tr/td[5]/button";
        internal static void DownloadModelo22(int ano)
        {
            driver.Navigate().GoToUrl("https://irc.portaldasfinancas.gov.pt/mod22/obter-comprovativo#!?ano=" + ano);
            //Por alguma razão só da segunda tentativa é que ele mete o ano certo....
            driver.Navigate().GoToUrl("https://irc.portaldasfinancas.gov.pt/mod22/obter-comprovativo#!?ano=" + ano);
            if (Util.IsElementPresent(driver, By.XPath(XPATH_MODELO22_BOTAO_OBTER)))
            {
                //Se o botão de obter existir, carrega nele
                ExpectDownload();
                driver.FindElement(By.XPath(XPATH_MODELO22_BOTAO_OBTER)).Click();
                WaitForDownloadFinish("Modelo 22.pdf");
            }
            else
            {
                Log("Modelo 22", "Sem resultados");
            }
        }

        const string XPATH_IES_BOTAO_OBTER = "/html/body/div/main/div/div[2]/div/section/div[3]/div[2]/div/div[3]/div/div/table/tbody/tr/td[3]/div/button";
        internal static void DownloadIES(int ano)
        {
            driver.Navigate().GoToUrl("https://oa.portaldasfinancas.gov.pt/ies/consultarIES.action?anoDeclaracoes=" + ano);
            if (Util.IsElementPresent(driver, By.XPath(XPATH_IES_BOTAO_OBTER)))
            {
                //Se o botão de obter existir, carrega nele
                ExpectDownload();
                driver.FindElement(By.XPath(XPATH_IES_BOTAO_OBTER)).Click();
                WaitForDownloadFinish("IES.pdf");
            }
            else
            {
                Log("IES", "Sem resultados");
            }
        }

        internal static void DownloadIVA(int ano)
        {
            driver.Navigate().GoToUrl("https://iva.portaldasfinancas.gov.pt/dpiva/portal/obter-comprovativo#!?ano=" + ano);

            var numDocumentos = driver.FindElements(By.XPath("/html/body/div/main/div/div[2]/div/section/div/obter-comprovativo-e-doc-pagamento-app/div[3]/obter-comprovativo-e-doc-pagamento-tabela/div/div/div/div/table/tbody/*")).Count;
            
            for(int i = 0; i<numDocumentos; i++)
            {
                string xpathRow = "/html/body/div/main/div/div[2]/div/section/div/obter-comprovativo-e-doc-pagamento-app/div[3]/obter-comprovativo-e-doc-pagamento-tabela/div/div/div/div/table/tbody/tr[" + (i+1) + "]";

                ExpectDownload();
                driver.FindElement(By.XPath(xpathRow + "/td[4]/div/a/button")).Click();

                string docIdentif = driver.FindElement(By.XPath(xpathRow + "/td[1]/p")).Text;
                string docPeriodo = driver.FindElement(By.XPath(xpathRow + "/td[2]/p")).Text;
                string docDataRececao = driver.FindElement(By.XPath(xpathRow + "/td[3]")).Text;
                string docAno = docDataRececao.Split('-')[0]; //Extrai o ano do campo data de receção

                string nomeFicheiro = String.Format("IVA {0} {1} {2}", docAno, docPeriodo, docIdentif);

                WaitForDownloadFinish(nomeFicheiro);
            }
        }
    }
}
