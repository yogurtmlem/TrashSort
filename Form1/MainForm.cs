using Form1;
using System;
using System.Windows.Forms;

namespace TrashSort
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            InitializeCustomControls();
        }

        private void InitializeCustomControls()
        {
            // Set Main Form properties
            this.Text = "TrashSort";
            this.Size = new System.Drawing.Size(800, 600);
            this.BackColor = System.Drawing.Color.White;

            // Set the background image
            this.BackgroundImage = Form1.Properties.Resources.background;  // Ensure you have an image named "BackgroundImage" in your resources
            this.BackgroundImageLayout = ImageLayout.Stretch; // Optionally, use Stretch, Center, etc.

            // Title Label
            Label titleLabel = new Label
            {
                Text = "TrashSort",
                Font = new System.Drawing.Font("Paytone One", 36, System.Drawing.FontStyle.Bold),
                ForeColor = System.Drawing.Color.Green,
                AutoSize = true,
                Location = new System.Drawing.Point(250, 50),
                BackColor = System.Drawing.Color.Transparent // Ensure the label's background is transparent
            };
            this.Controls.Add(titleLabel);

            // "Vào Game" Button
            Button btnVaoGame = new Button
            {
                Text = "Vào Game",
                Font = new System.Drawing.Font("Arial", 14, System.Drawing.FontStyle.Regular),
                Size = new System.Drawing.Size(200, 50),
                Location = new System.Drawing.Point(300, 200),
                BackColor = System.Drawing.Color.LightGreen
            };
            btnVaoGame.Click += BtnVaoGame_Click;
            this.Controls.Add(btnVaoGame);

            // "Luật Game" Button
            Button btnLuatGame = new Button
            {
                Text = "Luật Game",
                Font = new System.Drawing.Font("Arial", 14, System.Drawing.FontStyle.Regular),
                Size = new System.Drawing.Size(200, 50),
                Location = new System.Drawing.Point(300, 300),
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
                Location = new System.Drawing.Point(300, 400),
                BackColor = System.Drawing.Color.LightBlue
            };
            btnCredits.Click += BtnCredits_Click;
            this.Controls.Add(btnCredits);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // Your code to handle the form's load event here (if needed).
        }


        private void BtnVaoGame_Click(object sender, EventArgs e)
        {
            FormGame gameForm = new FormGame(); // Assuming FormGame is implemented as provided.
            this.Hide();
            gameForm.ShowDialog();
            
        }

        private void BtnLuatGame_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Chào mừng bạn đến với trò TrashSort! \n \n Vòng 1: \"Lớp Mầm\" có tổng cộng 5 câu hỏi về phân loại rác, trả lời đúng ít nhất 3 câu (30 điểm) sẽ bước tiếp vào vòng 2 \n \n" +
                "Vòng 2: \"Lớp Chồi\" người chơi sẽ tiến hành phân loại rác vào 3 thùng theo quy chuẩn của UEH, đạt ít nhất 60 điểm sẽ bước tiếp vào vòng 3. \n \n" +
                "Phím A để di chuyển qua trái, phím D di chuyển qua phải và phím S để bỏ rác vào thùng \n \n" +
                "Vòng 3: \"Lớp Lá\" người chơi sẽ tiến hành phân loại rác vào 7 thùng theo quy chuẩn của UEH, đạt ít nhất 70 điểm sẽ thắng game" , "Luật Game", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnCredits_Click(object sender, EventArgs e)
        {
            MessageBox.Show("TrashSort Game\nCreated by Thy, Vân, Vy", "Credits", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
