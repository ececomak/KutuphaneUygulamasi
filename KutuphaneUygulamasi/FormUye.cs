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
        DataGridView dataGridKitaplar;
        DataGridView dataGridOdunc;
        TextBox txtAra;
        Button btnCikis;

        public FormUye()
        {
            InitializeComponent();
            this.Text = "FormUye";
            this.Size = new Size(900, 700);
            this.BackColor = Color.White;
            this.AutoScroll = true;

            Label lblHosgeldiniz = new Label()
            {
                Text = $"Hoş geldiniz {Form1.GirisYapanKullaniciAdi}!",
                Font = new Font("Segoe UI", 14, FontStyle.Italic),
                Location = new Point(30, 30),
                AutoSize = true
            };
            this.Controls.Add(lblHosgeldiniz);

            txtAra = new TextBox()
            {
                Location = new Point(30, 70),
                Width = 300
            };
            txtAra.TextChanged += TxtAra_TextChanged;
            this.Controls.Add(txtAra);

            dataGridKitaplar = new DataGridView()
            {
                Location = new Point(30, 110),
                Size = new Size(800, 250),
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false
            };
            this.Controls.Add(dataGridKitaplar);
            KitaplariYukle();

            Button btnOduncAl = new Button()
            {
                Text = "Ödünç Al",
                Location = new Point(30, 380),
                Size = new Size(120, 35),
                BackColor = Color.Honeydew,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnOduncAl.Click += BtnOduncAl_Click;
            this.Controls.Add(btnOduncAl);

            Label lblOduncBaslik = new Label()
            {
                Text = "📚 Ödünç Aldığınız Kitaplar:",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Location = new Point(30, 430),
                AutoSize = true
            };
            this.Controls.Add(lblOduncBaslik);

            dataGridOdunc = new DataGridView()
            {
                Location = new Point(30, 470),
                Size = new Size(800, 150),
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            this.Controls.Add(dataGridOdunc);
            OduncKitaplariYukle();

            btnCikis = new Button()
            {
                Text = "Çıkış Yap",
                Size = new Size(100, 35),
                BackColor = Color.MistyRose,
                Font = new Font("Segoe UI", 9, FontStyle.Regular)
            };
            btnCikis.Click += (s, e) =>
            {
                this.Hide();
                new Form1().Show();
            };
            this.Controls.Add(btnCikis);

            this.Resize += FormUye_Resize;
            FormUye_Resize(null, null);
        }

        private void FormUye_Resize(object sender, EventArgs e)
        {
            btnCikis.Location = new Point(this.ClientSize.Width - btnCikis.Width - 30, this.ClientSize.Height - btnCikis.Height - 30);
        }

        private void KitaplariYukle()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    SqlDataAdapter da = new SqlDataAdapter("SELECT Id, KitapAdi, Yazar, Dil, YayinEvi, Tur, SayfaSayisi, BasimYili FROM Kitaplar", conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dataGridKitaplar.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("HATA: " + ex.Message);
                }
            }
        }

        private void TxtAra_TextChanged(object sender, EventArgs e)
        {
            string filtre = txtAra.Text.Trim();
            (dataGridKitaplar.DataSource as DataTable).DefaultView.RowFilter =
                $"KitapAdi LIKE '%{filtre}%' OR Yazar LIKE '%{filtre}%'";
        }

        private void BtnOduncAl_Click(object sender, EventArgs e)
        {
            if (dataGridKitaplar.SelectedRows.Count > 0)
            {
                int kitapId = Convert.ToInt32(dataGridKitaplar.SelectedRows[0].Cells["Id"].Value);
                DateTime alim = DateTime.Today;
                DateTime iade = alim.AddDays(30);

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    try
                    {
                        conn.Open();
                        SqlCommand cmd = new SqlCommand(@"INSERT INTO OduncAlinanlar (KitapId, KullaniciId, AlimTarihi, IadeTarihi)
                                                          SELECT @kitapId, Id, @alim, @iade FROM Kullanicilar WHERE KullaniciAdi = @adi", conn);
                        cmd.Parameters.AddWithValue("@kitapId", kitapId);
                        cmd.Parameters.AddWithValue("@adi", Form1.GirisYapanKullaniciAdi);
                        cmd.Parameters.AddWithValue("@alim", alim);
                        cmd.Parameters.AddWithValue("@iade", iade);
                        cmd.ExecuteNonQuery();

                        MessageBox.Show("Kitap başarıyla ödünç alındı!");
                        OduncKitaplariYukle();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("HATA: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Lütfen ödünç alınacak kitabı seçin.");
            }
        }

        private void OduncKitaplariYukle()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(@"SELECT k.KitapAdi, o.AlimTarihi, o.IadeTarihi
                                                      FROM OduncAlinanlar o
                                                      JOIN Kitaplar k ON o.KitapId = k.Id
                                                      JOIN Kullanicilar ku ON ku.Id = o.KullaniciId
                                                      WHERE ku.KullaniciAdi = @adi", conn);
                    cmd.Parameters.AddWithValue("@adi", Form1.GirisYapanKullaniciAdi);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dataGridOdunc.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("HATA: " + ex.Message);
                }
            }
        }
    }
}