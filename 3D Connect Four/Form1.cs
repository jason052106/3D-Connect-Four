using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media; // 新增這行來使用 SoundPlayer

namespace _3D_Connect_Four
{
    public partial class Form1 : Form
    {
        // 棋盤設定：7 行 (Column) x 6 列 (Row)
        private const int Cols = 7;
        private const int Rows = 6;
        private int[,] board = new int[Cols, Rows]; // 0:空, 1:玩家1(紅), 2:玩家2(黃)

        private int currentPlayer = 1;
        private Label lblStatus;
        private bool isGameOver = false; // 新增這行：記錄遊戲是否結束
        private int hoveredCol = -1;
        private TextBox txtPlayer1;
        private TextBox txtPlayer2;
        private CheckBox chkVsAI;
        private bool isVsAI;
        private Timer aiTimer;
        private SoundPlayer dropSound;

        // 視覺設定
        private int circleSize = 60;   // 圓孔大小
        private int padding = 15;      // 圓孔間距
        private int boardStartX = 50;  // 棋盤左上角 X 座標
        private int boardStartY = 80;  // 棋盤左上角 Y 座標
        public Form1(bool isVsAI)
        {
            InitializeComponent();
            this.isVsAI = isVsAI; // 將傳進來的參數存起來
            SetupUI();
            this.DoubleBuffered = true;
        }
        private void SetupUI()
        {
            this.Size = new Size(650, 650);
            this.Text = "四子棋 (Connect Four)";
            this.BackColor = Color.WhiteSmoke;

            // 狀態標籤 (對應圖片中的 "輪到您了！")
            lblStatus = new Label
            {
                Text = "輪到您了！(玩家 1)",
                Font = new Font("微軟正黑體", 24, FontStyle.Bold),
                ForeColor = Color.MediumVioletRed,
                AutoSize = true,
                Location = new Point(180, 20)
            };
            this.Controls.Add(lblStatus);

            // 玩家 1 名稱輸入框
            txtPlayer1 = new TextBox
            {
                Location = new Point(20, 20),
                Width = 100,
                Text = "玩家 1",
                ForeColor = Color.MediumVioletRed,
                Font = new Font("微軟正黑體", 10, FontStyle.Bold)
            };
            txtPlayer1.TextChanged += (s, e) => UpdateStatusLabel(); // 名字改變時即時更新標題
            this.Controls.Add(txtPlayer1);

            // 玩家 2 名稱輸入框
            txtPlayer2 = new TextBox
            {
                Location = new Point(20,50),
                Width = 100,
                Text = "玩家 2",
                ForeColor = Color.DarkGoldenrod,
                Font = new Font("微軟正黑體", 10, FontStyle.Bold)
            };
            txtPlayer2.TextChanged += (s, e) => UpdateStatusLabel();
            this.Controls.Add(txtPlayer2);

            // 綁定滑鼠點擊與重繪事件
            this.Paint += Form1_Paint;
            //this.MouseClick += Form1_MouseClick;

            this.MouseMove += Form1_MouseMove;
            this.MouseLeave += Form1_MouseLeave;

            // 新增：AI 思考的計時器 (設定 500 毫秒 = 0.5秒)
            aiTimer = new Timer();
            aiTimer.Interval = 500;
            aiTimer.Tick += AiTimer_Tick;

            try
            {
                // 這裡填入您的音效檔名稱，如果放在 Debug 資料夾下，直接寫檔名即可
                dropSound = new SoundPlayer("freesound_community-place-100513.wav");
                dropSound.LoadAsync(); // 非同步載入，避免卡頓
            }
            catch (Exception)
            {
                // 如果找不到檔案就不載入，避免程式崩潰
                dropSound = null;
            }
        }

        private void AiTimer_Tick(object sender, EventArgs e)
        {
            aiTimer.Stop(); // 停止計時，避免重複執行
            if (isGameOver) return;

            // 1. 攻擊：檢查 AI (玩家2) 是否差一步獲勝
            int winningCol = FindBestMove(2);
            if (winningCol != -1)
            {
                DropPiece(winningCol);
                return;
            }

            // 2. 防守：檢查玩家1 是否差一步獲勝，進行阻擋
            int blockingCol = FindBestMove(1);
            if (blockingCol != -1)
            {
                DropPiece(blockingCol);
                return;
            }

            // 3. 隨機：如果都沒有，隨機找一個沒滿的直行下棋
            Random rnd = new Random();
            int randomCol;
            do
            {
                randomCol = rnd.Next(0, Cols);
            } while (board[randomCol, 0] != 0); // 確保最頂端是空的 (這行沒滿)

            DropPiece(randomCol);
        }

