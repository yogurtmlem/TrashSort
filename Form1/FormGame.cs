
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
    // Form cho vòng 1
    public partial class FormGame : Form
    {
        private Timer gameTimer; // Bộ đếm thời gian để quản lý thời gian chơi game.
        private PictureBox trash; // Hiển thị hình ảnh rác trên màn hình bằng PictureBox.
        private int score = 0; // Điểm số hiện tại của người chơi.  
        private int highScore = 0; // Điểm số cao nhất được lưu từ trước đó.
        private int timeLeft = 30; // Thời gian còn lại của trò chơi, tính bằng giây.
        private string[] trashTypes = { "chatlong", "kimloai", "thucphamthua", "nhuataiche", "giay", "hopsua", "racthaiconlai" }; // Danh sách các loại rác có thể xuất hiện trong trò chơi.
        private Random random; // Sinh số ngẫu nhiên để chọn vị trí và loại rác.
        private string correctBin; // Thùng rác đúng cần phân loại cho rác hiện tại

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
        private TrashItem[,] trashItems = new TrashItem[,] //mảng đa chiều lưu danh sách các loại rác và thùng chứa tương ứng. Mỗi phần tử trong mảng là một đối tượng TrashItem.
        {
        { new TrashItem("tao", "thucphamthua"), new TrashItem("xuongca", "thucphamthua") },
        { new TrashItem("banhmi", "thucphamthua"), new TrashItem("huuco3", "thucphamthua") },
        { new TrashItem("huuco4", "thucphamthua"), new TrashItem("huuco5", "thucphamthua") },
        { new TrashItem("huuco6", "thucphamthua"), new TrashItem("huuco2", "thucphamthua") },

        { new TrashItem("nhua1", "nhuataiche"), new TrashItem("binhnuoc", "nhuataiche") },
        { new TrashItem("nhua2", "nhuataiche"), new TrashItem("nhua3", "nhuataiche") },

        { new TrashItem("tuinilong", "racthaiconlai"), new TrashItem("khautrang", "racthaiconlai") },
        { new TrashItem("muongnia", "racthaiconlai"), new TrashItem("hopnhua", "racthaiconlai") },
        { new TrashItem("giayan", "racthaiconlai"), new TrashItem("conlai3", "racthaiconlai") },
        { new TrashItem("conlai5", "racthaiconlai"), new TrashItem("conlai8", "racthaiconlai") },
        };

        public FormGame() //Khởi tạo trò chơi và bộ đếm thời gian. Phương thức 1
        {
            InitializeComponent();
            LoadHighScore();// Tải điểm cao từ file lưu trữ.
            InitializeGame();// Thiết lập giao diện và các thành phần của game.

            gameTimer = new Timer { Interval = 1000 };// Bộ đếm thời gian, mỗi tick là 1 giây.
            gameTimer.Tick += GameTimer_Tick;// Gán sự kiện mỗi lần tick.
            gameTimer.Start();// Bắt đầu bộ đếm thời gian.
        }

        private void InitializeGame() //Thiết lập giao diện, thêm các thành phần như thùng rác và rác. Phương thức 2
        {
            this.Text = "Lớp Mầm - Vòng 2";
            this.Size = new Size(1800, 1000);

            this.FormBorderStyle = FormBorderStyle.Sizable; // Hiện lên thanh tiêu đề của cửa sổ
            this.StartPosition = FormStartPosition.CenterScreen; // Căn cửa sổ vào giữa màn hình

            this.BackgroundImage = Properties.Resources.background;
            this.BackgroundImageLayout = ImageLayout.Stretch;

            random = new Random();

            // Nhãn cho điểm, điểm cao và bộ đếm thời gian
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

            //Khởi tạo thùng rác
            CreateTrashBin("thucphamthua", Properties.Resources.thungrachuuco, new Point(300, 600));
            CreateTrashBin("nhuataiche", Properties.Resources.thungractaiche, new Point(700, 600));
            CreateTrashBin("racthaiconlai", Properties.Resources.thungracconlai, new Point(1100, 600));

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
                "Rác tái chế: là những loại rác thải đã qua sử dụng nhưng vẫn có khả năng được thu gom, xử lý và chuyển đổi thành nguyên liệu mới hoặc sản phẩm tái chế. Thông thường rác tái chế gồm những loại giấy, nhựa và kim loại.\n\n" +
                "Rác hữu cơ: là các loại thực phẩm thức ăn thừa, có dấu hiệu hư hỏng, hết hạn sử dụng; phần thải bỏ sau khi sơ chế, chế biến như rau, củ, quả, trái cây hoặc phần bỏ đi của gia súc, gia cầm, thủy hải sản\n\n" +
                "Rác còn lại: là những loại rác không nằm trong 2 phân loại trên (không kể rác thải nguy hại) gồm có: túi nilon, hộp xốp, dụng cụ ăn uống, gỗ, khăn giấy, khẩu trang, chai lọ, mảnh vỡ thủy tinh!\n\n", "Kiến thức cần nhớ cho vòng 2"
             );
        }

        private Image ResizeImageMaintainAspect(Image img, int maxWidth, int maxHeight) //Điều chỉnh kích thước ảnh giữ nguyên tỉ lệ. Phương thức 5
        {
            int originalWidth = img.Width;
            int originalHeight = img.Height;

            // Tính toán tỉ lệ sao cho tỉ lệ ảnh gốc được tối ưu hóa
            float ratioX = (float)maxWidth / originalWidth;
            float ratioY = (float)maxHeight / originalHeight;
            float ratio = Math.Min(ratioX, ratioY); //Sử dụng System.Math để tính tỉ lệ ảnh

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

        private void CreateTrashBin(string name, Image image, Point location) //Tạo thùng rác trên giao diện. Phương thức 3
        {
            PictureBox bin = new PictureBox
            {
                Size = new Size(200, 300), // Kích thước thùng chứa
                Location = location, // Vị trí trên form
                Image = ResizeImageMaintainAspect(image, 200, 300), // Điều chỉnh kích thước hình ảnh để giữ tỷ lệ
                SizeMode = PictureBoxSizeMode.StretchImage,// Căn hình ảnh để vừa thùng
                Tag = name,// Gán thùng chứa với tên
                BackColor = Color.Transparent // Đảm bảo thùng chứa có nền trong suốt
            };

            this.Controls.Add(bin);// Thêm thùng vào form
        }

        private void AssignNewTrash() //Chọn ngẫu nhiên một loại rác và cập nhật hình ảnh. Phương thức 4
        {

            //Sử dụng vòng lặp lồng để thao tác với mảng đa chiều
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
                trash.Image = ResizeImageMaintainAspect(trashImage, 100, 100); //Chỉnh lại kích cỡ rác sao cho phù hợp
                trash.Size = new Size(100, 100);
                trash.SizeMode = PictureBoxSizeMode.StretchImage;
                trash.BackColor = Color.Transparent;
            }
            else
            {
                MessageBox.Show($"Không tìm thấy ảnh của rác: {randomTrash.Name}"); //Báo lỗi qua MessageBox, dùng System.Console 
                return;
            }

            //Đặt vị trí ngẫu nhiên cho rác
            trash.Left = random.Next(100, this.Width - trash.Width);
            trash.Top = 50;
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e) //Xử lý phím di chuyển rác. Phương thức 6
        {
            if (e.KeyCode == Keys.A && trash.Left > 0) //Phím A di chuyển qua trái
            {
                trash.Left -= 40;
            }
            else if (e.KeyCode == Keys.D && trash.Right < this.Width) //Phím D di chuyển qua phải
            {
                trash.Left += 40;
            }
            else if (e.KeyCode == Keys.S) //Phím S di chuyển đi xuống
            {
                trash.Top += 40;
                if (trash.Bounds.Bottom >= this.Height - 200) // Gần thùng rác
                {
                    foreach (Control ctrl in this.Controls) //Mảng một chiều và vòng lặp, khi người dùng nhấn S để di chuyển rác
                                                            //đoạn mã duyệt qua các thùng rác trên form, kiểm tra xem rác có rơi đúng thùng hay không.
                    {
                        if (ctrl is PictureBox bin && trash.Bounds.IntersectsWith(bin.Bounds))
                        {
                            if (bin.Tag != null) // Kiểm tra coi Thẻ có null không
                            {
                                CheckCorrectBin(bin.Tag.ToString());
                            }
                            else
                            {
                                MessageBox.Show("Lỗi: Thẻ chưa được đặt cho một hộp hình ảnh (PictureBox).");
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

        private void LoadHighScore() //Đọc điểm cao từ tệp. Phương thức 9
        {
            try //Xử lý ngoại lệ
            {
                if (File.Exists("highscore.txt"))
                {
                    highScore = int.Parse(File.ReadAllText("highscore.txt")); //Sử dụng file lưu trữ dữ liệu để đọc điểm cao
                }
                else
                {
                    //Tạo file nếu file chưa tồn tại 
                    File.WriteAllText("highscore.txt", "0");
                    highScore = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể đọc điểm cao: " + ex); //Hiện thông báo lỗi nếu xảy ra ngoại lệ
                highScore = 0;
            }
        }

        private void SaveHighScore() //Ghi điểm cao vào tệp. Phương thức 10
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "highscore.txt");

            try //Xử lý ngoại lệ
            {
                    highScore = score;
                File.WriteAllText(filePath, highScore.ToString()); //Sử dụng file lưu trữ dữ liệu để lưu điểm cao
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi xảy ra trong quá trình lưu điểm cao: {ex.Message}");
            }
        }


        private async void CheckCorrectBin(string bin) //Kiểm tra xem rác có được đặt vào đúng thùng không. Phương thức 7
        {
            PictureBox binControl = this.Controls.OfType<PictureBox>().FirstOrDefault(p => p.Tag.ToString() == bin);
            if (binControl != null)
            {
                if (string.Equals(bin, correctBin, StringComparison.OrdinalIgnoreCase)) // Kiểm tra nếu thùng đúng.
                {
                    score += 10;
                    this.Controls["lblScore"].Text = $"Score: {score}";
                    binControl.BackColor = Color.LightGreen; // Thùng có màu xanh nếu đúng.
                }
                else
                {
                    binControl.BackColor = Color.IndianRed; // Màu đỏ nếu sai.
                }
            }

            //Chờ một chút để hiển thị phản hồi.
            await Task.Delay(500);

            // Đặt lại màu viền và nền
            binControl.BackColor = Color.Transparent;
        }
        

        private void GameTimer_Tick(object sender, EventArgs e) //Xử lý logic khi hết thời gian. Phương thức 8
        {
            if (timeLeft <= 0)
            {
                gameTimer.Stop();

                // Kiểm tra xem điểm hiện tại có cao hơn điểm cao nhất không
                if (score > highScore)
                {
                    highScore = score;  // Cập nhật điểm cao
                    SaveHighScore();    // Lưu điểm cao mới nhất vào highscore.txt
                }

                // Cập nhật nhãn cho điểm cao
                this.Controls["lblHighScore"].Text = $"High Score: {highScore}";

                // Hiển thị kết quả
                if (score >= 60)
                {
                    MessageBox.Show("Làm tốt lắm! Bạn sẽ đi tiếp vào Vòng 3.");
                    FormCase2 case2Form = new FormCase2();
                    case2Form.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Điểm dưới 60. Khởi động lại Vòng 2.");
                    RestartGame();
                }
            }
            else
            {
                timeLeft--;
                this.Controls["lblTime"].Text = $"Time: {timeLeft}s";
            }
        }


        private void RestartGame() //Khởi động lại trò chơi. Phương thức 11
        {
            
            score = 0;
            timeLeft = 30;
            this.Controls["lblScore"].Text = "Score: 0";
            this.Controls["lblTime"].Text = "Time: 30s";
            gameTimer.Start();
        }

    }
}
