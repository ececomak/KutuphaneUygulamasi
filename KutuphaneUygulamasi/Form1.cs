using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace KutuphaneUygulamasi
{
    public partial class Form1 : Form
    {
        private Label lblKullaniciAdi, lblSifre, lblMesaj;
        private TextBox txtKullaniciAdi, txtSifre;
        private Button btnGiris;
        private PictureBox picture;

        public Form1()
        {
            this.Text = "Kütüphane Giriş";
            this.Size = new Size(450, 400);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(250, 245, 255); // pastel mor ton
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.Font = new Font("Segoe UI", 11);

            lblKullaniciAdi = new Label()
            {
                Text = "Kullanıcı Adı:",
                Location = new Point(70, 120),
                AutoSize = true,
                ForeColor = Color.FromArgb(60, 60, 60)
            };

            txtKullaniciAdi = new TextBox()
            {
                Location = new Point(180, 115),
                Width = 180,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };

            lblSifre = new Label()
            {
                Text = "Şifre:",
                Location = new Point(70, 170),
                AutoSize = true,
                ForeColor = Color.FromArgb(60, 60, 60)
            };

            txtSifre = new TextBox()
            {
                Location = new Point(180, 165),
                Width = 180,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                UseSystemPasswordChar = true
            };

            btnGiris = new Button()
            {
                Text = "GİRİŞ",
                Location = new Point(180, 220),
                Width = 180,
                Height = 40,
                BackColor = Color.FromArgb(200, 180, 255),
                FlatStyle = FlatStyle.Flat
            };
            btnGiris.FlatAppearance.BorderSize = 0;
            btnGiris.MouseEnter += (s, e) => btnGiris.BackColor = Color.FromArgb(180, 160, 240);
            btnGiris.MouseLeave += (s, e) => btnGiris.BackColor = Color.FromArgb(200, 180, 255);
            btnGiris.Click += BtnGiris_Click;

            lblMesaj = new Label()
            {
                Location = new Point(180, 275),
                ForeColor = Color.DarkRed,
                AutoSize = true
            };

            this.Controls.Add(picture);
            this.Controls.Add(lblKullaniciAdi);
            this.Controls.Add(txtKullaniciAdi);
            this.Controls.Add(lblSifre);
            this.Controls.Add(txtSifre);
            this.Controls.Add(btnGiris);
            this.Controls.Add(lblMesaj);
        }

        private void BtnGiris_Click(object sender, EventArgs e)
        {
            string kullaniciAdi = txtKullaniciAdi.Text.Trim();
            string sifre = txtSifre.Text.Trim();

            string connectionString = @"Server=LAPTOP-VO8CPHGU\SQLEXPRESS;Database=KutuphaneDB;Trusted_Connection=True;";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT Yetki FROM Kullanicilar WHERE KullaniciAdi=@adi AND Sifre=@sifre";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@adi", kullaniciAdi);
                    cmd.Parameters.AddWithValue("@sifre", sifre);

                    var yetki = cmd.ExecuteScalar();

                    if (yetki != null)
                    {
                        string rol = yetki.ToString().ToLower();

                        if (rol == "admin")
                        {
                            FormAdmin adminForm = new FormAdmin();
                            adminForm.Show();
                            this.Hide();
                        }
                        else
                        {
                            FormUye uyeForm = new FormUye();
                            uyeForm.Show();
                            this.Hide();
                        }
                    }

                    if (yetki != null)
                    {
                        string rol = yetki.ToString();
                        MessageBox.Show($"{rol.ToUpper()} olarak giriş başarılı!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        lblMesaj.Text = "Kullanıcı adı veya şifre yanlış!";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("HATA: " + ex.Message);
                }
            }
        }
    }
}