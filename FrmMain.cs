using System.Media;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrayNotify;

namespace TetrisGame
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();
        }

        private ResourceManager? resourceManager;

        private void buttonExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == Keys.Up || keyData == Keys.Down)
                return false;//是上下方向键则不处理，交给KeyDown事件
            if (keyData == Keys.Left || keyData == Keys.Right)
                return false;//是左右方向键则不处理，交给KeyDown事件
            return base.ProcessDialogKey(keyData);
        }

        private void FrmMain_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Back:
                    break;
                case Keys.Tab:
                    break;
                case Keys.Enter:
                    //tetrisPanel1.SimulateButtonRotate(true);
                    tetrisPanel.StepQuarterRotateTetrisShape();
                    break;
                case Keys.ShiftKey:
                    break;
                case Keys.ControlKey:
                    break;
                case Keys.CapsLock:
                    break;
                case Keys.Escape:
                    break;
                case Keys.Space:
                    if (tetrisPanel.Status != GameStatus.GamePlay)
                    {
                        tetrisPanel.PlayGame();
                    }
                    else
                    {
                        tetrisPanel.PauseGame();
                    }
                    break;
                case Keys.PageUp:
                    tetrisPanel.AdjustLevel(true);
                    break;
                case Keys.PageDown:
                    tetrisPanel.AdjustLevel(false);
                    break;
                case Keys.End:
                    break;
                case Keys.Home:
                    break;
                case Keys.Left:
                    //tetrisPanel1.SimulateButtonMoveLeft(true);
                    tetrisPanel.StepMoveTetrisLeft();
                    //System.Media.SystemSounds.Asterisk.Play();
                    //PlaySound("Click");
                    break;
                case Keys.Up:
                    //tetrisPanel1.SimulateButtonMoveUp(true);
                    tetrisPanel.StepQuarterRotateTetrisShape();
                    //System.Media.SystemSounds.Asterisk.Play();
                    //PlaySound("Click");
                    break;
                case Keys.Right:
                    //tetrisPanel1.SimulateButtonMoveRight(true);
                    tetrisPanel.StepMoveTetrisRight();
                    //System.Media.SystemSounds.Asterisk.Play();
                    //PlaySound("Click");
                    break;
                case Keys.Down:
                    //tetrisPanel1.SimulateButtonMoveDown(true);
                    tetrisPanel.StepMoveTetrisDown();
                    //System.Media.SystemSounds.Asterisk.Play();
                    //PlaySound("Click");
                    break;
                case Keys.Print:
                    break;
                case Keys.PrintScreen:
                    break;
                case Keys.Insert:
                    break;
                case Keys.Delete:
                    break;
                case Keys.D0:
                    break;
                case Keys.D1:
                    break;
                case Keys.D2:
                    break;
                case Keys.D3:
                    break;
                case Keys.D4:
                    break;
                case Keys.D5:
                    break;
                case Keys.D6:
                    break;
                case Keys.D7:
                    break;
                case Keys.D8:
                    break;
                case Keys.D9:
                    break;
                case Keys.A:
                    tetrisPanel.StepMoveTetrisLeft();
                    break;
                case Keys.D:
                    tetrisPanel.StepMoveTetrisRight();
                    break;
                case Keys.S:
                    tetrisPanel.StepMoveTetrisDown();
                    break;
                case Keys.W:
                    tetrisPanel.StepQuarterRotateTetrisShape();
                    break;
                case Keys.LWin:
                    break;
                case Keys.RWin:
                    break;
                case Keys.Apps:
                    break;
                case Keys.Sleep:
                    break;
                case Keys.NumPad0:
                    break;
                case Keys.NumPad1:
                    break;
                case Keys.NumPad2:
                    break;
                case Keys.NumPad3:
                    break;
                case Keys.NumPad4:
                    break;
                case Keys.NumPad5:
                    break;
                case Keys.NumPad6:
                    break;
                case Keys.NumPad7:
                    break;
                case Keys.NumPad8:
                    break;
                case Keys.NumPad9:
                    break;
                case Keys.Multiply:
                    break;
                case Keys.Add:
                    break;
                case Keys.Separator:
                    break;
                case Keys.Subtract:
                    break;
                case Keys.Decimal:
                    break;
                case Keys.Divide:
                    break;
                case Keys.F1:
                    break;
                case Keys.F2:
                    break;
                case Keys.F3:
                    break;
                case Keys.F4:
                    break;
                case Keys.F5:
                    break;
                case Keys.F6:
                    break;
                case Keys.F7:
                    break;
                case Keys.F8:
                    break;
                case Keys.F9:
                    break;
                case Keys.F10:
                    break;
                case Keys.F11:
                    break;
                case Keys.F12:
                    break;
                case Keys.NumLock:
                    break;
                case Keys.Scroll:
                    break;
                case Keys.LShiftKey:
                    break;
                case Keys.RShiftKey:
                    break;
                case Keys.LControlKey:
                    break;
                case Keys.RControlKey:
                    break;
                case Keys.LMenu:
                    break;
                case Keys.RMenu:
                    break;
                case Keys.Shift:
                    break;
                case Keys.Control:
                    break;
                case Keys.Alt:
                    break;
                default:
                    break;
            }
            Stream? stream = resourceManager.GetStream("Click");
            tetrisPanel.PlaySound(stream);
        }

        private void FrmMain_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Space:
                    tetrisPanel.SimulateButtonRotate(false);
                    break;
                case Keys.Left:
                    tetrisPanel.SimulateButtonMoveLeft(false);
                    break;
                case Keys.Up:
                    tetrisPanel.SimulateButtonMoveUp(false);
                    break;
                case Keys.Right:
                    tetrisPanel.SimulateButtonMoveRight(false);
                    break;
                case Keys.Down:
                    tetrisPanel.SimulateButtonMoveDown(false);
                    break;
                default:
                    break;
            }
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            resourceManager = new ResourceManager("TetrisGame.Properties.Resources",
                Assembly.GetExecutingAssembly());
            Stream? stream = resourceManager.GetStream("Background");
            tetrisPanel.PlayBackground(stream);
        }

        private void tetrisPanel_RowFillFull(object sender, EventArgs e)
        {
            Stream? stream = resourceManager?.GetStream("RowFillFull");
            tetrisPanel.PlaySound(stream);
        }

        private void tetrisPanel_GameOver(object sender, EventArgs e)
        {
            Stream? stream = resourceManager?.GetStream("GameOver");
            tetrisPanel.PlaySound(stream);
        }

    }
}
