﻿using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Download_PDFs_AT_e_SS
{
    partial class Downloader
    {
        static IWebDriver driver;
        static bool[] autenticadoEm; //Seviços em que está autenticado (Indices de Declaracao.Autenticacao)
        static Empresa empresaAutenticada;
        static string downloadFolderEmpresa;

        static List<string> filesToRename; //Na ordem que foram transferidos

        internal static List<string> errors = new List<string>();

        internal static StringBuilder logMessage;
        internal static void Log(string tipoDeclaracao, string message)
        {
            if(logMessage == null)
                logMessage = new StringBuilder();

            logMessage.Append("[");
            logMessage.Append(empresaAutenticada.NIF);
            logMessage.Append(" ");
            logMessage.Append(tipoDeclaracao);
            logMessage.Append("] ");
            logMessage.AppendLine(message);
        }

        internal static void Executar(Empresa[] empresas,
            Declaracao[] declaracoesMensais, Declaracao[] declaracoesAnuais,
            int ano, int mes,
            string downloadFolder)
        {
            filesToRename = new List<string>();
            logMessage = new StringBuilder();

            foreach (Empresa empresa in empresas)
            {
                
                //Cria a pasta, o driver e autentica essa empresa
                try
                {
                    //string nomePasta = empresa.NIF + " " + ano + "-" + mes;
                    string nomePasta = SmartFormat.Smart.Format("{empresa.NIF} {ano}-{mes}", new { empresa, ano, mes });
                    downloadFolderEmpresa = Path.Combine(downloadFolder, nomePasta);
                    Directory.CreateDirectory(downloadFolder);
                    Directory.CreateDirectory(downloadFolderEmpresa);
                    CriarDriver(downloadFolderEmpresa);
                    Autenticar(empresa, declaracoesMensais, declaracoesAnuais);
                }
                catch (Exception ex)
                {
                    LogError(ex);
                    break;
                }

                //Para cada declaração executa o que tem a fazer
                foreach (Declaracao declaracao in declaracoesMensais)
                {
                    try
                    {
                        if (declaracao.DownloadFunctionAnual != null)
                            declaracao.DownloadFunctionAnual.Invoke(ano);
                        else if (declaracao.DownloadFunctionMensal != null)
                            declaracao.DownloadFunctionMensal.Invoke(ano, mes);
                    }
                    catch (Exception ex)
                    {
                        //Se der erro regista-o mas prossegue
                        LogError(ex);
                    }
                }
                //TODO retirar codigo repetido (declaracoesMensais e declaracoesAnuais)
                //Para cada declaração executa o que tem a fazer
                foreach (Declaracao declaracao in declaracoesAnuais)
                {
                    try
                    {
                        if (declaracao.DownloadFunctionAnual != null)
                            declaracao.DownloadFunctionAnual.Invoke(ano);
                        else if (declaracao.DownloadFunctionMensal != null)
                            declaracao.DownloadFunctionMensal.Invoke(ano, mes);
                    }
                    catch (Exception ex)
                    {
                        //Se der erro regista-o mas prossegue
                        LogError(ex);
                    }
                }

                //Fecha o chrome quanto todas as transferências terminarem (já não existirem ficheiros ".crdownload")
                try
                {
                    Thread.Sleep(2000);
                    Util.WaitForAllFilesToDownload(downloadFolderEmpresa);
                    FecharDriver();
                }
                catch (Exception ex)
                {
                    LogError(ex);
                    break;
                }
                
                filesToRename.Clear();
            }

            FecharDriver();
        }

        static int numFilesInDownloadsFolder;
        internal static void ExpectDownload()
        {
            numFilesInDownloadsFolder = Directory.GetFiles(downloadFolderEmpresa).Length;
        }
        internal static void WaitForDownloadFinish(string newName)
        {
            //Espera que o download comece
            Util.WaitForFileCountToBeGreaterThan(downloadFolderEmpresa, numFilesInDownloadsFolder);
            //Espera que ele acabe
            Util.WaitForAllFilesToDownload(downloadFolderEmpresa);
            //Espera que o ficheiro final esteja pronto
            Util.WaitForFileCountToBeGreaterThan(downloadFolderEmpresa, numFilesInDownloadsFolder);
            //Muda o nome
            if(newName != null)
                Util.RenameLastModifiedFileInFolder(downloadFolderEmpresa, newName);
            numFilesInDownloadsFolder++;
        }

        internal static void DownloadFundoCompDocPag(int ano, int mes)
        {
            try
            {
                /*driver.Navigate().GoToUrl("https://www.fundoscompensacao.pt/fc/gfct/home?windowId=c05");
                driver.FindElement(By.XPath("/html/body/div[1]/div[2]/span/div/div/div/div[1]/div/form/div/div[4]/h3/a")).Click();
                Thread.Sleep(500);
                driver.FindElement(By.XPath("/html/body/div[1]/div[2]/span/div/div/div/div[1]/div/form/div/div[4]/div/ul/li/a")).Click();

                var printBtn = driver.FindElement(By.Id("form:btnPrintReport"));
                var gerarBtn = driver.FindElement(By.Id("form:btnGenReport"));
                ExpectDownload();
                if (gerarBtn.GetAttribute("aria-disabled") == "false")
                {
                    gerarBtn.Click();
                }
                else
                {
                    printBtn.Click();
                }
                WaitForDownloadFinish(null);*/
                //Util.RenameDownloadedFile(downloadFolderEmpresa, "");
            } catch(Exception ex)
            {

            }
            /*
            string 

            driver.Navigate().GoToUrl("https://www.fundoscompensacao.pt/fc/gfct/consulta/documentosEmpregador?frawMenu=1&windowId=automatedEntryPoint");
            ((IJavaScriptExecutor)driver).ExecuteScript("document.getElementById(\"form:inputDtInicioBegin:calendar\").value = \"" + anoMesDiaStr + "\"");
            ((IJavaScriptExecutor)driver).ExecuteScript("document.getElementById(\"form:inputDtInicioEnd:calendar\").value = \"" + anoMesDiaStr + "\"");
            driver.FindElement(By.XPath("/html/body/div[1]/div[2]/span/div/div/div/div[2]/div/div[3]/form/div[3]/div/div/div[3]"));
            driver.FindElement(By.XPath("/html/body/div[5]/div/ul/li[4]")).Click();
            driver.FindElement(By.XPath("/html/body/div[1]/div[2]/span/div/div/div/div[2]/div/div[3]/form/span/div[2]/div[1]/table/tbody/tr/td[2]/a")).Click();
              */  

            
        }

        /// <summary>
        /// Obtem o codigo que o botão tem no onclick e executa esse códugi
        /// </summary>
        /// <param name="by"></param>
        internal static void ButtonRunOnClick(By by)
        {
            string imprimirBtnOnClickCode = driver.FindElement(by).GetAttribute("onclick");
            ((IJavaScriptExecutor) driver).ExecuteScript(imprimirBtnOnClickCode);
        }

        //Cria a instacia do driver(chrome)
        private static void CriarDriver(string downloadFolder)
        {
            // this will make automatically download to the default folder.
            ChromeOptions chromeOptions = new ChromeOptions();
            chromeOptions.AddUserProfilePreference("plugins.always_open_pdf_externally", true);
            chromeOptions.AddUserProfilePreference("profile.default_content_settings.popups", 0);
            chromeOptions.AddUserProfilePreference("download.default_directory", downloadFolder);
            chromeOptions.AddUserProfilePreference("profile.default_content_setting_values.automatic_downloads", 1);
            //tentar --headless para nao mostrar nada

            ChromeDriverService chromeDriverService = ChromeDriverService.CreateDefaultService();
            chromeDriverService.HideCommandPromptWindow = true;

            driver = new ChromeDriver(chromeDriverService, chromeOptions);
            autenticadoEm = new bool[10];
        }

        private static void FecharDriver()
        {
            if (driver == null)
                return;

            driver.Quit();
            empresaAutenticada = null;
            driver = null;
            autenticadoEm = null;
        }

        private static void LogError(string error)
        {
            errors.Add(error);
        }
        private static void LogError(Exception ex)
        {
            errors.Add(ex.Message);
        }

        internal static void ClearErrorLogs()
        {
            errors.Clear();
        }

        private static void Autenticar(Empresa empresa, Declaracao[] declaracoesMensais, Declaracao[] declaracoesAnuais)
        {
            //Vê em que serviços é necessário autenticar
            foreach (Declaracao declaracao in declaracoesMensais)
                autenticadoEm[(int)declaracao.AutenticacaoNecessaria] = true;
            foreach (Declaracao declaracao in declaracoesAnuais)
                autenticadoEm[(int)declaracao.AutenticacaoNecessaria] = true;

            //Autenticar
            if (autenticadoEm[(int)Declaracao.Autenticacao.AT])
            {
                driver.Navigate().GoToUrl("https://www.acesso.gov.pt/v2/loginForm?partID=PFAP&path=/geral/dashboard");
                driver.FindElement(By.Id("username")).SendKeys(empresa.NIF);
                driver.FindElement(By.Id("password-nif")).SendKeys(empresa.PasswordAT);
                driver.FindElement(By.Id("sbmtLogin")).Click();
            }

            if (autenticadoEm[(int)Declaracao.Autenticacao.SSFundosCompensacao])
            {
                driver.Navigate().GoToUrl("https://www.fundoscompensacao.pt/sso/login");
                driver.FindElement(By.Id("username")).SendKeys(empresa.NISS);
                driver.FindElement(By.Id("password")).SendKeys(empresa.PasswordSS);
                driver.FindElement(By.XPath("//*[@id=\"credentials\"]/div[5]/input")).Click();
            }

            if (autenticadoEm[(int)Declaracao.Autenticacao.SSDireta])
            {
                driver.Navigate().GoToUrl("https://app.seg-social.pt/sso/login");
                driver.FindElement(By.Id("username")).SendKeys(empresa.NISS);
                driver.FindElement(By.Id("password")).SendKeys(empresa.PasswordSS);
                driver.FindElement(By.XPath("//*[@id=\"credentials\"]/div[5]/input")).Click();
            }

            empresaAutenticada = empresa;
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

            for (int i = 1; i<rows.Count - 1; i++) //Ignora a 1a linha(cabeçalho) e ultima linha(vazia)
            {
                var row = rows[i];
                var rowTds = row.FindElements(By.TagName("td"));

                if (rowTds.Count < 3)
                    continue;

                var linkTd = rowTds[rowTds.Count - 2]; //O td que contem o link

                var a = linkTd.FindElement(By.TagName("a"));
                requestsToDo[i - 1] = a.GetAttribute("href").Substring("javascript:".Length); //Regista o link(codigo de js)
            }

            for (int i = 0; i<requestsToDo.Length; i++)
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
                WaitForDownloadFinish("Retencao " + req.Substring("submitQuery('".Length, 11) + ".pdf");
                
                if (i < requestsToDo.Length-1)
                {
                    //Volta à pagina com a tabela, se não for o utlimo ficheiro a transferir(porque se for não vale a pena voltar a trás)
                    driver.Navigate().GoToUrl("https://www.portaldasfinancas.gov.pt/pt/main.jsp?body=/guias/consultarDeclsDividaByPeriodForm.jsp");
                    //Escolhe o mes e o ano
                    ((IJavaScriptExecutor)driver).ExecuteScript("queryDecls('" + ano + "','" + mes + "');");
                }
            }
        }
        
        internal static bool IsDialogPresent()
        {
            IAlert alert = ExpectedConditions.AlertIsPresent().Invoke(driver);
            return (alert != null);
        }
        internal static IAlert GetAlert()
        {
            IAlert alert = ExpectedConditions.AlertIsPresent().Invoke(driver);
            return alert;
        }
    }
}
