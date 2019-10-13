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
        internal static void DownloadExtratoRemuneracoes(int ano, int mes)
        {
            driver.Navigate().GoToUrl("https://app.seg-social.pt/ptss/gr/pesquisa/consultarDR?dswid=7064&frawMenu=1");

            //Coloca as datas nos campos
            string anoMesStr = ano + "-" + (mes < 10 ? ("0" + mes) : mes.ToString());
            string anoMesDiaStr = ano + "-" + (mes < 10 ? ("0" + mes) : mes.ToString()) + "-01";
            ((IJavaScriptExecutor)driver).ExecuteScript("document.getElementById(\"dadosPesquisaDeclaracoes:dataReferenciaFimMonthPicker:calendar_input\").value = \"" + anoMesStr + "\"");
            ((IJavaScriptExecutor)driver).ExecuteScript("document.getElementById(\"dadosPesquisaDeclaracoes:dataReferenciaInicioMonthPicker:calendar_input\").value = \"" + anoMesStr + "\"");
            ((IJavaScriptExecutor)driver).ExecuteScript("document.getElementById(\"dadosPesquisaDeclaracoes:dataEntregaInicio:calendar_input\").value = \"" + anoMesDiaStr + "\"");

            //Pesquisar (quando concluir cria um elemento com id=PesquisaConcluida)
            /*((IJavaScriptExecutor)driver).ExecuteScript("PrimeFaces.ab({s:\"dadosPesquisaDeclaracoes:pesquisa\",p:\"dadosPesquisaDeclaracoes\",u:\"dadosPesquisaDeclaracoes listaDeclaracoes\",onst:function(cfg){PF('frawPageBlocker').show(); try{PF('varTabelaDeclaracoes').getPaginator().setPage(0)}catch(err){};},onco:function(xhr,status,args){PF('frawPageBlocker').hide();" +
                "document.body.innerHTML += '<div id=\"PesquisaConcluida\"></div>';}});return false;");
                */

            ButtonRunOnClick(By.Id("dadosPesquisaDeclaracoes:pesquisa"));

            //Esperar que acabe de pesquisar (espera que apareça um elemento com id=PesquisaConcluida)
            Thread.Sleep(200);
            var waitableDriver = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            var element = waitableDriver.Until(ExpectedConditions.InvisibilityOfElementLocated(By.Id("j_idt28_blocker")));


            if (Util.IsElementPresent(driver, By.ClassName("ui-datatable-empty-message")))
            {
                //Se a procura não encontrou nenhuma declaração
            }
            else
            {
                //Se a procura encontrou alguma declaração
                var numExtratos = driver.FindElements(By.XPath("//*[@id=\"formListaDeclaracoes:tabelaDeclaracoes_data\"]/*")).Count;
                for (int i = 0; i < numExtratos; i++)
                {
                    ExpectDownload();
                    //Obtem o codigo que o botão de imprimir tem e executa-o, transferindo assim o PDF
                    ButtonRunOnClick(By.Id("formListaDeclaracoes:tabelaDeclaracoes:" + i + ":imprimirExtrato"));

                    WaitForDownloadFinish(GenNovoNomeFicheiro(Definicoes.estruturaNomesFicheiros.SS_ExtratoRemun),
                        Declaracao.SS_ExtratoRemun, mes);
                    Thread.Sleep(1000);
                }
            }
        }
    }
}
