using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Download_PDFs_AT_e_SS
{
    public partial class FormEditarEmpresa : Form
    {
        private Empresa empresaAEditarOriginal; //A que está na lista de empresas, quando se guardar, os campos desta vão ser alterados
        private Empresa empresaAEditarClone; //A que está a ser editada

        /// <summary>
        /// 
        /// </summary>
        /// <param name="empresaAEditar">Se for null é porque estamos a criar uma empresa nova. O resultado será colocado na empresaAEditar</param>
        public FormEditarEmpresa(Empresa empresaAEditar, bool consultarApenas)
        {
            if (empresaAEditar == null)
            {
                this.empresaAEditarClone = new Empresa("", "", "");
            }
            else
            {
                this.empresaAEditarOriginal = empresaAEditar;
                this.empresaAEditarClone = (Empresa)this.empresaAEditarOriginal.Clone();
            }


            InitializeComponent();
            this.DialogResult = DialogResult.Cancel;

            //Se estiver no modo de consultar não deixa alterar
            if(consultarApenas)
            {
                this.Text = "Consultar dados da empresa";
                List<TextBox> textBoxesToSetReadonly = new List<TextBox>();
                textBoxesToSetReadonly.AddRange(this.Controls.OfType<TextBox>());
                textBoxesToSetReadonly.AddRange(groupBox1.Controls.OfType<TextBox>());
                foreach (Control x in textBoxesToSetReadonly)
                {
                    ((TextBox)x).ReadOnly = true;
                }

                btnCancel.Visible = false;
                btnOK.Visible = false;
            }
        }

        private void FormEditarEmpresa_Load(object sender, EventArgs e)
        {
            txtNome.DataBindings.Add("Text", empresaAEditarClone, "Nome");
            txtCodigo.DataBindings.Add("Text", empresaAEditarClone, "Codigo");

            txtNIF.DataBindings.Add("Text", empresaAEditarClone, "NIF");
            txtPasswordAT.Text = empresaAEditarClone.PasswordAT;

            txtNISS.DataBindings.Add("Text", empresaAEditarClone, "NISS");
            txtPasswordSS.Text = empresaAEditarClone.PasswordSS;

            txtNomeDoResponsavel.DataBindings.Add("Text", empresaAEditarClone, "NomeDoResponsavel");
            txtTelefoneDoResponsavel.DataBindings.Add("Text", empresaAEditarClone, "TelefoneDoResponsavel");
            txtEmailDoResponsavel.DataBindings.Add("Text", empresaAEditarClone, "EmailDoResponsavel");

            txtCodigoCertidaoPermanente.DataBindings.Add("Text", empresaAEditarClone, "CodigoCertidaoPermanente");
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;

            //Encripta a password, caso tenha sido alterada
            empresaAEditarClone.PasswordAT = txtPasswordAT.Text;
            empresaAEditarClone.PasswordSS = txtPasswordSS.Text;

            //Guarda o resultado
            if (empresaAEditarOriginal == null)
            {
                //Nova empresa
                Dados.empresas.Add(empresaAEditarClone);
            }
            else
            {
                //A editar uma empresa
                Util.CopyAll<Empresa>(empresaAEditarClone, empresaAEditarOriginal);
            }
            Dados.SaveEmpresas();

            Close();
        }
    }
}
