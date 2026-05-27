using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _3D_Connect_Four
{
    public partial class MenuForm : Form
    {
        public MenuForm()
        {
            InitializeComponent();
            SetupMenu();
        }

        private void SetupMenu()
        {
            this.Size = new Size(400, 350);
            this.Text = "四子棋 - 主選單";
            this.StartPosition = FormStartPosition.CenterScreen; // 讓視窗在螢幕正中央開啟
 
            btnPvP.Click += (s, e) => StartGame(false); // 傳入 false 代表不是 AI 模式
            btnPvE.Click += (s, e) => StartGame(true); // 傳入 true 代表是 AI 模式
        }

        // 啟動遊戲的邏輯
        private void StartGame(bool vsAI)
        {
            // 產生遊戲表單，並把模式參數傳進去
            Form1 gameForm = new Form1(vsAI);

            // 當遊戲表單關閉時，把主選單重新顯示出來
            gameForm.FormClosed += (s, args) => this.Show();

            gameForm.Show(); // 顯示遊戲畫面
            this.Hide();     // 隱藏目前的主選單
        }
    }
}
