using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO; // Thư viện dùng để thực hiện các thao tác với tệp tin
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Form1
{
    public partial class Form1 : Form
    {
        private SoundPlayer backgroundMusicPlayer;
        //PHẦN QUIZ
        private Label lblTime; // Nhãn hiển thị thời gian còn lại
        private int timeLeft = 20; // Biến lưu thời gian còn lại (giây)
        private Timer gameTimer; // Đồng hồ đếm ngược
        int currentQuizQuestion = 0; // Biến đếm câu hỏi
        int quizScore = 0; // Điểm cho phần Quiz
        string[] questions = // Mảng chứa danh sách các câu hỏi
            {
          "Câu hỏi 1: Bạn đang cần đóng hàng gửi chuyển phát qua đường bưu điện. \nBạn nên chọn vật dụng nào để tái sử dụng và giảm phát thải nhất?",
         "Câu hỏi 2: Đâu là những thói quen không nên làm, vì sẽ gây lãng phí điện?",
         "Câu hỏi 3: Dùng để chế tạo túi nylon, lọ hóa chất. \nKhông được dùng trong lò vi sóng, độ bền kém",
         "Câu hỏi 4: Đố bạn loại nhựa nào có các đặc điểm sau đây: rất độc hại, rẻ tiền, \ndùng để sản xuất vật dụng đựng hóa chất hay bình đựng nước",
         "Câu hỏi 5: Rác thải điện tử là một vấn đề nghiêm trọng hiện nay. Bạn có biết \nlượng rác thải điện tử mỗi năm bị thải ra trên toàn cầu là bao nhiêu không?",
         "Câu hỏi 6: Bạn đang ở siêu thị và mua các mặt hàng sau đây (rau, cà tím, nấm, cà rốt). \nBạn hãy lựa chọn cách đựng các món hàng đã mua để giảm thiểu phát thải?",
         "Câu hỏi 7: iPhone 16 được ra mắt trong thời gian tới, bạn là người yêu \nthích công nghệ và có đủ tiền để mua, bạn sẽ làm gì?",
         "Câu hỏi 8: Bạn nên sử dụng thìa, dĩa nhựa dùng 1 lần để giảm \nphát thải trong các hoạt động tập thể nào?",
         "Câu hỏi 9: Bạn nghĩ đâu KHÔNG PHẢI là cách làm hữu hiệu nhất để \ngiảm thiểu rác thải nhựa từ vỏ chai đựng các chất tẩy rửa?",
         "Câu hỏi 10: Bạn hãy cho biết, hành động nào làm pin sạc mau hư? ",

     };
        string[,] answers = { // Mảng chứa các đáp án cho mỗi câu hỏi
         { "Mua thùng nhựa mới", "Mua thùng carton mới", "Tận dụng thùng nhựa hoặc carton cũ", "Không có đáp án đúng" },  // Đáp án cho câu 1
         { "Để tủ lạnh mở quá lâu", "Không rút sạc khi laptop và điện thoại đã được sạc đầy", "Bật quạt và đèn trong phòng trống", "Tất cả đáp án trên" }, // Đáp án cho câu 2
         { "PVC", "HDPE", "LDPE", "PET/PETE" }, // Đáp án cho câu 3
         {"Other" , "PET/PETE" , "LDPE", "HDPE"},
         {"54 triệu tấn", "24 triệu tấn", "44 triệu tấn","34 triệu tấn"},
         {"4 túi nilon, mỗi túi đựng 1 món", "1 túi nilon đựng 4 món hàng","2 túi nilon, mỗi túi đựng 2 món hàng","3 túi nilon, 1 túi đựng nắm, 1 túi đựng rau, 1 túi đựng cà tím và cà rốt"},
         {"Mua iPhone 16 và bán điện thoại cũ", "Mua iPhone 16 và dùng cả chiếc điện thoại cũ", "Tìm hiểu kỹ, nếu không có nhiều khác biệt và điện thoại hiện tại vẫn đáp ứng đủ nhu cầu của bản thân thì tiếp tục dùng điện thoại đang có","Mua iPhone 16 và cho người dân điện thoại cũ" },
         {"Không sử dụng trong bất kỳ hoạt động nào","Tiệc sinh nhật để đỡ phải dọn dẹp", "Buổi dễ ngoại ngoài trời để đỡ phải mang nặng và dọn dẹp","Câu B,C đúng" },
         {"Học cách tự làm nước tẩy rửa sinh học từ rác thải hữu cơ như từ vỏ dứa","Refill từ túi nhựa dùng 1 lần đựng hóa chất thể tích lớn","Lựa chọn các thương hiệu cho phép trả lại bao bì để sử dụng, refill" ,"Mua chai đựng hóa chất có thể tích lớn hơn" },
         {"Để pin trong máy quá lâu mà không sử dụng", "Để pin ở nơi có nhiệt độ không quá nóng","Sạc với thời gian vừa đủ","Sạc pin bằng bộ sạc tương thích" },

     };
        string[] correctAnswers = { "D", "D", "C", "A", "A", "B", "C", "A", "B", "A" }; // Mảng chứa các đáp án đúng
        private string selectedAnswer = "";  // Biến lưu đáp án đã chọn
        private int questionIndex = 0; // Vị trí câu hỏi hiện tại trong danh sách
        private Panel panel; // Giao diện cho câu hỏi
        private Label questionLabel; // Nhãn hiển thị câu hỏi
        private Button answerA, answerB, answerC, answerD; // Các nút cho đáp án
        private int currentQuestionIndex = 0; // Vị trí câu hỏi hiện tại để quản lý hiển thị


        public Form1()
        {
            //PHẦN QUIZ
            InitializeComponent();
            InitializeGame(); // Thiết lập giao diện game
            StartQuiz(); // Bắt đầu phần Quiz
            backgroundMusicPlayer = new SoundPlayer(Properties.Resources.background_music);
            backgroundMusicPlayer.PlayLooping();


        }
        private void ShowQuizQuestion(int questionIndex)
        {
            // Hiển thị câu hỏi hiện tại và các đáp án
            // Kiểm tra xem nhãn câu hỏi đã được khởi tạo chưa
            if (questionLabel == null)
            {
                // Khởi tạo các thành phần giao diện cho câu hỏi
                Form quizForm = this;
                quizForm.Width = 1000;
                quizForm.Height = 600;
                quizForm.Text = "Quiz";

                panel = new Panel();
                panel.Dock = DockStyle.Fill;
                panel.BackgroundImage = Properties.Resources.background3; // Nền giao diện Quiz
                panel.BackgroundImageLayout = ImageLayout.Stretch;
                quizForm.Controls.Add(panel);

                questionLabel = new Label(); // Nhãn chứa nội dung câu hỏi
                questionLabel.Font = new Font("Arial", 16, FontStyle.Bold);
                questionLabel.Location = new Point(20, 20);
                questionLabel.AutoSize = true;
                questionLabel.ForeColor = Color.Black;
                panel.Controls.Add(questionLabel);

                // Khởi tạo các nút cho các đáp án
                answerA = new Button();
                answerA.Left = 10;
                answerA.Top = 120;
                answerA.Width = 360;
                answerA.Height = 50;
                answerA.Font = new Font("Arial", 10, FontStyle.Bold);
                answerA.Click += (sender, e) => CheckAnswer("A", questionIndex); // Sự kiện khi chọn đáp án
                panel.Controls.Add(answerA);

                // Tương tự cho các nút còn lại
                answerB = new Button();
                answerB.Left = 10;
                answerB.Top = 180;
                answerB.Width = 360;
                answerB.Height = 50;
                answerB.Font = new Font("Arial", 10, FontStyle.Bold);
                answerB.Click += (sender, e) => CheckAnswer("B", questionIndex);
                panel.Controls.Add(answerB);

                answerC = new Button();
                answerC.Left = 10;
                answerC.Top = 240;
                answerC.Width = 360;
                answerC.Height = 50;
                answerC.Font = new Font("Arial", 10, FontStyle.Bold);
                answerC.Click += (sender, e) => CheckAnswer("C", questionIndex);
                panel.Controls.Add(answerC);

                answerD = new Button();
                answerD.Left = 10;
                answerD.Top = 300;
                answerD.Width = 360;
                answerD.Height = 50;
                answerD.Font = new Font("Arial", 10, FontStyle.Bold);
                answerD.Click += (sender, e) => CheckAnswer("D", questionIndex);
                panel.Controls.Add(answerD);
            }

            // Cập nhật nội dung câu hỏi và các đáp án
            questionLabel.Text = questions[questionIndex];
            answerA.Text = answers[questionIndex, 0];
            answerB.Text = answers[questionIndex, 1];
            answerC.Text = answers[questionIndex, 2];
            answerD.Text = answers[questionIndex, 3];
        }
        private void InitializeGame()
        {
            // Thiết lập giao diện trò chơi ban đầu
            this.Size = new Size(800, 600);
            // Initialize lblTime
            lblTime = new Label
            {
                Text = $"Time: {timeLeft}s",
                Font = new Font("Arial", 16),
                Location = new Point(20, 20),
                AutoSize = true
            };

            Label lblScore = new Label
            {
                Text = $"Score: {quizScore}",
                Font = new Font("Arial", 16),
                Location = new Point(20, 80),
                AutoSize = true
            };
            lblScore.Name = "lblScore";
            this.Controls.Add(lblScore);

            // // Thêm lblTime vào Controls của form để nó hiển thị trên giao diện người dùng
            this.Controls.Add(lblTime);


            gameTimer = new Timer
            {
                Interval = 1000 // Đếm ngược mỗi giây
            };
            gameTimer.Tick += GameTimer_Tick; // Hàm xử lý sự kiện, mỗi khi timer "tick" (tức mỗi giây), phương thức GameTimer_Tick sẽ được gọi tự động.

            // Bắt đầu timer
            gameTimer.Start();
        }





        private bool answered = false;  // Biến theo dõi xem câu hỏi đã được trả lời chưa

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            timeLeft--;  // Giảm thời gian

            // Cập nhật thời gian còn lại
            lblTime.Text = $"Time: {timeLeft}s";

            // Nếu hết thời gian
            if (timeLeft == 0)
            {
                gameTimer.Stop();  // Dừng đồng hồ

                // Nếu người chơi chưa trả lời
                if (!answered)
                {
                    MessageBox.Show($"Hết giờ! 😔 Đáp án đúng là: {correctAnswers[currentQuizQuestion]}", "Kết quả", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                // Cập nhật điểm và chuyển sang câu hỏi tiếp theo
                currentQuizQuestion++;  // Chuyển sang câu hỏi tiếp theo

                // Kiểm tra nếu đã hết câu hỏi
                if (currentQuizQuestion >= questions.Length)
                {
                    // Kiểm tra điểm sau khi hết câu hỏi
                    if (quizScore >= 50)
                    {
                        MessageBox.Show($"Hoàn thành Quiz! Bạn đã đạt điểm đủ: {quizScore}/{questions.Length}.", "Kết thúc", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        StartTrashSortingGame();  //Chạy Trash Sorting
                    }
                    else
                    {
                        MessageBox.Show($"Điểm của bạn là {quizScore}/{questions.Length}. Bạn chưa đủ điểm để qua màn!", "Kết thúc", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        StartQuiz();  //Bắt đầu lại quiz
                    }

                    quizScore = 0;  //Reset điểm cho vòng tiếp theo
                }
                else
                {
                    timeLeft = 20;  //Reset đồng hồ cho câu hỏi tiếp theo
                    gameTimer.Start();  //Bắt đầu tính thời gian
                    ShowQuizQuestion(currentQuizQuestion);  //Chiếu câu hỏi tiếp theo
                }

                // Đặt lại trạng thái câu hỏi chưa trả lời
                answered = false;
            }
        }





        //Start quizz bình thường
        private void StartQuiz()
        {
            currentQuestionIndex = 0; // Đảm bảo bắt đầu từ câu hỏi đầu tiên
            ShowQuizQuestion(currentQuestionIndex); // Bắt đầu từ câu hỏi đầu tiên
        }



        private void CheckAnswer(string selectedAnswer, int questionIndex)
        {
            if (timeLeft == 0) return; // Ngưng khi thời gian kết thúc

            // Kiểm tra xem câu trả lời có đúng không
            if (selectedAnswer == correctAnswers[currentQuizQuestion])
            {
                quizScore += 10;  // Cộng điểm nếu trả lời đúng
                MessageBox.Show("Đáp án đúng! 👍", "Kết quả", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show($"Sai rồi! 😞 Đáp án đúng là: {correctAnswers[currentQuizQuestion]}", "Kết quả", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            // Đặt cờ answered là true nhằm ngăn chặn việc trả lời nhiều lần cho cùng một câu hỏi
            answered = true;

            // Dừng đếm giờ và chuyển qua câu tiếp theo
            gameTimer.Stop();

            // Chuyển câu hỏi tiếp theo
            currentQuizQuestion++;


            // Nếu không còn câu hỏi nào nữa thì tính lại điểm
            if (currentQuizQuestion >= questions.Length)
            {
                // Sau khi trả lời hết câu hỏi, kiểm tra điểm có đủ điều kiện để qua màn tiếp theo không
                if (quizScore >= 50)
                {
                    MessageBox.Show($"Hoàn thành Quiz! Bạn đã đạt điểm đủ: {quizScore}/{questions.Length}.", "Kết thúc", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    StartTrashSortingGame();  // Chuyển qua màn Trash Sorting
                }
                else
                {
                    MessageBox.Show($"Điểm của bạn là {quizScore}/{questions.Length}. Bạn chưa đủ điểm để qua màn!", "Kết thúc", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    StartQuiz();  // Bắt đầu lại quiz
                }

                quizScore = 0;  // Reset điểm cho vòng kế tiếp
            }
            else
            {
                timeLeft = 20;  // Reset đồng hồ cho câu tiếp theo
                gameTimer.Start();  // Bắt đầu đếm giờ
                ShowQuizQuestion(currentQuizQuestion);  // Chiếu câu hỏi tiếp theo
            }
        }



        // HÀM BẮT ĐẦU PHÂN LOẠI RÁC
        private void StartTrashSortingGame()
        {
            // Chuyển sang game phân loại rác
            MessageBox.Show("Bây giờ chúng ta sẽ chuyển sang phần phân loại rác!");

            // Ẩn form quiz
            this.Hide();

            // Chạy lại game phân loại rác
            FormGame trashSortingForm = new FormGame();
            trashSortingForm.Show();  // Hiển thị game phân loại rác
        }


    }
}