        // 尋找能讓特定玩家獲勝的行數
        private int FindBestMove(int player)
        {
            for (int col = 0; col < Cols; col++)
            {
                int row = GetAvailableRow(col);
                if (row != -1)
                {
                    // 假裝把棋子下在這裡
                    board[col, row] = player;
                    bool isWin = CheckWin(col, row, player);
                    // 檢查完立刻把棋子拿起來 (復原)
                    board[col, row] = 0;

                    if (isWin) return col; // 如果這步會贏，就回傳這個行數
                }
            }
            return -1; // 找不到能贏的步
        }

        // 取得特定直行最低的空位 Row
        private int GetAvailableRow(int col)
        {
            for (int row = Rows - 1; row >= 0; row--)
            {
                if (board[col, row] == 0) return row;
            }
            return -1; // 滿了
        }

        private void UpdateStatusLabel()
        {
            if (isGameOver) return; // 如果遊戲結束就不改變狀態字眼

            string currentPlayerName = (currentPlayer == 1) ? txtPlayer1.Text : txtPlayer2.Text;
            lblStatus.Text = $"輪到 {currentPlayerName} 了！";
        }
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int boardWidth = Cols * (circleSize + padding) + padding;
            int boardHeight = Rows * (circleSize + padding) + padding;

            // 1. 畫出原本的淺灰色大底板
            Brush boardBrush = new SolidBrush(Color.FromArgb(230, 232, 235));
            g.FillRectangle(boardBrush, boardStartX, boardStartY, boardWidth, boardHeight);

            // --------------------------------------------------------
            // ▼ 新增：2. 如果滑鼠有停留在某一列，畫出深灰色高光區塊 ▼
            // --------------------------------------------------------
            if (hoveredCol != -1)
            {
                // 設定深灰色 (數值可依喜好微調)
                Brush hoverBrush = new SolidBrush(Color.FromArgb(205, 210, 215));

                // 計算這一列的 X 座標與寬度
                int hoverX = boardStartX + padding / 2 + hoveredCol * (circleSize + padding);
                int hoverWidth = circleSize + padding;

                g.FillRectangle(hoverBrush, hoverX, boardStartY, hoverWidth, boardHeight);
            }
            // --------------------------------------------------------

            // 3. 畫出 7x6 的圓孔或棋子 (原本的雙層迴圈保持不變)
            for (int col = 0; col < Cols; col++)
            {
                for (int row = 0; row < Rows; row++)
                {
                    int x = boardStartX + padding + col * (circleSize + padding);
                    int y = boardStartY + padding + row * (circleSize + padding);

                    Brush pieceBrush = Brushes.White;
                    if (board[col, row] == 1)
                        pieceBrush = Brushes.Crimson;
                    else if (board[col, row] == 2)
                        pieceBrush = Brushes.Gold;    // 或換成圖片中的湖水綠色 (Color.MediumAquamarine)

                    g.FillEllipse(pieceBrush, x, y, circleSize, circleSize);
                }
            }
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            // 如果遊戲已經結束，點擊畫面即可重新開始
            if (isGameOver)
            {
                ResetGame();
                return;
            }

            if (isVsAI && currentPlayer == 2) return;

            // 判斷點擊的是哪一個直行 (Column)
            int colWidth = circleSize + padding;
            int relativeX = e.X - boardStartX - padding / 2;

            if (relativeX < 0) return; // 點擊在棋盤外

            int clickedCol = relativeX / colWidth;

            // 確保點擊在有效的行數內
            if (clickedCol >= 0 && clickedCol < Cols)
            {
                DropPiece(clickedCol);
            }
        }

