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

                File.WriteAllText(FICHEIRO_DEFINICOES, JsonConvert.SerializeObject(new Definicoes(), Formatting.Indented));
            }
        }
    }

    class EstruturaNomesFicheiros
    {
        public string AT_DMRComprov { get; set; }
        public string AT_DMRDocPag { get; set; }
        public string AT_Retencoes { get; set; }
        public string SS_ExtratoRemun { get; set; }
        public string SS_FundosComp_DocPag { get; set; }
        public string AT_Modelo22 { get; set; }
        public string AT_IES { get; set; }
        public string AT_IVA { get; set; }

        public EstruturaNomesFicheiros()
        {
            //Valores default
            AT_DMRComprov = "{empresa.NIF}.{ano}.{mes}.DMRComprov.pdf";
            AT_DMRDocPag = "{empresa.NIF}.{ano}.{mes}.DMRDocPag.pdf";
            AT_Retencoes = "{empresa.NIF}.{ano}.{mes}.Retencoes.pdf";
            SS_ExtratoRemun = "{empresa.NIF}.{ano}.{mes}.ExtratoRemun.pdf";
            SS_FundosComp_DocPag = "{empresa.NIF}.{ano}.{mes}.FundosComp_DocPag.pdf";
            AT_Modelo22 = "{empresa.NIF}.{ano}.{mes}.Modelo22.pdf";
            AT_IES = "{empresa.NIF}.{ano}.{mes}.IES.pdf";
            AT_IVA = "{empresa.NIF}.{ano}.{mes}.IVA.pdf";
        }
    }
}
