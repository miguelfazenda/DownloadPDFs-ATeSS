using AutoUpdaterDotNET;
using Download_PDFs_AT_e_SS.Forms;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Download_PDFs_AT_e_SS
{
    public partial class Form1 : Form
    {
        public const string AUTOUPDATE_URL = "https://github.com/miguelfazenda/DownloadPDFs-ATeSS/releases/download/Latest/autoupdate.xml";
        public Form1()
        {
            InitializeComponent();


            AutoUpdater.Start(AUTOUPDATE_URL);

            Dados.Load();

            //Preenche listas
            foreach(Declaracao declacacao in Declaracao.declaracoes)
            {
                if (declacacao.Tipo == Declaracao.TipoDeclaracao.Mensal)
                    listaDeclaracoesMensais.Items.Add(declacacao);
                else if (declacacao.Tipo == Declaracao.TipoDeclaracao.Anual)
                    listaDeclaracoesAnuais.Items.Add(declacacao);
                else if (declacacao.Tipo == Declaracao.TipoDeclaracao.Pedido)
                    listaDeclaracoesPedidosCertidao.Items.Add(declacacao);
                else if (declacacao.Tipo == Declaracao.TipoDeclaracao.Lista)
                    listaDeclaracoesListas.Items.Add(declacacao);
            }
            
            listaEmpresas.Items.AddRange(Dados.empresas.ToArray());

            //Preenche comboboxes ano e mês
            comboMes.SelectedIndex = DateTime.Now.Month - 1;
            for (int ano = DateTime.Now.Year; ano > DateTime.Now.Year - 4; ano--)
                comboAno.Items.Add(ano);
            comboAno.SelectedIndex = 0;

            //Preenche o caminho da pasta de download
            if (Properties.Settings.Default.PastaDownload == null || Properties.Settings.Default.PastaDownload.Trim().Length == 0)
            {
                Properties.Settings.Default.PastaDownload = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            }
            txtDownloadFolderPath.Text = Properties.Settings.Default.PastaDownload;

            chkHeadless.Checked = Properties.Settings.Default.BrowserHeadless;

            //Mostra a versão
            lblVersao.Text = "Versão: " + Util.GetVersion();
        }

        private void btnExecutar_Click(object sender, EventArgs e)
        {
            if (listaEmpresas.CheckedIndices.Count == 0)
            {
                MessageBox.Show("Não selecionou empresas", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            //Justa as declarações selecionadas em cada lista
            Declaracao[] declaracoes = Util.MergeArrays(
                listaDeclaracoesAnuais.CheckedItems.Cast<Declaracao>().ToArray(),
                listaDeclaracoesMensais.CheckedItems.Cast<Declaracao>().ToArray(),
                listaDeclaracoesListas.CheckedItems.Cast<Declaracao>().ToArray(),
                listaDeclaracoesPedidosCertidao.CheckedItems.Cast<Declaracao>().ToArray());

            if (declaracoes.Length == 0)
            {
                MessageBox.Show("Não selecionou nenhuma declaração", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            int ano;
            if(!Int32.TryParse(comboAno.Text, out ano))
            {
                MessageBox.Show("Ano inválido!");
                return;
            }
            int mes = comboMes.SelectedIndex + 1;

            bool headless = chkHeadless.Checked;

            //Envia lista de empresas e declarações selecionadas, ano e mes selecionados para o background worker
            bgWorker.RunWorkerAsync(new object[] { listaEmpresas.CheckedItems.Cast<Empresa>().ToArray(),
                declaracoes,
                ano,
                mes,
                txtDownloadFolderPath.Text,
                headless } );

            EnableDisableControlsDuringExecution(false);
        }

        /// <summary>
        /// Ativa ou inativa botões enquanto o downloader está a correr
        /// </summary>
        /// <param name="en"></param>
        private void EnableDisableControlsDuringExecution(bool en)
        {
            btnExecutar.Enabled = en;
            btnBrowseDownloadFolder.Enabled = en;
            txtDownloadFolderPath.Enabled = en;
            chkHeadless.Enabled = en;
        }

        /// <summary>
        /// Esta função corre noutra Thread. É chamada quando o se clica no botão executar.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var argumentos = (object[])e.Argument;

            //Lista de empresas e declarações selecionadas
            var empresas = (Empresa[])argumentos[0];
            var declaracoes = (Declaracao[])argumentos[1];
            int ano = (int)argumentos[2];
            int mes = (int)argumentos[3];
            string downloadFolder = (string)argumentos[4];
            bool headless = (bool)argumentos[5];


            Downloader.Executar(empresas, declaracoes, ano, mes, downloadFolder, bgWorker.ReportProgress, headless);
        }

        private void bgWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
        }

        private void bgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            EnableDisableControlsDuringExecution(true);
            if(Downloader.errors.Count > 0)
            {
                MessageBox.Show(String.Join("\n", Downloader.errors), "Erros", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Downloader.ClearErrorLogs();
            }

            progressBar.Value = 0;

            MessageBox.Show("Concluído!", "Concluído", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        Empresa empresaRightClicked;

        private void listaEmpresas_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
            var index = listaEmpresas.IndexFromPoint(e.Location);
            bool empresaSelecionada = (index != ListBox.NoMatches);

            menuItemEditarEmpresa.Visible = empresaSelecionada;
            menuItemConsultarEmpresa.Visible = empresaSelecionada;
            menuItemRemoverEmpresa.Visible = empresaSelecionada;

            abrirCertidaoPermanenteToolStripMenuItem.Visible = empresaSelecionada;
            abrirFundosDeCompensaçãoToolStripMenuItem.Visible = empresaSelecionada;
            abrirPortalDasFinançasToolStripMenuItem.Visible = empresaSelecionada;
            abrirPortalEFaturaToolStripMenuItem.Visible = empresaSelecionada;
            abrirSegurançaSocialToolStripMenuItem.Visible = empresaSelecionada;

            if (empresaSelecionada)
            {
                empresaRightClicked = (Empresa)listaEmpresas.Items[index];
                ctxMenuEmpresa.Show(Cursor.Position);
                ctxMenuEmpresa.Visible = true;
            }
            else
            {
                ctxMenuEmpresa.Show(Cursor.Position);
                ctxMenuEmpresa.Visible = true;
            }
        }

        private void menuItemEditarEmpresa_Click(object sender, EventArgs e)
        {
            new FormEditarEmpresa(empresaRightClicked, false).ShowDialog();
        }

        private void menuItemRemoverEmpresa_Click(object sender, EventArgs e)
        {
            listaEmpresas.Items.Remove(empresaRightClicked);
            
            Dados.RemoveEmpresa(empresaRightClicked);

        }

        private void menuItemNovaEmpresa_Click(object sender, EventArgs e)
        {
            if(new FormEditarEmpresa(null, false).ShowDialog() == DialogResult.OK)
            {
                listaEmpresas.Items.Add(Dados.empresas.Last());
            }
        }

        private void btnBrowseDownloadFolder_Click(object sender, EventArgs e)
        {
            downloadBrowserDialog.SelectedPath = txtDownloadFolderPath.Text;

            if(downloadBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                txtDownloadFolderPath.Text = downloadBrowserDialog.SelectedPath;
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Browser.FechaDriversAbertos();

            Properties.Settings.Default.PastaDownload = txtDownloadFolderPath.Text;
            Properties.Settings.Default.BrowserHeadless = chkHeadless.Checked;
            Properties.Settings.Default.Save();
        }

        private void linkGithub_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://github.com/miguelfazenda/DownloadPDFs-ATeSS");
        }
        private void linkiefatura_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://iefatura.tws.pt/");
        }

        private void btnSelecionarTodasEmpresas_Click(object sender, EventArgs e)
        {
            bool check = listaEmpresas.CheckedItems.Count < listaEmpresas.Items.Count; //Se faz check ou uncheck

            for (int i = 0; i < listaEmpresas.Items.Count; i++)
            {
                listaEmpresas.SetItemChecked(i, check);
            }
        }

        private void abrirCertidaoPermanenteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Browser.CriarDriver();
            Browser.AbrePedidoCertidao(empresaRightClicked.CodigoCertidaoPermanente);
        }

        private void btnDefinicoes_Click(object sender, EventArgs e)
        {
            DefinicoesForm definicoesForm = new DefinicoesForm();
            definicoesForm.ShowDialog();
        }

        private void menuItemConsultarEmpresa_Click(object sender, EventArgs e)
        {
            new FormEditarEmpresa(empresaRightClicked, true).ShowDialog();
        }

        private void procurarAtualizaçõesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AutoUpdater.Start(AUTOUPDATE_URL);
        }

        private void abrirPortalDasFinançasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Browser.CriarDriver();
            Browser.AbrePortalDasFinancas(empresaRightClicked);
        }

        private void abrirPortalEFaturaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Browser.CriarDriver();
            Browser.AbreEFatura(empresaRightClicked);
        }

        private void abrirSegurançaSocialToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Browser.CriarDriver();
            Browser.AbreSegurancaSocial(empresaRightClicked);
        }

        private void abrirFundosDeCompensaçãoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Browser.CriarDriver();
            Browser.AbreFundosDeCompensacao(empresaRightClicked);
        }
    }
}
