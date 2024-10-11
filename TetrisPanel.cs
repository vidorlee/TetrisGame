using System.ComponentModel;
using System.Media;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;

namespace TetrisGame
{
    /// <summary>
    /// 俄罗斯方块面板
    /// </summary>
    public partial class TetrisPanel : UserControl
    {

        #region 【类型定义】
        //******************************************************************************************
        /// <summary>
        /// 俄罗斯方块游戏操作按钮类
        /// </summary>
        public class TetrisButton
        {
            public TetrisButton()
            {
            }

            public TetrisButton(string name, Color backcolor, string text)
            {
                Name = name;
                ColorBack = backcolor;
                Text = text;
            }

            public string Name { get; set; } = string.Empty;

            public string Text { get; set; } = string.Empty;

            public Rectangle Rect { get; set; } = Rectangle.Empty;

            public Color ColorBack { get; set; } = Color.Yellow;

            public bool IsHover { get; set; } = false;

            public Color ColorHover { get; set; } = Color.Orange;

            public bool IsPress { get; set; } = false;

            public Color ColorPress { get; set; } = Color.Red;


            /// <summary>
            /// 鼠标单击事件
            /// </summary>
            [Description("鼠标单击事件"), Category("操作")]
            public event EventHandler<MouseEventArgs>? MouseClick;
            /// <summary>
            /// 鼠标单击事件触发函数
            /// </summary>
            /// <param name="sender">事件关联实例</param>
            /// <param name="e">鼠标单击事件消息参数</param>
            public void OnMouseClick(object sender, MouseEventArgs e)
            {
                if (MouseClick != null)
                {
                    MouseClick.Invoke(this, e);
                }
            }
        }

        /// <summary>
        /// 俄罗斯方块游戏坐标点
        /// </summary>
        public class TetrisPoint
        {
            public int X { get; set; }

            public int Y { get; set; }

            public TetrisPoint(int x, int y) {  X = x; Y = y; }

            public override string ToString()
            {
                return $"X = {X}, Y = {Y}";
            }
        }

        //******************************************************************************************
        #endregion


        #region 【构造函数】
        //******************************************************************************************
        public TetrisPanel()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            //this.SetStyle(ControlStyles.ResizeRedraw, true);
            //this.SetStyle(ControlStyles.Selectable, true);
            //this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            this.InitializeLayout();
            this.DisplayMode = DisplayMode.Vertical;
            Speed = 1;
            Status = GameStatus.GameReady;
        }


        //******************************************************************************************
        #endregion


        #region 【私有变量】
        //******************************************************************************************

        /// <summary>
        /// 当前活动方块形状的格子坐标列表
        /// </summary>
        private List<TetrisPoint> ShapePoints = new List<TetrisPoint>();

        /// <summary>
        /// 下一个活动方块形状的格子点阵
        /// </summary>
        private List<TetrisPoint> NextShapePoints = new List<TetrisPoint>();


        private int preCellWidth = 15;
        private object lockObject = new object();
        private SoundPlayer bgSoundPlayer = new SoundPlayer();
        private ResourceManager resourceManager = new ResourceManager(typeof(TetrisPanel));

        private Dictionary<string, TetrisButton> OperateButtons = new Dictionary<string, TetrisButton>(4);

        private Rectangle RectDisplay = Rectangle.Empty;
        private Rectangle RectOption = Rectangle.Empty;
        private Rectangle RectMotion = Rectangle.Empty;
        private Rectangle RectControl = Rectangle.Empty;
        private Rectangle RectRotate = Rectangle.Empty;

        private Rectangle RectOptScore = Rectangle.Empty;
        private Rectangle RectOptNext = Rectangle.Empty;
        private Rectangle RectOptSpeed = Rectangle.Empty;
        private Rectangle RectOptLevel = Rectangle.Empty;

        private RectangleF RectMotUp = RectangleF.Empty;
        private RectangleF RectMotDown = RectangleF.Empty;
        private RectangleF RectMotLeft = RectangleF.Empty;
        private RectangleF RectMotRight = RectangleF.Empty;

        private Font FontStatus = new Font("Impact", 32.0f, FontStyle.Bold);
        private Font FontNotice = new Font("Impact", 32.0f, FontStyle.Bold);
        private Font FontScore = new Font("Impact", 20.0f, FontStyle.Bold);
        private Font FontLevel = new Font("Impact", 20.0f, FontStyle.Bold);
        private Font FontSpeed = new Font("Impact", 20.0f, FontStyle.Bold);
        private Font FontButtonSmall = new Font("隶书", 11.0f, FontStyle.Bold);
        private Font FontButtonLarge = new Font("隶书", 16.0f, FontStyle.Bold);

        //******************************************************************************************
        #endregion


        #region 【公开属性】
        //******************************************************************************************
        private DisplayMode dispMode = DisplayMode.Horizontal;
        /// <summary>
        /// 显示模式
        /// </summary>
        [Category("外观"), Description("显示模式")]
        public DisplayMode DisplayMode
        {
            get
            {
                return dispMode;
            }
            set
            {
                if (dispMode != value)
                {
                    dispMode = value;
                    if (dispMode == DisplayMode.Horizontal)
                    {
                        base.MinimumSize = new Size(480, 240);
                    }
                    else
                    {
                        base.MinimumSize = new Size(240, 360);
                    }
                    this.InitializeLayout();
                }
            }
        }


        private int rows = (ROW_NUM_MIN + ROW_NUM_MAX) / 2;
        /// <summary>
        /// 游戏面板的格子总行数
        /// </summary>
        [Category("外观"), Description("游戏面板的格子总行数")]
        public int Rows
        {
            get
            {
                return rows;
            }
            set
            {
                if (rows != value)
                {
                    this.AutoFitDisplayCellSize(value, Cols);
                    //this.InitializeMatrixs();
                    this.Invalidate();
                }
            }
        }


        private int cols = (COL_NUM_MIN + COL_NUM_MAX) / 2;
        /// <summary>
        /// 游戏面板的格子总列数
        /// </summary>
        [Category("外观"), Description("游戏面板的格子总列数")]
        public int Cols
        {
            get
            {
                return cols;
            }
            set
            {
                if (cols != value)
                {
                    this.AutoFitDisplayCellSize(Rows, value);
                    //this.InitializeMatrixs();
                    this.Invalidate();
                }
            }
        }

        private int speed = (SPEED_MIN + SPEED_MAX) / 2;
        /// <summary>
        /// 游戏速度(1到5)
        /// </summary>
        [Category("行为"), Description("游戏速度(1到5)")]
        public int Speed
        {
            get
            {
                return speed;
            }
            set
            {
                if (value < SPEED_MIN) value = SPEED_MIN;
                else if (value > SPEED_MAX) value = SPEED_MAX;
                if (speed != value)
                {
                    speed = value;
                    timerMove.Interval = 250 + 1000 / speed;
                    this.Invalidate();
                }
            }
        }

        private int level = 1;
        /// <summary>
        /// 游戏难度(1到99)
        /// </summary>
        [Category("行为"), Description("游戏难度(1到99)")]
        public int Level
        {
            get
            {
                return level;
            }
            set
            {
                if (value < 1) value = 1;
                else if (value > 99) value = 99;
                if (level != value)
                {
                    level = value;
                    this.Invalidate();
                }
            }
        }

        private int cellSize = (CELL_SIZE_MIN + CELL_SIZE_MAX) / 2;
        /// <summary>
        /// 方块单元格边长
        /// </summary>
        [Category("外观"), Description("方块单元格边长")]
        public int CellSize
        {
            get
            {
                return cellSize;
            }
            private set
            {
                if (cellSize != value)
                {
                    cellSize = value;
                    this.Invalidate();
                }
            }
        }

