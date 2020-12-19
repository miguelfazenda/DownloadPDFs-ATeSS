using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Download_PDFs_AT_e_SS
{
    class Dados
    {
        public static List<Empresa> empresas;

        public static readonly string FICHEIRO_EMPRESAS = "empresas.json";

        public static void Load()
        {
            if(File.Exists(FICHEIRO_EMPRESAS))
            {
                empresas = JsonConvert.DeserializeObject<List<Empresa>>(File.ReadAllText(FICHEIRO_EMPRESAS));
            }
            else
            {
                empresas = new List<Empresa>();
                SaveEmpresas();
            }

            //Desencripta as passwords para o programa porder usar
            foreach (Empresa empresa in empresas)
            {
                if (empresa.PasswordAT == null || empresa.PasswordAT == String.Empty)
                {
                    empresa.DesencriptarPasswordAT();
                }
                if (empresa.PasswordSS == null || empresa.PasswordSS == String.Empty)
                {
                    empresa.DesencriptarPasswordSS();
                }
            }
        }

        public static void SaveEmpresas()
        {
            File.WriteAllText(FICHEIRO_EMPRESAS, JsonConvert.SerializeObject(empresas, Formatting.Indented));
        }

        internal static void RemoveEmpresa(Empresa empresaRightClicked)
        {
            empresas.Remove(empresaRightClicked);
            SaveEmpresas();
        }
    }
}
