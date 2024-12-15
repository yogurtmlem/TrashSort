/*using System;
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

namespace Form1
{
    public partial class FormGame : Form
    {
        private Timer gameTimer;
        private PictureBox trash;
        private int score = 0;
        private int highScore = 0;
        private int timeLeft = 30;
        private Random random;
        private string[] trashTypes = { "chatlong", "kimloai", "thucphamthua", "nhuataiche", "giay", "hopsua", "racthaiconlai" };
        private string correctBin;
        private int currentRound = 1; // Biến theo dõi vòng hiện tại

        public FormGame()
        {
            InitializeComponent();
            LoadHighScore();
            InitializeGame();

            gameTimer = new Timer();
            gameTimer.Interval = 1000; // Ví dụ: mỗi giây một lần
            gameTimer.Tick += GameTimer_Tick; // Gắn sự kiện cho timer

            random = new Random(); // Khởi tạo Random
        }

        private Dictionary<string, string> trashCategoryMapping;
        private void InitializeGame()
        {
            this.Text = "Trash Sorting Game";
            this.Size = new Size(1200, 800); // Width of 1200, height of 800


            // Set the background image
            this.BackgroundImage = Properties.Resources.background2;  // Ensure you have an image named "BackgroundImage" in your resources
            this.BackgroundImageLayout = ImageLayout.Stretch; // Optionally, use Stretch, Center, etc.

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
            CreateTrashBin("thucphamthua", Properties.Resources.thungrachuuco, new Point(200, 400));
            CreateTrashBin("nhuataiche", Properties.Resources.thungractaiche, new Point(500, 400));
            CreateTrashBin("racthaiconlai", Properties.Resources.thungracconlai, new Point(800, 400));

            // Initialize the trash category mapping
            trashCategoryMapping = new Dictionary<string, string>
    {
             { "tao", "thucphamthua" },
            { "xuongca", "thucphamthua" },
            { "banhmi", "thucphamthua" },
            { "huuco3", "thucphamthua" },
            { "huuco4", "thucphamthua" },
            { "huuco5", "thucphamthua" },
            { "huuco6", "thucphamthua" },
            { "nhua1", "nhuataiche" },
            { "nhua2", "nhuataiche" },
            { "nhua3", "nhuataiche" },
            { "binhnuoc", "nhuataiche" },
            { "tuinilong", "racthaiconlai" },
            { "giayan", "racthaiconlai" },
            { "muongnia", "racthaiconlai" },
            { "hopnhua", "racthaiconlai" },
            { "khautrang", "racthaiconlai" },
            { "conlai3", "racthaiconlai" },
            { "conlai5", "racthaiconlai" },
            { "conlai8", "racthaiconlai" }
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
                BackColor = Color.Transparent // Ensure the background is transparent
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

        private void ResetGame()
        {
            score = 0; // Reset điểm số
            currentRound = 1; // Reset vòng chơi
            timeLeft = 30; // Reset thời gian
            InitializeRound(); // Gọi lại hàm khởi tạo vòng 1
            AssignNewTrash();  // Tạo lại rác mới
            gameTimer.Start();  // Khởi động lại game
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

        private bool isRoundCompleted = false;

        private void HandleRoundCompletion()
        {
            if (isRoundCompleted) return;  // Nếu vòng này đã được hoàn thành, thì không làm gì thêm.

            isRoundCompleted = true;  // Đánh dấu là vòng này đã hoàn thành.

            // Kiểm tra nếu điểm đạt yêu cầu để tiếp tục vòng tiếp theo
            if (score >= 30)
            {
                // Kiểm tra nếu đây là vòng cuối cùng
                if (currentRound == 3)
                {
                    MessageBox.Show($"Chúc mừng! Bạn đã hoàn thành tất cả các vòng!");
                    ResetGame();  // Reset trò chơi sau khi hoàn thành
                }
                else
                {
                    MessageBox.Show($"Bạn đã thắng vòng {currentRound}! Tiến đến vòng {currentRound + 1}.");
                    currentRound++;  // Tăng số vòng
                    InitializeRound();  // Khởi tạo vòng mới
                    gameTimer.Start();  // Tiếp tục đếm ngược thời gian cho vòng tiếp theo
                }
            }
            else
            {
                // Nếu điểm chưa đủ, quay lại vòng 1
                MessageBox.Show($"Điểm của bạn chưa đủ, chơi lại vòng 1.");
                currentRound = 1;  // Reset lại vòng 1
                timeLeft = 10;
                InitializeRound();  // Khởi tạo vòng 1
                gameTimer.Start();  // Bắt đầu lại thời gian cho vòng 1
            }
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            if (timeLeft <= 0)
            {
                gameTimer.Stop();  // Dừng đồng hồ khi hết thời gian
                HandleRoundCompletion();  // Gọi hàm để xử lý vòng hiện tại (hoàn thành)
            }
            else
            {
                timeLeft--;
                this.Controls["lblTime"].Text = $"Time: {timeLeft}s";  // Cập nhật thời gian
            }
        }

        private void InitializeRound()
        {
            // Đặt lại cờ khi bắt đầu vòng mới
            isRoundCompleted = false;  // Đánh dấu rằng vòng này chưa hoàn thành

            // Reset lại timeLeft cho từng vòng
            if (currentRound == 1)
            {
                timeLeft = 30;  // Đặt lại thời gian cho vòng 1 là 30s
            }
            else if (currentRound == 2)
            {
                timeLeft = 45;  // Đặt lại thời gian cho vòng 2 là 45s
            }
            else if (currentRound == 3)
            {
                timeLeft = 60;  // Đặt lại thời gian cho vòng 3 là 60s
            }

            // Tiếp theo là code để khởi tạo vòng chơi, ví dụ như tạo các thùng rác
            switch (currentRound)
            {
                case 1:
                    foreach (var control in this.Controls.OfType<PictureBox>().Where(c => c.Tag != null && c.Name != "lblScore" && c.Name != "lblTime").ToList())
                    {
                        this.Controls.Remove(control);
                    }
                    score = 0;
                    trashCategoryMapping = new Dictionary<string, string>
            {
            { "tao", "thucphamthua" },
            { "xuongca", "thucphamthua" },
            { "banhmi", "thucphamthua" },
            { "huuco3", "thucphamthua" },
            { "huuco4", "thucphamthua" },
            { "huuco5", "thucphamthua" },
            { "huuco6", "thucphamthua" },
            { "nhua1", "nhuataiche" },
            { "nhua2", "nhuataiche" },
            { "nhua3", "nhuataiche" },
            { "binhnuoc", "nhuataiche" },
            { "tuinilong", "racthaiconlai" },
            { "giayan", "racthaiconlai" },
            { "muongnia", "racthaiconlai" },
            { "hopnhua", "racthaiconlai" },
            { "khautrang", "racthaiconlai" },
            { "conlai3", "racthaiconlai" },
            { "conlai5", "racthaiconlai" },
            { "conlai8", "racthaiconlai" }
                    };
                    CreateTrashBin("thucphamthua", Properties.Resources.thungrachuuco, new Point(200, 400));
                    CreateTrashBin("nhuataiche", Properties.Resources.thungractaiche, new Point(500, 400));
                    CreateTrashBin("racthaiconlai", Properties.Resources.thungracconlai, new Point(800, 400));
                    break;

                case 2:
                    // Remove existing trash bins and trash image
                    foreach (var control in this.Controls.OfType<PictureBox>().Where(c => c.Tag != null).ToList())
                    {
                        this.Controls.Remove(control);
                    }

                    // Reset the score and category mappings for round 2
                    score = 0;
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

                    // Add new bins for Case 2
                    CreateTrashBin("chatlong", Properties.Resources.thungracchatlong, new Point(30, 400));
                    CreateTrashBin("thucphamthua", Properties.Resources.thungrachuuco, new Point(190, 400));
                    CreateTrashBin("kimloai", Properties.Resources.thungrackimloai, new Point(350, 400));
                    CreateTrashBin("nhuataiche", Properties.Resources.thungracnhua, new Point(510, 400));
                    CreateTrashBin("giay", Properties.Resources.thungracgiay, new Point(670, 400));
                    CreateTrashBin("hopsua", Properties.Resources.thungrachopsua, new Point(830, 400));
                    CreateTrashBin("racthaiconlai", Properties.Resources.thungracconlai, new Point(990, 400));

                    // Assign the first trash item for the new round
                    AssignNewTrash();
                    break;

                case 3:
                    score = 0;
                    trashCategoryMapping = new Dictionary<string, string>
            {
                { "tao", "Organic" },
            { "xuongca", "Organic" },
            { "banhmi", "Organic" },
            { "huuco3", "Organic" },
            { "huuco4", "Organic" },
            { "huuco5", "Organic" },
            { "huuco6", "Organic" },
            { "giaybao", "Inorganic" },
            { "coca", "Inorganic" },
            { "kimloai2", "Inorganic" },
            { "kimloai5", "Inorganic" },
            { "kimloai6", "Inorganic" },
            { "kimloai7", "Inorganic" },
            { "kimloai8", "Inorganic" },
            { "nhua1", "Inorganic" },
            { "nhua2", "Inorganic" },
            { "nhua3", "Inorganic" },
            { "hop1", "Inorganic" },
            { "hop2", "Inorganic" },
            { "hop3", "Inorganic" },
            { "hop4", "Inorganic" },
            { "binhnuoc", "Inorganic" },
            { "tuinilong", "Non-recyclable" },
            { "giayan", "Non-recyclable" },
            { "muongnia", "Non-recyclable" },
            { "hopnhua", "Non-recyclable" },
            { "khautrang", "Non-recyclable" }
            };
                    CreateTrashBin("chatlong", Properties.Resources.thungracchatlong, new Point(20, 250));
                    CreateTrashBin("huuco", Properties.Resources.thungrachuuco, new Point(100, 250));
                    CreateTrashBin("nhua", Properties.Resources.thungractaiche, new Point(300, 250));
                    CreateTrashBin("racconlai", Properties.Resources.thungracconlai, new Point(500, 250));
                    break;
            }
        }

        private void UpdateUI()
        {
            this.Controls["lblTime"].Text = $"Time: {timeLeft}s";
            this.Controls["lblScore"].Text = $"Score: {score}";
            this.Controls["lblRound"].Text = $"Round: {currentRound}/3";  // Thêm phần hiển thị số vòng
        }
    }
}*/

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

