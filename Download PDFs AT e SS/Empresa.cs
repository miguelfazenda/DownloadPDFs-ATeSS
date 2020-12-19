using Newtonsoft.Json;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Download_PDFs_AT_e_SS
{
    public class Empresa : ICloneable
    {
        public string Nome { get; set; }
        public string Codigo { get; set; }

        public string NIF { get; set; }


        public string PasswordAT { get; set; }
        public string PasswordATEncriptada { get; set; }

        public string NISS { get; set; }
        public string PasswordSS { get; set; }
        public string PasswordSSEncriptada { get; set; }

        public string NomeDoResponsavel { get; set; }
        public string TelefoneDoResponsavel { get; set; }
        public string EmailDoResponsavel { get; set; }

        public string CodigoCertidaoPermanente { get; set; }

        public Empresa(string nome, string codigo, string nif)
        {
            this.Nome = nome;
            this.Codigo = codigo;
            this.NIF = nif;
        }

        public override string ToString()
        {
            return NIF + " " + Nome;
        }

        internal void DesencriptarPasswordAT()
        {
            if (PasswordATEncriptada != null && PasswordATEncriptada.Length != 0)
                PasswordAT = Encoding.UTF32.GetString(
                        ProtectedData.Unprotect(Util.HexStringToByteArray(PasswordATEncriptada), null, DataProtectionScope.LocalMachine));
        }

        internal void DesencriptarPasswordSS()
        {
            if (PasswordSSEncriptada != null && PasswordSSEncriptada.Length != 0)
                PasswordSS = Encoding.UTF32.GetString(
                        ProtectedData.Unprotect(Util.HexStringToByteArray(PasswordSSEncriptada), null, DataProtectionScope.LocalMachine));
        }
        public object Clone()
        {
            Empresa empresa = new Empresa(Nome, Codigo, NIF);
            empresa.PasswordAT = this.PasswordAT;
            empresa.PasswordATEncriptada = this.PasswordATEncriptada;

            empresa.NISS = this.NISS;
            empresa.PasswordSS = this.PasswordSS;
            empresa.PasswordSSEncriptada = this.PasswordSSEncriptada;

            empresa.NomeDoResponsavel = this.NomeDoResponsavel;
            empresa.TelefoneDoResponsavel = this.TelefoneDoResponsavel;
            empresa.EmailDoResponsavel = this.EmailDoResponsavel;
            

            empresa.CodigoCertidaoPermanente = this.CodigoCertidaoPermanente;

            return empresa;
        }

    }
}