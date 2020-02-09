using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilitarios
{
    class Fatura
    {
        public DateTime Data;
        public string TipoDoc;
        public string NumDoc;
        public string NIF;
        public string Nome;
        /*decimal Base0, Base1, Base2, Base3;
        decimal Iva0, Iva1, Iva2, Iva3;
        decimal RetIRS, Total;*/
        public decimal[] valores;

        public Fatura(DateTime data, string tipoDoc, string numDoc, string nIF, string nome, decimal[] valores)
        {
            Data = data;
            TipoDoc = tipoDoc;
            NumDoc = numDoc;
            Nome = nome;
            NIF = nIF;
            this.valores = valores;
        }

        /*public Fatura(DateTime data, string tipoDoc, string numDoc, string nIF, decimal base0, decimal base1, decimal base2, decimal base3, decimal iva0, decimal iva1, decimal iva2, decimal iva3, decimal retIrs, decimal total)
        {
            Data = data;
            TipoDoc = tipoDoc;
            NumDoc = numDoc;
            NIF = nIF;
            Base0 = base0;
            Base1 = base1;
            Base2 = base2;
            Base3 = base3;
            Iva0 = iva0;
            Iva1 = iva1;
            Iva2 = iva2;
            Iva3 = iva3;
            RetIRS = retIrs;
            Total = total;
        }*/
    }
}
