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
    [Serializable]
    public class FicheiroEmpresas
    {
        public List<Empresa> empresas;

        public FicheiroEmpresas()
        {
            empresas = new List<Empresa>();
        }

        public FicheiroEmpresas(List<Empresa> emp)
        {
            empresas = new List<Empresa>(emp);
        }
    }
    class Dados
    {
        public static FicheiroEmpresas ficheiroEmpresas;
        public static List<Empresa> empresas;

        public static readonly string FICHEIRO_EMPRESAS = "empresas.json";

        public static void Load()
        {
            if(File.Exists(FICHEIRO_EMPRESAS))
            {
                //empresas = JsonConvert.DeserializeObject<List<Empresa>>(File.ReadAllText(FICHEIRO_EMPRESAS));
                ficheiroEmpresas = new FicheiroEmpresas(JsonConvert.DeserializeObject<List<Empresa>>(File.ReadAllText(FICHEIRO_EMPRESAS)));
                empresas = ficheiroEmpresas.empresas;
            }
            else
            {
                ficheiroEmpresas = new FicheiroEmpresas();
                empresas = ficheiroEmpresas.empresas;
                SaveEmpresas();
            }

            /*if(Licenciamento.Licenciamento.Demo && empresas.Count > 1)
            {
                //DEMO mas alguém tem uma lista de empresas com mais que 1.
                
                //Obtem apenas um elemeto da lista de empresas mas faz sort da lista para mostrar só o com nif mais baixo
                empresas.Sort((x, y) => { return x.NIF.CompareTo(y.NIF); });
                var primeiraEmpresa = empresas[0];

                empresas = new List<Empresa>();
                empresas.Add(primeiraEmpresa);
            }*/
        }

        public static void DesencriptarPasswords(string masterPassword)
        {
            //Desencripta as passwords para o programa porder usar
            foreach (Empresa empresa in empresas)
            {
                //empresa.EncriptarPasswords(masterPassword);
                //empresa.DesencriptarPasswords(masterPassword);
                //if (empresa.PasswordAT == null || empresa.PasswordAT == String.Empty)
                //{
                    //empresa.DesencriptarPasswordAT();
                //}
                //if (empresa.PasswordSS == null || empresa.PasswordSS == String.Empty)
                //{
                    //empresa.DesencriptarPasswordSS();
                //}
            }
        }

        public static void SaveEmpresas()
        {
            //File.WriteAllText(FICHEIRO_EMPRESAS, JsonConvert.SerializeObject(empresas, Formatting.Indented));
            File.WriteAllText(FICHEIRO_EMPRESAS, JsonConvert.SerializeObject(ficheiroEmpresas.empresas, Formatting.Indented));
        }

        internal static void RemoveEmpresa(Empresa empresaRightClicked)
        {
            empresas.Remove(empresaRightClicked);
            SaveEmpresas();
        }
    }
}
