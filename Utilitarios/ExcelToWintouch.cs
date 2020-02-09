using CommandLine;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using SmartFormat;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilitarios
{
    public class ExcelToWintouch
    {
        //Parametros para correr pela linha de comandos
        [Verb("excelparawintouch", HelpText = "Converte as faturas numa folha de excel para um ficheiro a importar pelo Wintouch")]
        public class ExcelToWintouchOptions
        {
            [Option('i', "input", Required = true, HelpText = "Ficheiro de excel")]
            public string FicheiroExcel { get; set; }
            [Option('f', "folha", Required = true, HelpText = "Folha do ficheiro excel")]
            public string FolhaExcel { get; set; }
            [Option('o', "output", Required = true, HelpText = "Ficheiro de saída")]
            public string FicheiroWintouch { get; set; }

            [Option("formato-descricao", Required = false, HelpText = "Formato descricao", Default = "{fatura.NumDoc} {fatura.NIF}")]
            public string FormatoDescricao { get; set; }
        }

        public static DataFormatter df;

        private static ExcelToWintouchOptions options;
        public static int Run(ExcelToWintouchOptions options)
        {
            df = new DataFormatter();
            ExcelToWintouch.options = options;

            IWorkbook workbook;
            using (FileStream file = new FileStream(options.FicheiroExcel, FileMode.Open, FileAccess.Read))
            {
                using (StreamWriter outputStream = new StreamWriter(options.FicheiroWintouch))
                {
                    outputStream.WriteLine("WCONTAB5.60");

                    workbook = new XSSFWorkbook(file);
                    ISheet sheet = workbook.GetSheet(options.FolhaExcel);

                    int colStartValores = 5;
                    int colEndValores = 13;
                    int numValores = colEndValores - colStartValores;

                    Classificacoes classificacoes = ReadClassificacoes(sheet, numValores);

                    int colData = 0;
                    int colTipoDoc = 1;
                    int colNumDoc = 2;
                    int colNIF = 3;
                    int colNome = 4;

                    for (int rowIdx = 1; rowIdx <= sheet.LastRowNum; rowIdx++)
                    {
                        IRow row = sheet.GetRow(rowIdx);
                        if (row != null) //Se a linha tiver dados continua
                        {
                            string nifValStr = df.FormatCellValue(row.GetCell(colNIF));
                            if (nifValStr != null && nifValStr.Length > 0)
                            {
                                decimal[] valores = new decimal[colEndValores - colStartValores + 1];

                                for (int col = colStartValores; col <= colEndValores; col++)
                                {
                                    valores[col - colStartValores] = (decimal)row.GetCell(col).NumericCellValue;
                                }

                                Fatura fatura = new Fatura(row.GetCell(colData).DateCellValue,
                                    df.FormatCellValue(row.GetCell(colTipoDoc)),
                                    df.FormatCellValue(row.GetCell(colNumDoc)),
                                    df.FormatCellValue(row.GetCell(colNIF)),
                                    df.FormatCellValue(row.GetCell(colNome)),
                                    valores);

                                ExportFatura(outputStream, fatura, classificacoes);
                            }
                        }
                    }
                }
            }
            return 0;
        }

        private static void ExportFatura(StreamWriter fileStream, Fatura fatura, Classificacoes classificacoes)
        {
            int numLinha = 0;
            for(int i = 0; i<fatura.valores.Length; i++)
            {
                //Escreve uma linha por cada valor da fatura (se o valor não for 0)
                decimal valor = fatura.valores[i];
                if(valor != 0)
                {
                    if (!classificacoes.contasPorValor[i].contas.ContainsKey(fatura.TipoDoc))
                        throw new Exception(String.Format("Tipo de documento \"{0}\" desconhecido (NumDoc {1})", fatura.TipoDoc, fatura.NumDoc));

                    ContasParaValor contasParaValor = classificacoes.contasPorValor[i];

                    ExportLinhaFatura(fileStream, fatura, valor, contasParaValor, classificacoes, numLinha);
                    numLinha++;
                }
            }
        }
        private static void ExportLinhaFatura(StreamWriter fileStream, Fatura fatura, decimal valor, ContasParaValor contasParaValor, Classificacoes classificacoes, int numLinha)
        {
            string natureza = contasParaValor.natureza;
            string conta = contasParaValor.contas[fatura.TipoDoc];
            Tuple<string, string> diarioECodigoDoc = classificacoes.diarioECodigoDocPorTipoDoc[fatura.TipoDoc];
            string codigoDiario = diarioECodigoDoc.Item1;
            string codigoDocumento = diarioECodigoDoc.Item2;
            int serie = 1;

            string descricao = GetFaturaDescricao(fatura);

            int dia = fatura.Data.Day;
            int mes = fatura.Data.Month;
            string contibuinte = fatura.NIF;

            string nomeEntidade = fatura.Nome;
            nomeEntidade = nomeEntidade.Length > 50 ? nomeEntidade.Substring(0, 50) : nomeEntidade;

            string valorStr = String.Format("{0,18:F2}", valor).Replace(",", ".");
            //                            diario            serie             descric       nat  dia     mes      contrib       numLinha     nomeenti anulado
            string linha = String.Format("{0,10}{1,20}{2,20}{3,4}{4,10}{5,-20}{6,-50}{7,18}{8,1}{9,2:D2}{10,2:D2}{11,-20}F{12,20}{13,1}{14,5}{15,20}{16,-50}{17,1}",
                -1, codigoDiario, codigoDocumento, //2
                serie, fatura.NumDoc, conta, //5
                descricao, valorStr, natureza, dia, mes, //10
                contibuinte, "", "C", numLinha, "", //14
                nomeEntidade, 0);

            fileStream.WriteLine(linha);
        }

        private static string GetFaturaDescricao(Fatura fatura)
        {
            string descricao = Smart.Format(options.FormatoDescricao, new { fatura });
            if (descricao.Length > 50)
                descricao = descricao.Substring(0, 50);
            return descricao;
        }

        private static Classificacoes ReadClassificacoes(ISheet sheet, int numValores)
        {
            int startCol = 15;

            //Ler Cabecalho dos tipos doc
            IRow rowCabecalho = sheet.GetRow(1);
            string[] cabecalhoTiposDoc = new string[40]; //Diz o tipo de documento por coluna (indexado pela coluna)

            for (int col = startCol; col <= startCol + 20; col++)
            {
                var cell = rowCabecalho.GetCell(col);
                if (cell == null)
                {
                    cabecalhoTiposDoc[col] = null;
                    break;
                }
                cabecalhoTiposDoc[col] = cell.StringCellValue;
            }

            Classificacoes classificacoes = new Classificacoes();
            //Lê as contas
            ContasParaValor contasBase0 = ReadContasParaValor(sheet.GetRow(2), startCol, cabecalhoTiposDoc, "Base0");
            ContasParaValor contasBase1 = ReadContasParaValor(sheet.GetRow(3), startCol, cabecalhoTiposDoc, "Base1");
            ContasParaValor contasBase2 = ReadContasParaValor(sheet.GetRow(4), startCol, cabecalhoTiposDoc, "Base2");
            ContasParaValor contasBase3 = ReadContasParaValor(sheet.GetRow(5), startCol, cabecalhoTiposDoc, "Base3");
            ContasParaValor contasIva1 = ReadContasParaValor(sheet.GetRow(6), startCol, cabecalhoTiposDoc, "Iva1");
            ContasParaValor contasIva2 = ReadContasParaValor(sheet.GetRow(7), startCol, cabecalhoTiposDoc, "Iva2");
            ContasParaValor contasIva3 = ReadContasParaValor(sheet.GetRow(8), startCol, cabecalhoTiposDoc, "Iva3");
            ContasParaValor contasRetIva = ReadContasParaValor(sheet.GetRow(9), startCol, cabecalhoTiposDoc, "RetIva");
            ContasParaValor contasTotal = ReadContasParaValor(sheet.GetRow(10), startCol, cabecalhoTiposDoc, "Total");

            classificacoes.contasPorValor = new ContasParaValor[] {
                contasBase0,
                contasBase1,
                contasBase2,
                contasBase3,
                contasIva1,
                contasIva2,
                contasIva3,
                contasRetIva,
                contasTotal
                };

            //Lê os diarios codigos de documento para cada tipo de documento
            for (int rowIdx = 12; rowIdx <= sheet.LastRowNum; rowIdx++)
            {
                IRow row = sheet.GetRow(rowIdx);
                if (row != null) //Se a linha tiver dados continua
                {
                    string tipoDoc = row.GetCell(14).StringCellValue;
                    if (tipoDoc == null || tipoDoc.Trim().Length == 0)
                        break;

                    //Obtem o codigo de documento e dirio para este tipoDoc
                    string codigoDoc = df.FormatCellValue(row.GetCell(15));
                    string diario = df.FormatCellValue(row.GetCell(16));
                    classificacoes.diarioECodigoDocPorTipoDoc.Add(tipoDoc, new Tuple<string, string>(diario, codigoDoc));
                }
            }

            return classificacoes;
        }

        private static ContasParaValor ReadContasParaValor(IRow row, int startCol, string[] cabecalhoTiposDoc, string valor)
        {
            ContasParaValor contas = new ContasParaValor(valor);

            for (int col = startCol; col <= startCol+20; col++)
            {
                if(cabecalhoTiposDoc[col] != null)
                {
                    string tipoDoc = cabecalhoTiposDoc[col];
                    string conta = df.FormatCellValue(row.GetCell(col));

                    contas.contas.Add(tipoDoc, conta);
                }
                else
                {
                    //Tem a natureza (D ou C)
                    contas.natureza = df.FormatCellValue(row.GetCell(col));
                    break;
                }
            }
            return contas;
        }
    }

    class Classificacoes
    {
        public ContasParaValor[] contasPorValor;
        public Dictionary<string, Tuple<string, string>> diarioECodigoDocPorTipoDoc; //Contem o diario e o codigo do documento por tipo de documento

        public Classificacoes()
        {
            diarioECodigoDocPorTipoDoc = new Dictionary<string, Tuple<string, string>>();
        }
    }

    /// <summary>
    /// Contem as contas que devem ser utilizadas para um certo valor
    /// Ex.: Para o base0, deve ser usada a conta X para Faturas-Recibo, a Y para Notas de crédito...
    /// </summary>
    struct ContasParaValor
    {
        public string valor; //Base0 ou Iva1, etc.
        public Dictionary<string, string> contas; //Contas por tipo documento (Fatura-Recibo, etc.)
        public string natureza; //D(débito) ou C(crédito)

        public ContasParaValor(string valor) : this()
        {
            this.valor = valor;
            contas = new Dictionary<string, string>();
        }
    }
}
