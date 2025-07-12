using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Download_PDFs_AT_e_SS.RecibosVerdes
{
    internal class ScraperRecibosVerdes
    {
        private static readonly Regex regexReciboVerdeDataEmissao = new Regex("\\d\\d\\d\\d-\\d\\d-\\d\\d");
        /**
         * Esta função navega até à pagina de detalhes, e obtem os dados do recibo, 
         * tipo: Prestador ou Adquirente
         **/
        internal static ReciboVerde ObterDadosReciboVerde(string detailsUrl, TipoReciboVerdePrestOUAdquir tipo, IWebDriver driver, int ano)
        {
            driver.Navigate().GoToUrl(detailsUrl);


            ReciboVerde reciboVerde = new ReciboVerde();
            reciboVerde.detailsUrl = detailsUrl;
            reciboVerde.tipo = tipo; // Prestador ou Adquirente


            

            // /html/body/div/main/div/div[2]/div/section/div[2]/div/div/div[2]/div/h1/text()
            //Obter dados
            if (ano >= 2023)
            {
                string[] a = driver.FindElement(By.XPath("/html/body/div/main/div/div[2]/div/section/div[2]/div/div/div[2]/div/h1")).Text.Split('/');
                reciboVerde.numDoc = a[1];
            }
            else
            {
                //var title_element_text = driver.FindElement(By.XPath("//h1")).Text;
                string[] doc = driver.FindElement(By.XPath("/html/body/div/main/div/div[2]/div/section/div[2]/div/div/div[2]/div/h1")).Text.Split(' ');
                reciboVerde.numDoc = doc[2];
            }

            // Remove todos os carateres nao numericos do numDoc
            reciboVerde.numDoc = new string(reciboVerde.numDoc.Where(c => char.IsDigit(c)).ToArray());






            // Extrai dados da legenda
            var dados_legend = driver.FindElement(By.CssSelector("legend")).Text.ToString().Replace("\r", "").Split('\n');

            var linha_data_emissao = dados_legend.First((x) => x.StartsWith("Emitid"));
            string dataEmissao = Regex.Match(linha_data_emissao, "\\b\\d{4}-\\d{2}-\\d{2}\\b").Value;
            reciboVerde.dataEmissao = DateTime.ParseExact(dataEmissao, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            reciboVerde.tipoDoc = dados_legend[0];




            //Seleciona os XPaths para obter cada valor do HTML, dependendo do tipo de documento
            RecibosVerdesXPaths xPaths = null;
            if (reciboVerde.tipoDoc == "Fatura-Recibo")
                xPaths = RecibosVerdesXPaths.RECIBOS_VERDES_XPATHS_FATURA_RECIBO;
            else if (reciboVerde.tipoDoc == "Fatura")
                xPaths = RecibosVerdesXPaths.RECIBOS_VERDES_XPATHS_FATURA;
            else if (reciboVerde.tipoDoc == "Recibo")
                xPaths = RecibosVerdesXPaths.RECIBOS_VERDES_XPATHS_RECIBO;
            else
                throw new Exception(String.Format("Tipo de recibo verde \"{0}\" desconhecido", reciboVerde.tipoDoc));






            string estado = driver.FindElement(By.XPath("/html/body/div/main/div/div[2]/div/section/div[2]/div/div/div[2]/div/h1/span")).Text;
            reciboVerde.anulado = estado.ToLower() == "anulado";

            string xPathDataEmissao = ano >= 2023 ? xPaths.dataEmissaoPos2023 : xPaths.dataEmissaoPre2023;

            
            string dataTransmissao = null;
            if (xPaths.dataTransmissao != null)
            {
                dataTransmissao = driver.FindElement(By.XPath(xPaths.dataTransmissao)).Text;
                reciboVerde.dataPrestacaoServico = DateTime.ParseExact(dataTransmissao, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            }
            reciboVerde.nifPrestadorServicos = driver.FindElement(By.XPath(xPaths.nifTransmitente)).Text;
            reciboVerde.nifAdquirente = driver.FindElement(By.XPath(xPaths.nifAdquirente)).Text;
            if (xPaths.descricao != null)
            {
                reciboVerde.descricao = driver.FindElement(By.XPath(xPaths.descricao)).Text;
            }
            else
            {
                reciboVerde.descricao = "sem descricao";
            }
            reciboVerde.nomeAdquirente = driver.FindElement(By.XPath(xPaths.nomeAdquirente)).Text;
            reciboVerde.nomePrestador = driver.FindElement(By.XPath(xPaths.nomeTrasmitente)).Text;
            if (xPaths.paisAdquirente != null)
            {
                reciboVerde.paisAdquirente = driver.FindElement(By.XPath(xPaths.paisAdquirente)).Text;
            }
            string valorBaseStr = driver.FindElement(By.XPath(xPaths.valorBaseStr)).Text;
            string valorIvaContinenteStr = driver.FindElement(By.XPath(xPaths.valorIvaContinenteStr)).Text;
            string importanciaRecebidaStr = driver.FindElement(By.XPath(xPaths.importanciaRecebidaStr)).Text;
            reciboVerde.valores = default(RecibosVerdesValores);
            reciboVerde.valores.valorBase = Convert.ToDecimal(valorBaseStr.Remove(valorBaseStr.Length - 2), Downloader.culture);
            reciboVerde.valores.valorIvaContinente = Convert.ToDecimal(valorIvaContinenteStr.Remove(valorIvaContinenteStr.Length - 2), Downloader.culture);
            reciboVerde.valores.importanciaRecebida = Convert.ToDecimal(importanciaRecebidaStr.Remove(importanciaRecebidaStr.Length - 2), Downloader.culture);
            if(xPaths.impostoSeloStr != null)
            {
                string impostoSeloStr = driver.FindElement(By.XPath(xPaths.impostoSeloStr)).Text;
                reciboVerde.valores.impostoSelo = Convert.ToDecimal(impostoSeloStr.Remove(impostoSeloStr.Length - 2), Downloader.culture);
            }
            if (xPaths.irsSemRetencaoStr != null)
            {
                string irsSemRetencaoStr = driver.FindElement(By.XPath(xPaths.irsSemRetencaoStr)).Text;
                reciboVerde.valores.irsSemRetencao = Convert.ToDecimal(irsSemRetencaoStr.Remove(irsSemRetencaoStr.Length - 2), Downloader.culture);
            }



            if (reciboVerde.tipoDoc == "Fatura")
                reciboVerde.tipoReciboVerde = TipoReciboVerde.Fatura;
            else if (reciboVerde.tipoDoc == "Recibo")
                reciboVerde.tipoReciboVerde = TipoReciboVerde.Recibo;
            else if (reciboVerde.tipoDoc == "Fatura-Recibo")
            {
                //Obtem o tipo de recibo verde, vendo qual checkbox est]a checked
                IWebElement checkboxTipoPagamento = driver.FindElement(By.XPath("/html/body/div/main/div/div[2]/div/section/div[5]/div[2]/div/div[1]/dl/dt[2]/div/div[1]/label/input"));
                IWebElement checkboxTipoAdiantamento = driver.FindElement(By.XPath("/html/body/div/main/div/div[2]/div/section/div[5]/div[2]/div/div[1]/dl/dt[2]/div/div[2]/label/input"));
                IWebElement checkboxTipoAdiantamentoPagam = driver.FindElement(By.XPath("/html/body/div/main/div/div[2]/div/section/div[5]/div[2]/div/div[1]/dl/dt[2]/div/div[3]/label/input"));

                if (checkboxTipoPagamento.Selected)
                    reciboVerde.tipoReciboVerde = TipoReciboVerde.Pagamento;
                else if (checkboxTipoAdiantamento.Selected)
                    reciboVerde.tipoReciboVerde = TipoReciboVerde.Adiantamento;
                else
                    reciboVerde.tipoReciboVerde = TipoReciboVerde.AdiantamentoPagamento;
            }
            else
            {
                throw new Exception(String.Format("Tipo de recibo verde \"{0}\" desconhecido", reciboVerde.tipoDoc));
            }


            return reciboVerde;
        }

        public static string GetReciboTipoString(TipoReciboVerde tipoReciboVerde)
        {
            switch (tipoReciboVerde)
            {
                case TipoReciboVerde.Pagamento:
                    return "Pagamento";
                case TipoReciboVerde.Adiantamento:
                    return "Adiantamento";
                case TipoReciboVerde.AdiantamentoPagamento:
                    return "Adiantamento para pagamento";
            }

            return "-";
        }
    }
}