        // 落子邏輯 (地心引力)
        private void DropPiece(int col)
        {
            for (int row = Rows - 1; row >= 0; row--)
            {
                if (board[col, row] == 0)
                {
                    board[col, row] = currentPlayer;
                    if (dropSound != null) dropSound.Play();

                    // 檢查剛下的這步棋是否達成獲勝條件
                    if (CheckWin(col, row, currentPlayer))
                    {
                        lblStatus.Text = $"遊戲結束！玩家 {currentPlayer} 獲勝！";
                        lblStatus.ForeColor = Color.ForestGreen;
                        isGameOver = true;
                        this.Invalidate(); // 更新畫面
                        MessageBox.Show($"恭喜玩家 {currentPlayer} 獲勝！\n點擊棋盤任意處重新開始。");
                        return;
                    }

                    // 檢查是否平手 (最上面那列全滿)
                    if (CheckDraw())
                    {
                        lblStatus.Text = "平手！";
                        lblStatus.ForeColor = Color.Gray;
                        isGameOver = true;
                        this.Invalidate();
                        MessageBox.Show("雙方平手！\n點擊棋盤任意處重新開始。");
                        return;
                    }

                    // 切換玩家
                    currentPlayer = (currentPlayer == 1) ? 2 : 1;
                    lblStatus.ForeColor = (currentPlayer == 1) ? Color.MediumVioletRed : Color.DarkGoldenrod;
                    UpdateStatusLabel();

                    this.Invalidate();

                    if (currentPlayer == 2 && isVsAI && !isGameOver)
                    {
                        lblStatus.Text = "AI 思考中...";
                        aiTimer.Start();
                    }
                    return;
                }
            }

            MessageBox.Show("這行已經滿了！");

        }

        // 檢查是否獲勝 (四個方向)
        private bool CheckWin(int c, int r, int player)
        {
            // 只要有任何一個方向連成 4 顆就回傳 true
            return CheckDirection(c, r, player, 1, 0) || // 水平 (左右)
                   CheckDirection(c, r, player, 0, 1) || // 垂直 (上下)
                   CheckDirection(c, r, player, 1, 1) || // 正斜線 (\)
                   CheckDirection(c, r, player, 1, -1);  // 反斜線 (/)
        }

        // 輔助方法：計算特定方向上連續同色棋子的數量
        private bool CheckDirection(int c, int r, int player, int dc, int dr)
        {
            int count = 1; // 包含剛落下的那顆棋子本身

            // 往正方向尋找 (例如往右)
            int i = c + dc;
            int j = r + dr;
            while (i >= 0 && i < Cols && j >= 0 && j < Rows && board[i, j] == player)
            {
                count++;
                i += dc;
                j += dr;
            }

            // 往反方向尋找 (例如往左)
            i = c - dc;
            j = r - dr;
            while (i >= 0 && i < Cols && j >= 0 && j < Rows && board[i, j] == player)
            {
                count++;
                i -= dc;
                j -= dr;
            }

            return count >= 4; // 如果大於或等於 4 顆，代表連線成功
        }

        // 檢查是否平手
        private bool CheckDraw()
        {
            // 只要最頂層 (row = 0) 還有空位，就還沒平手
            for (int col = 0; col < Cols; col++)
            {
                if (board[col, 0] == 0)
                    return false;
            }
            return true;
        }

        // 重新開始遊戲
        private void ResetGame()
        {
            Array.Clear(board, 0, board.Length); // 清空二維陣列
            currentPlayer = 1;
            isGameOver = false;
            lblStatus.Text = "輪到您了！(玩家 1)";
            lblStatus.ForeColor = Color.MediumVioletRed;
            this.Invalidate(); // 重新繪製空白棋盤
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ResetGame();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            ResetGame();
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isGameOver) return; // 遊戲結束就不顯示提示

            int colWidth = circleSize + padding;
            int relativeX = e.X - boardStartX - padding / 2;
            int newHoveredCol = -1;

            // 檢查滑鼠是否在棋盤的 X 軸範圍內
            if (relativeX >= 0 && relativeX < Cols * colWidth)
            {
                // 檢查滑鼠是否在棋盤的 Y 軸範圍內 (避免滑鼠在棋盤上方或下方也觸發)
                int boardHeight = Rows * (circleSize + padding) + padding;
                if (e.Y >= boardStartY && e.Y <= boardStartY + boardHeight)
                {
                    newHoveredCol = relativeX / colWidth;
                }
            }

            // 如果滑鼠移動到了不同的列，才重新繪製畫面 (節省效能)
            if (newHoveredCol != hoveredCol)
            {
                hoveredCol = newHoveredCol;
                this.Invalidate();
            }
        }

        private void Form1_MouseLeave(object sender, EventArgs e)
        {
            if (hoveredCol != -1)
            {
                hoveredCol = -1;
                this.Invalidate();
            }
        }
    }
}
