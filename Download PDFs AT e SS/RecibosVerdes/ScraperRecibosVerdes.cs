using Newtonsoft.Json;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
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


            string dados = driver.FindElement(By.XPath("//*[@id=\"main-content\"]/div/div/detalhe-fatura-recibo-app-v2")).GetAttribute("info-documento");
            // 1️⃣ Decode HTML
            string json = WebUtility.HtmlDecode(dados);

            // 2️⃣ Deserialize
            ReciboVerdeDto dto = JsonConvert.DeserializeObject<ReciboVerdeDto>(json);

            ReciboVerde recibo = new ReciboVerde();

            recibo.tipoDoc = dto.tipoDocumento;
            recibo.numDoc = dto.numDocumento.ToString();

            recibo.nifPrestadorServicos = dto.nifPrestadorServicos.ToString();
            recibo.nifAdquirente = dto.nifAdquirente;
            recibo.nomePrestador = dto.nomePrestador;
            recibo.nomeAdquirente = dto.nomeAdquirente;

            if (dto.linhasMapeadas != null && dto.linhasMapeadas.Length > 0)
                recibo.descricao = dto.linhasMapeadas[0].descricao;
            else
                recibo.descricao = "";

            recibo.paisAdquirente = dto.paisDescr;

            recibo.dataEmissao = DateTime.Parse(dto.dataEmissao);
            recibo.dataPrestacaoServico = DateTime.Parse(dto.dataPrestacaoServico);

            recibo.detailsUrl = dto.urlNotasCreditoDoDoc;

            // Anulado logic
            recibo.anulado = dto.situacaoCod != "E";

            // Valores
            RecibosVerdesValores valores = new RecibosVerdesValores();
            valores.valorBase = dto.valorBase;
            valores.valorIvaContinente = dto.valorIVA;
            valores.impostoSelo = dto.valorImpostoSelo;
            valores.irsSemRetencao = dto.valorIRS;
            valores.importanciaRecebida = dto.importanciaRecebida;

            recibo.valores = valores;

            // TipoReciboVerde (C# 7 switch)
            switch (dto.tipoDocumentoCodigo)
            {
                case "FR":
                    recibo.tipoReciboVerde = TipoReciboVerde.Pagamento;
                    break;

                case "F":
                    recibo.tipoReciboVerde = TipoReciboVerde.Fatura;
                    break;

                case "R":
                    recibo.tipoReciboVerde = TipoReciboVerde.Recibo;
                    break;

                default:
                    recibo.tipoReciboVerde = TipoReciboVerde.Pagamento;
                    break;
            }

            recibo.tipo = TipoReciboVerdePrestOUAdquir.Prestador;

            return recibo;
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

    internal class ReciboVerdeDto
    {
        public int numDocumento { get; set; }
        public string tipoDocumento { get; set; }
        public string tipoDocumentoCodigo { get; set; }
        public string situacaoCod { get; set; }

        public int nifPrestadorServicos { get; set; }
        public string nifAdquirente { get; set; }
        public string nomePrestador { get; set; }
        public string nomeAdquirente { get; set; }
        public string paisDescr { get; set; }

        public string dataEmissao { get; set; }
        public string dataPrestacaoServico { get; set; }

        public decimal valorBase { get; set; }
        public decimal valorIVA { get; set; }
        public decimal valorIRS { get; set; }
        public decimal valorImpostoSelo { get; set; }
        public decimal importanciaRecebida { get; set; }

        public string urlNotasCreditoDoDoc { get; set; }

        public LinhaDto[] linhasMapeadas { get; set; }
    }

    internal class LinhaDto
    {
        public string descricao { get; set; }
    }

}
