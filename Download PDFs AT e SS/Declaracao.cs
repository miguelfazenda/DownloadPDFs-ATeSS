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

        public static List<Declaracao> declaracoes = new List<Declaracao>();

        //Mensais
        public static Declaracao AT_DMRComprov = new Declaracao("DMR - Comprovativo", true, Downloader.DownloadDMRComprovativo, Autenticacao.AT);
        public static Declaracao AT_DMRDocPag = new Declaracao("DMR - Doc. Pagamento", true, Downloader.DownloadDMRDocPag, Autenticacao.AT);
        public static Declaracao AT_Retencoes = new Declaracao("Retenções", true, Downloader.DownloadRetencoes, Autenticacao.AT);
        public static Declaracao SS_ExtratoRemun = new Declaracao("Extrato de remunerações", true, Downloader.DownloadExtratoRemuneracoes, Autenticacao.SSDireta);
        public static Declaracao SS_FundosComp_DocPag = new Declaracao("Fundos de comp. - Doc. Pagamento", true, Downloader.DownloadFundoCompDocPag, Autenticacao.SSFundosCompensacao);

        //Anuais
        public static Declaracao AT_Modelo22 = new Declaracao("Modelo 22", false, Downloader.DownloadModelo22, Autenticacao.AT);
        public static Declaracao AT_IES = new Declaracao("IES", false, Downloader.DownloadIES, Autenticacao.AT);
        public static Declaracao AT_IVA = new Declaracao("IVA", false, Downloader.DownloadIVA, Autenticacao.AT);


        public string Nome { get; set; }
        public bool Mensal { get; set; } //true mensal, false anual

        public Action<int> DownloadFunctionAnual { get; set; } //A funcao de download (atribuido quando é uma declaracao anual)
        public Action<int, int> DownloadFunctionMensal { get; set; } //A funcao de download (atribuido quando é uma declaracao mensal)
        public Autenticacao AutenticacaoNecessaria { get; set; }

        public Declaracao(string nome, bool mensal, Action<int, int> downloadFunction, Autenticacao autenticacaoNecessaria)
        {
            this.Nome = nome;
            this.Mensal = mensal;
            this.DownloadFunctionMensal = downloadFunction;
            this.AutenticacaoNecessaria = autenticacaoNecessaria;

            declaracoes.Add(this);
        }

        public Declaracao(string nome, bool mensal, Action<int> downloadFunction, Autenticacao autenticacaoNecessaria)
        {
            this.Nome = nome;
            this.Mensal = mensal;
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
