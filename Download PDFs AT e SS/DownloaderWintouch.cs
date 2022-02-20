using ConsoleTableExt;
using Download_PDFs_AT_e_SS.RecibosVerdes;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

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
            RecibosVerdesValores totaisTipoFatura = new RecibosVerdesValores();
            RecibosVerdesValores totaisTipoRecibo = new RecibosVerdesValores();
            RecibosVerdesValores totaisAnulados = new RecibosVerdesValores();

            List<ReciboVerde> recibosVerdes = new List<ReciboVerde>(detailsURLs.Count);

            //List<ReciboVerde> recibosVerdes = (List<ReciboVerde>)new BinaryFormatter().Deserialize(new FileStream(@"C:\users\miguel\desktop\a.txt", FileMode.Open, FileAccess.Read));//TEMP

            //Depois de obtidos os URLs, navegar até à pagina de detalhes de cada um
            foreach (string detailsUrl in detailsURLs)
            {
                //Obtem os dados do recibo verde, navegado até à página de detalhes
                ReciboVerde reciboVerde = ScraperRecibosVerdes.ObterDadosReciboVerde(detailsUrl, tipo, driver);
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
                    if (reciboVerde.tipoReciboVerde == TipoReciboVerde.Fatura)
                        totaisTipoFatura += reciboVerde.valores;
                    if (reciboVerde.tipoReciboVerde == TipoReciboVerde.Recibo)
                        totaisTipoRecibo += reciboVerde.valores;
                }
                else
                {
                    totaisAnulados += reciboVerde.valores;
                }
            }

            //new BinaryFormatter().Serialize(new FileStream(@"c:\users\miguel\desktop\b.txt", FileMode.Create), recibosVerdes);
            GeraTabelaTxtTotais(totaisTipoPagamento, totaisTipoAdiantamento, totaisTipoAdiantamentoPagamento, totaisTipoFatura, totaisTipoRecibo, totaisAnulados, mes, tipo);
            ExportarFicheiroWintouch(recibosVerdes, mes, tipo);
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
                case TipoReciboVerde.Fatura:
                case TipoReciboVerde.Recibo:
                    definicoesExportTipoReciboVerde = defExportFaturaRecibo.defExportTipoFaturaOuRecibo;
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
                contibuinte = reciboVerde.nifPrestadorServicos;
                nomeEntidade = reciboVerde.nomePrestador;
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
            RecibosVerdesValores totaisTipoFatura, RecibosVerdesValores totaisTipoRecibo,
            RecibosVerdesValores totaisAnulados,
            int mes, TipoReciboVerdePrestOUAdquir tipo)
        {
            //Gera a tabela para txt

            RecibosVerdesValores total = totaisTipoPagamento + totaisTipoAdiantamento + totaisTipoAdiantamentoPagamento;

            // Cria a tabela
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

            /*table.Rows.Add("Faturas", totaisTipoAdiantamentoPagamento.valorBase, totaisTipoAdiantamentoPagamento.valorIvaContinente,
                totaisTipoAdiantamentoPagamento.impostoSelo, totaisTipoAdiantamentoPagamento.irsSemRetencao,
                totaisTipoAdiantamentoPagamento.importanciaRecebida);*/

            table.Rows.Add("Faturas", totaisTipoFatura.valorBase, totaisTipoFatura.valorIvaContinente,
                totaisTipoFatura.impostoSelo, totaisTipoFatura.irsSemRetencao,
                totaisTipoFatura.importanciaRecebida);

            table.Rows.Add("Total", total.valorBase, total.valorIvaContinente, total.impostoSelo,
                total.irsSemRetencao, total.importanciaRecebida);

            table.Rows.Add("Total anulados", totaisAnulados.valorBase, totaisAnulados.valorIvaContinente, totaisAnulados.impostoSelo,
                totaisAnulados.irsSemRetencao, totaisAnulados.importanciaRecebida);

            table.Rows.Add("Recibos", totaisTipoRecibo.valorBase, totaisTipoRecibo.valorIvaContinente,
                totaisTipoRecibo.impostoSelo, totaisTipoRecibo.irsSemRetencao,
                totaisTipoRecibo.importanciaRecebida);

            //Gera o texto
            var cabecalho = String.Format("ANO:{0}\nMES:{1}\nNIF: {2}\n\n", Ano, Mes, empresaAutenticada.NIF);
            var text = cabecalho + ConsoleTableBuilder.From(table).Export().ToString();


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
}