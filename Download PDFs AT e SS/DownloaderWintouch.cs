using ConsoleTableExt;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;

namespace Download_PDFs_AT_e_SS
{
    /**
     * Esta Classe tem os downloaders que não são para formato PDF, mas sim para serem importados pelo wintouch
     */
    partial class Downloader
    {
        //A cultura, para converter string para decimals, usando a virgula para as casas decimais
        public readonly static CultureInfo culture = new CultureInfo("pt-PT");
        static Downloader()
        {
            culture.NumberFormat.NumberDecimalSeparator = ",";
            culture.NumberFormat.NumberGroupSeparator = ".";
        }

        internal static void DownloadRecibosVerdesEmitidosWintouchPrestados(int ano, int mes) =>
            DownloadRecibosVerdesEmitidosWintouch(ano, mes, TipoReciboVerdePrestOUAdquir.Prestador);
        internal static void DownloadRecibosVerdesEmitidosWintouchAdquiridos(int ano, int mes) =>
            DownloadRecibosVerdesEmitidosWintouch(ano, mes, TipoReciboVerdePrestOUAdquir.Adquirente);

        //Tipo: Prestador e Adquirente
        internal static void DownloadRecibosVerdesEmitidosWintouch(int ano, int mes, TipoReciboVerdePrestOUAdquir tipo)
        {
            List<string> detailsURLs = new List<string>();

            //Vai à lista de recibos verdes, para obter o URL de detalhes de cada um
            RecibosVerdesEmitidosNavegarPorCadaRecibo(ano, mes, tipo, (string downloadURL, string numRecibo, string nomeCliente) =>
            {
                //Para cada recibo, regista o URL para obter os detalhes
                string detailsUrl = downloadURL.Replace("/imprimir/", "/detalhe/").Replace("/normal", "");
                detailsURLs.Add(detailsUrl);
            });

            //Para contar os totais por tipo de recibo
            RecibosVerdesValores totaisTipoPagamento = new RecibosVerdesValores();
            RecibosVerdesValores totaisTipoAdiantamento = new RecibosVerdesValores();
            RecibosVerdesValores totaisTipoAdiantamentoPagamento = new RecibosVerdesValores();
            RecibosVerdesValores totaisAnulados = new RecibosVerdesValores();

            List<ReciboVerde> recibosVerdes = new List<ReciboVerde>(detailsURLs.Count);

            //List<ReciboVerde> recibosVerdes = (List<ReciboVerde>)new BinaryFormatter().Deserialize(new FileStream(@"C:\users\miguel\desktop\a.txt", FileMode.Open, FileAccess.Read));//TEMP

            //Depois de obtidos os URLs, navegar até à pagina de detalhes de cada um
            foreach (string detailsUrl in detailsURLs)
            {
                //Obtem os dados do recibo verde, navegado até à página de detalhes
                ReciboVerde reciboVerde = ObterDadosReciboVerde(detailsUrl, tipo);
                recibosVerdes.Add(reciboVerde);

                //Soma os valores para obter um total por tipo de recibo verde
                if (!reciboVerde.anulado)
                {
                    if (reciboVerde.tipoReciboVerde == TipoReciboVerde.Pagamento)
                        totaisTipoPagamento += reciboVerde.valores;
                    if (reciboVerde.tipoReciboVerde == TipoReciboVerde.Adiantamento)
                        totaisTipoAdiantamento += reciboVerde.valores;
                    if (reciboVerde.tipoReciboVerde == TipoReciboVerde.AdiantamentoPagamento)
                        totaisTipoAdiantamentoPagamento += reciboVerde.valores;
                }
                else
                {
                    totaisAnulados += reciboVerde.valores;
                }
            }

            //new BinaryFormatter().Serialize(new FileStream(@"c:\users\miguel\desktop\b.txt", FileMode.Create), recibosVerdes);
            GeraTabelaTxtTotais(totaisTipoPagamento, totaisTipoAdiantamento, totaisTipoAdiantamentoPagamento, totaisAnulados, mes, tipo);
            ExportarFicheiroWintouch(recibosVerdes, mes, tipo);
        }

       


