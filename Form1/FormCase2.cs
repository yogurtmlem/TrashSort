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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace Form1
{
    public partial class FormCase2 : Form
    {
        private Timer gameTimer;// Bộ đếm thời gian để quản lý thời gian chơi game.
        private PictureBox trash;// Hiển thị hình ảnh rác trên màn hình bằng PictureBox.
        private int score = 0;// Điểm số hiện tại của người chơi.
        private int highScore = 0;// Điểm số cao nhất được lưu từ trước đó.
        private int timeLeft = 45;// Thời gian còn lại của trò chơi, tính bằng giây.
        private string[] trashTypes = { "chatlong", "kimloai", "thucphamthua", "nhuataiche", "giay", "hopsua", "racthaiconlai" };// Danh sách các loại rác có thể xuất hiện trong trò chơi.
        private Random random;// Sinh số ngẫu nhiên để chọn vị trí và loại rác.
        private string correctBin;// Thùng rác đúng cần phân loại cho rác hiện tại.

        // Lớp TrashItem để quản lý tên rác và thùng chứa
        public class TrashItem
        {
            public string Name { get; set; }
            public string Bin { get; set; }

            public TrashItem(string name, string bin)
            {
                Name = name;
                Bin = bin;
            }
        }

        // Mảng đa chiều để lưu các loại rác và thùng rác tương ứng
        private TrashItem[,] trashItems = new TrashItem[,]
        {
        { new TrashItem("tao", "thucphamthua"), new TrashItem("xuongca", "thucphamthua") },
        { new TrashItem("banhmi", "thucphamthua"), new TrashItem("huuco3", "thucphamthua") },
        { new TrashItem("huuco4", "thucphamthua"), new TrashItem("huuco5", "thucphamthua") },
        { new TrashItem("huuco6", "thucphamthua"), new TrashItem("giaybao", "giay") },
        { new TrashItem("giay1", "giay"), new TrashItem("giay2", "giay") },

        { new TrashItem("coca", "kimloai"), new TrashItem("kimloai2", "kimloai") },
        { new TrashItem("kimloai5", "kimloai"), new TrashItem("kimloai6", "kimloai") },
        { new TrashItem("kimloai7", "kimloai"), new TrashItem("kimloai8", "kimloai") },

        { new TrashItem("nhua1", "nhuataiche"), new TrashItem("nhua2", "nhuataiche") },
        { new TrashItem("nhua3", "nhuataiche"), new TrashItem("binhnuoc", "nhuataiche") },

        { new TrashItem("chatlong1", "chatlong"), new TrashItem("chatlong2", "chatlong") },
        { new TrashItem("chatlong3", "chatlong"), new TrashItem("hop1", "hopsua") },
        { new TrashItem("hop2", "hopsua"), new TrashItem("hop3", "hopsua") },
        { new TrashItem("hop4", "hopsua"), new TrashItem("tuinilong", "racthaiconlai") },
        { new TrashItem("giayan", "racthaiconlai"), new TrashItem("muongnia", "racthaiconlai") },
        { new TrashItem("hopnhua", "racthaiconlai"), new TrashItem("khautrang", "racthaiconlai") },
        { new TrashItem("conlai3", "racthaiconlai"), new TrashItem("conlai5", "racthaiconlai") },
        { new TrashItem("conlai8", "racthaiconlai"), null } // Phần tử cuối cùng có thể để null nếu không có rác nào đi kèm
        };

        public FormCase2() //Khởi tạo trò chơi và bộ đếm thời gian.
        {
            InitializeComponent();
            LoadHighScore();// Tải điểm cao từ file lưu trữ.
            InitializeGame();// Khởi tạo trò chơi.

            gameTimer = new Timer { Interval = 1000 };// Khởi tạo Timer với thời gian lặp mỗi giây.
            gameTimer.Tick += GameTimer_Tick;
            gameTimer.Start();// Bắt đầu Timer.
        }

        private void InitializeGame()
        {
            // Show the "Hello!" message box when the game starts
            MessageBox.Show("Hello!");

            this.Text = "Lớp Chồi - Vòng 3"; // Tiêu đề của cửa sổ.
            this.Size = new Size(1800, 1000); // Kích thước cửa sổ.

            this.FormBorderStyle = FormBorderStyle.Sizable; // Hiện lên thanh tiêu đề của cửa sổ
            this.StartPosition = FormStartPosition.CenterScreen; // Căn cửa sổ vào giữa màn hình

            this.BackgroundImage = Properties.Resources.background2; // Đặt hình nền cho form.
            this.BackgroundImageLayout = ImageLayout.Stretch;

            random = new Random();// Khởi tạo đối tượng Random để tạo số ngẫu nhiên.

            // Tạo các nhãn (Labels) cho thời gian, điểm và điểm cao.
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
            lblHighScore.Name = "lblHighScore"; 
            this.Controls.Add(lblHighScore);

            // Tạo các thùng rác.
            CreateTrashBin("chatlong", Properties.Resources.thungracchatlong, new Point(30, 600));
            CreateTrashBin("thucphamthua", Properties.Resources.thungrachuuco, new Point(230, 600));
            CreateTrashBin("kimloai", Properties.Resources.thungrackimloai, new Point(430, 600));
            CreateTrashBin("nhuataiche", Properties.Resources.thungracnhua, new Point(630, 600));
            CreateTrashBin("giay", Properties.Resources.thungracgiay, new Point(830, 600));
            CreateTrashBin("hopsua", Properties.Resources.thungrachopsua, new Point(1030, 600));
            CreateTrashBin("racthaiconlai", Properties.Resources.thungracconlai, new Point(1230, 600));

            trash = new PictureBox
            {
                Size = new Size(50, 50),
                Location = new Point(random.Next(100, 600), 50),
            };
            this.Controls.Add(trash);

            AssignNewTrash();

            this.KeyDown += MainForm_KeyDown;

            // Hiển thị hộp thoại cung cấp cho người chơi kiến thức phân loại rác
            MessageBox.Show
             (
                "Chất lỏng: là chất lỏng hữu cơ không cặn và tinh hòa, tan cao như nước, sữa, cà phê, nước ngọt, nước trái cây, nước tương,... \n\n" +
                "Thực phẩm thừa: là những thực phẩm đã qua chế biến, chất lỏng hữu cơ có cặn, rác sân vườn.\n\n" +
                "Kim loại: bao gồm hộp thực phẩm, nước ngọt làm bằng kim loại và các phế phẩm kim loại khác, đinh, ốc\n\n" +
                "Nhựa tái chế: gồm có chai, lọ, hộp nhựa, ống hút nhựa, muỗng nĩa ăn bằng nhựa\n\n" +
                "Giấy: giấy báo, giấy vở, sách, bìa carton, túi giấy, ly giấy, tạp chí\n\n" +
                "Hộp sữa: bao gồm những loại hộp sữa giấy\n\n" +
                "Rác thải còn lại: túi nilon, hộp xốp, dụng cụ ăn uống, gỗ, khăn giấy, khẩu trang, chai lọ, mảnh vỡ thủy tinh và thiết bị điện tử", "Kiến thức cần nhớ cho vòng 3\n\n"
             );

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
                Size = new Size(200, 300),// Kích thước thùng rác.
                Location = location,// Vị trí thùng rác.
                Image = ResizeImageMaintainAspect(image, 200, 300), // Chỉnh kích cỡ ảnh thùng rác giữ nguyên tỷ lệ.
                SizeMode = PictureBoxSizeMode.StretchImage,
                Tag = name, // Gán tên thùng rác để phân biệt.
                BackColor = Color.Transparent
            };

            this.Controls.Add(bin); // Thêm thùng rác vào form.
        }

        private void AssignNewTrash()
        {
            // Lấy một chỉ số ngẫu nhiên từ mảng đa chiều
            int row = random.Next(0, trashItems.GetLength(0)); // Chọn hàng ngẫu nhiên
            int col = random.Next(0, trashItems.GetLength(1)); // Chọn cột ngẫu nhiên

            TrashItem randomTrash = trashItems[row, col]; // Lấy rác ngẫu nhiên

            // Gán loại thùng chứa đúng cho loại rác này
            correctBin = randomTrash.Bin;

            // Cập nhật hình ảnh rác
            Image trashImage = (Image)Properties.Resources.ResourceManager.GetObject(randomTrash.Name);
            if (trashImage != null)
            {
                trash.Image = ResizeImageMaintainAspect(trashImage, 100, 100);
                trash.Size = new Size(100, 100);
                trash.SizeMode = PictureBoxSizeMode.StretchImage;
                trash.BackColor = Color.Transparent;
            }
            else
            {
                MessageBox.Show($"Trash image not found for: {randomTrash.Name}");
                return;
            }

            trash.Left = random.Next(100, this.Width - trash.Width);
            trash.Top = 50;
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.A && trash.Left > 0) // Di chuyển rác sang trái.
            {
                trash.Left -= 40;
            }
            else if (e.KeyCode == Keys.D && trash.Right < this.Width) // Di chuyển rác sang phải.
            {
                trash.Left += 40;
            }
            else if (e.KeyCode == Keys.S) // Khi nhấn S, rác rơi xuống.
            {
                trash.Top += 40;
                if (trash.Bounds.Bottom >= this.Height - 200) // Khi rác gần thùng.
                {
                    foreach (Control ctrl in this.Controls)
                    {
                        if (ctrl is PictureBox bin && trash.Bounds.IntersectsWith(bin.Bounds)) // Kiểm tra xem rác có chạm thùng nào không.
                        {
                            if (bin.Tag != null) // Nếu thùng có Tag.
                            {
                                CheckCorrectBin(bin.Tag.ToString()); // Kiểm tra xem rác có vào đúng thùng không.
                            }
                            else
                            {
                                MessageBox.Show("Error: Tag not set for a bin PictureBox."); // Thông báo lỗi nếu thùng không có Tag.
                            }
                            break;
                        }
                    }
                    trash.Top = 50; // Đưa rác trở lại vị trí ban đầu.
                    trash.Left = random.Next(100, 600); // Đặt vị trí ngẫu nhiên cho rác.
                    AssignNewTrash(); // Gán loại rác mới.
                }
            }
        }

        private void LoadHighScore() //Tải điểm cao từ file (nếu có). Nếu file không tồn tại, tạo file mới và đặt điểm cao là 0.
        {
            try
            {
                if (File.Exists("highscore_case2.txt"))
                {
                    highScore = int.Parse(File.ReadAllText("highscore_case2.txt"));
                }
                else
                {
                    //Tạo file nếu file chưa tồn tại 
                    File.WriteAllText("highscore_case2.txt", "0");
                    highScore = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not read high scores: " + ex);
                highScore = 0;
            }
        }

        private void SaveHighScore() //Lưu điểm cao vào file.
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "highscore_case2.txt");

            try
            {
                highScore = score;
                File.WriteAllText(filePath, highScore.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving high score: {ex.Message}");
            }
        }

        private void ResetGame()
        {
            score = 0;
            timeLeft = 45;
            AssignNewTrash();
            gameTimer.Start();
        }

        private async void CheckCorrectBin(string bin) //Kiểm tra xem rác có vào đúng thùng hay không, và cập nhật điểm số.
        {
            PictureBox binControl = this.Controls.OfType<PictureBox>().FirstOrDefault(p => p.Tag.ToString() == bin);
            if (binControl != null)
            {
                if (string.Equals(bin, correctBin, StringComparison.OrdinalIgnoreCase)) // Kiểm tra nếu thùng đúng.
                {
                    score += 10;
                    this.Controls["lblScore"].Text = $"Score: {score}";
                    binControl.BackColor = Color.LightGreen;  // Thùng có màu xanh nếu đúng.
                }
                else
                {
                    binControl.BackColor = Color.IndianRed; // Màu đỏ nếu sai.
                }

                // Đợi 500ms để người chơi nhìn thấy phản hồi.
                await Task.Delay(500);

                // Đặt lại màu nền của thùng.
                binControl.BackColor = Color.Transparent;
            }
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            if (timeLeft <= 0)
            {
                gameTimer.Stop();

                // Kiểm tra nếu điểm cao hơn điểm cao trước.
                if (score > highScore)
                {
                    highScore = score;  // Cập nhật điểm cao.
                    SaveHighScore();    
                }

                // Update the high score label
                this.Controls["lblHighScore"].Text = $"High Score: {highScore}";


                // Close the form (exit the game window)
                if (score >= 90)
                {
                    MessageBox.Show($"Time's up! Your score is {score}");
                    this.Close(); // Đóng cửa sổ khi trò chơi kết thúc.
                }
                else
                {
                    MessageBox.Show("Score below 90. Restarting Round 2.");
                    RestartGame(); // Khởi động lại trò chơi nếu điểm dưới 90.
                }
            }
            else
            {
                timeLeft--; // Giảm thời gian mỗi giây.
                this.Controls["lblTime"].Text = $"Time: {timeLeft}s"; // Cập nhật hiển thị thời gian.
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



