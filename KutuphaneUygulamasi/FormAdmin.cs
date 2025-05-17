using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace KutuphaneUygulamasi
{
    public partial class FormAdmin : Form
    {
        string connectionString = @"Server=LAPTOP-VO8CPHGU\SQLEXPRESS;Database=KutuphaneDB;Trusted_Connection=True;";
        Control[] kutular;
        DataGridView dataGridView;

        public FormAdmin()
        {
            InitializeComponent();
            this.AutoScroll = true;

            Label lbl = new Label()
            {
                Text = "Hoş geldiniz Admin!",
                Location = new Point(30, 30),
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                AutoSize = true
            };
            this.Controls.Add(lbl);

            TextBox txtAra = new TextBox()
            {
                Width = 300,
                Location = new Point(30, 70)
            };
            txtAra.TextChanged += (s, e) =>
            {
                string filter = txtAra.Text.Trim().ToLower();
                (dataGridView.DataSource as DataTable).DefaultView.RowFilter =
                    $"KitapAdi LIKE '%{filter}%' OR Yazar LIKE '%{filter}%'";
            };
            this.Controls.Add(txtAra);

            dataGridView = new DataGridView()
            {
                Location = new Point(30, 100),
                Size = new Size(720, 300),
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false
            };
            this.Controls.Add(dataGridView);
            LoadKitaplar(dataGridView);

            dataGridView.CellClick += (s, e) =>
            {
                if (dataGridView.CurrentRow != null && dataGridView.CurrentRow.Index >= 0)
                {
                    ((TextBox)kutular[0]).Text = dataGridView.CurrentRow.Cells["KitapAdi"].Value.ToString();
                    ((TextBox)kutular[1]).Text = dataGridView.CurrentRow.Cells["Yazar"].Value.ToString();
                    ((TextBox)kutular[2]).Text = dataGridView.CurrentRow.Cells["Dil"].Value.ToString();
                    ((TextBox)kutular[3]).Text = dataGridView.CurrentRow.Cells["YayinEvi"].Value.ToString();
                    ((TextBox)kutular[4]).Text = dataGridView.CurrentRow.Cells["Tur"].Value.ToString();
                    ((NumericUpDown)kutular[5]).Value = Convert.ToInt32(dataGridView.CurrentRow.Cells["SayfaSayisi"].Value);
                    ((NumericUpDown)kutular[6]).Value = Convert.ToInt32(dataGridView.CurrentRow.Cells["BasimYili"].Value);
                }
            };

            int x = 30, y = 420;
            int padding = 30;

            Label lblBaslik = new Label()
            {
                Text = "Yeni Kitap Ekle",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(x, y),
                AutoSize = true
            };
            this.Controls.Add(lblBaslik);

            string[] etiketler = { "Kitap Adı", "Yazar", "Dil", "Yayınevi", "Tür", "Sayfa Sayısı", "Basım Yılı" };
            kutular = new Control[etiketler.Length];

            for (int i = 0; i < etiketler.Length; i++)
            {
                Label lblEtiket = new Label()
                {
                    Text = etiketler[i] + ":",
                    Location = new Point(x, y + 30 + i * padding),
                    AutoSize = true
                };
                this.Controls.Add(lblEtiket);

                Control txt;
                if (etiketler[i].Contains("Sayfa") || etiketler[i].Contains("Yılı"))
                {
                    txt = new NumericUpDown()
                    {
                        Width = 150,
                        Location = new Point(x + 120, y + 30 + i * padding),
                        Maximum = 9999,
                        Minimum = 1
                    };
                }
                else
                {
                    txt = new TextBox()
                    {
                        Width = 150,
                        Location = new Point(x + 120, y + 30 + i * padding)
                    };
                }

                kutular[i] = txt;
                this.Controls.Add(txt);
            }

            Button btnEkle = new Button()
            {
                Text = "EKLE",
                Location = new Point(x + 40, y + 30 + etiketler.Length * padding + 10),
                Size = new Size(100, 35),
                BackColor = Color.FromArgb(232, 204, 255),
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnEkle.Click += (s, e) =>
            {
                string kitapAdi = ((TextBox)kutular[0]).Text.Trim();
                string yazar = ((TextBox)kutular[1]).Text.Trim();
                string dil = ((TextBox)kutular[2]).Text.Trim();
                string yayinevi = ((TextBox)kutular[3]).Text.Trim();
                string tur = ((TextBox)kutular[4]).Text.Trim();
                int sayfa = (int)((NumericUpDown)kutular[5]).Value;
                int yil = (int)((NumericUpDown)kutular[6]).Value;

                if (kitapAdi == "" || yazar == "")
                {
                    MessageBox.Show("Kitap adı ve yazar boş olamaz.");
                    return;
                }

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    try
                    {
                        conn.Open();
                        SqlCommand cmd = new SqlCommand(
                            "INSERT INTO Kitaplar (KitapAdi, Yazar, Dil, YayinEvi, Tur, SayfaSayisi, BasimYili) VALUES (@a,@b,@c,@d,@e,@f,@g)", conn);
                        cmd.Parameters.AddWithValue("@a", kitapAdi);
                        cmd.Parameters.AddWithValue("@b", yazar);
                        cmd.Parameters.AddWithValue("@c", dil);
                        cmd.Parameters.AddWithValue("@d", yayinevi);
                        cmd.Parameters.AddWithValue("@e", tur);
                        cmd.Parameters.AddWithValue("@f", sayfa);
                        cmd.Parameters.AddWithValue("@g", yil);
                        cmd.ExecuteNonQuery();

                        MessageBox.Show("Kitap başarıyla eklendi!");

                        foreach (Control c in kutular)
                        {
                            if (c is TextBox tb) tb.Text = "";
                            if (c is NumericUpDown nud) nud.Value = 1;
                        }

                        LoadKitaplar(dataGridView);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("HATA: " + ex.Message);
                    }
                }
            };
            this.Controls.Add(btnEkle);

            Button btnSil = new Button()
            {
                Text = "SİL",
                Location = new Point(x + 160, btnEkle.Location.Y),
                Size = new Size(100, 35),
                BackColor = Color.FromArgb(255, 204, 204),
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnSil.Click += (s, e) =>
            {
                if (dataGridView.SelectedRows.Count > 0)
                {
                    int kitapId = Convert.ToInt32(dataGridView.SelectedRows[0].Cells["Id"].Value);
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        try
                        {
                            conn.Open();
                            SqlCommand cmd = new SqlCommand("DELETE FROM Kitaplar WHERE Id = @id", conn);
                            cmd.Parameters.AddWithValue("@id", kitapId);
                            cmd.ExecuteNonQuery();
                            MessageBox.Show("Kitap silindi!");
                            LoadKitaplar(dataGridView);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("HATA: " + ex.Message);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Lütfen silinecek kitabı seçin.");
                }
            };
            this.Controls.Add(btnSil);

            Button btnGuncelle = new Button()
            {
                Text = "GÜNCELLE",
                Location = new Point(x + 280, btnEkle.Location.Y),
                Size = new Size(100, 35),
                BackColor = Color.FromArgb(204, 255, 229),
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnGuncelle.Click += (s, e) =>
            {
                if (dataGridView.SelectedRows.Count > 0)
                {
                    int kitapId = Convert.ToInt32(dataGridView.SelectedRows[0].Cells["Id"].Value);

                    string kitapAdi = ((TextBox)kutular[0]).Text.Trim();
                    string yazar = ((TextBox)kutular[1]).Text.Trim();
                    string dil = ((TextBox)kutular[2]).Text.Trim();
                    string yayinevi = ((TextBox)kutular[3]).Text.Trim();
                    string tur = ((TextBox)kutular[4]).Text.Trim();
                    int sayfa = (int)((NumericUpDown)kutular[5]).Value;
                    int yil = (int)((NumericUpDown)kutular[6]).Value;

                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        try
                        {
                            conn.Open();
                            SqlCommand cmd = new SqlCommand("UPDATE Kitaplar SET KitapAdi=@a, Yazar=@b, Dil=@c, YayinEvi=@d, Tur=@e, SayfaSayisi=@f, BasimYili=@g WHERE Id=@id", conn);
                            cmd.Parameters.AddWithValue("@a", kitapAdi);
                            cmd.Parameters.AddWithValue("@b", yazar);
                            cmd.Parameters.AddWithValue("@c", dil);
                            cmd.Parameters.AddWithValue("@d", yayinevi);
                            cmd.Parameters.AddWithValue("@e", tur);
                            cmd.Parameters.AddWithValue("@f", sayfa);
                            cmd.Parameters.AddWithValue("@g", yil);
                            cmd.Parameters.AddWithValue("@id", kitapId);
                            cmd.ExecuteNonQuery();
                            MessageBox.Show("Kitap başarıyla güncellendi!");
                            LoadKitaplar(dataGridView);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("HATA: " + ex.Message);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Lütfen güncellenecek kitabı seçin.");
                }
            };
            this.Controls.Add(btnGuncelle);

            Button btnYenile = new Button()
            {
                Text = "YENİLE",
                Location = new Point(x + 400, btnEkle.Location.Y),
                Size = new Size(100, 35),
                BackColor = Color.FromArgb(189, 215, 238),
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnYenile.Click += (s, e) => LoadKitaplar(dataGridView);
            this.Controls.Add(btnYenile);

            Button btnKullanicilar = new Button()
            {
                Text = "KULLANICILARI GÖRÜNTÜLE",
                Location = new Point(x + 520, btnEkle.Location.Y),
                Size = new Size(190, 35),
                BackColor = Color.FromArgb(255, 250, 205),
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            btnKullanicilar.Click += (s, e) => new FormKullanicilar().Show();
            this.Controls.Add(btnKullanicilar);

            Label spacer = new Label()
            {
                Size = new Size(1, 50),
                Location = new Point(0, btnEkle.Location.Y + btnEkle.Height + 10)
            };
            this.Controls.Add(spacer);

            this.FormClosed += (s, e) => Application.Exit();

            Button btnCikis = new Button()
            {
                Text = "Çıkış Yap",
                Size = new Size(100, 30),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                BackColor = Color.FromArgb(255, 230, 230),
                ForeColor = Color.Black,
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                FlatStyle = FlatStyle.Flat
            };
            btnCikis.FlatAppearance.BorderColor = Color.Gray;
            btnCikis.FlatAppearance.BorderSize = 1;

            btnCikis.Location = new Point(this.ClientSize.Width - btnCikis.Width - 20, this.ClientSize.Height - btnCikis.Height - 20);

            btnCikis.Click += (s, e) =>
            {
                this.Hide();
                Form1 giris = new Form1();
                giris.Show();
            };

            this.Controls.Add(btnCikis);
        }

        private void LoadKitaplar(DataGridView grid)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Kitaplar", conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    grid.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("HATA: " + ex.Message);
                }
            }
        }
    }
}