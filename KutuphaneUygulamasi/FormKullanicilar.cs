using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace KutuphaneUygulamasi
{
    public partial class FormKullanicilar : Form
    {
        string connectionString = @"Server=LAPTOP-VO8CPHGU\SQLEXPRESS;Database=KutuphaneDB;Trusted_Connection=True;";
        DataGridView dataGridView;

        public FormKullanicilar()
        {
            InitializeComponent();
            this.Text = "Kullanıcı Listesi";
            this.Size = new Size(800, 450);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

            Label lbl = new Label()
            {
                Text = "Sistemde Kayıtlı Kullanıcılar",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(30, 30)
            };
            this.Controls.Add(lbl);

            dataGridView = new DataGridView()
            {
                Location = new Point(30, 80),
                Size = new Size(720, 280),
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            this.Controls.Add(dataGridView);

            Button btnKapat = new Button()
            {
                Text = "KAPAT",
                Location = new Point(30, 370),
                Size = new Size(100, 35),
                BackColor = Color.FromArgb(255, 204, 204),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            btnKapat.Click += (s, e) => this.Close();
            this.Controls.Add(btnKapat);

            LoadKullanicilar();
        }

        private void LoadKullanicilar()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    SqlDataAdapter da = new SqlDataAdapter("SELECT Id, KullaniciAdi, Sifre, Yetki FROM Kullanicilar", conn);
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
    }
}