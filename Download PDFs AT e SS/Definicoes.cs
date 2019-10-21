using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Download_PDFs_AT_e_SS
{
    class Definicoes
    {
        public static string FICHEIRO_DEFINICOES = "definicoes.json";

        /*[JsonProperty("totals")]
        public static string totals { get; set; }*/

        //
        //
        //IMPORTANTE 
        //TODO NOTA: isto não está a ler do ficheiro, vai sempre usar o default por agora, falta implementar
        //
        //
        //[JsonProperty("estruturaNomesFicheiros")]
        public static EstruturaNomesFicheiros nomesFicheiros;
        public static EstruturaNomesFicheiros nomesFicheirosDefault = new EstruturaNomesFicheiros(true);

        public static EstruturaNomesFicheiros estruturaNomesFicheiros
        {
            get
            {
                if (nomesFicheiros != null)
                    return nomesFicheiros;
                else
                    return nomesFicheirosDefault;
            }
            set
            {
                nomesFicheiros = value;
            }
        }

        public static void Load()
        {
            if (File.Exists(FICHEIRO_DEFINICOES))
            {
                JsonConvert.DeserializeObject<Definicoes>(File.ReadAllText(FICHEIRO_DEFINICOES));
            }
            else
            {
                estruturaNomesFicheiros = new EstruturaNomesFicheiros();
            }
            estruturaNomesFicheiros.DefineValoresDefaultSeNaoDefinidos();
            File.WriteAllText(FICHEIRO_DEFINICOES, JsonConvert.SerializeObject(new Definicoes(), Formatting.Indented));
        }
    }

    class EstruturaNomesFicheiros
    {
        public string AT_DMRComprov { get; set; }
        public string AT_DMRDocPag { get; set; }
        public string AT_Retencoes { get; set; }
        public string SS_ExtratoRemun { get; set; }
        public string SS_FundosComp_DocPag { get; set; }
        public string AT_IRS { get; set; }
        public string AT_Modelo22 { get; set; }
        public string AT_IES { get; set; }
        public string AT_IVA { get; set; }
        public string AT_CerticaoDivida { get; set; }

        public EstruturaNomesFicheiros()
        {
        }

        public EstruturaNomesFicheiros(bool comValoresDefault)
        {
            if(comValoresDefault)
                DefineValoresDefaultSeNaoDefinidos();
        }

        //O int a é para diferenciar do construtor JSON
        public void DefineValoresDefaultSeNaoDefinidos()
        {
            //Valores default
            AT_DMRComprov = AT_DMRComprov != null ? AT_DMRComprov : "{ano}.{mes}.{empresa.Codigo}.DMRComprov.{empresa.NIF}.pdf";
            AT_DMRDocPag = AT_DMRDocPag != null ? AT_DMRDocPag : "{ano}.{mes}.{empresa.Codigo}.DMRDocPag.{empresa.NIF}.pdf";
            AT_Retencoes = AT_Retencoes != null ? AT_Retencoes : "{ano}.{mes}.{empresa.Codigo}.Retencoes.{empresa.NIF}.pdf";
            SS_ExtratoRemun = SS_ExtratoRemun != null ? SS_ExtratoRemun : "{ano}.{mes}.{empresa.Codigo}.ExtratoRemun.{empresa.NIF}.pdf";
            SS_FundosComp_DocPag = SS_FundosComp_DocPag != null ? SS_FundosComp_DocPag : "{dataMesAnteior.ano}.{dataMesAnteior.mes}.{empresa.Codigo}.FundosComp_DocPag.{empresa.NIF}.pdf";

            AT_IRS = AT_IRS != null ? AT_IRS : "{ano}.{empresa.Codigo}.IRS.{empresa.NIF}.pdf";
            AT_Modelo22 = AT_Modelo22 != null ? AT_Modelo22 : "{ano}.{empresa.Codigo}.Modelo22.{empresa.NIF}.pdf";
            AT_IES = AT_IES != null ? AT_IES : "{ano}.{empresa.Codigo}.IES.{empresa.NIF}.pdf";
            AT_IVA = AT_IVA != null ? AT_IVA : "{ano}.{empresa.Codigo}.IVA {parametros.docAno} {parametros.docPeriodo} {parametros.docIdentif}.{empresa.NIF}.pdf";

            AT_CerticaoDivida = AT_CerticaoDivida != null ? AT_CerticaoDivida : "{empresa.Codigo}.CertidaoAT.{empresa.NIF}.{dataHoje.ano}.{dataHoje.mes}.{dataHoje.dia}.pdf";
        }
    }
}
