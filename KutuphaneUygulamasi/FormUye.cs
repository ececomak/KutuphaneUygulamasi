using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace KutuphaneUygulamasi
{
    public partial class FormUye : Form
    {
        string connectionString = @"Server=LAPTOP-VO8CPHGU\SQLEXPRESS;Database=KutuphaneDB;Trusted_Connection=True;";
        DataGridView dataGridView;
        TextBox txtAra;

        public FormUye()
        {
            InitializeComponent();
            this.AutoScroll = true;

            Label lbl = new Label()
            {
                Text = "Hoş geldiniz Üye!",
                Location = new Point(30, 30),
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                AutoSize = true
            };
            this.Controls.Add(lbl);

            txtAra = new TextBox()
            {
                Width = 400,
                Location = new Point(30, 70)
            };
            txtAra.TextChanged += TxtAra_TextChanged;
            this.Controls.Add(txtAra);

            dataGridView = new DataGridView()
            {
                Location = new Point(30, 100),
                Size = new Size(800, 300),
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            this.Controls.Add(dataGridView);

            LoadKitaplar();

            Button btnCikis = new Button()
            {
                Text = "Çıkış Yap",
                Size = new Size(90, 30),
                BackColor = Color.FromArgb(255, 230, 230),
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right
            };
            btnCikis.Location = new Point(this.ClientSize.Width - btnCikis.Width - 20, this.ClientSize.Height - btnCikis.Height - 20);
            btnCikis.Click += (s, e) =>
            {
                this.Hide();
                Form1 girisFormu = new Form1();
                girisFormu.Show();
            };
            this.Controls.Add(btnCikis);
        }

        private void LoadKitaplar()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Kitaplar", conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dataGridView.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("HATA: " + ex.Message);
                }
            }
        }

        private void TxtAra_TextChanged(object sender, EventArgs e)
        {
            (dataGridView.DataSource as DataTable).DefaultView.RowFilter =
                string.Format("KitapAdi LIKE '%{0}%' OR Yazar LIKE '%{0}%'", txtAra.Text.Trim());
        }
    }
}