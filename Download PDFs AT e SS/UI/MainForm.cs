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

        public Form1()
        {
            InitializeComponent();
            Dados.Load();

            

            listaDeclaracoesMensais.Items.AddRange(Declaracao.declaracoes.Where(dec => dec.Mensal).ToArray());
            listaDeclaracoesAnuais.Items.AddRange(Declaracao.declaracoes.Where(dec => !dec.Mensal).ToArray());
            listaEmpresas.Items.AddRange(Dados.empresas.ToArray());

            //Preenche comboboxes ano e mês
            comboMes.SelectedIndex = DateTime.Now.Month - 1;
            for (int ano = DateTime.Now.Year; ano > DateTime.Now.Year - 4; ano--)
                comboAno.Items.Add(ano);
            comboAno.SelectedIndex = 0;

            //Preenche o caminho da pasta de download
            if(!Uri.IsWellFormedUriString(Properties.Settings.Default.PastaDownload, UriKind.Absolute))
            {
                Properties.Settings.Default.PastaDownload = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            }
            txtDownloadFolderPath.Text = Properties.Settings.Default.PastaDownload;

            //Mostra a versão
            lblVersao.Text = Util.GetVersion();
        }

        private void btnExecutar_Click(object sender, EventArgs e)
        {
            if (listaEmpresas.CheckedIndices.Count == 0)
            {
                MessageBox.Show("Não selecionou empresas", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (listaDeclaracoesMensais.CheckedIndices.Count == 0 && listaDeclaracoesAnuais.CheckedIndices.Count == 0)
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

            //Envia lista de empresas e declarações selecionadas, ano e mes selecionados para o background worker
            bgWorker.RunWorkerAsync(new object[] { listaEmpresas.CheckedItems.Cast<Empresa>().ToArray(),
                listaDeclaracoesMensais.CheckedItems.Cast<Declaracao>().ToArray(),
                listaDeclaracoesAnuais.CheckedItems.Cast<Declaracao>().ToArray(),
                ano,
                mes,
                txtDownloadFolderPath.Text } );

            EnableDisableControlsDuringExecution(false);
        }

        private void EnableDisableControlsDuringExecution(bool en)
        {
            btnExecutar.Enabled = en;
            btnBrowseDownloadFolder.Enabled = en;
            txtDownloadFolderPath.Enabled = en;
        }

        private void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var argumentos = (object[])e.Argument;

            //Lista de empresas e declarações selecionadas
            var empresas = (Empresa[])argumentos[0];
            var declaracoesMensais = (Declaracao[])argumentos[1];
            var declaracoesAnuais = (Declaracao[])argumentos[2];
            int ano = (int)argumentos[3];
            int mes = (int)argumentos[4];
            string downloadFolder = (string)argumentos[5];

            Downloader.Executar(empresas, declaracoesMensais, declaracoesAnuais, ano, mes, downloadFolder);
        }

        private void bgWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

        }

        private void bgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            EnableDisableControlsDuringExecution(true);
            if(Downloader.errors.Count > 0)
            {
                MessageBox.Show(String.Join("\n", Downloader.errors), "Erros", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Downloader.ClearErrorLogs();
            }
        }

        Empresa empresaRightClicked;

        private void listaEmpresas_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
            var index = listaEmpresas.IndexFromPoint(e.Location);
            bool empresaSelecionada = (index != ListBox.NoMatches);

            menuItemEditarEmpresa.Visible = empresaSelecionada;
            menuItemRemoverEmpresa.Visible = empresaSelecionada;

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
            new FormEditarEmpresa(empresaRightClicked).ShowDialog();
        }

        private void menuItemRemoverEmpresa_Click(object sender, EventArgs e)
        {
            listaEmpresas.Items.Remove(empresaRightClicked);
            
            Dados.RemoveEmpresa(empresaRightClicked);

        }

        private void menuItemNovaEmpresa_Click(object sender, EventArgs e)
        {
            if(new FormEditarEmpresa(null).ShowDialog() == DialogResult.OK)
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
            Properties.Settings.Default.PastaDownload = txtDownloadFolderPath.Text;
            Properties.Settings.Default.Save();
        }

        private void linkGithub_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://github.com/miguel71/DownloadPDFs-ATeSS");
        }
    }
}
