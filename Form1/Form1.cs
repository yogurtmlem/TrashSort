using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO; // For File operations
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Form1
{
    public partial class Form1 : Form
    {
        private Timer gameTimer;
        private PictureBox trash;
        private int score = 0;
        private int highScore = 0;
        private int timeLeft = 30;
        private Random random;
        private string[] trashTypes = { "Organic", "Inorganic", "Non-recyclable" };
        private string correctBin;

        public Form1()
        {
            InitializeComponent();
            LoadHighScore();
            InitializeGame();
        }

        private Dictionary<string, string> trashCategoryMapping;
        private void InitializeGame()
        {
            this.Text = "Trash Sorting Game";
            this.Size = new Size(800, 600);
            this.BackColor = Color.White;

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
            this.Controls.Add(lblHighScore);

            // Trash bins with appropriate images
            CreateTrashBin("Organic", Properties.Resources.thucphamthua, new Point(100, 350));
            CreateTrashBin("Inorganic", Properties.Resources.ractaiche, new Point(300, 350));
            CreateTrashBin("Non-recyclable", Properties.Resources.racconlai, new Point(500, 350));

            // Initialize the trash category mapping
            trashCategoryMapping = new Dictionary<string, string>
    {
            { "tao", "Organic" },
{ "xuongca", "Organic" },
{ "banhmi", "Organic" },
{ "giaybao", "Inorganic" },
{ "coca", "Inorganic" },
{ "binhnuoc", "Inorganic" },
{ "tuinilong", "Non-recyclable" },
{ "giayan", "Non-recyclable" },
{ "muongnia", "Non-recyclable" },
{ "hopnhua", "Non-recyclable" },
{ "khautrang", "Non-recyclable" }
    };
            // Falling trash
            trash = new PictureBox
            {
                Size = new Size(50, 50),
                Location = new Point(random.Next(100, 600), 50),
            };
            this.Controls.Add(trash);

            // Game Timer
            gameTimer = new Timer
            {
                Interval = 1000
            };
            gameTimer.Tick += GameTimer_Tick;
            gameTimer.Start();

            this.KeyDown += MainForm_KeyDown;
            AssignNewTrash();
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
                Size = new Size(200, 300), // Increased size for bigger trash cans
                Location = location,
                Image = ResizeImageMaintainAspect(image, 200, 300), // Adjust image size proportionally
                SizeMode = PictureBoxSizeMode.StretchImage,
                Tag = name,
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
                trash.Left -= 20;
            }
            else if (e.KeyCode == Keys.D && trash.Right < this.Width)
            {
                trash.Left += 20;
            }
            else if (e.KeyCode == Keys.S)
            {
                trash.Top += 20;
                if (trash.Bounds.Bottom >= this.Height - 200) // Near bins
                {
                    foreach (Control ctrl in this.Controls)
                    {
                        if (ctrl is PictureBox bin && trash.Bounds.IntersectsWith(bin.Bounds))
                        {
                            CheckCorrectBin(bin.Tag.ToString());
                            break;
                        }
                    }
                    trash.Top = 50;
                    trash.Left = random.Next(100, 600);
                    AssignNewTrash();
                }
            }
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
            timeLeft--;
            this.Controls["lblTime"].Text = $"Time: {timeLeft}s";

            if (timeLeft <= 0)
            {
                gameTimer.Stop();
                MessageBox.Show($"Time's up! Final Score: {score}");
                SaveHighScore();
                this.Close();
            }
        }

        private void LoadHighScore()
        {
            try
            {
                if (File.Exists("highscore.txt"))
                {
                    highScore = int.Parse(File.ReadAllText("highscore.txt"));
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
                File.WriteAllText("highscore.txt", score.ToString());
            }
        }
    }
}