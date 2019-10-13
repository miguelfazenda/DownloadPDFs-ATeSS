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
        [JsonProperty("estruturaNomesFicheiros")]
        public static EstruturaNomesFicheiros estruturaNomesFicheiros { get; set; }

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
        
        public EstruturaNomesFicheiros()
        {
        }

        //O int a é para diferenciar do construtor JSON
        public void DefineValoresDefaultSeNaoDefinidos()
        {
            //Valores default
            AT_DMRComprov = AT_DMRComprov != null ? AT_DMRComprov : "{ano}.{mes}.{empresa.Codigo}.DMRComprov.{empresa.NIF}.pdf";
            AT_DMRDocPag = AT_DMRDocPag != null ? AT_DMRDocPag : "{ano}.{mes}.{empresa.Codigo}.DMRDocPag.{empresa.NIF}.pdf";
            AT_Retencoes = AT_Retencoes != null ? AT_Retencoes : "{ano}.{mes}.{empresa.Codigo}.Retencoes.{empresa.NIF}.pdf";
            SS_ExtratoRemun = SS_ExtratoRemun != null ? SS_ExtratoRemun : "{ano}.{mes}.{empresa.Codigo}.ExtratoRemun.{empresa.NIF}.pdf";
            SS_FundosComp_DocPag = SS_FundosComp_DocPag != null ? SS_FundosComp_DocPag : "{ano}.{mes}.{empresa.Codigo}.FundosComp_DocPag.{empresa.NIF}.pdf";
            AT_IRS = AT_IRS != null ? AT_IRS : "{ano}.{mes}.{empresa.Codigo}.IRS.{empresa.NIF}.pdf";
            AT_Modelo22 = AT_Modelo22 != null ? AT_Modelo22 : "{ano}.{mes}.{empresa.Codigo}.Modelo22.{empresa.NIF}.pdf";
            AT_IES = AT_IES != null ? AT_IES : "{ano}.{mes}.{empresa.Codigo}.IES.{empresa.NIF}.pdf";
            AT_IVA = AT_IVA != null ? AT_IVA : "{ano}.{mes}.{empresa.Codigo}.IVA.{empresa.NIF}.pdf";
        }
    }
}
