﻿namespace Download_PDFs_AT_e_SS
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.listaEmpresas = new System.Windows.Forms.CheckedListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.listaDeclaracoesMensais = new System.Windows.Forms.CheckedListBox();
            this.comboAno = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.comboMes = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.listaDeclaracoesAnuais = new System.Windows.Forms.CheckedListBox();
            this.btnExecutar = new System.Windows.Forms.Button();
            this.bgWorker = new System.ComponentModel.BackgroundWorker();
            this.ctxMenuEmpresa = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuItemEditarEmpresa = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemRemoverEmpresa = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemNovaEmpresa = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.abrirCertidaoPermanenteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label6 = new System.Windows.Forms.Label();
            this.txtDownloadFolderPath = new System.Windows.Forms.TextBox();
            this.btnBrowseDownloadFolder = new System.Windows.Forms.Button();
            this.downloadBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.lblVersao = new System.Windows.Forms.Label();
            this.linkiefatura = new System.Windows.Forms.LinkLabel();
            this.linkGithub = new System.Windows.Forms.LinkLabel();
            this.label7 = new System.Windows.Forms.Label();
            this.listaDeclaracoesListas = new System.Windows.Forms.CheckedListBox();
            this.label8 = new System.Windows.Forms.Label();
            this.listaDeclaracoesPedidosCertidao = new System.Windows.Forms.CheckedListBox();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.btnSelecionarTodasEmpresas = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.ctxMenuEmpresa.SuspendLayout();
            this.SuspendLayout();
            // 
            // listaEmpresas
            // 
            this.listaEmpresas.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.listaEmpresas.FormattingEnabled = true;
            this.listaEmpresas.Location = new System.Drawing.Point(12, 31);
            this.listaEmpresas.Name = "listaEmpresas";
            this.listaEmpresas.Size = new System.Drawing.Size(173, 379);
            this.listaEmpresas.TabIndex = 0;
            this.listaEmpresas.MouseDown += new System.Windows.Forms.MouseEventHandler(this.listaEmpresas_MouseDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Empresas:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(231, 69);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(111, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Declarações mensais:";
            // 
            // listaDeclaracoesMensais
            // 
            this.listaDeclaracoesMensais.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.listaDeclaracoesMensais.FormattingEnabled = true;
            this.listaDeclaracoesMensais.Location = new System.Drawing.Point(234, 85);
            this.listaDeclaracoesMensais.Name = "listaDeclaracoesMensais";
            this.listaDeclaracoesMensais.Size = new System.Drawing.Size(173, 364);
            this.listaDeclaracoesMensais.TabIndex = 2;
            // 
            // comboAno
            // 
            this.comboAno.FormattingEnabled = true;
            this.comboAno.Location = new System.Drawing.Point(286, 12);
            this.comboAno.Name = "comboAno";
            this.comboAno.Size = new System.Drawing.Size(121, 21);
            this.comboAno.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(231, 15);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Ano:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(231, 42);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(30, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Mês:";
            // 
            // comboMes
            // 
            this.comboMes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboMes.FormattingEnabled = true;
            this.comboMes.Items.AddRange(new object[] {
            "Janeiro",
            "Fevereiro",
            "Março",
            "Abril",
            "Maio",
            "Junho",
            "Julho",
            "Agosto",
            "Setembro",
            "Outubro",
            "Novembro",
            "Dezembro"});
            this.comboMes.Location = new System.Drawing.Point(286, 39);
            this.comboMes.Name = "comboMes";
            this.comboMes.Size = new System.Drawing.Size(121, 21);
            this.comboMes.TabIndex = 6;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(410, 69);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(104, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Declarações anuais:";
            // 
            // listaDeclaracoesAnuais
            // 
            this.listaDeclaracoesAnuais.FormattingEnabled = true;
            this.listaDeclaracoesAnuais.Location = new System.Drawing.Point(413, 85);
            this.listaDeclaracoesAnuais.Name = "listaDeclaracoesAnuais";
            this.listaDeclaracoesAnuais.Size = new System.Drawing.Size(173, 109);
            this.listaDeclaracoesAnuais.TabIndex = 8;
            // 
            // btnExecutar
            // 
            this.btnExecutar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExecutar.Location = new System.Drawing.Point(391, 517);
            this.btnExecutar.Name = "btnExecutar";
            this.btnExecutar.Size = new System.Drawing.Size(75, 23);
            this.btnExecutar.TabIndex = 10;
            this.btnExecutar.Text = "Executar";
            this.btnExecutar.UseVisualStyleBackColor = true;
            this.btnExecutar.Click += new System.EventHandler(this.btnExecutar_Click);
            // 
            // bgWorker
            // 
            this.bgWorker.WorkerReportsProgress = true;
            this.bgWorker.WorkerSupportsCancellation = true;
            this.bgWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgWorker_DoWork);
            this.bgWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.bgWorker_ProgressChanged);
            this.bgWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgWorker_RunWorkerCompleted);
            // 
            // ctxMenuEmpresa
            // 
            this.ctxMenuEmpresa.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemEditarEmpresa,
            this.menuItemRemoverEmpresa,
            this.menuItemNovaEmpresa,
            this.toolStripSeparator1,
            this.abrirCertidaoPermanenteToolStripMenuItem});
            this.ctxMenuEmpresa.Name = "ctxMenuEmpresa";
            this.ctxMenuEmpresa.Size = new System.Drawing.Size(214, 98);
            // 
            // menuItemEditarEmpresa
            // 
            this.menuItemEditarEmpresa.Name = "menuItemEditarEmpresa";
            this.menuItemEditarEmpresa.Size = new System.Drawing.Size(213, 22);
            this.menuItemEditarEmpresa.Text = "Editar";
            this.menuItemEditarEmpresa.Click += new System.EventHandler(this.menuItemEditarEmpresa_Click);
            // 
            // menuItemRemoverEmpresa
            // 
            this.menuItemRemoverEmpresa.Name = "menuItemRemoverEmpresa";
            this.menuItemRemoverEmpresa.Size = new System.Drawing.Size(213, 22);
            this.menuItemRemoverEmpresa.Text = "Remover";
            this.menuItemRemoverEmpresa.Click += new System.EventHandler(this.menuItemRemoverEmpresa_Click);
            // 
            // menuItemNovaEmpresa
            // 
            this.menuItemNovaEmpresa.Name = "menuItemNovaEmpresa";
            this.menuItemNovaEmpresa.Size = new System.Drawing.Size(213, 22);
            this.menuItemNovaEmpresa.Text = "Nova Empresa";
            this.menuItemNovaEmpresa.Click += new System.EventHandler(this.menuItemNovaEmpresa_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(210, 6);
            // 
            // abrirCertidaoPermanenteToolStripMenuItem
            // 
            this.abrirCertidaoPermanenteToolStripMenuItem.Name = "abrirCertidaoPermanenteToolStripMenuItem";
            this.abrirCertidaoPermanenteToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
            this.abrirCertidaoPermanenteToolStripMenuItem.Text = "Abrir certidão permanente";
            this.abrirCertidaoPermanenteToolStripMenuItem.Click += new System.EventHandler(this.abrirCertidaoPermanenteToolStripMenuItem_Click);
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 494);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(101, 13);
            this.label6.TabIndex = 11;
            this.label6.Text = "Pasta tranferências:";
            // 
            // txtDownloadFolderPath
            // 
            this.txtDownloadFolderPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDownloadFolderPath.Location = new System.Drawing.Point(119, 491);
            this.txtDownloadFolderPath.Name = "txtDownloadFolderPath";
            this.txtDownloadFolderPath.Size = new System.Drawing.Size(266, 20);
            this.txtDownloadFolderPath.TabIndex = 12;
            // 
            // btnBrowseDownloadFolder
            // 
            this.btnBrowseDownloadFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseDownloadFolder.Location = new System.Drawing.Point(391, 491);
            this.btnBrowseDownloadFolder.Name = "btnBrowseDownloadFolder";
            this.btnBrowseDownloadFolder.Size = new System.Drawing.Size(29, 20);
            this.btnBrowseDownloadFolder.TabIndex = 13;
            this.btnBrowseDownloadFolder.Text = "...";
            this.btnBrowseDownloadFolder.UseVisualStyleBackColor = true;
            this.btnBrowseDownloadFolder.Click += new System.EventHandler(this.btnBrowseDownloadFolder_Click);
            // 
            // lblVersao
            // 
            this.lblVersao.AutoSize = true;
            this.lblVersao.Location = new System.Drawing.Point(512, 30);
            this.lblVersao.Name = "lblVersao";
            this.lblVersao.Size = new System.Drawing.Size(75, 13);
            this.lblVersao.TabIndex = 14;
            this.lblVersao.Text = "Versão: x.x.x.x";
            // 
            // linkiefatura
            // 
            this.linkiefatura.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.linkiefatura.AutoSize = true;
            this.linkiefatura.Location = new System.Drawing.Point(12, 463);
            this.linkiefatura.Name = "linkiefatura";
            this.linkiefatura.Size = new System.Drawing.Size(339, 13);
            this.linkiefatura.TabIndex = 16;
            this.linkiefatura.TabStop = true;
            this.linkiefatura.Text = "Conheça também o nosso importador do E-Fatura para a contabilidade";
            this.linkiefatura.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkiefatura_LinkClicked);
            // 
            // linkGithub
            // 
            this.linkGithub.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.linkGithub.AutoSize = true;
            this.linkGithub.Location = new System.Drawing.Point(544, 488);
            this.linkGithub.Name = "linkGithub";
            this.linkGithub.Size = new System.Drawing.Size(40, 13);
            this.linkGithub.TabIndex = 18;
            this.linkGithub.TabStop = true;
            this.linkGithub.Text = "GitHub";
            this.linkGithub.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkGithub_LinkClicked);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(410, 205);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(37, 13);
            this.label7.TabIndex = 20;
            this.label7.Text = "Listas:";
            // 
            // listaDeclaracoesListas
            // 
            this.listaDeclaracoesListas.FormattingEnabled = true;
            this.listaDeclaracoesListas.Location = new System.Drawing.Point(413, 221);
            this.listaDeclaracoesListas.Name = "listaDeclaracoesListas";
            this.listaDeclaracoesListas.Size = new System.Drawing.Size(173, 109);
            this.listaDeclaracoesListas.TabIndex = 19;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(411, 339);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(104, 13);
            this.label8.TabIndex = 22;
            this.label8.Text = "Pedidos de certidão:";
            // 
            // listaDeclaracoesPedidosCertidao
            // 
            this.listaDeclaracoesPedidosCertidao.FormattingEnabled = true;
            this.listaDeclaracoesPedidosCertidao.Location = new System.Drawing.Point(414, 355);
            this.listaDeclaracoesPedidosCertidao.Name = "listaDeclaracoesPedidosCertidao";
            this.listaDeclaracoesPedidosCertidao.Size = new System.Drawing.Size(173, 94);
            this.listaDeclaracoesPedidosCertidao.TabIndex = 21;
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Location = new System.Drawing.Point(119, 519);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(266, 20);
            this.progressBar.TabIndex = 23;
            // 
            // btnSelecionarTodasEmpresas
            // 
            this.btnSelecionarTodasEmpresas.Location = new System.Drawing.Point(110, 4);
            this.btnSelecionarTodasEmpresas.Name = "btnSelecionarTodasEmpresas";
            this.btnSelecionarTodasEmpresas.Size = new System.Drawing.Size(75, 21);
            this.btnSelecionarTodasEmpresas.TabIndex = 24;
            this.btnSelecionarTodasEmpresas.Text = "Sel. todas";
            this.btnSelecionarTodasEmpresas.UseVisualStyleBackColor = true;
            this.btnSelecionarTodasEmpresas.Click += new System.EventHandler(this.btnSelecionarTodasEmpresas_Click);
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(12, 413);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(173, 27);
            this.label9.TabIndex = 25;
            this.label9.Text = "Carregue com o botão do lado direito para adicionar empresas";
            // 
            // Form1
            // 
            this.AcceptButton = this.btnExecutar;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(596, 552);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.btnSelecionarTodasEmpresas);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.listaDeclaracoesPedidosCertidao);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.listaDeclaracoesListas);
            this.Controls.Add(this.linkGithub);
            this.Controls.Add(this.linkiefatura);
            this.Controls.Add(this.lblVersao);
            this.Controls.Add(this.btnBrowseDownloadFolder);
            this.Controls.Add(this.txtDownloadFolderPath);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.btnExecutar);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.listaDeclaracoesAnuais);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.comboMes);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.comboAno);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.listaDeclaracoesMensais);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.listaEmpresas);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "TWS Importador de Guias";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.ctxMenuEmpresa.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckedListBox listaEmpresas;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckedListBox listaDeclaracoesMensais;
        private System.Windows.Forms.ComboBox comboAno;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox comboMes;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckedListBox listaDeclaracoesAnuais;
        private System.Windows.Forms.Button btnExecutar;
        private System.ComponentModel.BackgroundWorker bgWorker;
        private System.Windows.Forms.ContextMenuStrip ctxMenuEmpresa;
        private System.Windows.Forms.ToolStripMenuItem menuItemEditarEmpresa;
        private System.Windows.Forms.ToolStripMenuItem menuItemRemoverEmpresa;
        private System.Windows.Forms.ToolStripMenuItem menuItemNovaEmpresa;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtDownloadFolderPath;
        private System.Windows.Forms.Button btnBrowseDownloadFolder;
        private System.Windows.Forms.FolderBrowserDialog downloadBrowserDialog;
        private System.Windows.Forms.Label lblVersao;
        private System.Windows.Forms.LinkLabel linkiefatura;
        private System.Windows.Forms.LinkLabel linkGithub;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.CheckedListBox listaDeclaracoesListas;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.CheckedListBox listaDeclaracoesPedidosCertidao;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Button btnSelecionarTodasEmpresas;
        private System.Windows.Forms.ToolStripMenuItem abrirCertidaoPermanenteToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.Label label9;
    }
}

