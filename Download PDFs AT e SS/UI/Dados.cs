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

            //Verifica se alguma empresa ainda nao tem as passwords encriptadas
            //Se sim encripta a password, e apaga a plain-text
            bool temDeGravarAlteracoes = EncriptaPasswordsPlainText();
            
            if (temDeGravarAlteracoes)
                File.WriteAllText(FICHEIRO_EMPRESAS, JsonConvert.SerializeObject(empresas, Formatting.Indented));

            //Desencripta as passwords para o programa porder usar
            foreach (Empresa empresa in empresas)
            {
                if (empresa.PasswordATDesencriptada != String.Empty)
                {
                    empresa.DesencriptarPasswordAT();
                }
                if (empresa.PasswordSSDesencriptada != String.Empty)
                {
                    empresa.DesencriptarPasswordSS();
                }
            }
        }

        /// <summary>
        /// Verifica se alguma empresa tem uma password plain-text
        ///   Se sim encripta a password, e apaga a plain-text
        /// </summary>
        /// <returns>Devolve se houve alguma alteracao</returns>
        public static bool EncriptaPasswordsPlainText()
        {
            //Verifica se alguma empresa ainda nao tem as passwords encriptadas
            //Se sim encripta a password, e apaga a plain-text
            bool temDeGravarAlteracoes = false;
            foreach (Empresa empresa in empresas)
            {
                if (empresa.PasswordAT != String.Empty)
                {
                    empresa.PasswordATDesencriptada = empresa.PasswordAT;
                    empresa.EncriptarPasswordAT(empresa.PasswordAT);
                    temDeGravarAlteracoes = true;
                }
                if (empresa.PasswordSS != String.Empty)
                {
                    empresa.PasswordATDesencriptada = empresa.PasswordAT;
                    empresa.EncriptarPasswordSS(empresa.PasswordSS);
                    temDeGravarAlteracoes = true;
                }
            }
            return temDeGravarAlteracoes;
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
