using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Download_PDFs_AT_e_SS
{
    class Declaracao
    {
        public enum Autenticacao
        {
            None = 0,
            AT = 1, //Autentica no site da Autoridade tributaria
            SSFundosCompensacao = 2, //Autentica no site dos fundos de compensacao
            SSDireta = 3, //Autentica no site da Autoridade tributaria
        }

        public enum TipoDeclaracao
        {
            Mensal = 0,
            Anual = 1,
            Lista = 2,
            Pedido = 3
        }

        public static List<Declaracao> declaracoes = new List<Declaracao>();

        //Mensais
        public static Declaracao AT_DMRComprov = new Declaracao("DMR - Comprovativo", TipoDeclaracao.Mensal, Downloader.DownloadDMRComprovativo, Autenticacao.AT);
        public static Declaracao AT_DMRDocPag = new Declaracao("DMR - Doc. Pagamento", TipoDeclaracao.Mensal, Downloader.DownloadDMRDocPag, Autenticacao.AT);
        public static Declaracao AT_Retencoes = new Declaracao("Retenções", TipoDeclaracao.Mensal, Downloader.DownloadRetencoes, Autenticacao.AT);
        public static Declaracao SS_ExtratoRemun = new Declaracao("Extrato de remunerações", TipoDeclaracao.Mensal, Downloader.DownloadExtratoRemuneracoes, Autenticacao.SSDireta);
        public static Declaracao SS_FundosComp_DocPag = new Declaracao("Fundos de comp. - Doc. Pagamento", TipoDeclaracao.Mensal, Downloader.DownloadFundoCompDocPag, Autenticacao.SSFundosCompensacao);

        public static Declaracao AT_LISTA_RECIBOS_VERDES = new Declaracao("R. Verdes Emitidos - PDFs", TipoDeclaracao.Mensal, Downloader.DownloadRecibosVerdesEmitidos, Autenticacao.AT);
        public static Declaracao AT_LISTA_RECIBOS_VERDES_PARA_WINTOUCH_PRESTADOS = new Declaracao("R. Verdes Emitidos - Wintouch", TipoDeclaracao.Mensal, Downloader.DownloadRecibosVerdesEmitidosWintouchPrestados, Autenticacao.AT);
        public static Declaracao AT_LISTA_RECIBOS_VERDES_PARA_WINTOUCH_ADQUIRIDOS = new Declaracao("R. Verdes Adquiridos - Wintouch", TipoDeclaracao.Mensal, Downloader.DownloadRecibosVerdesEmitidosWintouchAdquiridos, Autenticacao.AT);

        //Anuais
        public static Declaracao AT_IRS = new Declaracao("IRS - Modelo 3", TipoDeclaracao.Anual, Downloader.DownloadIRS, Autenticacao.AT);
        public static Declaracao AT_Modelo22 = new Declaracao("Modelo 22", TipoDeclaracao.Anual, Downloader.DownloadModelo22, Autenticacao.AT);
        public static Declaracao AT_IES = new Declaracao("IES", TipoDeclaracao.Anual, Downloader.DownloadIES, Autenticacao.AT);
        public static Declaracao AT_IVA = new Declaracao("IVA", TipoDeclaracao.Anual, Downloader.DownloadIVA, Autenticacao.AT);

        public static Declaracao AT_LISTA_RECIBOS_VERDES_PARA_EXCEL_PRESTADOS = new Declaracao("R. Verdes Emitidos - Excel", TipoDeclaracao.Anual, Downloader.DownloadRecibosVerdesEmitidosExcelAnual, Autenticacao.AT);

        //Pedidos
        public static Declaracao AT_CerticaoDivida = new Declaracao("Certidão AT", TipoDeclaracao.Pedido, Downloader.DownloadCerticaoDivida, Autenticacao.AT);
        public static Declaracao SS_Pedir_Certidao = new Declaracao("SS Pedir certidão", TipoDeclaracao.Pedido, Downloader.SSPedirCertidao, Autenticacao.SSDireta);
        public static Declaracao SS_Transferir_Ultima_Certidao = new Declaracao("SS Tranferir última certidão", TipoDeclaracao.Pedido, Downloader.SSDownloadUltimaCertidao, Autenticacao.SSDireta);
        
        public string Nome { get; set; }
        public TipoDeclaracao Tipo { get; set; } //Se é mensal, anual, pedido, etc.

        public Action<int> DownloadFunctionAnual { get; set; } //A funcao de download (atribuido quando é uma declaracao anual)
        public Action<int, int> DownloadFunctionMensal { get; set; } //A funcao de download (atribuido quando é uma declaracao mensal)
        public Autenticacao AutenticacaoNecessaria { get; set; }

        public Declaracao(string nome, TipoDeclaracao tipoDeclaracao, Action<int, int> downloadFunction, Autenticacao autenticacaoNecessaria)
        {
            this.Nome = nome;
            this.Tipo = tipoDeclaracao;
            this.DownloadFunctionMensal = downloadFunction;
            this.AutenticacaoNecessaria = autenticacaoNecessaria;

            declaracoes.Add(this);
        }

        public Declaracao(string nome, TipoDeclaracao tipoDeclaracao, Action<int> downloadFunction, Autenticacao autenticacaoNecessaria)
        {
            this.Nome = nome;
            this.Tipo = tipoDeclaracao;
            this.DownloadFunctionAnual = downloadFunction;
            this.AutenticacaoNecessaria = autenticacaoNecessaria;

            declaracoes.Add(this);
        }

        public override string ToString()
        {
            return Nome;
        }
    }
}