        /**
         * Esta função navega até à pagina de detalhes, e obtem os dados do recibo, 
         * tipo: Prestador ou Adquirente
         **/
        private static ReciboVerde ObterDadosReciboVerde(string detailsUrl, TipoReciboVerdePrestOUAdquir tipo)
        {
            driver.Navigate().GoToUrl(detailsUrl);

            ReciboVerde reciboVerde = new ReciboVerde();
            reciboVerde.detailsUrl = detailsUrl;
            reciboVerde.tipo = tipo; // Prestador ou Adquirente

            //Obter dados
            string[] doc = driver.FindElement(By.XPath("/html/body/div/main/div/div[2]/div/section/div[2]/div/div/div[1]/div[1]/h1")).Text.Split(' ');
            reciboVerde.tipoDoc = doc[0];
            reciboVerde.numDoc = doc[2];

            string estado = driver.FindElement(By.XPath("/html/body/div/main/div/div[2]/div/section/div[2]/div/div/div[1]/div[1]/h1/span")).Text;
            reciboVerde.anulado = estado.ToLower() == "anulado";

            string dataEmissao = driver.FindElement(By.XPath("/html/body/div/main/div/div[2]/div/section/div[2]/div/div/div[2]/div/legend/small"))
                .Text.Replace("Emitida a ", "");
            string dataTransmissao = driver.FindElement(By.XPath("/html/body/div/main/div/div[2]/div/section/div[5]/div[2]/div/div[2]/dl/dd")).Text;
            reciboVerde.dataEmissao = DateTime.ParseExact(dataEmissao, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            reciboVerde.dataTransmissao = DateTime.ParseExact(dataTransmissao, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            reciboVerde.nifTransmitente = driver.FindElement(By.XPath("/html/body/div/main/div/div[2]/div/section/div[3]/div[2]/div/div[1]/dl/dd")).Text;
            reciboVerde.nifAdquirente = driver.FindElement(By.XPath("/html/body/div/main/div/div[2]/div/section/div[4]/div[2]/div[1]/div[1]/dl/dd")).Text;
            reciboVerde.descricao = driver.FindElement(By.XPath("/html/body/div/main/div/div[2]/div/section/div[5]/div[2]/div/div[3]/dl/dd")).Text;
            reciboVerde.nomeAdquirente = driver.FindElement(By.XPath("/html/body/div/main/div/div[2]/div/section/div[4]/div[2]/div[1]/div[2]/dl/dd")).Text;
            reciboVerde.nomeTrasmitente = driver.FindElement(By.XPath("/html/body/div/main/div/div[2]/div/section/div[3]/div[2]/div/div[2]/dl/dd")).Text;
            reciboVerde.paisAdquirente = driver.FindElement(By.XPath("/html/body/div/main/div/div[2]/div/section/div[4]/div[2]/div[2]/div/dl/dd")).Text;

            //Obtem as string que têm os valores
            string valorBaseStr = driver.FindElement(By.XPath("/html/body/div/main/div/div[2]/div/section/div[5]/div[2]/div/div[4]/dl/div[2]/dd")).Text;
            string valorIvaContinenteStr = driver.FindElement(By.XPath("/html/body/div/main/div/div[2]/div/section/div[5]/div[2]/div/div[4]/dl/div[4]/dd")).Text;
            string impostoSeloStr = driver.FindElement(By.XPath("/html/body/div/main/div/div[2]/div/section/div[5]/div[2]/div/div[4]/dl/div[6]/dd")).Text;
            string irsSemRetencaoStr = driver.FindElement(By.XPath("/html/body/div/main/div/div[2]/div/section/div[5]/div[2]/div/div[4]/dl/div[8]/dd")).Text;
            string importanciaRecebidaStr = driver.FindElement(By.XPath("/html/body/div/main/div/div[2]/div/section/div[5]/div[2]/div/div[4]/dl/div[10]/dd")).Text;

            //Converte os valores para decimal
            reciboVerde.valores = new RecibosVerdesValores();
            reciboVerde.valores.valorBase = Convert.ToDecimal(valorBaseStr.Remove(valorBaseStr.Length - 2), culture);
            reciboVerde.valores.valorIvaContinente = Convert.ToDecimal(valorIvaContinenteStr.Remove(valorIvaContinenteStr.Length - 2), culture);
            reciboVerde.valores.impostoSelo = Convert.ToDecimal(impostoSeloStr.Remove(impostoSeloStr.Length - 2), culture);
            reciboVerde.valores.irsSemRetencao = Convert.ToDecimal(irsSemRetencaoStr.Remove(irsSemRetencaoStr.Length - 2), culture);
            reciboVerde.valores.importanciaRecebida = Convert.ToDecimal(importanciaRecebidaStr.Remove(importanciaRecebidaStr.Length - 2), culture);

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

            return reciboVerde;
        }

        /// <summary>
        /// Esta função exporta os recibos para o ficheiro do wintouch
        /// </summary>
        private static void ExportarFicheiroWintouch(List<ReciboVerde> recibosVerdes, int mes, TipoReciboVerdePrestOUAdquir tipo)
        {

            var diretorio = Path.Combine(DownloadFolder, GetFolderTipoDeclaracao(Declaracao.AT_LISTA_RECIBOS_VERDES_PARA_WINTOUCH_PRESTADOS, mes),
               empresaAutenticada.Codigo + "-" + empresaAutenticada.NIF);
            Directory.CreateDirectory(diretorio);

            string nomeFicheiro;
            if (tipo == TipoReciboVerdePrestOUAdquir.Adquirente)
                nomeFicheiro = Path.Combine(diretorio, GenNovoNomeFicheiro(Definicoes.estruturaNomesFicheiros.AT_LISTA_RECIBOS_VERDES_WINTOUCH_ADQUIRIDOS));
            else
                nomeFicheiro = Path.Combine(diretorio, GenNovoNomeFicheiro(Definicoes.estruturaNomesFicheiros.AT_LISTA_RECIBOS_VERDES_WINTOUCH_PRESTADOS));

            using (StreamWriter fileStream = new StreamWriter(nomeFicheiro))
            {
                fileStream.WriteLine("WCONTAB5.60");
                foreach (ReciboVerde reciboVerde in recibosVerdes)
                {

                    WintouchExportarRecibo(fileStream, reciboVerde);
                }
            }
        }
        
        /// <summary>
        /// Exporta o recibo, escrevendo várias linhas no ficheiro
        /// </summary>
        /// <param name="fileStream"></param>
        /// <param name="reciboVerde"></param>
        /// <returns></returns>
        private static bool WintouchExportarRecibo(StreamWriter fileStream, ReciboVerde reciboVerde)
        {
            //As definiões de exportação indicam para que conta cada valor deve ir, etc.. Porque para tipos de documento (fatura-recibo, etc.)
            // e tipos de recibo (Pagamento, Adiantamento, etc.) são diferentes
            DefinicoesExportTipoReciboVerde definicoesExportTipoReciboVerde = ObterDefinicoesExportacaoRecibo(reciboVerde);

            int numLinha = 1;

            //Exporta uma linha para cada valor (valor base, iva, etc.)

            //A conta para qual o valor base vai depende se o valor de IVA é 0 ou não
            if(reciboVerde.valores.valorIvaContinente > 0)
            {
                WintouchExportarLinha(fileStream, reciboVerde, definicoesExportTipoReciboVerde,
                    reciboVerde.valores.valorBase, definicoesExportTipoReciboVerde.contaValBase, 'C', ref numLinha);
            }
            else
            {
                WintouchExportarLinha(fileStream, reciboVerde, definicoesExportTipoReciboVerde,
                    reciboVerde.valores.valorBase, definicoesExportTipoReciboVerde.contaValBaseIsento, 'C', ref numLinha);
            }

            WintouchExportarLinha(fileStream, reciboVerde, definicoesExportTipoReciboVerde, 
                reciboVerde.valores.valorIvaContinente, definicoesExportTipoReciboVerde.contaIVA, 'C', ref numLinha);

            WintouchExportarLinha(fileStream, reciboVerde, definicoesExportTipoReciboVerde,
                reciboVerde.valores.impostoSelo, definicoesExportTipoReciboVerde.contaSelo, 'C', ref numLinha);

            WintouchExportarLinha(fileStream, reciboVerde, definicoesExportTipoReciboVerde,
                reciboVerde.valores.irsSemRetencao, definicoesExportTipoReciboVerde.contaIRS, 'D', ref numLinha);

            WintouchExportarLinha(fileStream, reciboVerde, definicoesExportTipoReciboVerde,
                reciboVerde.valores.importanciaRecebida, definicoesExportTipoReciboVerde.contaValRecebida, 'D', ref numLinha);


            return true;
        }

        /// <summary>
        /// Obtem as definiões de exportação para o recibo verde, que indicam para que conta cada valor deve ir, etc..
        /// Porque para tipos de documento (fatura-recibo, etc.) e tipos de recibo (Pagamento, Adiantamento, etc.) são diferentes
        /// </summary>
        /// <param name="reciboVerde"></param>
        /// <returns></returns>
        private static DefinicoesExportTipoReciboVerde ObterDefinicoesExportacaoRecibo(ReciboVerde reciboVerde)
        {
            //Seleciona se usa as definicoes de Prestador ou Adquirente
            DefinicoesExportacao definicoesExportacaoTipo;
            if (reciboVerde.tipo == TipoReciboVerdePrestOUAdquir.Prestador)
                definicoesExportacaoTipo = Definicoes.definicoesExportacaoPrestador;
            else
                definicoesExportacaoTipo = Definicoes.definicoesExportacaoAdquirente;

            //Obtem para que conta cada valor deve ir, etc.
            //Seleciona consoante o tipo de documento (fatura-recibo, etc.)
            DefinicoesExportTipoDoc defExportFaturaRecibo;
            switch (reciboVerde.tipoDoc)
            {
                case "Fatura-Recibo":
                    defExportFaturaRecibo = definicoesExportacaoTipo.defExportFaturaRecibo;
                    break;
                case "Fatura":
                    defExportFaturaRecibo = definicoesExportacaoTipo.defExportFatura;
                    break;
                case "Recibo":
                    defExportFaturaRecibo = definicoesExportacaoTipo.defExportRecibo;
                    break;
                default:
                    throw new Exception(String.Format("Não está previsto o tipo documento {0}", reciboVerde.tipoDoc));
            }

            //Seleciona consoante o tipo de Recibo Verde (Pagamento, Adiantamento, etc.)
            DefinicoesExportTipoReciboVerde definicoesExportTipoReciboVerde = new DefinicoesExportTipoReciboVerde();
            switch (reciboVerde.tipoReciboVerde)
            {
                case TipoReciboVerde.Pagamento:
                    definicoesExportTipoReciboVerde = defExportFaturaRecibo.defExportTipoPagamento;
                    break;
                case TipoReciboVerde.Adiantamento:
                    definicoesExportTipoReciboVerde = defExportFaturaRecibo.defExportTipoAdiantamento;
                    break;
                case TipoReciboVerde.AdiantamentoPagamento:
                    definicoesExportTipoReciboVerde = defExportFaturaRecibo.defExportTipoAdiantamentoPagamento;
                    break;
            }
            return definicoesExportTipoReciboVerde;
        }

        /// <summary>
        /// Escreve uma linha no ficheiro do wintouch, caso o valor não seja zero, na conta especificada
        /// Return: Devolve se escreveu alguma coisa ou não
        /// </summary>
        private static bool WintouchExportarLinha(StreamWriter fileStream, ReciboVerde reciboVerde, DefinicoesExportTipoReciboVerde definicoesExportTipoReciboVerde,
            decimal valor, string codigoConta, char natureza, ref int numLinha)
        {
            if (valor == 0)
                return false; //Se o valor for 0 não escreve a linha

            string codigoDiario = definicoesExportTipoReciboVerde.diario;
            string codigoDocumento = definicoesExportTipoReciboVerde.tipoDoc;
            int serie = 1;

            string descricao = reciboVerde.descricao.Length > 50 ? reciboVerde.descricao.Substring(0, 50) : reciboVerde.descricao;

            int dia = reciboVerde.dataEmissao.Day;
            int mes = reciboVerde.dataEmissao.Month;

            string contibuinte;
            string nomeEntidade;
            if (reciboVerde.tipo == TipoReciboVerdePrestOUAdquir.Adquirente)
            {
                contibuinte = reciboVerde.nifTransmitente;
                nomeEntidade = reciboVerde.nomeTrasmitente;
            }
            else
            {
                contibuinte = reciboVerde.nifAdquirente;
                nomeEntidade = reciboVerde.nomeAdquirente;

            }
            nomeEntidade = nomeEntidade.Length > 50 ? nomeEntidade.Substring(0, 50) : nomeEntidade;

            int anulado = reciboVerde.anulado ? 1 : 0;

            string valorStr = String.Format("{0,18:F2}", valor).Replace(",", ".");
            //                            diario            serie             descric       nat  dia     mes      contrib       numLinha     nomeenti anulado
            string linha = String.Format("{0,10}{1,20}{2,20}{3,4}{4,10}{5,-20}{6,-50}{7,18}{8,1}{9,2:D2}{10,2:D2}{11,-20}F{12,20}{13,1}{14,5}{15,20}{16,-50}{17,1}",
                -1, codigoDiario, codigoDocumento, //2
                serie, reciboVerde.numDoc, codigoConta, //5
                descricao, valorStr, natureza, dia, mes, //10
                contibuinte, "", "C", numLinha, "", //14
                nomeEntidade, anulado);

            fileStream.WriteLine(linha);
            numLinha++;
            return true;
        }

        /// <summary>
        /// Gera a tabela com os totais e escreve-a num ficheiro
        /// </summary>
        /// <param name="totaisTipoPagamento"></param>
        /// <param name="totaisTipoAdiantamento"></param>
        /// <param name="totaisTipoAdiantamentoPagamento"></param>
        /// <param name="totaisAnulados"></param>
        /// <param name="tipo">Prestador ou Adquirente</param>
        private static void GeraTabelaTxtTotais(RecibosVerdesValores totaisTipoPagamento,
            RecibosVerdesValores totaisTipoAdiantamento, RecibosVerdesValores totaisTipoAdiantamentoPagamento,
            RecibosVerdesValores totaisAnulados,
            int mes, TipoReciboVerdePrestOUAdquir tipo)
        {
            //Gera a tabela para txt

            RecibosVerdesValores total = totaisTipoPagamento + totaisTipoAdiantamento + totaisTipoAdiantamentoPagamento;

            DataTable table = new DataTable();
            table.Columns.Add("Tipo", typeof(string));
            table.Columns.Add("Valor base", typeof(decimal));
            table.Columns.Add("IVA", typeof(decimal));
            table.Columns.Add("Imp. Selo", typeof(decimal));
            table.Columns.Add("IRS", typeof(decimal));
            table.Columns.Add("Recebido", typeof(decimal));

            table.Rows.Add("Pagamento", totaisTipoPagamento.valorBase, totaisTipoPagamento.valorIvaContinente,
                totaisTipoPagamento.impostoSelo, totaisTipoPagamento.irsSemRetencao,
                totaisTipoPagamento.importanciaRecebida);

            table.Rows.Add("Adiantamento", totaisTipoAdiantamento.valorBase, totaisTipoAdiantamento.valorIvaContinente,
                totaisTipoAdiantamento.impostoSelo, totaisTipoAdiantamento.irsSemRetencao,
                totaisTipoAdiantamento.importanciaRecebida);

            table.Rows.Add("Adiant. para despesas", totaisTipoAdiantamentoPagamento.valorBase, totaisTipoAdiantamentoPagamento.valorIvaContinente,
                totaisTipoAdiantamentoPagamento.impostoSelo, totaisTipoAdiantamentoPagamento.irsSemRetencao,
                totaisTipoAdiantamentoPagamento.importanciaRecebida);

            table.Rows.Add("Total", total.valorBase, total.valorIvaContinente, total.impostoSelo,
                total.irsSemRetencao, total.importanciaRecebida);

            table.Rows.Add("Total anulados", totaisAnulados.valorBase, totaisAnulados.valorIvaContinente, totaisAnulados.impostoSelo,
                totaisAnulados.irsSemRetencao, totaisAnulados.importanciaRecebida);


            var text = ConsoleTableBuilder.From(table).Export().ToString();

            var diretorio = Path.Combine(DownloadFolder, GetFolderTipoDeclaracao(Declaracao.AT_LISTA_RECIBOS_VERDES_PARA_WINTOUCH_PRESTADOS, mes),
               empresaAutenticada.Codigo + "-" + empresaAutenticada.NIF);
            Directory.CreateDirectory(diretorio);

            string nomeFicheiro;
            if(tipo == TipoReciboVerdePrestOUAdquir.Adquirente)
                nomeFicheiro = "Totais adquiridos.txt";
            else
                nomeFicheiro = "Totais emitidos.txt";

            File.WriteAllText(Path.Combine(diretorio, nomeFicheiro), text);
        }
    }

    [Serializable()]
    struct RecibosVerdesValores
    {
        public decimal valorBase;
        public decimal valorIvaContinente;
        public decimal impostoSelo;
        public decimal irsSemRetencao;
        public decimal importanciaRecebida;

        public static RecibosVerdesValores operator +(RecibosVerdesValores a, RecibosVerdesValores b)
        {
            RecibosVerdesValores soma = new RecibosVerdesValores();
            soma.valorBase = a.valorBase + b.valorBase;
            soma.valorIvaContinente = a.valorIvaContinente + b.valorIvaContinente;
            soma.impostoSelo = a.impostoSelo + b.impostoSelo;
            soma.irsSemRetencao = a.irsSemRetencao + b.irsSemRetencao;
            soma.importanciaRecebida = a.importanciaRecebida + b.importanciaRecebida;
            return soma;
        }
    }

    [Serializable()]
    enum TipoReciboVerde
    {
        Pagamento,
        Adiantamento,
        AdiantamentoPagamento
    }

    enum TipoReciboVerdePrestOUAdquir
    {
        Prestador = 0,
        Adquirente = 1
    }

    [Serializable()]
    class ReciboVerde
    {
        

        public string tipoDoc, numDoc;
        public string nifTransmitente, nifAdquirente, descricao, nomeAdquirente, nomeTrasmitente;
        public string paisAdquirente;
        public DateTime dataEmissao, dataTransmissao;
        public string detailsUrl;
        public bool anulado;
        public RecibosVerdesValores valores;
        public TipoReciboVerde tipoReciboVerde;

        public TipoReciboVerdePrestOUAdquir tipo; //Prestador ou Adquirente (PRESTADOR ou ADQUIRENTE)
    }
}