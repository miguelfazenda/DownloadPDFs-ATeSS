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

        public static List<Declaracao> declaracoes;
        public static void Load()
        {
            declaracoes = new List<Declaracao>();

            //Mensais
            declaracoes.Add(new Declaracao("DMR - Comprovativo", true, Downloader.DownloadDMRComprovativo, Autenticacao.AT));
            declaracoes.Add(new Declaracao("DMR - Doc. Pagamento", true, Downloader.DownloadDMRDocPag, Autenticacao.AT));
            declaracoes.Add(new Declaracao("Retenções", true, Downloader.DownloadRetencoes, Autenticacao.AT));
            declaracoes.Add(new Declaracao("Extrato de remunerações", true, Downloader.DownloadExtratoRemuneracoes, Autenticacao.SSDireta));

            //Anuais
            //vvv Não é bem anual nem mensal vvv
            declaracoes.Add(new Declaracao("Fundos de comp. - Doc. Pagamento", false, Downloader.DownloadFundoCompDocPag, Autenticacao.SSFundosCompensacao));
        }

        public string Nome { get; set; }
        public bool Mensal { get; set; } //true mensal, false anual

        public Action<int, int> DownloadFunction { get; set; }
        public Autenticacao AutenticacaoNecessaria { get; set; }

        public Declaracao(string nome, bool mensal, Action<int, int> downloadFunction, Autenticacao autenticacaoNecessaria)
        {
            this.Nome = nome;
            this.Mensal = mensal;
            this.DownloadFunction = downloadFunction;
            this.AutenticacaoNecessaria = autenticacaoNecessaria;
        }

        public override string ToString()
        {
            return Nome;
        }
    }
}
