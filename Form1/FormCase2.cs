using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.IO;
using Form1.Properties;
using System.Media;

namespace Form1
{
    public partial class FormCase2 : Form
    {
        private Timer gameTimer;
        private PictureBox trash;
        private int score = 0;
        private int highScore = 0;
        private int timeLeft = 45;
        private string[] trashTypes = { "chatlong", "kimloai", "thucphamthua", "nhuataiche", "giay", "hopsua", "racthaiconlai" };
        private Random random;
        private string correctBin;
        private Dictionary<string, string> trashCategoryMapping;

        public FormCase2()
        {
            InitializeComponent();
            LoadHighScore();
            InitializeGame();

            gameTimer = new Timer { Interval = 1000 };
            gameTimer.Tick += GameTimer_Tick;
            gameTimer.Start();
        }

        private void InitializeGame()
        {
            this.Text = "Trash Sorting Game - Case 2";
            this.Size = new Size(1200, 800);
            this.BackgroundImage = Properties.Resources.background2;
            this.BackgroundImageLayout = ImageLayout.Stretch;

            random = new Random();

            // Labels for score, high score, and timer
            Label lblTime = new Label
            {
                Text = $"Time: {timeLeft}s",
                Font = new Font("Arial", 16),
                Location = new Point(20, 20),
                AutoSize = true
            };
            lblTime.Name = "lblTime";
            this.Controls.Add(lblTime);

            Label lblScore = new Label
            {
                Text = $"Score: {score}",
                Font = new Font("Arial", 16),
                Location = new Point(20, 60),
                AutoSize = true
            };
            lblScore.Name = "lblScore";
            this.Controls.Add(lblScore);

            Label lblHighScore = new Label
            {
                Text = $"High Score: {highScore}",
                Font = new Font("Arial", 16),
                Location = new Point(20, 100),
                AutoSize = true
            };
            lblHighScore.Name = "lblHighScore";  // This makes it accessible by name
            this.Controls.Add(lblHighScore);

            CreateTrashBin("chatlong", Properties.Resources.thungracchatlong, new Point(30, 400));
            CreateTrashBin("thucphamthua", Properties.Resources.thungrachuuco, new Point(190, 400));
            CreateTrashBin("kimloai", Properties.Resources.thungrackimloai, new Point(350, 400));
            CreateTrashBin("nhuataiche", Properties.Resources.thungracnhua, new Point(510, 400));
            CreateTrashBin("giay", Properties.Resources.thungracgiay, new Point(670, 400));
            CreateTrashBin("hopsua", Properties.Resources.thungrachopsua, new Point(830, 400));
            CreateTrashBin("racthaiconlai", Properties.Resources.thungracconlai, new Point(990, 400));

            trashCategoryMapping = new Dictionary<string, string>
            {
                { "tao", "thucphamthua" },
            { "xuongca", "thucphamthua" },
            { "banhmi", "thucphamthua" },
            { "huuco3", "thucphamthua" },
            { "huuco4", "thucphamthua" },
            { "huuco5", "thucphamthua" },
            { "huuco6", "thucphamthua" },
            { "giaybao", "giay" },
            { "giay1", "giay" },
            { "giay2", "giay" },
            { "coca", "kimloai" },
            { "kimloai2", "kimloai" },
            { "kimloai5", "kimloai" },
            { "kimloai6", "kimloai" },
            { "kimloai7", "kimloai" },
            { "kimloai8", "kimloai" },
            { "nhua1", "nhuataiche" },
            { "nhua2", "nhuataiche" },
            { "nhua3", "nhuataiche" },
            { "binhnuoc", "nhuataiche" },
            { "chatlong1", "chatlong" },
            { "chatlong2", "chatlong" },
            { "chatlong3", "chatlong" },
            { "hop1", "hopsua" },
            { "hop2", "hopsua" },
            { "hop3", "hopsua" },
            { "hop4", "hopsua" },
            { "tuinilong", "racthaiconlai" },
            { "giayan", "racthaiconlai" },
            { "muongnia", "racthaiconlai" },
            { "hopnhua", "racthaiconlai" },
            { "khautrang", "racthaiconlai" },
            { "conlai3", "racthaiconlai" },
            { "conlai5", "racthaiconlai" },
            { "conlai8", "racthaiconlai" }
            };

            trash = new PictureBox
            {
                Size = new Size(50, 50),
                Location = new Point(random.Next(100, 600), 50),
            };
            this.Controls.Add(trash);

            AssignNewTrash();

            this.KeyDown += MainForm_KeyDown;

        }
        private Image ResizeImageMaintainAspect(Image img, int maxWidth, int maxHeight)
        {
            int originalWidth = img.Width;
            int originalHeight = img.Height;

            // Calculate the new dimensions while maintaining the aspect ratio
            float ratioX = (float)maxWidth / originalWidth;
            float ratioY = (float)maxHeight / originalHeight;
            float ratio = Math.Min(ratioX, ratioY);

            int newWidth = (int)(originalWidth * ratio);
            int newHeight = (int)(originalHeight * ratio);

            Bitmap resizedImg = new Bitmap(newWidth, newHeight);
            using (Graphics g = Graphics.FromImage(resizedImg))
            {
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                g.DrawImage(img, 0, 0, newWidth, newHeight);
            }
            return resizedImg;
        }

