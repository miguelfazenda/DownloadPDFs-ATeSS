using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Download_PDFs_AT_e_SS.Forms
{
    public partial class DefinicoesForm : Form
    {
        public DefinicoesForm()
        {
            InitializeComponent();

            gridViewClassifEmitidos.DataSource = CriaListaElementosClassificacao(Definicoes.definicoesExportacaoPrestador);
            gridViewClassifEmitidos.CellEnter += (sender, e) => ((DataGridView)sender).BeginEdit(true);

            gridViewClassifAdquridos.DataSource = CriaListaElementosClassificacao(Definicoes.definicoesExportacaoAdquirente);
            gridViewClassifAdquridos.CellEnter += (sender, e) => ((DataGridView)sender).BeginEdit(true);
        }

        private BindingList<DefinicoesExportTipoReciboVerde> CriaListaElementosClassificacao(DefinicoesExportacao definicoesExportacao)
        {
            BindingList<DefinicoesExportTipoReciboVerde> listaDefExport = new BindingList<DefinicoesExportTipoReciboVerde>();
            listaDefExport.Add(definicoesExportacao.defExportFaturaRecibo.defExportTipoPagamento);
            listaDefExport.Add(definicoesExportacao.defExportFaturaRecibo.defExportTipoAdiantamento);
            listaDefExport.Add(definicoesExportacao.defExportFaturaRecibo.defExportTipoAdiantamentoPagamento);
            listaDefExport.Add(definicoesExportacao.defExportFatura.defExportTipoPagamento);
            listaDefExport.Add(definicoesExportacao.defExportFatura.defExportTipoAdiantamento);
            listaDefExport.Add(definicoesExportacao.defExportFatura.defExportTipoAdiantamentoPagamento);
            listaDefExport.Add(definicoesExportacao.defExportRecibo.defExportTipoPagamento);
            listaDefExport.Add(definicoesExportacao.defExportRecibo.defExportTipoAdiantamento);
            listaDefExport.Add(definicoesExportacao.defExportRecibo.defExportTipoAdiantamentoPagamento);
            return listaDefExport;
        }

        bool saveDefinicoes = false;

        private void btnOK_Click(object sender, EventArgs e)
        {
            saveDefinicoes = true; //Informa que quando o form fechar não é para fazer reset às definicoes
            Definicoes.Save();
            Close();
        }

        private void DefinicoesForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(!saveDefinicoes)
            {
                //Se não tiver carregado no OK, faz reset às definicoes
                Definicoes.Load();
            }
        }
    }
}
