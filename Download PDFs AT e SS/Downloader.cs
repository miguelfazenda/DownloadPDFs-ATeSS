using OpenQA.Selenium;
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
        static string DownloadFolder { get; set; }

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
            Declaracao[] declaracoes,
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
                    DownloadFolder = Path.Combine(downloadFolder, ano.ToString());
                    Directory.CreateDirectory(DownloadFolder);
                    CriarDriver(DownloadFolder);
                    Autenticar(empresa, declaracoes);
                }
                catch (Exception ex)
                {
                    LogError(ex);
                    break;
                }

                //Para cada declaração executa o que tem a fazer
                foreach (Declaracao declaracao in declaracoes)
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
                    Util.WaitForAllFilesToDownload(DownloadFolder);
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
            numFilesInDownloadsFolder = Directory.GetFiles(DownloadFolder).Length;
        }
        internal static void WaitForDownloadFinish(string newName, Declaracao declaracao, int mes)
        {
            //Espera que o download comece
            Util.WaitForFileCountToBeGreaterThan(DownloadFolder, numFilesInDownloadsFolder);
            //Espera que ele acabe
            Util.WaitForAllFilesToDownload(DownloadFolder);
            //Espera que o ficheiro final esteja pronto
            Util.WaitForFileCountToBeGreaterThan(DownloadFolder, numFilesInDownloadsFolder);

            string folderTipoDeclaracao = "";
            if (declaracao.Tipo == Declaracao.TipoDeclaracao.Anual)
                folderTipoDeclaracao = "Anuais";
            else if (declaracao.Tipo == Declaracao.TipoDeclaracao.Mensal)
                folderTipoDeclaracao = mes.ToString();
            else if (declaracao.Tipo == Declaracao.TipoDeclaracao.Lista)
                folderTipoDeclaracao = "Listas";
            else if (declaracao.Tipo == Declaracao.TipoDeclaracao.Pedido)
                folderTipoDeclaracao = "Pedidos";

            var diretorio = Path.Combine(DownloadFolder, folderTipoDeclaracao,
                empresaAutenticada.Codigo + "-" + empresaAutenticada.NIF);

            //Muda o nome
            Util.RenameLastModifiedFileInFolder(DownloadFolder, newName, diretorio);
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

        /// <summary>
        /// Carrega num botão, se ele não estiver presente espera que apareça (espera no max. 5*500 ms)
        /// </summary>
        /// <param name="by"></param>
        /// <returns>Se conseguiu encontrar ou não o botão</returns>
        internal static bool ClickButtonWaitForItToAppear(By by)
        {
            //Carrega no botão de transferir
            IReadOnlyCollection<IWebElement> btn = null;
            bool foundBtn = false;
            int tries = 0;
            while (!foundBtn && tries < 5)
            {
                btn = driver.FindElements(by);

                if (btn.Count == 0)
                    Thread.Sleep(500);
                else
                    foundBtn = true;

                tries++;
            }
            if (foundBtn)
            {
                btn.First().Click();
                return true;
            }
            return false;
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

        private static void Autenticar(Empresa empresa, Declaracao[] declaracoes)
        {
            //Vê em que serviços é necessário autenticar
            foreach (Declaracao declaracao in declaracoes)
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
