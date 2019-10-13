using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SmartFormat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

            ExpectDownload();
            if (ClickButtonWaitForItToAppear(By.XPath(XPATH_MODELO22_BOTAO_OBTER)))
            {
                //Se o botão de obter existir, expera que o ficheiro seja transferido
                WaitForDownloadFinish(GenNovoNomeFicheiro(Definicoes.estruturaNomesFicheiros.AT_Modelo22), Declaracao.AT_Modelo22, 0);
            }
            else
            {
                //Se não existe botão, não faz nada
                Log("Modelo 22", "Sem resultados");
            }
        }
        internal static void DownloadIRS(int ano)
        {
            driver.Navigate().GoToUrl("https://irs.portaldasfinancas.gov.pt/consultarIRSCompList.action");

            ExpectDownload();

            var numDocumentos = driver.FindElements(By.XPath("/html/body/div/main/div/div[2]/div/section/div[4]/div/div/div[2]/div/table/tbody/*")).Count;

            for (int i = 0; i < numDocumentos; i++)
            {
                string xpathRow = "/html/body/div/main/div/div[2]/div/section/div[4]/div/div/div[2]/div/table/tbody/tr[" + (i + 1) + "]";
                string anoXPath = xpathRow + "/td[2]";

                string anoStr = driver.FindElement(By.XPath(anoXPath)).Text;
                int anoInt = 0;
                if(Int32.TryParse(anoStr, out anoInt))
                {
                    if(anoInt == Ano)
                    {
                        //Se for não for 
                        string anchorPath = xpathRow + "/td[4]/a";
                        ExpectDownload();
                        driver.FindElement(By.XPath(anchorPath)).Click();
                        WaitForDownloadFinish(GenNovoNomeFicheiro(Definicoes.estruturaNomesFicheiros.AT_IRS), Declaracao.AT_IRS, 0);
                    }
                }                
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
                WaitForDownloadFinish(GenNovoNomeFicheiro(Definicoes.estruturaNomesFicheiros.AT_IES), Declaracao.AT_IES, 0);
            }
            else
            {
                Log("IES", "Sem resultados");
            }
        }

        internal static void DownloadIVA(int ano)
        {
            driver.Navigate().GoToUrl("https://iva.portaldasfinancas.gov.pt/dpiva/portal/obter-comprovativo#!?ano=" + ano);
            //Por alguma razão só da segunda tentativa é que ele mete o ano certo....
            driver.Navigate().GoToUrl("https://iva.portaldasfinancas.gov.pt/dpiva/portal/obter-comprovativo#!?ano=" + ano);

            var numDocumentos = driver.FindElements(By.XPath("/html/body/div/main/div/div[2]/div/section/div/obter-comprovativo-e-doc-pagamento-app/div[3]/obter-comprovativo-e-doc-pagamento-tabela/div/div/div/div/table/tbody/*")).Count;
            
            for(int i = 0; i<numDocumentos; i++)
            {
                string xpathRow = "/html/body/div/main/div/div[2]/div/section/div/obter-comprovativo-e-doc-pagamento-app/div[3]/obter-comprovativo-e-doc-pagamento-tabela/div/div/div/div/table/tbody/tr[" + (i+1) + "]";

                ExpectDownload();
                ClickButtonWaitForItToAppear(By.XPath(xpathRow + "/td[4]/div/a"));

                string docIdentif = driver.FindElement(By.XPath(xpathRow + "/td[1]/p")).Text;
                string docPeriodo = driver.FindElement(By.XPath(xpathRow + "/td[2]/p")).Text;
                string docDataRececao = driver.FindElement(By.XPath(xpathRow + "/td[3]")).Text;
                string docAno = docDataRececao.Split('-')[0]; //Extrai o ano do campo data de receção

                //string nomeFicheiro = String.Format("IVA {0} {1} {2}", docAno, docPeriodo, docIdentif);

                WaitForDownloadFinish(GenNovoNomeFicheiro(Definicoes.estruturaNomesFicheiros.AT_IVA,
                    new { docAno, docPeriodo, docIdentif}), Declaracao.AT_IVA, 0);
            }
        }

        internal static void DownloadDMRComprovativo(int ano, int mes)
        {
            DownloadDMR(ano, mes, "Obter comprovativo", Declaracao.AT_DMRComprov, Definicoes.estruturaNomesFicheiros.AT_DMRComprov);
        }

        internal static void DownloadDMRDocPag(int ano, int mes)
        {
            DownloadDMR(ano, mes, "Obter documento de pagamento", Declaracao.AT_DMRDocPag, Definicoes.estruturaNomesFicheiros.AT_DMRDocPag);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ano"></param>
        /// <param name="mes"></param>
        /// <param name="linkAClicar">Trata-se do nome do link em que é para carregar, na lista de opções ("Obter comprovativo" ou "Obter documento de pagamento")</param>
        internal static void DownloadDMR(int ano, int mes, string linkAClicar, Declaracao declaracao, string estruturaFicheiro)
        {
            driver.Navigate().GoToUrl("https://www.portaldasfinancas.gov.pt/pt/external/oadmrsv/consultarDMR.action");

            //Selecionar ano e mes
            var selectMes = new SelectElement(driver.FindElement(By.Id("mes")));
            selectMes.SelectByValue(mes.ToString());
            var selectAno = new SelectElement(driver.FindElement(By.Id("ano")));
            selectAno.SelectByValue(ano.ToString());

            driver.FindElement(By.Id("pesquisar")).Click();
            Thread.Sleep(500);
            //Download
            var elementosLista = driver.FindElements(By.XPath("//*[@id=\"tab1\"]/div[1]/div[3]/div/ul/*/a"));
            foreach (var elemento in elementosLista)
            {
                if (elemento.GetAttribute("innerHTML").Contains(linkAClicar))
                {
                    ExpectDownload();
                    driver.Navigate().GoToUrl(elemento.GetAttribute("href"));

                    ClickButtonWaitForItToAppear(By.Id("obter-btn"));

                    WaitForDownloadFinish(GenNovoNomeFicheiro(estruturaFicheiro),
                        declaracao, mes);
                }
            }
        }

        internal static void DownloadCerticaoDivida(int ano)
        {
            driver.Navigate().GoToUrl("https://www.portaldasfinancas.gov.pt/pt/emissaoCertidao.action?tipoCertidao=N");
            driver.Navigate().GoToUrl("https://www.portaldasfinancas.gov.pt/pt/emissaoCertidao.action?tipoCertidao=N");

            ExpectDownload();
            ClickButtonWaitForItToAppear(By.Id("certidaoBtn"));
            WaitForDownloadFinish(GenNovoNomeFicheiro(Definicoes.estruturaNomesFicheiros.AT_CerticaoDivida), Declaracao.AT_CerticaoDivida, 0);
        }


        internal static void DownloadRetencoes(int ano, int mes)
        {
            driver.Navigate().GoToUrl("https://www.portaldasfinancas.gov.pt/pt/main.jsp?body=/guias/consultarDeclsDividaByPeriodForm.jsp");
            //Escolhe o mes e o ano
            ((IJavaScriptExecutor)driver).ExecuteScript("queryDecls('" + ano + "','" + mes + "');");

            if (IsDialogPresent())
            {
                //Se der erro, regista-o, e sai desta função
                IAlert alert = GetAlert();
                LogError(String.Format("Empresa: {0}  {1}", empresaAutenticada.ToString(), alert.Text));
                alert.Dismiss();
                return;
            }


            //Na tabela obtem os requests que tem de fazer para os ficheiros todos
            var table = driver.FindElement(By.XPath("//*[@id=\"main_middle_body\"]/div/div[3]/table/tbody/tr/td/table"));
            var rows = table.FindElements(By.TagName("tr"));

            string[] requestsToDo = new string[rows.Count - 2];

            for (int i = 1; i < rows.Count - 1; i++) //Ignora a 1a linha(cabeçalho) e ultima linha(vazia)
            {
                var row = rows[i];
                var rowTds = row.FindElements(By.TagName("td"));

                if (rowTds.Count < 3)
                    continue;

                var linkTd = rowTds[rowTds.Count - 2]; //O td que contem o link

                var a = linkTd.FindElement(By.TagName("a"));
                requestsToDo[i - 1] = a.GetAttribute("href").Substring("javascript:".Length); //Regista o link(codigo de js)
            }

            for (int i = 0; i < requestsToDo.Length; i++)
            {
                var req = requestsToDo[i];

                //Obtem cada ficheiro
                if (req == null)
                    continue;

                ExpectDownload();
                ((IJavaScriptExecutor)driver).ExecuteScript(req);
                driver.FindElement(By.XPath("//*[@id=\"main_middle_body\"]/div/div[3]/table/tbody/tr[3]/td/table/tbody/tr/td/input")).Click();

                //Adiciona o novo nome do ficheiro
                //filesToRename.Add("Retencao " + req.Substring("submitQuery('".Length, 11) + ".pdf");
                string codigoFicheiro = req.Substring("submitQuery('".Length, 11);

                WaitForDownloadFinish(GenNovoNomeFicheiro(Definicoes.estruturaNomesFicheiros.AT_Retencoes),
                        Declaracao.AT_Retencoes, mes);

                if (i < requestsToDo.Length - 1)
                {
                    //Volta à pagina com a tabela, se não for o utlimo ficheiro a transferir(porque se for não vale a pena voltar a trás)
                    driver.Navigate().GoToUrl("https://www.portaldasfinancas.gov.pt/pt/main.jsp?body=/guias/consultarDeclsDividaByPeriodForm.jsp");
                    //Escolhe o mes e o ano
                    ((IJavaScriptExecutor)driver).ExecuteScript("queryDecls('" + ano + "','" + mes + "');");
                }
            }
        }

        
    }
}