namespace Form1
{
    // Form for Case 1
    public partial class FormGame : Form
    {
        private Timer gameTimer;
        private PictureBox trash;
        private int score = 0;
        private int highScore = 0;
        private int timeLeft = 30;
        private string[] trashTypes = { "chatlong", "kimloai", "thucphamthua", "nhuataiche", "giay", "hopsua", "racthaiconlai" };
        private Random random;
        private string correctBin;
        private Dictionary<string, string> trashCategoryMapping;

        public FormGame()
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
            this.Text = "Trash Sorting Game - Case 1";
            this.Size = new Size(1200, 800);
            this.BackgroundImage = Properties.Resources.background;
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


            CreateTrashBin("thucphamthua", Properties.Resources.thungrachuuco, new Point(200, 400));
            CreateTrashBin("nhuataiche", Properties.Resources.thungractaiche, new Point(500, 400));
            CreateTrashBin("racthaiconlai", Properties.Resources.thungracconlai, new Point(800, 400));

            trashCategoryMapping = new Dictionary<string, string>
            {
                { "tao", "thucphamthua" },
            { "xuongca", "thucphamthua" },
            { "banhmi", "thucphamthua" },
            { "huuco3", "thucphamthua" },
            { "huuco4", "thucphamthua" },
            { "huuco5", "thucphamthua" },
            { "huuco6", "thucphamthua" },
            { "nhua1", "nhuataiche" },
            { "nhua2", "nhuataiche" },
            { "nhua3", "nhuataiche" },
            { "binhnuoc", "nhuataiche" },
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
                Size = new Size(200, 300), // Increased size for bigger trash cans
                Location = location,
                Image = ResizeImageMaintainAspect(image, 200, 300), // Adjust image size proportionally
                SizeMode = PictureBoxSizeMode.StretchImage,
                Tag = name,
                BackColor = Color.Transparent // Ensure the background is transparent
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

  

        private void ResetGame()
        {
            score = 0; // Reset điểm số
            timeLeft = 30; // Reset thời gian
            AssignNewTrash();  // Tạo lại rác mới
            gameTimer.Start();  // Khởi động lại game
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
        

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            if (timeLeft <= 0)
            {
                gameTimer.Stop();

                // Check if the current score is higher than the high score
                if (score > highScore)
                {
                    highScore = score;  // Update the high score
                    SaveHighScore();    // Save the new high score to highscore.txt
                }

                // Update the high score label
                this.Controls["lblHighScore"].Text = $"High Score: {highScore}";

                // Display results and transition logic
                if (score >= 60)
                {
                    MessageBox.Show("Well done! Moving to Case 2.");
                    FormCase2 case2Form = new FormCase2();
                    case2Form.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Score below 60. Restarting Case 1.");
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
            timeLeft = 30;
            this.Controls["lblScore"].Text = "Score: 0";
            this.Controls["lblTime"].Text = "Time: 30s";
            gameTimer.Start();
        }

    }
}