        private GameStatus status = GameStatus.GameReady;
        /// <summary>
        /// 当前游戏状态
        /// </summary>
        public GameStatus Status
        {
            get
            {
                return status;
            }
            private set
            {
                if (status != value)
                {
                    status = value;
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// 整个面板的全部格子点阵
        /// </summary>
        public List<List<int>> PanelMatrix { get; private set; } = new List<List<int>>();

        public int Score { get; private set; } = 0;

        /// <summary>
        /// 当前活动方块的形态
        /// </summary>
        public TetrisShape Shape { get; private set; }

        ///// <summary>
        ///// 当前活动方块形状的格子点阵
        ///// </summary>
        //public List<List<int>> ShapeMatrix { get; private set; } = new List<List<int>>();

        /// <summary>
        /// 下一个活动方块的形态
        /// </summary>
        public TetrisShape NextShape { get; private set; }

        ///// <summary>
        ///// 下一个活动方块形状的格子点阵
        ///// </summary>
        //public List<List<int>> NextShapeMatrix { get; private set; } = new List<List<int>>();

        [Browsable(false)]
        public new Size MinimumSize
        {
            get
            {
                return base.MinimumSize;
            }
            set
            {
                if (DisplayMode == DisplayMode.Horizontal)
                {
                    value = new Size(480, 240);
                }
                else
                {
                    value = new Size(240, 360);
                }
                if (base.MinimumSize != value)
                {
                    base.MinimumSize = value;
                    this.Invalidate();
                }
            }
        }
        //******************************************************************************************
        #endregion


        #region 【静态常量】
        //******************************************************************************************
        private static readonly int SPACE = 3;

        private static readonly int COL_NUM_MIN = 12;
        private static readonly int COL_NUM_MAX = 24;
        private static readonly int ROW_NUM_MIN = 16;
        private static readonly int ROW_NUM_MAX = 48;
        /// <summary>
        /// 方块单元格最小尺寸
        /// </summary>
        private static readonly int CELL_SIZE_MIN = 8;
        /// <summary>
        /// 方块单元格最大尺寸
        /// </summary>
        private static readonly int CELL_SIZE_MAX = 64;

        /// <summary>
        /// 游戏速度最小档位
        /// </summary>
        private static readonly int SPEED_MIN = 1;
        /// <summary>
        /// 游戏速度最大档位
        /// </summary>
        private static readonly int SPEED_MAX = 5;

        /// <summary>
        /// 游戏难度最小档位
        /// </summary>
        private static readonly int LEVEL_MIN = 1;
        /// <summary>
        /// 游戏难度最大档位
        /// </summary>
        private static readonly int LEVEL_MAX = 99;

        private static readonly string TEXT_STATUS = $"\n Press ‘Space’ Start/Pause Game!";
        /// <summary>
        /// 方块形态颜色列表
        /// </summary>
        public static readonly List<Color> ColorList = new List<Color>()
        {
            Color.FromArgb(255, 250, 250, 250), Color.FromArgb(255, 250, 0, 0),
            Color.FromArgb(255, 0, 250, 0),     Color.FromArgb(255, 0, 0, 250),
            Color.FromArgb(255, 250, 250, 0),   Color.FromArgb(255, 0, 250, 250),
            Color.FromArgb(255, 250, 0, 250),   Color.FromArgb(255, 250, 100, 0), 
        };

        //******************************************************************************************
        #endregion


        #region 【公开事件】
        //******************************************************************************************

        /// <summary>
        /// 满行事件
        /// </summary>
        [Description("满行事件"), Category("操作")]
        public event EventHandler<List<int>>? RowFillFull;
        /// <summary>
        /// 满行事件触发函数
        /// </summary>
        /// <param name="sender">事件关联实例</param>
        /// <param name="e">事件消息参数</param>
        public void OnRowFillFull(object sender, List<int> rowList)
        {
            if (RowFillFull != null)
            {
                RowFillFull.Invoke(sender, rowList);
            }
        }

        /// <summary>
        /// 游戏结束事件
        /// </summary>
        [Description("游戏结束事件"), Category("操作")]
        public event EventHandler? GameOver;
        /// <summary>
        /// 游戏结束事件触发函数
        /// </summary>
        /// <param name="sender">事件关联实例</param>
        /// <param name="e">事件消息参数</param>
        public void OnGameOver(object sender, EventArgs e)
        {
            bgSoundPlayer?.Stop();//停止背景音乐播放

            if (GameOver != null)
            {
                GameOver.Invoke(sender, e);
            }
        }
        //******************************************************************************************
        #endregion


        #region 【私有方法】
        //******************************************************************************************

        private void InitializeLayout()
        {
            if (DisplayMode == DisplayMode.Horizontal)
            {
                //水平20%~60% 竖直 02%~98%
                RectDisplay = new Rectangle((int)(Width * 0.20f + SPACE), (int)(Height * 0.02f + SPACE),
                    (int)(Width * 0.40f - 2 * SPACE), (int)(Height * 0.96f - 2 * SPACE));
                //水平60%~80% 竖直 02%~98%
                RectOption = new Rectangle((int)(Width * 0.60f + SPACE), (int)(Height * 0.02f + SPACE),
                    (int)(Width * 0.20f - 2 * SPACE), (int)(Height * 0.96f - 2 * SPACE));
                //水平02%~20% 竖直 02%~98%
                RectMotion = new Rectangle((int)(Width * 0.02f + SPACE), (int)(Height * 0.02f + SPACE),
                    (int)(Width * 0.18f - 2 * SPACE), (int)(Height * 0.96f - 2 * SPACE));
                //水平80%~98% 竖直 02%~50%
                RectControl = new Rectangle((int)(Width * 0.80f + SPACE), (int)(Height * 0.02f + SPACE),
                    (int)(Width * 0.15f - 2 * SPACE), (int)(Height * 0.48f - 2 * SPACE));
                //水平80%~98% 竖直 50%~98%
                RectRotate = new Rectangle((int)(Width * 0.80f + SPACE), (int)(Height * 0.50f + SPACE),
                    (int)(Width * 0.15f - 2 * SPACE), (int)(Height * 0.48f - 2 * SPACE));

            }
            else if (DisplayMode == DisplayMode.Vertical)
            {
                //水平02%~62% 竖直 02%~74%
                RectDisplay = new Rectangle((int)(Width * 0.02f + SPACE), (int)(Height * 0.02f + SPACE),
                    (int)(Width * 0.60f - 2 * SPACE), (int)(Height * 0.72f - 2 * SPACE));
                //水平62%~98% 竖直 02%~74%
                RectOption = new Rectangle((int)(Width * 0.62f + SPACE), (int)(Height * 0.02f + SPACE),
                    (int)(Width * 0.36f - 2 * SPACE), (int)(Height * 0.72f - 2 * SPACE));
                //水平02%~62% 竖直 74%~98%
                RectMotion = new Rectangle((int)(Width * 0.02f + SPACE), (int)(Height * 0.74f + SPACE),
                    (int)(Width * 0.60f - 2 * SPACE), (int)(Height * 0.24f - 2 * SPACE));
                //水平62%~98% 竖直 74%~84%
                RectControl = new Rectangle((int)(Width * 0.62f + SPACE), (int)(Height * 0.74f + SPACE),
                    (int)(Width * 0.36f - 2 * SPACE), (int)(Height * 0.10f - 2 * SPACE));
                //水平62%~98% 竖直 84%~98%
                RectRotate = new Rectangle((int)(Width * 0.62f + SPACE), (int)(Height * 0.84f + SPACE),
                    (int)(Width * 0.36f - 2 * SPACE), (int)(Height * 0.14f - 2 * SPACE));
            }

            //------------------------------------------------------------------------------
            // 添加全部按钮实例到的字典键值集合
            //------------------------------------------------------------------------------
            OperateButtons.Clear();
            OperateButtons.Add("Up", new TetrisButton("Up", Color.Yellow, "上"));
            OperateButtons.Add("Down", new TetrisButton("Down", Color.Yellow, "下"));
            OperateButtons.Add("Left", new TetrisButton("Left", Color.Yellow, "左"));
            OperateButtons.Add("Right", new TetrisButton("Right", Color.Yellow, "右"));
            OperateButtons.Add("Rotate", new TetrisButton("Rotate", Color.GreenYellow, "旋转"));
            OperateButtons.Add("Start", new TetrisButton("Start", Color.LightGreen, "启停"));
            OperateButtons.Add("Reset", new TetrisButton("Reset", Color.LightCyan, "复位"));
            OperateButtons.Add("Speed", new TetrisButton("Speed", Color.LightBlue, "调速"));
            OperateButtons.Add("LevelAdd", new TetrisButton("LevelAdd", Color.LightPink, "难度+"));
            OperateButtons.Add("LevelSub", new TetrisButton("LevelSub", Color.LightSkyBlue, "难度-"));

            //------------------------------------------------------------------------------
            // 设置配置信息区各模块矩形区域
            //------------------------------------------------------------------------------
            RectOptScore = new Rectangle(
                RectOption.X + SPACE,
                RectOption.Y + SPACE,
                RectOption.Width - 2 * SPACE,
                (int)(RectOption.Height * 0.25f - 2 * SPACE));

            RectOptNext = new Rectangle(
                RectOption.X + SPACE,
                (int)(RectOption.Y + SPACE + RectOption.Height * 0.25f),
                RectOption.Width - 2 * SPACE,
                (int)(RectOption.Height * 0.25f - 2 * SPACE));

            RectOptSpeed = new Rectangle(
                RectOption.X + SPACE,
                (int)(RectOption.Y + SPACE + RectOption.Height * 0.50f),
                RectOption.Width - 2 * SPACE,
                (int)(RectOption.Height * 0.25f - 2 * SPACE));

            RectOptLevel = new Rectangle(
                RectOption.X + SPACE,
                (int)(RectOption.Y + SPACE + RectOption.Height * 0.75f),
                RectOption.Width - 2 * SPACE,
                (int)(RectOption.Height * 0.25f - 2 * SPACE));

            //------------------------------------------------------------------------------
            // 设置所有按钮的关联矩形区域
            //------------------------------------------------------------------------------
            int x, y;
            float bwidth = RectMotion.Width - 2 * SPACE;
            float bheight = RectMotion.Height - 2 * SPACE;
            int bsize = (int)(bwidth < bheight ? bwidth : bheight);
            int radius = (int)(bsize * 0.4f);

            if (this.DisplayMode == DisplayMode.Horizontal)
            {
                x = RectMotion.X + SPACE + (RectMotion.Width - bsize) / 2 + bsize / 2 - radius / 2;
                y = RectMotion.Y + SPACE + (RectMotion.Height - bsize) / 2 - radius / 2;
                OperateButtons["Up"].Rect = new Rectangle(x, y, radius, radius);

                x = RectMotion.X + SPACE + (RectMotion.Width - bsize) / 2 + bsize / 2 - radius / 2;
                y = RectMotion.Y + SPACE + (RectMotion.Height - bsize) / 2 + bsize - radius / 2;
                OperateButtons["Down"].Rect = new Rectangle(x, y, radius, radius);

                x = RectMotion.X + SPACE + (RectMotion.Width - bsize) / 2 + bsize / 4 - radius / 2;
                y = RectMotion.Y + SPACE + (RectMotion.Height - bsize) / 2 + bsize / 2 - radius / 2;
                OperateButtons["Left"].Rect = new Rectangle(x, y, radius, radius);

                x = RectMotion.X + SPACE + (RectMotion.Width - bsize) / 2 + bsize * 3 / 4 - radius / 2;
                y = RectMotion.Y + SPACE + (RectMotion.Height - bsize) / 2 + bsize / 2 - radius / 2;
                OperateButtons["Right"].Rect = new Rectangle(x, y, radius, radius);
            }
            else
            {
                x = RectMotion.X + SPACE + (RectMotion.Width - bsize) / 2 + bsize / 2 - radius / 2;
                y = RectMotion.Y + SPACE + (RectMotion.Height - bsize) / 2 + bsize / 4 - radius / 2;
                OperateButtons["Up"].Rect = new Rectangle(x, y, radius, radius);

                x = RectMotion.X + SPACE + (RectMotion.Width - bsize) / 2 + bsize / 2 - radius / 2;
                y = RectMotion.Y + SPACE + (RectMotion.Height - bsize) / 2 + bsize * 3 / 4 - radius / 2;
                OperateButtons["Down"].Rect = new Rectangle(x, y, radius, radius);

                x = RectMotion.X + SPACE + (RectMotion.Width - bsize) / 2 - radius / 2;
                y = RectMotion.Y + SPACE + (RectMotion.Height - bsize) / 2 + bsize / 2 - radius / 2;
                OperateButtons["Left"].Rect = new Rectangle(x, y, radius, radius);

                x = RectMotion.X + SPACE + (RectMotion.Width - bsize) / 2 + bsize - radius / 2;
                y = RectMotion.Y + SPACE + (RectMotion.Height - bsize) / 2 + bsize / 2 - radius / 2;
                OperateButtons["Right"].Rect = new Rectangle(x, y, radius, radius);
            }

            //旋转按钮
            bwidth = RectRotate.Width - 2 * SPACE;
            bheight = RectRotate.Height - 2 * SPACE;
            bsize = (int)((bwidth < bheight ? bwidth : bheight) * 0.9f);

            x = RectRotate.X + SPACE + (RectRotate.Width - bsize) / 2;
            y = RectRotate.Y + SPACE + (RectRotate.Height - bsize) / 2;
            OperateButtons["Rotate"].Rect = new Rectangle(x, y, bsize, bsize);

            //五个功能控制按钮
            float ctrlNums = 5.0f;
            if (this.DisplayMode == DisplayMode.Horizontal)
            {
                bwidth = RectControl.Width - 2 * SPACE;
                bheight = RectControl.Height / ctrlNums - (ctrlNums + 2) * SPACE;
                bsize = (int)(bwidth < bheight ? bwidth : bheight);
                x = RectControl.X + SPACE + (int)((bwidth - bsize) / 2.0f);
                y = (int)(RectControl.Y + 0 * RectControl.Height / ctrlNums + SPACE + (RectControl.Height / ctrlNums - bsize) / 2);
                OperateButtons["Start"].Rect = new Rectangle(x, y, bsize, bsize);
                x = RectControl.X + SPACE + (int)((bwidth - bsize) / 2.0f);
                y = (int)(RectControl.Y + 1 * RectControl.Height / ctrlNums + SPACE + (RectControl.Height / ctrlNums - bsize) / 2);
                OperateButtons["Reset"].Rect = new Rectangle(x, y, bsize, bsize);
                x = RectControl.X + SPACE + (int)((bwidth - bsize) / 2.0f);
                y = (int)(RectControl.Y + 2 * RectControl.Height / ctrlNums + SPACE + (RectControl.Height / ctrlNums - bsize) / 2);
                OperateButtons["Speed"].Rect = new Rectangle(x, y, bsize, bsize);
                x = RectControl.X + SPACE + (int)((bwidth - bsize) / 2.0f);
                y = (int)(RectControl.Y + 3 * RectControl.Height / ctrlNums + SPACE + (RectControl.Height / ctrlNums - bsize) / 2);
                OperateButtons["LevelAdd"].Rect = new Rectangle(x, y, bsize, bsize);
                x = RectControl.X + SPACE + (int)((bwidth - bsize) / 2.0f);
                y = (int)(RectControl.Y + 4 * RectControl.Height / ctrlNums + SPACE + (RectControl.Height / ctrlNums - bsize) / 2);
                OperateButtons["LevelSub"].Rect = new Rectangle(x, y, bsize, bsize);
            }
            else
            {
                bwidth = RectControl.Width / ctrlNums - (ctrlNums + 2) * SPACE;
                bheight = RectControl.Height - 4 * SPACE;
                bsize = (int)(bwidth < bheight ? bwidth : bheight);
                x = (int)(RectControl.X + 0 * RectControl.Width / ctrlNums + SPACE + (RectControl.Width / ctrlNums - bsize) / 2);
                y = RectControl.Y + SPACE + (int)((bheight - bsize) / 2.0f);
                OperateButtons["Start"].Rect = new Rectangle(x, y, bsize, bsize);
                x = (int)(RectControl.X + 1 * RectControl.Width / ctrlNums + SPACE + (RectControl.Width / ctrlNums - bsize) / 2);
                y = RectControl.Y + SPACE + (int)((bheight - bsize) / 2.0f);
                OperateButtons["Reset"].Rect = new Rectangle(x, y, bsize, bsize);
                x = (int)(RectControl.X + 2 * RectControl.Width / ctrlNums + SPACE + (RectControl.Width / ctrlNums - bsize) / 2);
                y = RectControl.Y + SPACE + (int)((bheight - bsize) / 2.0f);
                OperateButtons["Speed"].Rect = new Rectangle(x, y, bsize, bsize);
                x = (int)(RectControl.X + 3 * RectControl.Width / ctrlNums + SPACE + (RectControl.Width / ctrlNums - bsize) / 2);
                y = RectControl.Y + SPACE + (int)((bheight - bsize) / 2.0f);
                OperateButtons["LevelAdd"].Rect = new Rectangle(x, y, bsize, bsize);
                x = (int)(RectControl.X + 4 * RectControl.Width / ctrlNums + SPACE + (RectControl.Width / ctrlNums - bsize) / 2);
                y = RectControl.Y + SPACE + (int)((bheight - bsize) / 2.0f);
                OperateButtons["LevelSub"].Rect = new Rectangle(x, y, bsize, bsize);
            }

            //------------------------------------------------------------------------------
            // 设置所有按钮的关联单击事件
            //------------------------------------------------------------------------------
            OperateButtons["Up"].MouseClick += (sender, e) =>
            {
                //RotateShape();
                StepQuarterRotateTetrisShape();
            };
            OperateButtons["Down"].MouseClick += (sender, e) =>
            {
                //TryMoveDown();
                StepMoveTetrisDown();
            };
            OperateButtons["Left"].MouseClick += (sender, e) =>
            {
                //TryMoveLeft();
                StepMoveTetrisLeft();
            };
            OperateButtons["Right"].MouseClick += (sender, e) =>
            {
                //TryMoveRight();
                StepMoveTetrisRight();
            };
            OperateButtons["Rotate"].MouseClick += (sender, e) =>
            {
                //RotateShape();
                StepQuarterRotateTetrisShape();
            };
            OperateButtons["Start"].MouseClick += (sender, e) =>
            {
                TetrisButton? btn = sender as TetrisButton;
                if (btn != null)
                {
                    if (Status != GameStatus.GamePlay)
                    {
                        PlayGame();
                        btn.Text = "关";
                    }
                    else
                    {
                        PauseGame();
                        btn.Text = "开";
                    }
                }
            };
            OperateButtons["Reset"].MouseClick += (sender, e) =>
            {
                InitializeMatrixs();
            };
            OperateButtons["Speed"].MouseClick += (sender, e) =>
            {
                AdjustSpeed();
            };
            OperateButtons["LevelAdd"].MouseClick += (sender, e) =>
            {
                AdjustLevel(true);
            };
            OperateButtons["LevelSub"].MouseClick += (sender, e) =>
            {
                AdjustLevel(false);
            };

            //------------------------------------------------------------------------------
            // 更新主显示区和预显示区方块格子宽
            //------------------------------------------------------------------------------
            this.AutoFitDisplayCellSize(Rows, Cols);

            x = (RectOptNext.Width - SPACE * 2) / 5;
            y = (RectOptNext.Height - SPACE * 2) / 5;
            preCellWidth = x < y ? x : y;

            //------------------------------------------------------------------------------
            // 自适应调整所有字体大小
            //------------------------------------------------------------------------------
            this.AutoFitTextFontSize();

            this.Invalidate();
        }

        /// <summary>
        /// 初始化所有方块点阵数据
        /// </summary>
        private void InitializeMatrixs()
        {
            PanelMatrix = new List<List<int>>();
            for (int i = 0; i <= Rows; i++)
            {
                PanelMatrix.Add(new List<int>());
                for (int j = 0; j < Cols; j++)
                {
                    PanelMatrix[i].Add(0);
                }
            }

            int index = new Random().Next((int)TetrisShape.I, (int)TetrisShape.Z);
            Shape = (TetrisShape)Enum.Parse(typeof(TetrisShape), index.ToString());
            ShapePoints = BuildTetrisShapePoint(Shape, Cols / 2 - 1);

            index = new Random().Next((int)TetrisShape.I, (int)TetrisShape.Z);
            NextShape = (TetrisShape)Enum.Parse(typeof(TetrisShape), index.ToString());
            NextShapePoints = BuildTetrisShapePoint(NextShape);
        }

        /// <summary>
        /// 自动适应调整方块单元格边长
        /// 考虑到游戏时左右操作受限，而上下空间能大则大，所以优先按宽度适配
        /// </summary>
        /// <param name="rownum"></param>
        /// <param name="colnum"></param>
        /// <returns></returns>
        private void AutoFitDisplayCellSize(int rownum, int colnum)
        {
            int cellLen = CELL_SIZE_MAX;
            //int widthmax = COL_NUM_MAX * CELL_SIZE_MAX;
            //int widthmin = COL_NUM_MIN * CELL_SIZE_MIN;
            //int heightmax = ROW_NUM_MAX * CELL_SIZE_MAX;
            //int heightmin = ROW_NUM_MIN * CELL_SIZE_MIN;

            int wDisp = RectDisplay.Width - 2 * SPACE;
            int hDisp = RectDisplay.Height - 2 * SPACE;
            //if (wDisp < widthmin)
            //{
            //    cellLen = wDisp / COL_NUM_MAX;//最小方块尺寸
            //}
            //else if (wDisp > widthmax)
            //{
            //    cellLen = wDisp / COL_NUM_MIN;//最大方块尺寸
            //}
            //else if (hDisp < heightmin)
            //{
            //    cellLen = hDisp / ROW_NUM_MAX;//最小方块尺寸
            //}
            //else if (hDisp > heightmax)
            //{
            //    cellLen = hDisp / ROW_NUM_MIN;//最大方块尺寸
            //}
            //else
            //{
                //int cellmax = wDisp / COL_NUM_MIN;
                //int cellmin = wDisp / COL_NUM_MAX;

                int colsmax = wDisp / CELL_SIZE_MIN;
                int colsmin = wDisp / CELL_SIZE_MAX;
                int rowsmax = hDisp / CELL_SIZE_MIN;
                int rowsmin = hDisp / CELL_SIZE_MAX;

                colsmax = (colsmax > COL_NUM_MAX) ? COL_NUM_MAX : colsmax;
                colsmin = (colsmin < COL_NUM_MIN) ? COL_NUM_MIN : colsmin;

                rowsmax = (rowsmax > ROW_NUM_MAX) ? ROW_NUM_MAX : rowsmax;
                rowsmin = (rowsmin < ROW_NUM_MIN) ? ROW_NUM_MIN : rowsmin;

                colnum = (colnum < colsmin) ? colsmin : colnum;
                colnum = (colnum > colsmax) ? colsmax : colnum;
                rownum = (rownum < rowsmin) ? rowsmin : rownum;
                rownum = (rownum > rowsmax) ? rowsmax : rownum;

                bool flag = false;
                List<(int Col, int Row, int CellSize)> tList = new List<(int Col, int Row, int CellSize)>();
                for (int i = colsmax; i >= colsmin; i--)
                {
                    for (int j = rowsmax; j >= rowsmin; j--)
                    {
                        int w = wDisp / colnum;
                        w = w > CELL_SIZE_MAX ? CELL_SIZE_MAX : w;
                        for(int k = w; k >= CELL_SIZE_MIN; k--)
                        {
                            if (i * k < wDisp && j * k < hDisp)
                            {
                                tList.Add((i, j, k));
                                //flag = true;
                            }
                        }
                    }
                }

                flag = false;
                foreach (var item in tList)
                {
                    if (item.Col == colnum && item.Row == rownum)
                    {
                        cellLen = item.CellSize;
                        flag = true;
                        break;
                    }
                }

                if (!flag)
                {
                    flag = false;
                }

            //}

            //更新全局字段
            this.cellSize = cellLen;
            this.rows = rownum;
            this.cols = colnum;

            //同步更新点阵数据
            this.InitializeMatrixs();
        }

        private void AutoFitTextFontSize()
        {
            Size textSize;
            int timeout = 20;
            while (timeout-- > 0)
            {
                textSize = TextRenderer.MeasureText($"{Status}", FontStatus);
                if (textSize.Width > RectDisplay.Width)
                {
                    float fontsize = FontStatus.Size * 0.9f;
                    FontStatus = new Font(FontStatus.FontFamily, fontsize);
                }
                else if (textSize.Width < RectDisplay.Width * 3 / 4)
                {
                    float fontsize = FontStatus.Size * 1.1f;
                    FontStatus = new Font(FontStatus.FontFamily, fontsize);
                }
                else
                {
                    break;
                }
            }

            timeout = 20;
            while (timeout-- > 0)
            {
                textSize = TextRenderer.MeasureText(TEXT_STATUS, FontNotice);
                if (textSize.Width > RectDisplay.Width)
                {
                    float fontsize = FontNotice.Size * 0.9f;
                    FontNotice = new Font(FontNotice.FontFamily, fontsize);
                }
                else if (textSize.Width < RectDisplay.Width * 3 / 4)
                {
                    float fontsize = FontNotice.Size * 1.1f;
                    FontNotice = new Font(FontNotice.FontFamily, fontsize);
                }
                else
                {
                    break;
                }
            }

            timeout = 20;
            while (timeout-- > 0)
            {
                textSize = TextRenderer.MeasureText($"Score\n {Score * 1000}", FontScore);
                if (textSize.Width > RectOptScore.Width || textSize.Height > RectOptScore.Height)
                {
                    float fontsize = FontScore.Size * 0.9f;
                    FontScore = new Font(FontScore.FontFamily, fontsize);
                }
                else if (textSize.Width < RectOptScore.Width / 2)
                {
                    float fontsize = FontScore.Size * 1.1f;
                    FontScore = new Font(FontScore.FontFamily, fontsize);
                }
                else
                {
                    break;
                }
            }

            timeout = 20;
            while (timeout-- > 0)
            {
                textSize = TextRenderer.MeasureText($"Level\n {Level}", FontLevel);
                if (textSize.Width > RectOptLevel.Width || textSize.Height > RectOptLevel.Height)
                {
                    float fontsize = FontLevel.Size * 0.9f;
                    FontLevel = new Font(FontLevel.FontFamily, fontsize);
                }
                else if (textSize.Width < RectOptScore.Width / 2)
                {
                    float fontsize = FontLevel.Size * 1.1f;
                    FontLevel = new Font(FontLevel.FontFamily, fontsize);
                }
                else
                {
                    FontSpeed = new Font(FontLevel.FontFamily, FontLevel.Size);//字体相同
                    break;
                }
            }
        }

        /// <summary>
        /// List<List<int>>二维列表的深度拷贝（真实数据复制，而不是复制引用）
        /// </summary>
        /// <param name="source">源二维列表</param>
        /// <returns>新拷贝创建的List<List<int>>二维列表</returns>
        private List<List<int>> DeepCopyMatrix(List<List<int>> source)
        {
            List<List<int>> destin = new List<List<int>>();
            for (int i = 0; i < source.Count; i++)
            {
                destin.Add(new List<int>());
                for (int j = 0; j < source[i].Count; j++)
                {
                    destin[i].Add(source[i][j]);
                }
            }
            return destin;
        }

        /// <summary>
        /// List<List<int>>二维列表的深度拷贝（真实数据复制，而不是复制引用）
        /// </summary>
        /// <param name="source">源二维列表</param>
        /// <returns>新拷贝创建的List<List<int>>二维列表</returns>
        private List<TetrisPoint> DeepCopyListPoints(List<TetrisPoint> source)
        {
            List<TetrisPoint> destin = new List<TetrisPoint>();
            for (int i = 0; i < source.Count; i++)
            {
                destin.Add(new TetrisPoint(source[i].X, source[i].Y));
            }
            return destin;
        }

        /// <summary>
        /// 从资源文件播放背景音乐
        /// </summary>
        /// <returns>操作结果</returns>
        private bool PlayBackground()
        {
            try
            {
                if (resourceManager == null)
                {
                    resourceManager = new ResourceManager(typeof(TetrisPanel));
                }
                Stream? stream = resourceManager.GetStream("Background");
                if (stream != null)
                {
                    // 使用资源流创建 SoundPlayer 实例
                    bgSoundPlayer = new SoundPlayer(stream);
                    //循环播放音频
                    bgSoundPlayer.PlayLooping();
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return false;
        }

        [DllImport("winmm.dll", SetLastError = true)]
        private static extern bool sndPlaySound(byte[] audioData, uint soundLength, uint uFlags);

        const uint SND_MEMORY = 0x0004;// SND_MEMORY 表示音频数据在内存中
        const uint SND_ASYNC = 0x0001;// SND_ASYNC 表示异步播放
        const uint SND_NODEFAULT = 0x0002;// SND_NODEFAULT 表示如果找不到音频则不播放系统默认声音

        /// <summary>
        /// 从资源文件播放背景音乐
        /// </summary>
        /// <param name="resxNameWav">资源文件名</param>
        /// <returns>操作结果</returns>
        public bool PlaySound(Stream? stream)
        {
            try
            {
                if (stream != null)
                {
                    byte[] audioData = new byte[stream.Length];
                    int readCount = stream.Read(audioData, 0, audioData.Length);
                    uint soundLength = (uint)readCount;

                    bool result = sndPlaySound(audioData, soundLength, SND_MEMORY | SND_ASYNC | SND_NODEFAULT);

                    int errorCode = Marshal.GetLastWin32Error();
                    return result;
                }
                else
                {
                    Console.WriteLine("Resource not found.");
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
                Console.WriteLine(ex.Message);
            }
            return false;
        }

        /// <summary>
        /// 碰撞检测
        /// </summary>
        /// <param name="tp">本体俄罗斯面板</param>
        /// <returns>碰撞类型</returns>
        private CollisionType CheckTetrisCollision(List<TetrisPoint> tpList)
        {
            for (int i = 0; i < 4; i++)
            {
                if (tpList[i].Y > this.Rows - 1)
                    return CollisionType.Down;
                else if (tpList[i].Y < 0)
                    return CollisionType.Up;
                else if (tpList[i].X > this.Cols - 1)
                    return CollisionType.Right;
                else if (tpList[i].X < 0)
                    return CollisionType.Left;
                else if (this.PanelMatrix[tpList[i].Y][tpList[i].X] != 0)
                {
                    return CollisionType.Note;// 5: note crash
                }
            }
            return CollisionType.None;//0: no crash
        }

        /// <summary>
        /// 碰撞检测
        /// </summary>
        /// <param name="tp">本体俄罗斯面板</param>
        /// <returns>碰撞类型</returns>
        private CollisionType CheckCollision(List<List<int>> matrix)
        {
            for (int i = 0; i < 4; i++)
            {
                int m = matrix[i][0];
                int n = matrix[i][1];

                if (m > this.Rows - 1)
                    return CollisionType.Down;
                else if (m < 0)
                    return CollisionType.Up;
                else if (n > this.Cols - 1)
                    return CollisionType.Right;
                else if (n < 0)
                    return CollisionType.Left;
                else if (this.PanelMatrix[m][n] != 0)
                {
                    return CollisionType.Note;// 5: note crash
                }
            }
            return CollisionType.None;//0: no crash
        }


        /// <summary>
        /// 检查是否有满行，如果有则进行消除，并更新
        /// </summary>
        /// <returns>返回满行的行数</returns>
        private int CheckPanelHasRowFillFullAndErase()
        {
            foreach (var ptr in this.ShapePoints)
            {
                this.PanelMatrix[ptr.Y][ptr.X] = (int)Shape;
            }

            List<int> listDel = new List<int>();
            for (int i = 0; i < this.PanelMatrix.Count; i++)
            {
                bool flag = true;
                for (int j = 0; j < this.PanelMatrix[i].Count; j++)
                {
                    if (this.PanelMatrix[i][j] == 0)
                    {
                        flag = false;
                    }
                }
                if (flag)
                { //判断是否满行
                    listDel.Add(i);
                }
            }

            if (listDel.Count > 0)
            {
                //PlaySound("FullLine");//播放满行消除提示音效
                OnRowFillFull(this, listDel);
            }

            for (int i = listDel.Count - 1; i >= 0; i--)
            {
                this.PanelMatrix.RemoveAt(listDel[i]);//删除对应的满行
                List<int> listNew = new List<int>();
                for (int j = 0; j < Cols; j++)
                {
                    listNew.Add(0);
                }
                this.PanelMatrix.Insert(0, listNew);
            }

            return listDel.Count;
        }

        //******************************************************************************************
        #endregion


        #region 【公开方法】
        //******************************************************************************************

        /// <summary>
        /// 尝试将俄罗斯方块向下移动一行，如果不会产生碰撞就移动，否则忽略
        /// </summary>
        /// <returns>成功移动则为true，遇到障碍则为false</returns>
        public bool TryStepMoveTetrisDown(ref List<TetrisPoint> shapePoints)
        {
            if (Status != GameStatus.GamePlay)
                return false;

            bool bResult = false;
            lock (lockObject)
            {
                List<TetrisPoint> points = DeepCopyListPoints(shapePoints);
                for (int i = 0; i < points.Count; i++)
                {
                    points[i].Y += 1;//尝试评估能否整体下移一行
                }

                CollisionType collisionType = this.CheckTetrisCollision(points);
                if (collisionType == CollisionType.None)
                {//评估该操作不会产生碰撞，则执行该操作
                    for (int i = 0; i < shapePoints.Count; i++)
                    {
                        shapePoints[i].Y += 1;//当前形态方块组执行整体下移一行
                    }
                    this.Invalidate();
                    bResult = true;
                }
            }
            //PlaySound("Click");
            this.Invalidate();
            return bResult;
        }

        /// <summary>
        /// 尝试将俄罗斯方块向上移动一行，如果不发生碰撞就移动，否则无效
        /// </summary>
        /// <returns>成功移动则为true，遇到障碍则为false</returns>
        public bool TryStepMoveTetrisUp(ref List<TetrisPoint> shapePoints)
        {
            if (Status != GameStatus.GamePlay)
                return false;

            bool bResult = false;
            lock (lockObject)
            {
                List<TetrisPoint> points = DeepCopyListPoints(shapePoints);
                for (int i = 0; i < points.Count; i++)
                {
                    points[i].Y -= 1;//当前形态方块组尝试整体上移一行
                }

                CollisionType collisionType = this.CheckTetrisCollision(points);
                if (collisionType == CollisionType.None)
                {//评估该操作不会产生碰撞，则执行该操作

                    for (int i = 0; i < shapePoints.Count; i++)
                    {
                        shapePoints[i].Y -= 1;//当前形态方块组执行整体上移一行
                    }
                    this.Invalidate();
                    bResult = true;
                }
            }
            //PlaySound("Click");
            this.Invalidate();
            return bResult;
        }

        /// <summary>
        /// 尝试将俄罗斯方块向左移动一列，如果不发生碰撞就移动，否则无效
        /// </summary>
        /// <returns>成功移动则为true，遇到障碍则为false</returns>
        public bool TryStepMoveTetrisLeft(ref List<TetrisPoint> shapePoints)
        {
            if (Status != GameStatus.GamePlay)
                return false;

            bool bResult = false;
            lock (lockObject)
            {
                List<TetrisPoint> points = DeepCopyListPoints(shapePoints);
                for (int i = 0; i < points.Count; i++)
                {
                    points[i].X -= 1;//当前形态方块组尝试评估能否整体左移一列
                }

                CollisionType collisionType = this.CheckTetrisCollision(points);
                if (collisionType == CollisionType.None)
                {//评估该操作不会产生碰撞，则执行该操作

                    for (int i = 0; i < shapePoints.Count; i++)
                    {
                        shapePoints[i].X -= 1;//当前形态方块组执行整体左移一列
                    }
                    this.Invalidate();
                    bResult = true;
                }
            }

            //PlaySound("Click");
            this.Invalidate();
            return bResult;
        }

        /// <summary>
        /// 尝试将俄罗斯方块向右移动一列，如果不发生碰撞就移动，否则无效
        /// </summary>
        /// <returns>成功移动则为true，遇到障碍则为false</returns>
        public bool TryStepMoveTetrisRight(ref List<TetrisPoint> shapePoints)
        {
            if (Status != GameStatus.GamePlay)
                return false;

            bool bResult = false;
            lock (lockObject)
            {
                List<TetrisPoint> points = DeepCopyListPoints(shapePoints);
                for (int i = 0; i < points.Count; i++)
                {
                    points[i].X += 1;//当前形态方块组尝试能否整体右移一列
                }

                CollisionType collisionType = this.CheckTetrisCollision(points);
                if (collisionType == CollisionType.None)
                {//评估该操作不会产生碰撞，则执行该操作

                    for (int i = 0; i < shapePoints.Count; i++)
                    {
                        shapePoints[i].X += 1;//当前形态方块组执行整体右移一列
                    }
                    this.Invalidate();
                    bResult = true;
                }
            }

            //PlaySound("Click");
            this.Invalidate();
            return bResult;
        }

        /// <summary>
        /// 将俄罗斯方块整体逆时针旋转90度
        /// </summary>
        /// <returns>成功移动则为true，遇到障碍则为false</returns>
        public bool QuarterRotateTetrisShape(ref List<TetrisPoint> shapePoints)
        {
            if (Status != GameStatus.GamePlay)
                return false;

            lock (lockObject)
            {
                bool bResult = false;
                List<TetrisPoint> points = DeepCopyListPoints(shapePoints);

                if (Shape == TetrisShape.O)
                    return true;

                for (int i = 0; i < shapePoints.Count; i++)
                {//以第2个数据为中心逆时针旋转处理, 以显示坐标系向右向下为正
                    int dx = shapePoints[i].X - shapePoints[1].X;
                    int dy = shapePoints[i].Y - shapePoints[1].Y;
                    points[i].X = shapePoints[1].X + dy;
                    points[i].Y = shapePoints[1].Y - dx;

                    //if (dx >= 0 && dy < 0)
                    //{//第1象限转第2象限
                    //    points[i].X = shapePoints[1].X + dy;
                    //    points[i].Y = shapePoints[1].Y - dx;
                    //}
                    //else if (dx < 0 && dy < 0)
                    //{//第2象限转第3象限
                    //    points[i].X = shapePoints[1].X + dy;
                    //    points[i].Y = shapePoints[1].Y - dx;
                    //}
                    //else if (dx < 0 && dy >= 0)
                    //{//第3象限转第4象限
                    //    points[i].X = shapePoints[1].X + dy;
                    //    points[i].Y = shapePoints[1].Y - dx;
                    //}
                    //else if (dx >= 0 && dy >= 0)
                    //{//第4象限转第1象限
                    //    points[i].X = shapePoints[1].X + dy;
                    //    points[i].Y = shapePoints[1].Y - dx;
                    //}
                }
                CollisionType collisionType = this.CheckTetrisCollision(points);

                switch (collisionType)
                {
                    case CollisionType.None:
                        bResult = true;
                        break;
                    case CollisionType.Down:
                        if (TryStepMoveTetrisUp(ref points)) //下边界碰撞则上移调整
                            bResult = true;
                        break;
                    case CollisionType.Up:
                        if (TryStepMoveTetrisDown(ref points))   //上边界碰撞则下移调整
                            bResult = true;
                        break;
                    case CollisionType.Left:
                        while (!TryStepMoveTetrisRight(ref points))
                        {

                        }
                        //if (TryMoveRight())  //左边界碰撞则右移调整
                        bResult = true;
                        break;
                    case CollisionType.Right:
                        while (!TryStepMoveTetrisLeft(ref points))
                        {

                        }
                        //if (TryMoveLeft())   //右边界碰撞则左移调整
                        bResult = true;
                        break;
                    case CollisionType.Note:
                    default:
                        break;
                }

                collisionType = this.CheckTetrisCollision(points);
                if (collisionType == CollisionType.None)
                {//评估该操作不会产生碰撞，则执行该操作
                    shapePoints = this.DeepCopyListPoints(points);
                }

                this.Invalidate();
                return bResult;
            }
        }

        public void AdjustSpeed()
        {
            lock (lockObject)
            {
                int spTemp = Speed;
                spTemp++;
                if (spTemp > SPEED_MAX)
                {
                    spTemp = SPEED_MIN;
                }
                Speed = spTemp;
            }
        }

        /// <summary>
        /// 调整游戏难度
        /// </summary>
        /// <param name="postive">true：增加难度; false：减小难度</param>
        public void AdjustLevel(bool postive)
        {
            lock (lockObject)
            {
                int spTemp = Level;
                if (postive && ++spTemp > LEVEL_MAX)
                {
                    spTemp = LEVEL_MIN;
                }
                else if (!postive && --spTemp < LEVEL_MIN)
                {
                    spTemp = LEVEL_MAX;
                }
                Level = spTemp;
            }
        }

        public void StepMoveTetrisUp()
        {
            TryStepMoveTetrisUp(ref ShapePoints);
        }
        public void StepMoveTetrisDown()
        {
            TryStepMoveTetrisDown(ref ShapePoints);
        }
        public void StepMoveTetrisLeft()
        {
            TryStepMoveTetrisLeft(ref ShapePoints);
        }
        public void StepMoveTetrisRight()
        {
            TryStepMoveTetrisRight(ref ShapePoints);
        }
        public void StepQuarterRotateTetrisShape()
        {
            QuarterRotateTetrisShape(ref ShapePoints);
        }

        public void SimulateButtonMoveUp(bool press)
        {            
            TetrisButton btn = OperateButtons["Up"];
            MouseEventArgs e = new MouseEventArgs(MouseButtons.Left, 1,
                btn.Rect.X + btn.Rect.Width / 2, btn.Rect.Y + btn.Rect.Height / 2, 0);

            if (press)
                OnMouseDown(e);// 模拟鼠标按下事件
            else
                OnMouseUp(e);// 模拟鼠标抬起事件

            //btn.OnMouseClick(btn, e);
        }
        public void SimulateButtonMoveDown(bool press)
        {
            TetrisButton btn = OperateButtons["Down"];
            MouseEventArgs e = new MouseEventArgs(MouseButtons.Left, 1,
                btn.Rect.X + btn.Rect.Width / 2, btn.Rect.Y + btn.Rect.Height / 2, 0);

            if(press)
                OnMouseDown(e);// 模拟鼠标按下事件
            else
                OnMouseUp(e);// 模拟鼠标抬起事件

            //btn.OnMouseClick(btn, e);
        }
        public void SimulateButtonMoveLeft(bool press)
        {
            TetrisButton btn = OperateButtons["Left"];
            MouseEventArgs e = new MouseEventArgs(MouseButtons.Left, 1,
                btn.Rect.X + btn.Rect.Width / 2, btn.Rect.Y + btn.Rect.Height / 2, 0);

            if (press)
                OnMouseDown(e);// 模拟鼠标按下事件
            else
                OnMouseUp(e);// 模拟鼠标抬起事件

            //btn.OnMouseClick(btn, e);
        }
        public void SimulateButtonMoveRight(bool press)
        {
            TetrisButton btn = OperateButtons["Right"];
            MouseEventArgs e = new MouseEventArgs(MouseButtons.Left, 1,
                btn.Rect.X + btn.Rect.Width / 2, btn.Rect.Y + btn.Rect.Height / 2, 0);

            if (press)
                OnMouseDown(e);// 模拟鼠标按下事件
            else
                OnMouseUp(e);// 模拟鼠标抬起事件

            //btn.OnMouseClick(btn, e);
        }

        public void SimulateButtonRotate(bool press)
        {
            TetrisButton btn = OperateButtons["Rotate"];
            MouseEventArgs e = new MouseEventArgs(MouseButtons.Left, 1,
                btn.Rect.X + btn.Rect.Width / 2, btn.Rect.Y + btn.Rect.Height / 2, 0);

            if (press)
                OnMouseDown(e);// 模拟鼠标按下事件
            else
                OnMouseUp(e);// 模拟鼠标抬起事件

            //btn.OnMouseClick(btn, e);
        }

        /// <summary>
        /// 游戏结束判断
        /// </summary>
        /// <returns></returns>
        public bool JudgeGameOver()
        {
            for (int i = 0; i < 2; i++)
            {
                foreach (var value in this.PanelMatrix[i])
                {
                    if (value != 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 根据俄罗斯方块形态获取对应的形态坐标数据包
        /// </summary>
        /// <param name="shape">俄罗斯方块形态</param>
        /// <param name="start">俄罗斯方块中心起始位置</param>
        public List<TetrisPoint> BuildTetrisShapePoint(TetrisShape shape, int start = 0)
        {
            //this.Shape = shape;
            List<TetrisPoint> shapePoints = new List<TetrisPoint>(4);
            switch (shape)
            {
                case TetrisShape.I:
                    shapePoints.Add(new TetrisPoint(start + 0, 0));
                    shapePoints.Add(new TetrisPoint(start + 1, 0));
                    shapePoints.Add(new TetrisPoint(start + 2, 0));
                    shapePoints.Add(new TetrisPoint(start + 3, 0));
                    break;
                case TetrisShape.J:
                    shapePoints.Add(new TetrisPoint(start + 0, 0));
                    shapePoints.Add(new TetrisPoint(start + 1, 0));
                    shapePoints.Add(new TetrisPoint(start + 2, 0));
                    shapePoints.Add(new TetrisPoint(start + 2, 1));
                    //this.ShapePoints = new List<List<int>>()
                    //{
                    //    new List<int> { 0, start + 0 }, new List<int> { 0, start + 1 },
                    //    new List<int> { 0, start + 2 }, new List<int> { 1, start + 0 },
                    //};
                    break;
                case TetrisShape.L:
                    shapePoints.Add(new TetrisPoint(start + 0, 0));
                    shapePoints.Add(new TetrisPoint(start + 1, 0));
                    shapePoints.Add(new TetrisPoint(start + 2, 0));
                    shapePoints.Add(new TetrisPoint(start + 0, 1));
                    //this.ShapePoints = new List<List<int>>()
                    //{
                    //    new List<int> { 0, start + 0 }, new List<int> { 0, start + 1 },
                    //    new List<int> { 0, start + 2 }, new List<int> { 1, start + 2 },
                    //};
                    break;
                case TetrisShape.O:
                    shapePoints.Add(new TetrisPoint(start + 2, 0));
                    shapePoints.Add(new TetrisPoint(start + 1, 0));
                    shapePoints.Add(new TetrisPoint(start + 1, 1));
                    shapePoints.Add(new TetrisPoint(start + 2, 1));
                    //this.ShapePoints = new List<List<int>>()
                    //{
                    //    new List<int> { 0, start + 1 }, new List<int> { 0, start + 2 },
                    //    new List<int> { 1, start + 1 }, new List<int> { 1, start + 2 },
                    //};
                    break;
                case TetrisShape.S:
                    shapePoints.Add(new TetrisPoint(start + 2, 0));
                    shapePoints.Add(new TetrisPoint(start + 1, 0));
                    shapePoints.Add(new TetrisPoint(start + 1, 1));
                    shapePoints.Add(new TetrisPoint(start + 0, 1));
                    //this.ShapePoints = new List<List<int>>()
                    //{
                    //    new List<int> { 0, start + 1 }, new List<int> { 0, start + 2 },
                    //    new List<int> { 1, start + 0 }, new List<int> { 1, start + 1 },
                    //};
                    break;
                case TetrisShape.T:
                    shapePoints.Add(new TetrisPoint(start + 0, 0));
                    shapePoints.Add(new TetrisPoint(start + 1, 0));
                    shapePoints.Add(new TetrisPoint(start + 2, 0));
                    shapePoints.Add(new TetrisPoint(start + 1, 1));
                    //this.ShapePoints = new List<List<int>>()
                    //{
                    //    new List<int> { 0, start + 0 }, new List<int> { 0, start + 1 },
                    //    new List<int> { 0, start + 2 }, new List<int> { 1, start + 1 },
                    //};
                    break;
                case TetrisShape.Z:
                    shapePoints.Add(new TetrisPoint(start + 0, 0));
                    shapePoints.Add(new TetrisPoint(start + 1, 0));
                    shapePoints.Add(new TetrisPoint(start + 1, 1));
                    shapePoints.Add(new TetrisPoint(start + 2, 1));
                    //this.ShapePoints = new List<List<int>>()
                    //{
                    //    new List<int> { 0, start + 0 }, new List<int> { 0, start + 1 },
                    //    new List<int> { 1, start + 1 }, new List<int> { 1, start + 2 },
                    //};
                    break;
                default:
                    break;
            }
            return shapePoints;
        }

        public void PlayGame()
        {
            if (Status == GameStatus.GameOver)
            {
                InitializeMatrixs();
                this.PlayBackground();
            }

            timerMove.Enabled = true;
            Status = GameStatus.GamePlay;
        }

        public void PauseGame()
        {
            timerMove.Enabled = false;
            Status = GameStatus.GamePause;
        }


        /// <summary>
        /// 从资源文件播放背景音乐
        /// </summary>
        /// <returns>操作结果</returns>
        public bool PlayBackground(Stream? stream)
        {
            try
            {
                if (stream != null)
                {
                    // 使用资源流创建 SoundPlayer 实例
                    bgSoundPlayer = new SoundPlayer(stream);
                    //循环播放音频
                    bgSoundPlayer.PlayLooping();
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return false;
        }

        //******************************************************************************************
        #endregion


        #region 【控件事件】
        //******************************************************************************************
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            this.InitializeLayout();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            try
            {
                //------------------------------------------------------------------------------
                // 绘制主显示区
                //------------------------------------------------------------------------------
                PointF location = new PointF();
                location.X = RectDisplay.X + (RectDisplay.Width - Cols * CellSize) / 2.0f;
                location.Y = RectDisplay.Y + (RectDisplay.Height - Rows * CellSize) / 2.0f;

                using (Pen penGrid = new Pen(Color.Gray, 1.0f))
                {
                    for (int i = 0; i <= Rows; i++)
                    {
                        PointF pl = new PointF(location.X, location.Y + i * CellSize);
                        PointF pr = new PointF(location.X + Cols * CellSize, location.Y + i * CellSize);
                        e.Graphics.DrawLine(penGrid, pl, pr);
                    }

                    for (int i = 0; i <= Cols; i++)
                    {
                        PointF pu = new PointF(location.X + i * CellSize, location.Y);
                        PointF pd = new PointF(location.X + i * CellSize, location.Y + Rows * CellSize);
                        e.Graphics.DrawLine(penGrid, pu, pd);
                    }
                }

                for (int i = 0; i < PanelMatrix.Count; i++)
                {
                    for (int j = 0; j < PanelMatrix[i].Count; j++)
                    {
                        int data = PanelMatrix[i][j];
                        if (data != 0)
                        {
                            using (SolidBrush brush = new SolidBrush(ColorList[data]))
                            {
                                e.Graphics.FillRectangle(brush, location.X + CellSize * j + 1,
                                    location.Y + CellSize * i + 1, CellSize - 3, CellSize - 3);
                            }
                        }
                    }
                }

                for (int i = 0; i < ShapePoints.Count; i++)
                {
                    using (SolidBrush brush = new SolidBrush(ColorList[(int)Shape]))
                    {
                        float x = location.X + CellSize * ShapePoints[i].X + 1;
                        float y = location.Y + CellSize * ShapePoints[i].Y + 1;
                        RectangleF rect = new RectangleF(x, y, CellSize - 3, CellSize - 3);
                        e.Graphics.FillRectangle(brush, rect);
                    }
                }

                if (Status != GameStatus.GamePlay)
                {
                    Size sizeStatus = TextRenderer.MeasureText(Status.ToString(), FontStatus);
                    using (SolidBrush brush = new SolidBrush(Color.Red))
                    {
                        float x = RectDisplay.X + SPACE * 2;
                        float y = RectDisplay.Y + SPACE + (RectDisplay.Height - sizeStatus.Height * 2) / 2;
                        RectangleF rect = new RectangleF(x, y, RectDisplay.Width - 4 * SPACE,
                            RectDisplay.Height - (RectDisplay.Height - sizeStatus.Height * 2));
                        e.Graphics.FillRectangle(brush, rect);
                    }

                    TextRenderer.DrawText(e.Graphics, $"{Status}", FontStatus, RectDisplay, Color.Yellow, 
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

                    TextRenderer.DrawText(e.Graphics, TEXT_STATUS, FontNotice, RectDisplay, Color.Yellow, 
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.Bottom);
                    //TextRenderer.DrawText(e.Graphics, $"{RectDisplay.Size}, {Rows}行, {Cols}列, 格宽{CellSize}", new Font("Impact", 16.0f, FontStyle.Bold),
                    //    RectDisplay, Color.Yellow, TextFormatFlags.HorizontalCenter | TextFormatFlags.Bottom);
                }
                using (Pen pen = new Pen(Color.Red, 1.0f))
                {
                    e.Graphics.DrawRectangle(pen, RectDisplay);
                }

                //------------------------------------------------------------------------------
                // 绘制配置信息区
                //------------------------------------------------------------------------------

                //显示游戏得分
                TextRenderer.DrawText(e.Graphics, $"Score\n {Score * 100}", FontScore, RectOptScore, Color.Yellow, 
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                using (Pen pen = new Pen(Color.Gray, 1.0f))
                {
                    e.Graphics.DrawRectangle(pen, RectOptScore);
                }

                //显示下一组俄罗斯方块
                for (int i = 0; i < NextShapePoints.Count; i++)
                {
                    using (SolidBrush brush = new SolidBrush(ColorList[(int)NextShape]))
                    {
                        float x = RectOptNext.X + (RectOptNext.Width - 3 * preCellWidth) / 2 + preCellWidth * NextShapePoints[i].X;
                        float y = RectOptNext.Y + (RectOptNext.Height - 2 * preCellWidth) / 2 + preCellWidth * NextShapePoints[i].Y;
                        RectangleF rect = new RectangleF(x, y, preCellWidth - 2, preCellWidth - 2);
                        e.Graphics.FillRectangle(brush, rect);
                    }
                }
                using (Pen pen = new Pen(Color.Gray, 1.0f))
                {
                    e.Graphics.DrawRectangle(pen, RectOptNext);
                }

                //显示游戏难度
                TextRenderer.DrawText(e.Graphics, $"Level\n {Level}", FontLevel, RectOptLevel, Color.Yellow, 
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                using (Pen pen = new Pen(Color.Gray, 1.0f))
                {
                    e.Graphics.DrawRectangle(pen, RectOptLevel);
                }

                //显示游戏速度
                TextRenderer.DrawText(e.Graphics, $"Speed\n {Speed}", FontSpeed, RectOptSpeed, Color.Yellow, 
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                using (Pen pen = new Pen(Color.Gray, 1.0f))
                {
                    e.Graphics.DrawRectangle(pen, RectOptSpeed);
                }

                using (Pen pen = new Pen(Color.Red, 1.0f))
                {
                    e.Graphics.DrawRectangle(pen, RectOption);
                }

                //------------------------------------------------------------------------------
                // 绘制所有按钮区（上下左右方向、旋转、启停、复位、调速等按钮）
                //------------------------------------------------------------------------------
                //e.Graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                //e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                Color colorMot;
                foreach (var button in OperateButtons)
                {
                    colorMot = button.Value.IsHover ? button.Value.ColorHover : button.Value.ColorBack;
                    colorMot = button.Value.IsPress ? button.Value.ColorPress : colorMot;
                    using (SolidBrush brush = new SolidBrush(colorMot))
                    {
                        e.Graphics.FillEllipse(brush, button.Value.Rect);

                        if (button.Value.Name == "Rotate")
                        {
                            TextRenderer.DrawText(e.Graphics, $"{button.Value.Text}", FontButtonLarge, button.Value.Rect,
                                Color.Black, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                        }
                        else
                        {
                            TextRenderer.DrawText(e.Graphics, $"{button.Value.Text}", FontButtonSmall, button.Value.Rect,
                                Color.Black, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                        }
                    }
                }

                //------------------------------------------------------------------------------
                // 绘制所有按钮区域边界（上下左右方向、旋转、启停、复位、调速等按钮）
                //------------------------------------------------------------------------------
                // 绘制方向按钮区域边界
                using (Pen pen = new Pen(Color.Red, 1.0f))
                {
                    e.Graphics.DrawRectangle(pen, RectMotion);
                }

                // 绘制旋转按钮区域边界
                using (Pen pen = new Pen(Color.Red, 1.0f))
                {
                    e.Graphics.DrawRectangle(pen, RectRotate);
                }
                
                // 绘制控制按钮区域边界
                using (Pen pen = new Pen(Color.Red, 1.0f))
                {
                    e.Graphics.DrawRectangle(pen, RectControl);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            foreach (var button in OperateButtons)
            {
                if (button.Value.IsPress != button.Value.Rect.Contains(e.Location))
                {
                    button.Value.IsPress = button.Value.Rect.Contains(e.Location);
                    button.Value.OnMouseClick(button.Value, e);
                    this.Invalidate(button.Value.Rect);
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            foreach (var button in OperateButtons)
            {
                if(button.Value.IsHover != button.Value.Rect.Contains(e.Location))
                {
                    button.Value.IsHover = button.Value.Rect.Contains(e.Location);
                    this.Cursor = button.Value.IsHover ? Cursors.Hand : Cursors.Default;
                    this.Invalidate(button.Value.Rect);
                }
            }

        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            foreach (var button in OperateButtons)
            {
                if (button.Value.IsPress != false)
                {
                    button.Value.IsPress = false;
                    this.Invalidate(button.Value.Rect);
                }
            }
        }

        private void timerMove_Tick(object sender, EventArgs e)
        {
            lock (lockObject)
            {
                switch (Status)
                {
                    case GameStatus.GameReady:
                        break;
                    case GameStatus.GamePlay:
                        //if (TryMoveDown() == false)
                        if (TryStepMoveTetrisDown(ref ShapePoints) == false)
                        {
                            //Score += CheckRowFillFullAndErase();
                            Score += CheckPanelHasRowFillFullAndErase();
                            Shape = NextShape;
                            ShapePoints = BuildTetrisShapePoint(Shape, Cols / 2 - 1);

                            int index = new Random().Next((int)TetrisShape.I, (int)TetrisShape.Z + 1);
                            NextShape = (TetrisShape)Enum.Parse(typeof(TetrisShape), index.ToString());

                            //count++;
                            //if (count > (int)TetrisShape.Z)
                            //    count = (int)TetrisShape.I;
                            //NextShape = (TetrisShape)Enum.Parse(typeof(TetrisShape), count.ToString());
                            NextShapePoints = BuildTetrisShapePoint(NextShape);
                        }

                        if (JudgeGameOver())
                        {
                            Status = GameStatus.GameOver;
                            //PlaySound("GameOver");
                            OnGameOver(this, new EventArgs());
                        }
                        break;
                    case GameStatus.GamePause:
                        break;
                    case GameStatus.GameOver:
                        break;
                    default:
                        break;
                }
            }
        }
        //******************************************************************************************
        #endregion

    }
}
