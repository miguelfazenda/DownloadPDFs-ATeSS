using Download_PDFs_AT_e_SS.RecibosVerdes;
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

        /**
         * Esta função vai a cada recibo verde, navegando todas as páginas, e corre a action, dizendo qual o URL para transferir o PDF
         * ModoConsulta: Prestador ou Adquirente
         * Mes: -1 é o ano todo
         */
        internal static void RecibosVerdesEmitidosNavegarPorCadaRecibo(int ano, int mes, TipoReciboVerdePrestOUAdquir modoConsulta, Action<string, string, string> action)
        {
            int diaInicial = 1;
            int diaFinal;
            int mesInicial;
            int mesFinal;

            if (mes == -1)
            {
                //De 1-Jan a 31-Dez
                diaFinal = 31;
                mesInicial = 1;
                mesFinal = 12;
            }
            else
            {
                diaFinal = DateTime.DaysInMonth(ano, mes);
                mesInicial = mes;
                mesFinal = mes;
            }


            string url = String.Format("https://irs.portaldasfinancas.gov.pt/recibos/portal/consultar#?isAutoSearchOn=on&dataEmissaoInicio={0}-{1}-{2}&dataEmissaoFim={0}-{3}-{4}",
                ano, mesInicial, diaInicial, mesFinal, diaFinal);

            if(modoConsulta == TipoReciboVerdePrestOUAdquir.Prestador)
                url += String.Format("&modoConsulta=Prestador&nifPrestadorServicos={0}", empresaAutenticada.NIF);
            else
                url += String.Format("&modoConsulta=Adquirente&nifAdquirente={0}", empresaAutenticada.NIF);

            driver.Navigate().GoToUrl(url);
            Thread.Sleep(500);
            driver.Navigate().GoToUrl(url);

            Thread.Sleep(500);

            var xPathTotalRecibos = By.XPath("/html/body/div/main/div/div[2]/div/section/div/div/consultar-app/div[3]/div/div[1]/consultar-tabela/div/div/table/tfoot/tr/td/div/div[1]/p");
            
            //Se não encontrar o numero de recibos, é porque há um erro. Faz log desse erro
            if (!Util.IsElementPresent(driver, xPathTotalRecibos))
            {
                LogError(driver.FindElement(By.XPath("/html/body/div/main/div/div[2]/div/section/div/div/consultar-app/div[2]")).Text);
                return;
            }

            int totalResultados = Int32.Parse(driver.FindElement(xPathTotalRecibos).Text);
            if (totalResultados == 0)
                return;

            //Escolhe mostrar 50 items por pagina
            driver.FindElement(By.XPath("/html/body/div/main/div/div[2]/div/section/div/div/consultar-app/div[2]/div/div/div/div/div/pf-table-size-picker/div/button")).Click();
            driver.FindElement(By.XPath("//*[@id=\"main-content\"]/div/div/consultar-app/div[2]/div/div/div/div/div/pf-table-size-picker/div/ul/li[4]/a")).Click();
            const int NUM_RESULTADOS_POR_PAG = 50;
            Thread.Sleep(500);

            //Obtem o numero de paginas
            var xPathNumeroPaginas = By.XPath("//*[@id=\"main-content\"]/div/div/consultar-app/div[3]/div/div[1]/consultar-tabela/div/div/table/tfoot/tr/td/div/div[3]/st-pagination/ul/li[last()-1]/a");
            int numeroPaginas = 1;
            if (Util.IsElementPresent(driver, xPathNumeroPaginas))
                numeroPaginas = Int32.Parse(driver.FindElement(xPathNumeroPaginas).Text);
            else
                numeroPaginas = 1; //Se não houver nada a dizer o numero de paginas é porque é a única
            

            for (int pag = 0; pag < numeroPaginas; pag++)
            {
                //Se é a ultima página
                bool ultimaPagina = pag == numeroPaginas - 1;

                //Calcula o numero de resultados que devem estar nesta pagina
                int numResultadosNestaPagina;
                if (ultimaPagina)
                {
                    numResultadosNestaPagina = totalResultados % NUM_RESULTADOS_POR_PAG;
                    if (numResultadosNestaPagina == 0)
                        numResultadosNestaPagina = NUM_RESULTADOS_POR_PAG;
                }
                else
                {
                    numResultadosNestaPagina = NUM_RESULTADOS_POR_PAG;
                }



                for (int i = 0; i < numResultadosNestaPagina; i++)
                {
                    string xPathConjuntoBotoes = "/html/body/div/main/div/div[2]/div/section/div/div/consultar-app/div[3]/div/div[1]/consultar-tabela/div/div/table/tbody/tr[" + (i + 1) + "]/td[5]/div";

                    //Obtem nr recibo
                    string numRecibo = driver.FindElement(By.XPath("/html/body/div/main/div/div[2]/div/section/div/div/consultar-app/div[3]/div/div[1]/consultar-tabela/div/div/table/tbody/tr[" + (i + 1) + "]/td[1]/p[1]")).Text;
                    numRecibo = numRecibo.Split('º')[1].Trim();

                    string nomeCliente = driver.FindElement(By.XPath("/html/body/div/main/div/div[2]/div/section/div/div/consultar-app/div[3]/div/div[1]/consultar-tabela/div/div/table/tbody/tr[" + (i + 1) + "]/td[1]/p[2]")).Text;

                    string downloadURL = driver.FindElement(By.XPath(xPathConjuntoBotoes + "/ul/li[2]/a")).GetAttribute("href");

                    //Invoca a action
                    action.Invoke(downloadURL, numRecibo, nomeCliente);
                }

                if (ultimaPagina)
                    continue; //Não anda uma página para a frente se for a ultima
                //Anda uma pagina para a frente
                driver.FindElement(By.XPath("//*[@id=\"main-content\"]/div/div/consultar-app/div[3]/div/div[1]/consultar-tabela/div/div/table/tfoot/tr/td/div/div[3]/st-pagination/ul/li[last()]/a")).Click();
            }
        }

        public const int NOME_CLIENTE_CLIP_LENGTH = 10;

        internal static void DownloadRecibosVerdesEmitidos(int ano, int mes)
        {
            RecibosVerdesEmitidosNavegarPorCadaRecibo(ano, mes, TipoReciboVerdePrestOUAdquir.Prestador, (string downloadURL, string numRecibo, string nomeCliente) =>
            {
                //Para cada recibo, transfere-o

                //Os parametros que vao ser usados para criar o nome do PDF
                string nomeClienteClipped = nomeCliente.Length > NOME_CLIENTE_CLIP_LENGTH ? nomeCliente.Substring(0, NOME_CLIENTE_CLIP_LENGTH) : nomeCliente;
                object newNameParams = new { numRecibo, nomeClienteClipped };

                //Transfere
                ExpectDownload();
                ((IJavaScriptExecutor)driver).ExecuteScript("window.open(\"" + downloadURL + "\")");
                WaitForDownloadFinish(GenNovoNomeFicheiro(Definicoes.estruturaNomesFicheiros.AT_LISTA_RECIBOS_VERDES, newNameParams), Declaracao.AT_LISTA_RECIBOS_VERDES, mes);
            });
        }

        const string XPATH_IES_BOTAO_OBTER = "/html/body/div/main/div/div[2]/div/section/div[3]/div[2]/div/div[3]/div/div/table/tbody/tr/td[3]/div/button";
        internal static void DownloadIES(int ano)
        {
            driver.Navigate().GoToUrl("https://oa.portaldasfinancas.gov.pt/ies/consultarIES.action?anoDeclaracoes=" + ano);
            if (Util.IsElementPresentWaitAWhile(driver, By.XPath(XPATH_IES_BOTAO_OBTER)))
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

            Thread.Sleep(1000);

            int NUM_DOC_POR_PAGINA = 10;
            var numTotalDocumentos = Int32.Parse(driver.FindElement(By.XPath("/html/body/div/main/div/div[2]/div/section/div/obter-comprovativo-e-doc-pagamento-app/div[3]/obter-comprovativo-e-doc-pagamento-tabela/div/div/div/div/div/div[1]/p")).Text);
            int numPaginas = (numTotalDocumentos - 1) / NUM_DOC_POR_PAGINA + 1;

            //Guarda o numero na lista do ficheiro que deve ser transferido para cada periodo (docPeriodo, {#na lista, página})
            Dictionary<string, int[]> indicesDeCadaPeriodo = new Dictionary<string, int[]>();

            for(int pagina = 0; pagina< numPaginas; pagina++)
            {
                if(numPaginas > 1)
                {
                    //Carrega no botão para selecionar a página certa
                    string pageButton = "/html/body/div/main/div/div[2]/div/section/div/obter-comprovativo-e-doc-pagamento-app/div[3]/obter-comprovativo-e-doc-pagamento-tabela/div/div/div/div/div/div[3]/ul/li[" + (2 + pagina) + "]/a";
                    driver.FindElement(By.XPath(pageButton)).Click();
                }

                //Para cada página
                var numDocumentos = driver.FindElements(By.XPath("/html/body/div/main/div/div[2]/div/section/div/obter-comprovativo-e-doc-pagamento-app/div[3]/obter-comprovativo-e-doc-pagamento-tabela/div/div/div/div/table/tbody/*")).Count;

                //Coloca no dicionario indicesDeCasaPeriodo os documentos a transferir (indice na tabela de onde transferir)
                for (int i = 0; i < numDocumentos; i++)
                {
                    string xpathRow = "/html/body/div/main/div/div[2]/div/section/div/obter-comprovativo-e-doc-pagamento-app/div[3]/obter-comprovativo-e-doc-pagamento-tabela/div/div/div/div/table/tbody/tr[" + (i + 1) + "]";
                    string docPeriodo = driver.FindElement(By.XPath(xpathRow + "/td[2]/p")).Text;

                    if (indicesDeCadaPeriodo.ContainsKey(docPeriodo))
                        //Se encontrar outra vez um documento do mesmo periodo substitui o indice de onde tranferir
                        indicesDeCadaPeriodo[docPeriodo] = new int[] { i, pagina };
                    else
                        indicesDeCadaPeriodo.Add(docPeriodo, new int[] { i, pagina });
                }
            }

            //Transfere os indices guardados para tal
            foreach (int[] idxEPagina in indicesDeCadaPeriodo.Values)
            {
                int i = idxEPagina[0];

                if(numPaginas > 1)
                {
                    int pagina = idxEPagina[1];
                    //Carrega no botão para selecionar a página certa
                    string pageButton = "/html/body/div/main/div/div[2]/div/section/div/obter-comprovativo-e-doc-pagamento-app/div[3]/obter-comprovativo-e-doc-pagamento-tabela/div/div/div/div/div/div[3]/ul/li[" + (2 + pagina) + "]/a";
                    driver.FindElement(By.XPath(pageButton)).Click();
                }

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

        public static string TableToText(string tableBodyXPath)
        {
            int numRows = driver.FindElements(By.XPath(tableBodyXPath + "/*")).Count;
            int numColumns = -1;

            //string[,] tableText = null;
            string tableText = "";

            //Obtem os valores da tabela
            for (int i = 0; i < numRows; i++)
            {
                string rowXPath = tableBodyXPath + "/tr[" + (i + 1) + "]";
                if(numColumns == -1)
                {
                    //Se for a primeira linha conta o numero de colunas e aloca a matriz
                    numColumns = driver.FindElements(By.XPath(tableBodyXPath + "/*")).Count;
                    //tableText = new string[numRows, numColumns];
                }

                for(int j = 0; j < numColumns; j++)
                {
                    string cellXPath;
                    if (i == 0)
                        cellXPath = rowXPath + "/th[" + (j + 1) + "]";
                    else
                        cellXPath = rowXPath + "/td[" + (j + 1) + "]";

                    //tableText[i, j] = driver.FindElement(By.XPath(cellXPath)).Text;
                    string cellText = driver.FindElement(By.XPath(cellXPath)).Text;
                    tableText += cellText + new String(' ', 17-cellText.Length);
                }
                tableText += "\n";
            }

            return tableText;
        }

        public static void DownloadIMINotasCobranca(int ano)
        {
            driver.Navigate().GoToUrl("https://www.portaldasfinancas.gov.pt/pt/main.jsp?body=/ca/notasCobrancaForm.jsp");
            ((IJavaScriptExecutor)driver).ExecuteScript("submitNotasCobrancaIMI('" + ano + "');");

            string tableText = (string) ((IJavaScriptExecutor)driver).ExecuteScript("" +
                "  opt_cellValueGetter = opt_cellValueGetter || function(td) { return td.textContent || td.innerText; };\n" +
                "  var tableText = '';\n" +
                "  var rowCount = tbl.rows.length\n" +
                "  for (var rowIndex = 0, tr; rowIndex < rowCount; rowIndex++) {\n" +
                "    var tr = tbl.rows[rowIndex];\n" +
                "    for (var colIndex = 0, colCount = tr.cells.length, offset = 0; colIndex < colCount; colIndex++) {\n" +
                "      var td = tr.cells[colIndex], text = opt_cellValueGetter(td, colIndex, rowIndex, tbl);\n" +
                "      for (var i = 0, colSpan = parseInt(td.colSpan, 10) || 1; i < colSpan; i++) {\n" +
                "        for (var j = 0, rowSpan = parseInt(td.rowSpan, 10) || 1; j < rowSpan; j++) {\n" +
                "          tableText += text + ' '.repeat(18 - text.length);\n" +
                "        }\n" +
                "      }\n" +
                "    }\n" +
                "    tableText += '\n';\n" +
                "  }\n" +
                "  return tableText;\n");

            Log("", tableText);
        }
    }
}
