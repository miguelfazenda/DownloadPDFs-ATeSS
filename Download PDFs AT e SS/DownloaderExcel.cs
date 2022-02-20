using Download_PDFs_AT_e_SS.RecibosVerdes;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Download_PDFs_AT_e_SS
{
    partial class Downloader
    {
        /**
        * Obtem os recibos verdes e coloca num excel
        */
        internal static void DownloadRecibosVerdesEmitidosExcelAnual(int ano)
        {
            List<string> detailsURLs = new List<string>();

            var tipo = TipoReciboVerdePrestOUAdquir.Prestador;

            //Vai à lista de recibos verdes, para obter o URL de detalhes de cada um
            RecibosVerdesEmitidosNavegarPorCadaRecibo(ano, -1, tipo, (string downloadURL, string numRecibo, string nomeCliente) =>
            {
                //Para cada recibo, regista o URL para obter os detalhes
                string detailsUrl = downloadURL.Replace("/imprimir/", "/detalhe/").Replace("/normal", "");
                detailsURLs.Add(detailsUrl);
            });

            List<ReciboVerde> recibosVerdes = new List<ReciboVerde>(detailsURLs.Count);

            //List<ReciboVerde> recibosVerdes = (List<ReciboVerde>)new BinaryFormatter().Deserialize(new FileStream(@"C:\users\miguel\desktop\a.txt", FileMode.Open, FileAccess.Read));//TEMP

            //Depois de obtidos os URLs, navegar até à pagina de detalhes de cada um
            foreach (string detailsUrl in detailsURLs)
            {
                //Obtem os dados do recibo verde, navegado até à página de detalhes
                ReciboVerde reciboVerde = ScraperRecibosVerdes.ObterDadosReciboVerde(detailsUrl, tipo, driver);
                recibosVerdes.Add(reciboVerde);
            }

            //Cria o excel
            XSSFWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("Recibos verdes");

            //Formato para celulas com data
            var newDataFormat = workbook.CreateDataFormat();
            var dateCellStyle = workbook.CreateCellStyle();
            dateCellStyle.BorderBottom = BorderStyle.None;
            dateCellStyle.BorderLeft = BorderStyle.None;
            dateCellStyle.BorderTop = BorderStyle.None;
            dateCellStyle.BorderRight = BorderStyle.None;
            dateCellStyle.DataFormat = newDataFormat.GetFormat("dd/MM/yyyy");


            var headerRow = sheet.CreateRow(0);

            string[] headers = new string[] { "Data", "Data de transmissao",
                "Estado", "Tipodoc.", "Nº", "NIF", "Nome", "País", "Recebia a título de ",
                "Descrição", "Base", "IVA", "Selo", "IRS", "Importancia recebida" };

            for (int i = 0; i < headers.Length; i++)
            {
                var dataCell = headerRow.CreateCell(i);
                dataCell.SetCellValue(headers[i]);
            }


            for (int i = 0; i < recibosVerdes.Count; i++)
            {
                ReciboVerde recibo = recibosVerdes[i];
                var row = sheet.CreateRow(i + 1);

                var dataCell = row.CreateCell(0);
                dataCell.SetCellValue(recibo.dataEmissao);
                dataCell.CellStyle = dateCellStyle;
                var dataTransmissaoCell = row.CreateCell(1);
                dataTransmissaoCell.SetCellValue(recibo.dataPrestacaoServico);
                dataTransmissaoCell.CellStyle = dateCellStyle;
                row.CreateCell(2).SetCellValue(recibo.anulado ? "Anulado" : "Emitido");
                row.CreateCell(3).SetCellValue(recibo.tipoDoc);
                row.CreateCell(4).SetCellValue(recibo.numDoc);
                row.CreateCell(5).SetCellValue(recibo.nifAdquirente);
                row.CreateCell(6).SetCellValue(recibo.nomeAdquirente);
                row.CreateCell(7).SetCellValue(recibo.paisAdquirente);
                row.CreateCell(8).SetCellValue(ScraperRecibosVerdes.GetReciboTipoString(recibo.tipoReciboVerde));
                row.CreateCell(9).SetCellValue(recibo.descricao);
                row.CreateCell(10).SetCellValue((double)recibo.valores.valorBase);
                row.CreateCell(11).SetCellValue((double)recibo.valores.valorIvaContinente);
                row.CreateCell(12).SetCellValue((double)recibo.valores.impostoSelo);
                row.CreateCell(13).SetCellValue((double)recibo.valores.irsSemRetencao);
                row.CreateCell(14).SetCellValue((double)recibo.valores.importanciaRecebida);
            }

            //Redimensiona as colunas
            for (int col = 0; col < headers.Length; col++)
            {
                sheet.AutoSizeColumn(col);
            }


            //Escreve o ficheiro
            string filePath = Path.Combine(GetDiretorioEmpresa(Declaracao.AT_LISTA_RECIBOS_VERDES_PARA_EXCEL_PRESTADOS, -1), "Lista recibos verdes.xlsx");
            new System.IO.FileInfo(filePath).Directory.Create();

            using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                workbook.Write(fs);
            }
        }

        
    }
}
