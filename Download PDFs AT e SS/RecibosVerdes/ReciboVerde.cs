using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Download_PDFs_AT_e_SS.RecibosVerdes
{
    [Serializable()]
    internal class ReciboVerde
    {
        public string tipoDoc, numDoc;
        public string nifPrestadorServicos, nifAdquirente, descricao, nomeAdquirente, nomePrestador;
        public string paisAdquirente;
        public DateTime dataEmissao, dataPrestacaoServico;
        public string detailsUrl;
        public bool anulado;
        public RecibosVerdesValores valores;
        public TipoReciboVerde tipoReciboVerde;

        public TipoReciboVerdePrestOUAdquir tipo; //Prestador ou Adquirente (PRESTADOR ou ADQUIRENTE)
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
    internal enum TipoReciboVerde
    {
        Pagamento,
        Adiantamento,
        AdiantamentoPagamento,
        Fatura,
        Recibo
    }

    internal enum TipoReciboVerdePrestOUAdquir
    {
        Prestador = 0,
        Adquirente = 1
    }

}
