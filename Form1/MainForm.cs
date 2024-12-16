using Form1; // Tham chiếu đến lớp Form1
using System; // Thư viện hệ thống cơ bản
using System.Media; // Thư viện xử lý âm thanh
using System.Windows.Forms; // Thư viện các điều khiển Windows Forms

namespace TrashSort
{
    public partial class MainForm : Form
    {

        // Hàm khởi tạo form chính
        public MainForm()
        {
            InitializeComponent(); // Khởi tạo các thành phần trên form
            InitializeCustomControls(); // Khởi tạo các điều khiển tùy chỉnh
        }

        // Hàm khởi tạo các điều khiển tùy chỉnh
        private void InitializeCustomControls()
        {

            // Đặt các thuộc tính cho Form chính
            this.Text = "TrashSort"; // tiêu đề cửa sổ
            this.Size = new System.Drawing.Size(800, 600); // kích thước cửa sổ
            this.BackColor = System.Drawing.Color.White; // màu nền cửa sổ

            // Đặt hình nền cho cửa sổ
            this.BackgroundImage = Form1.Properties.Resources.background;  // cài đặt hình ảnh trong tài nguyên có tên là background làm hình nền cho cửa sổ
            this.BackgroundImageLayout = ImageLayout.Stretch; // Căn chỉnh hình nền theo cách kéo giãn

            // Tạo nhãn tiêu đề
            Label titleLabel = new Label
            {
                Text = "TrashSort", //tên hiển thị
                Font = new System.Drawing.Font("Paytone One", 36, System.Drawing.FontStyle.Bold), // cỡ chữ, kiểu chữ
                ForeColor = System.Drawing.Color.Green, // màu chữ
                AutoSize = true, // tự động điều khiển khích thước nhãn
                BackColor = System.Drawing.Color.Transparent // cài đặt nền trong suốt
            };
            this.Controls.Add(titleLabel); //thêm nhãn vào form

            // "Vào Game" Button
            Button btnVaoGame = new Button
            {
                Text = "Vào Game", // nội dung hiển thị trong button 
                Font = new System.Drawing.Font("Arial", 14, System.Drawing.FontStyle.Regular), // cỡ chữ kiểu chữ
                Size = new System.Drawing.Size(200, 50), // kích thước button
                BackColor = System.Drawing.Color.LightGreen // màu nền của button
            };
            btnVaoGame.Click += BtnVaoGame_Click; // sự kiện click button
            this.Controls.Add(btnVaoGame); // thêm button vào form

            // "Luật Game" Button
            Button btnLuatGame = new Button
            {
                Text = "Luật Game",
                Font = new System.Drawing.Font("Arial", 14, System.Drawing.FontStyle.Regular),
                Size = new System.Drawing.Size(200, 50),
                //Location = new System.Drawing.Point(300, 300),
                BackColor = System.Drawing.Color.LightYellow
            };
            btnLuatGame.Click += BtnLuatGame_Click;
            this.Controls.Add(btnLuatGame);

            // "Credits" Button
            Button btnCredits = new Button
            {
                Text = "Credits",
                Font = new System.Drawing.Font("Arial", 14, System.Drawing.FontStyle.Regular),
                Size = new System.Drawing.Size(200, 50),
                //Location = new System.Drawing.Point(300, 400),
                BackColor = System.Drawing.Color.LightBlue
            };
            btnCredits.Click += BtnCredits_Click;
            this.Controls.Add(btnCredits);

            // đính kèm sự kiện thay đổi kích thước
            this.Resize += (sender, e) => CenterControls(titleLabel, btnVaoGame, btnLuatGame, btnCredits);

            // gọi phương thức CenterControls và truyền vào các đối số để căn giữa
            CenterControls(titleLabel, btnVaoGame, btnLuatGame, btnCredits);
        }



        // Hàm căn chỉnh các điều khiển trong form
        private void CenterControls(Label titleLabel, Button btnVaoGame, Button btnLuatGame, Button btnCredits)
        {
            // Căn giữa nhãn tiêu đề
            titleLabel.Location = new System.Drawing.Point((this.ClientSize.Width - titleLabel.Width) / 2, 50);

            // Căn giữa 
            int buttonSpacing = 20; // khoảng cách giữa các nút
            int totalHeight = (btnVaoGame.Height + buttonSpacing) * 3 - buttonSpacing; // tổng chiều cao của các nút và khoảng cách giữa các nút
            int startY = (this.ClientSize.Height - totalHeight) / 2; // vị trí bắt đầu theo chiều dọc cho nút đầu tiên


            btnVaoGame.Location = new System.Drawing.Point((this.ClientSize.Width - btnVaoGame.Width) / 2, startY);
            btnLuatGame.Location = new System.Drawing.Point((this.ClientSize.Width - btnLuatGame.Width) / 2, startY + btnVaoGame.Height + buttonSpacing);
            btnCredits.Location = new System.Drawing.Point((this.ClientSize.Width - btnCredits.Width) / 2, startY + (btnVaoGame.Height + buttonSpacing) * 2);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        // Hàm xử lý sự kiện khi nút "Luật Game" được nhấn
        private void BtnVaoGame_Click(object sender, EventArgs e)
        {
            Form1.Form1 gameForm = new Form1.Form1(); // Giả sử FormGame được triển khai như đã chỉ định
            this.Hide(); // Ẩn form hiện tại
            gameForm.ShowDialog(); // Hiển thị gameForm dưới dạng hộp thoại

        }


        // Hàm xử lý sự kiện khi nút "Luật Game" được nhấn
        private void BtnLuatGame_Click(object sender, EventArgs e)
        {
            // Hiển thị hộp thoại thông báo luật game
            MessageBox.Show("Chào mừng bạn đến với trò TrashSort! \n \n Vòng 1: \"Lớp Mầm\" có tổng cộng 10 câu hỏi về phân loại rác, trả lời đúng ít nhất 5 câu (50 điểm) sẽ bước tiếp vào vòng 2 \n \n" +
                "Vòng 2: \"Lớp Chồi\" người chơi sẽ tiến hành phân loại rác vào 3 thùng theo quy chuẩn của UEH, đạt ít nhất 60 điểm sẽ bước tiếp vào vòng 3. \n \n" +
                "Phím A để di chuyển qua trái, phím D di chuyển qua phải và phím S để bỏ rác vào thùng \n \n" +
                "Vòng 3: \"Lớp Lá\" người chơi sẽ tiến hành phân loại rác vào 7 thùng theo quy chuẩn của UEH, đạt ít nhất 70 điểm sẽ thắng game", "Luật Game", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }


        // Hàm xử lý sự kiện khi nút "Credits" được nhấn
        private void BtnCredits_Click(object sender, EventArgs e)
        {
            // Hiển thị hộp thoại thông tin về credits (tác giả)
            MessageBox.Show("TrashSort Game\nCreated by Thy, Vân, Vy", "Credits", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