        private void CreateTrashBin(string name, Image image, Point location)
        {
            PictureBox bin = new PictureBox
            {
                Size = new Size(200, 300),
                Location = location,
                Image = ResizeImageMaintainAspect(image, 200, 300),
                SizeMode = PictureBoxSizeMode.StretchImage,
                Tag = name,
                BackColor = Color.Transparent
            };
            this.Controls.Add(bin);
        }

        private void AssignNewTrash()
        {
            var randomTrash = trashCategoryMapping.ElementAt(random.Next(trashCategoryMapping.Count));
            string trashImageName = randomTrash.Key;
            correctBin = randomTrash.Value;

            Image trashImage = (Image)Properties.Resources.ResourceManager.GetObject(trashImageName);
            if (trashImage != null)
            {
                trash.Image = ResizeImageMaintainAspect(trashImage, 100, 100);
                trash.Size = new Size(100, 100);
                trash.SizeMode = PictureBoxSizeMode.StretchImage;
                trash.BackColor = Color.Transparent;
            }
            else
            {
                MessageBox.Show($"Trash image not found for: {trashImageName}");
                return;
            }

            trash.Left = random.Next(100, this.Width - trash.Width);
            trash.Top = 50;
        }
        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.A && trash.Left > 0)
            {
                trash.Left -= 40;
            }
            else if (e.KeyCode == Keys.D && trash.Right < this.Width)
            {
                trash.Left += 40;
            }
            else if (e.KeyCode == Keys.S)
            {
                trash.Top += 40;
                if (trash.Bounds.Bottom >= this.Height - 200) // Near bins
                {
                    foreach (Control ctrl in this.Controls)
                    {
                        if (ctrl is PictureBox bin && trash.Bounds.IntersectsWith(bin.Bounds))
                        {
                            if (bin.Tag != null) // Check if Tag is not null
                            {
                                CheckCorrectBin(bin.Tag.ToString());
                            }
                            else
                            {
                                MessageBox.Show("Error: Tag not set for a bin PictureBox.");
                            }
                            break;
                        }
                    }
                    trash.Top = 50;
                    trash.Left = random.Next(100, 600);
                    AssignNewTrash();
                }
            }
        }

        private void LoadHighScore()
        {
            try
            {
                if (File.Exists("highscore_case2.txt"))
                {
                    highScore = int.Parse(File.ReadAllText("highscore_case2.txt"));
                }
            }
            catch
            {
                highScore = 0;
            }
        }

        private void SaveHighScore()
        {
            if (score > highScore)
            {
                highScore = score;
                File.WriteAllText("highscore_case2.txt", highScore.ToString());
            }
        }

        private void ResetGame()
        {
            score = 0;
            timeLeft = 45;
            AssignNewTrash();
            gameTimer.Start();
        }

        private async void CheckCorrectBin(string bin)
        {
            PictureBox binControl = this.Controls.OfType<PictureBox>().FirstOrDefault(p => p.Tag.ToString() == bin);
            if (binControl != null)
            {
                if (string.Equals(bin, correctBin, StringComparison.OrdinalIgnoreCase))
                {
                    score += 10;
                    this.Controls["lblScore"].Text = $"Score: {score}";
                    binControl.BackColor = Color.LightGreen;
                }
                else
                {
                    binControl.BackColor = Color.IndianRed;
                }

                // Wait briefly to show feedback
                await Task.Delay(500);

                // Reset border and background color
                binControl.BackColor = Color.Transparent;
            }
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            if (timeLeft <= 0)
            {
                gameTimer.Stop();

                // Check if the current score is higher than the high score
                if (score > highScore)
                {
                    highScore = score;  // Update the high score
                    SaveHighScore();    // Save the new high score
                }

                // Update the high score label
                this.Controls["lblHighScore"].Text = $"High Score: {highScore}";


                // Close the form (exit the game window)
                if (score >= 70)
                {
                    MessageBox.Show($"Time's up! Your score is {score}");
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Score below 70. Restarting Round 2.");
                    RestartGame();
                }
            }
            else
            {
                timeLeft--;
                this.Controls["lblTime"].Text = $"Time: {timeLeft}s";
            }
        }
        private void RestartGame()
        {
            score = 0;
            timeLeft = 45;
            this.Controls["lblScore"].Text = "Score: 0";
            this.Controls["lblTime"].Text = "Time: 45s";
            gameTimer.Start();
        }


    }

}



