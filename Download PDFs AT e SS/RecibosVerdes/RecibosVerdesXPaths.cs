using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Download_PDFs_AT_e_SS.RecibosVerdes
{
    internal class RecibosVerdesXPaths
    {
        public string dataTransmissao;

        public string dataEmissao;

        public string nifTransmitente;

        public string nifAdquirente;

        public string descricao;

        public string nomeAdquirente;

        public string nomeTrasmitente;

        public string paisAdquirente;

        public string valorBaseStr;

        public string valorIvaContinenteStr;

        public string impostoSeloStr;

        public string irsSemRetencaoStr;

        public string importanciaRecebidaStr;

        public string checkboxTipoPagamento;

        public string checkboxTipoAdiantamento;

        public string checkboxTipoAdiantamentoPagam;

        public static readonly RecibosVerdesXPaths RECIBOS_VERDES_XPATHS_FATURA_RECIBO = new RecibosVerdesXPaths
        {
            dataTransmissao = "/html/body/div/main/div/div[2]/div/section/div[last()]/div[2]/div/div[2]/dl/dd",
            nifTransmitente = "/html/body/div/main/div/div[2]/div/section/div[3]/div[2]/div/div[1]/dl/dd",
            dataEmissao = "/html/body/div/main/div/div[2]/div/section/div[2]/div/div/div[2]/div/legend/small",
            nifAdquirente = "/html/body/div/main/div/div[2]/div/section/div[last()-1]/div[2]/div[1]/div[1]/dl/dd",
            descricao = "/html/body/div/main/div/div[2]/div/section/div[last()]/div[2]/div/div[3]/dl/dd",
            nomeAdquirente = "/html/body/div/main/div/div[2]/div/section/div[last()-1]/div[2]/div[1]/div[2]/dl/dd",
            nomeTrasmitente = "/html/body/div/main/div/div[2]/div/section/div[3]/div[2]/div/div[2]/dl/dd",
            paisAdquirente = "/html/body/div/main/div/div[2]/div/section/div[last()-1]/div[2]/div[2]/div/dl/dd",
            valorBaseStr = "/html/body/div/main/div/div[2]/div/section/div[last()]/div[2]/div/div[4]/dl/div[2]/dd",
            valorIvaContinenteStr = "/html/body/div/main/div/div[2]/div/section/div[last()]/div[2]/div/div[4]/dl/div[4]/dd",
            impostoSeloStr = "/html/body/div/main/div/div[2]/div/section/div[last()]/div[2]/div/div[4]/dl/div[6]/dd",
            irsSemRetencaoStr = "/html/body/div/main/div/div[2]/div/section/div[last()]/div[2]/div/div[4]/dl/div[8]/dd",
            importanciaRecebidaStr = "/html/body/div/main/div/div[2]/div/section/div[last()]/div[2]/div/div[4]/dl/div[10]/dd",
            checkboxTipoPagamento = "/html/body/div/main/div/div[2]/div/section/div[last()]/div[2]/div/div[1]/dl/dt[2]/div/div[1]/label/input",
            checkboxTipoAdiantamento = "/html/body/div/main/div/div[2]/div/section/div[last()]/div[2]/div/div[1]/dl/dt[2]/div/div[2]/label/input",
            checkboxTipoAdiantamentoPagam = "/html/body/div/main/div/div[2]/div/section/div[last()]/div[2]/div/div[1]/dl/dt[2]/div/div[3]/label/input"
        };

        public static readonly RecibosVerdesXPaths RECIBOS_VERDES_XPATHS_FATURA = new RecibosVerdesXPaths
        {
            dataTransmissao = "/html/body/div/main/div/div[2]/div/section/div[last()]/div[2]/div/div[1]/dl/dd",
            nifTransmitente = "/html/body/div/main/div/div[2]/div/section/div[last()-2]/div[2]/div/div[1]/dl/dd",
            dataEmissao = "/html/body/div/main/div/div[2]/div/section/div[2]/div/div/div[2]/div/legend/small",
            nifAdquirente = "/html/body/div/main/div/div[2]/div/section/div[last()-1]/div[2]/div[1]/div[1]/dl/dd",
            descricao = "/html/body/div/main/div/div[2]/div/section/div[last()]/div[2]/div/div[2]/dl/dd",
            nomeAdquirente = "/html/body/div/main/div/div[2]/div/section/div[last()-1]/div[2]/div[1]/div[2]/dl/dd",
            nomeTrasmitente = "/html/body/div/main/div/div[2]/div/section/div[last()-2]/div[2]/div/div[2]/dl/dd",
            paisAdquirente = "/html/body/div/main/div/div[2]/div/section/div[last()-1]/div[2]/div[2]/div[1]/dl/dd",
            valorBaseStr = "/html/body/div/main/div/div[2]/div/section/div[last()]/div[2]/div/div[3]/dl/div[2]/dd",
            valorIvaContinenteStr = "/html/body/div/main/div/div[2]/div/section/div[last()]/div[2]/div/div[3]/dl/div[4]/dd",
            impostoSeloStr = null,//"/html/body/div/main/div/div[2]/div/section/div[last()]/div[2]/div/div[3]/dl/div[last()]/dd",
            irsSemRetencaoStr = null,
            importanciaRecebidaStr = "/html/body/div/main/div/div[2]/div/section/div[last()]/div[2]/div/div[3]/dl/div[6]/dd",
            checkboxTipoPagamento = null,
            checkboxTipoAdiantamento = null,
            checkboxTipoAdiantamentoPagam = null
        };
        public static readonly RecibosVerdesXPaths RECIBOS_VERDES_XPATHS_RECIBO = new RecibosVerdesXPaths
        {
            dataTransmissao = null,
            nifTransmitente = "/html/body/div/main/div/div[2]/div/section/div[3]/div[2]/div/div[1]/dl/dd",
            dataEmissao = "/html/body/div/main/div/div[2]/div/section/div[last()]/div[2]/div/div[2]/dl/dd",
            nifAdquirente = "/html/body/div/main/div/div[2]/div/section/div[last()-1]/div[2]/div[1]/div[1]/dl/dd",
            descricao = "/html/body/div/main/div/div[2]/div/section/div[2]/div/div/div[2]/div/legend/small",
            nomeAdquirente = "/html/body/div/main/div/div[2]/div/section/div[last()-1]/div[2]/div[1]/div[2]/dl/dd",
            nomeTrasmitente = "/html/body/div/main/div/div[2]/div/section/div[3]/div[2]/div/div[2]/dl/dd",
            paisAdquirente = "/html/body/div/main/div/div[2]/div/section/div[last()-1]/div[2]/div[2]/div[1]/dl/dd",
            valorBaseStr = "/html/body/div/main/div/div[2]/div/section/div[last()]/div[2]/div/div[last()]/dl/div[2]/dd",
            valorIvaContinenteStr = "/html/body/div/main/div/div[2]/div/section/div[last()]/div[2]/div/div[last()]/dl/div[4]/dd",
            impostoSeloStr = "/html/body/div/main/div/div[2]/div/section/div[last()]/div[2]/div/div[last()]/dl/div[last()-4]/dd",
            irsSemRetencaoStr = "/html/body/div/main/div/div[2]/div/section/div[last()]/div[2]/div/div[last()]/dl/div[8]/dd",
            importanciaRecebidaStr = "/html/body/div/main/div/div[2]/div/section/div[last()]/div[2]/div/div[last()]/dl/div[last()]/dd",
            checkboxTipoPagamento = "/html/body/div/main/div/div[2]/div/section/div[last()]/div[2]/div/div[4]/dl/dt[2]/div/div[1]/label/input",
            checkboxTipoAdiantamento = "/html/body/div/main/div/div[2]/div/section/div[last()]/div[2]/div/div[4]/dl/dt[2]/div/div[2]/label/input",
            checkboxTipoAdiantamentoPagam = "/html/body/div/main/div/div[2]/div/section/div[last()]/div[2]/div/div[4]/dl/dt[2]/div/div[3]/label/input"
        };
    }
}
