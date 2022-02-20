using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SmartFormat;
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
        static ChromeOptions chromeOptions;

        static IWebDriver driver;
        static bool[] autenticadoEm; //Seviços em que está autenticado (Indices de Declaracao.Autenticacao)
        static Empresa empresaAutenticada;
        static string DownloadFolder { get; set; }
        static int Ano { get; set; }
        static int Mes { get; set; }

        static List<string> filesToRename; //Na ordem que foram transferidos

        internal static List<string> errors = new List<string>();

        private static int numEmpresaAProcessar, totalEmpresas; //Para calcular o progresso
        private static int numDeclaracaoAProcessar, totalDeclaracoes; //Para calcular o progresso
        private static Action<int> reportProgress; //Funcao para reportar o progresso

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
            string downloadFolder,
            Action<int> reportProgress,
            bool headless)
        {
            Downloader.reportProgress = reportProgress;
            filesToRename = new List<string>();
            logMessage = new StringBuilder();

            Ano = ano;
            Mes = mes;
            
            totalEmpresas = empresas.Length;
            //Para cada empresa
            for (int iEmpresa = 0; iEmpresa < totalEmpresas; iEmpresa++)
            {
                Empresa empresa = empresas[iEmpresa];
                //Cria a pasta, o driver e autentica essa empresa
                try
                {
                    DownloadFolder = Path.Combine(downloadFolder, ano.ToString());
                    Directory.CreateDirectory(DownloadFolder);
                    CriarDriver(DownloadFolder, headless);
                    Autenticar(empresa, declaracoes);
                }
                catch (Exception ex)
                {
                    LogError(ex, empresa, null);
                    break;
                }


                totalDeclaracoes = declaracoes.Length;
                //Para cada declaração executa o que tem a fazer
                for (int iDeclaracao = 0; iDeclaracao < declaracoes.Length; iDeclaracao++)
                {
                    numDeclaracaoAProcessar = iDeclaracao;
                    Declaracao declaracao = declaracoes[iDeclaracao];
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
                        LogError(ex, empresa, declaracao);
                    }

                    CalcularProgresso();
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
                    LogError(ex, empresa, null);
                    break;
                }
                
                filesToRename.Clear();
            }

            FecharDriver();
        }

        public static void CalcularProgresso()
        {
            int progresso = (int)
                (((double)numEmpresaAProcessar / totalEmpresas + (((double)numDeclaracaoAProcessar / totalDeclaracoes) / totalEmpresas)) * 100);
            reportProgress(progresso);
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

            Thread.Sleep(500);

            //Espera que o ficheiro final esteja pronto
            Util.WaitForFileCountToBeGreaterThan(DownloadFolder, numFilesInDownloadsFolder);

            var diretorio = GetDiretorioEmpresa(declaracao, mes);

            Thread.Sleep(1000);
            //Muda o nome
            Util.RenameLastModifiedFileInFolder(DownloadFolder, newName, diretorio);
            numFilesInDownloadsFolder++;
        }

        public static string GetDiretorioEmpresa(Declaracao declaracao, int mes)
        {
            string folderTipoDeclaracao = GetFolderTipoDeclaracao(declaracao, mes);
            return Path.Combine(DownloadFolder, folderTipoDeclaracao,
                empresaAutenticada.Codigo + "-" + empresaAutenticada.NIF);
        }

        public static string GetFolderTipoDeclaracao(Declaracao declaracao, int mes)
        {
            if (declaracao.Tipo == Declaracao.TipoDeclaracao.Anual)
                return "Anuais";
            else if (declaracao.Tipo == Declaracao.TipoDeclaracao.Mensal)
                return mes.ToString();
            else if (declaracao.Tipo == Declaracao.TipoDeclaracao.Lista)
                return "Listas";
            else if (declaracao.Tipo == Declaracao.TipoDeclaracao.Pedido)
                return "Pedidos";

            return "A";
        }

        internal static void DownloadFundoCompDocPag(int ano, int mes)
        {
            try
            {
                driver.Navigate().GoToUrl("https://www.fundoscompensacao.pt/fc/gfct/home?windowId=c05");
                ClickButtonWaitForItToAppear(By.XPath("/html/body/div[1]/div[2]/span/div/div/div/div[1]/div/form/div/div[4]/h3/a"));
                Thread.Sleep(500);
                ClickButtonWaitForItToAppear(By.XPath("/html/body/div[1]/div[2]/span/div/div/div/div[1]/div/form/div/div[4]/div/ul/li/a"));

                var printBtn = driver.FindElement(By.Id("form:btnPrintReport"));
                var gerarBtn = driver.FindElement(By.Id("form:btnGenReport"));
                ExpectDownload();
                if (gerarBtn.GetAttribute("aria-disabled") == "false")
                {
                    gerarBtn.Click();
                    ClickButtonWaitForItToAppear(By.Id("form:yesGenReport"));
                }
                else
                {
                    printBtn.Click();
                }

                //Obtem o ano e mes a que isto se refere
                string[] anoMesDeRef = driver.FindElement(By.Id("form:anomesref2")).Text.Trim().Split('-');
                int anoDeReferencia = Int32.Parse(anoMesDeRef[0]);
                int mesDeReferencia = Int32.Parse(anoMesDeRef[1]);

                WaitForDownloadFinish(GenNovoNomeFicheiro(Definicoes.estruturaNomesFicheiros.SS_FundosComp_DocPag, new { anoDeReferencia, mesDeReferencia }),
                    Declaracao.SS_FundosComp_DocPag, mesDeReferencia);
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
            var element = driver.FindElement(by);
            ButtonRunOnClick(element);
        }
        internal static void ButtonRunOnClick(IWebElement element)
        {
            string onClickCode = element.GetAttribute("onclick");
            if (onClickCode != null)
            {
                ((IJavaScriptExecutor)driver).ExecuteScript(onClickCode);
            }
            else
            {
                string href = element.GetAttribute("href");
                driver.Navigate().GoToUrl(href);
            }
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
                try
                {
                    //Tenta carregar no botão do modo normal
                    btn.First().Click();
                }
                catch (Exception ex)
                {
                    //Se não conseguir tenta carregar pelo onclick ou href
                    ButtonRunOnClick(btn.First());
                }
                return true;
            }
            return false;
        }

        //Cria a instacia do driver(chrome)
        private static void CriarDriver(string downloadFolder, bool headless)
        {
            // this will make automatically download to the default folder.
            chromeOptions = new ChromeOptions();
            chromeOptions.AddUserProfilePreference("plugins.always_open_pdf_externally", true);
            chromeOptions.AddUserProfilePreference("profile.default_content_settings.popups", 0);
            chromeOptions.AddUserProfilePreference("directory_upgrade", true);
            chromeOptions.AddUserProfilePreference("download.default_directory", downloadFolder);
            chromeOptions.AddUserProfilePreference("profile.default_content_setting_values.automatic_downloads", 1);
            chromeOptions.AddUserProfilePreference("safebrowsing.disable_download_protection", 1);
            chromeOptions.AddUserProfilePreference("credentials_enable_service", false);
            chromeOptions.AddUserProfilePreference("profile.password_manager_enabled", false);
            chromeOptions.AddUserProfilePreference("profile.default_content_setting_values.images", 2);
            if (headless)
                chromeOptions.AddArgument("--headless");


            //tentar --headless para nao mostrar nada

            //ChromeDriverService chromeDriverService = ChromeDriverService.CreateDefaultService("chromedriver.exe", "Chrome-bin\\chrome.exe");
            ChromeDriverService chromeDriverService = ChromeDriverService.CreateDefaultService();
            chromeDriverService.HideCommandPromptWindow = true;

            driver = new ChromeDriver(chromeDriverService, chromeOptions);
            autenticadoEm = new bool[10];
        }

        private static void FecharDriver()
        {
            if (driver == null)
                return;

            driver.Close();
            driver.Quit();
            empresaAutenticada = null;
            driver = null;
            autenticadoEm = null;
        }


        private static string GenNovoNomeFicheiro(string estruturaNomesFicheiro, object parametros)
        {
            //object dataMesAnteior = new { ano = (Mes == 1 ? Ano - 1 : Ano), mes = (Mes == 1 ? 12 : Mes - 1) };

            object dataHoje = new { ano = DateTime.Now.Year, mes = DateTime.Now.Month, dia = DateTime.Now.Day };
            
            return Smart.Format(estruturaNomesFicheiro, new { mes = Mes, ano = Ano/*, dataMesAnteior*/, dataHoje, empresa = empresaAutenticada, parametros = parametros });
        }
        private static string GenNovoNomeFicheiro(string estruturaNomesFicheiro)
        {
            return GenNovoNomeFicheiro(estruturaNomesFicheiro, null);
        }

        private static void LogError(string error)
        {
            errors.Add(error);
        }
        private static void LogError(Exception ex, Empresa empresa, Declaracao declaracao)
        {
            if(declaracao == null)
                errors.Add(empresa.NIF + " erro: " + ex.Message);
            else
                errors.Add(empresa.NIF + " erro em " + declaracao.Nome + ": " + ex.Message);
        }

        internal static void ClearErrorLogs()
        {
            errors.Clear();
        }

        public static void Autenticar(Empresa empresa, Declaracao[] declaracoes)
        {
            //Vê em que serviços é necessário autenticar
            foreach (Declaracao declaracao in declaracoes)
                autenticadoEm[(int)declaracao.AutenticacaoNecessaria] = true;

            //Autenticar
            if (autenticadoEm[(int)Declaracao.Autenticacao.AT])
            {
                Autenticacao.AutenticarAT(driver, empresa);
            }

            if (autenticadoEm[(int)Declaracao.Autenticacao.SSFundosCompensacao])
            {
                Autenticacao.AutenticarFundosCompensacao(driver, empresa);
            }

            if (autenticadoEm[(int)Declaracao.Autenticacao.SSDireta])
            {
                Autenticacao.AutenticarSS(driver, empresa);
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
