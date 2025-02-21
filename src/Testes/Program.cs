using System;
using System.Windows.Forms;
using PdfiumViewer;
using System.IO;

namespace PdfiumExample
{
    public partial class MainForm : Form
    {
        private PdfDocument _pdfDocument;

        public MainForm()
        {
            InitializeComponent();
        }

        // Botão para carregar o PDF
        private void btnCarregar_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Arquivos PDF (*.pdf)|*.pdf";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // Liberar documento anterior
                        _pdfDocument?.Dispose();

                        // Carregar novo documento
                        _pdfDocument = PdfDocument.Load(openFileDialog.FileName);

                        // Criar visualizador e adicionar ao formulário
                        var pdfViewer = new PdfViewer();
                        pdfViewer.Dock = DockStyle.Fill;
                        pdfViewer.Document = _pdfDocument;

                        // Limpar controles antigos
                        this.Controls.Clear();
                        this.Controls.Add(pdfViewer);
                        this.Controls.Add(btnCarregar);
                        this.Controls.Add(btnImprimir);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Erro ao carregar PDF: {ex.Message}");
                    }
                }
            }
        }

        // Botão para imprimir
        private void btnImprimir_Click(object sender, EventArgs e)
        {
            if (_pdfDocument == null)
            {
                MessageBox.Show("Carregue um PDF primeiro!");
                return;
            }

            using (PrintDialog printDialog = new PrintDialog())
            {
                if (printDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (var printDocument = _pdfDocument.CreatePrintDocument())
                        {
                            printDocument.PrinterSettings = printDialog.PrinterSettings;
                            printDocument.Print();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Erro ao imprimir: {ex.Message}");
                    }
                }
            }
        }

        // Componentes do formulário
        private Button btnCarregar;
        private Button btnImprimir;

        private void InitializeComponent()
        {
            this.btnCarregar = new System.Windows.Forms.Button();
            this.btnImprimir = new System.Windows.Forms.Button();
            this.SuspendLayout();

            // btnCarregar
            this.btnCarregar.Location = new System.Drawing.Point(12, 12);
            this.btnCarregar.Text = "Carregar PDF";
            this.btnCarregar.Click += new System.EventHandler(this.btnCarregar_Click);

            // btnImprimir
            this.btnImprimir.Location = new System.Drawing.Point(120, 12);
            this.btnImprimir.Text = "Imprimir";
            this.btnImprimir.Click += new System.EventHandler(this.btnImprimir_Click);

            // MainForm
            this.ClientSize = new System.Drawing.Size(800, 600);
            this.Controls.Add(this.btnCarregar);
            this.Controls.Add(this.btnImprimir);
            this.ResumeLayout(false);
        }
    }

    class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}