using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetrisGame
{
    /// <summary>
    /// 游戏状态
    /// </summary>
    public enum GameStatus
    {
        /// <summary>
        /// 游戏已准备
        /// </summary>
        GameReady,
        /// <summary>
        /// 游戏正进行
        /// </summary>
        GamePlay,
        /// <summary>
        /// 游戏已暂停
        /// </summary>
        GamePause,
        /// <summary>
        /// 游戏已结束
        /// </summary>
        GameOver,
    }


    /// <summary>
    /// 俄罗斯方块形态枚举
    /// </summary>
    public enum TetrisShape
    {
        /// <summary>
        /// 空
        /// </summary>
        Empty,
        /// <summary>
        /// I-TetrisShape：通常被称为“直线”或“棍子”，因为它由一条直线的四个方块组成。其他名称包括“直棒”、“长线”、“长条”等
        /// </summary>
        I,
        /// <summary>
        /// J-TetrisShape：类似于字母“J”，可以用来填补空隙。其他名称包括“蓝L”、“左L”、“左钩”等 
        /// </summary>
        J,
        /// <summary>
        /// L-TetrisShape：类似于字母“L”，也可以用来填补空隙。其他名称包括“橙L”、“右L”、“右钩”等 
        /// </summary>
        L,
        /// <summary>
        /// O-TetrisShape：是一个2x2的正方形，是唯一一个没有独特旋转状态的Tetromino。其他名称包括“方块”、“圆”、“太阳”等 
        /// </summary>
        O,
        /// <summary>
        /// S-TetrisShape：类似于字母“S”，可以用来填补带有台阶的空隙。其他名称包括“右拐”、“逆斜线”、“右蛇”等 
        /// </summary>
        S,
        /// <summary>
        /// T-TetrisShape：类似于字母“T”，可以用来填补金字塔形状的空隙。其他名称包括“箭”、“小楼梯”、“领奖台”、“金字塔”等 
        /// </summary>
        T,
        /// <summary>
        /// Z-TetrisShape：类似于字母“Z”，与S-Tetromino形状相似，但是镜像对称。其他名称包括“左拐”、“斜线”、“左蛇”等 
        /// </summary>
        Z,
    }

    /// <summary>
    /// 俄罗斯方块碰撞类型枚举
    /// </summary>
    public enum CollisionType
    {
        /// <summary>
        /// 无碰撞
        /// </summary>
        None,
        /// <summary>
        /// 下边碰撞
        /// </summary>
        Down,
        /// <summary>
        /// 上边碰撞
        /// </summary>
        Up,
        /// <summary>
        /// 左边碰撞
        /// </summary>
        Left,
        /// <summary>
        /// 右边碰撞
        /// </summary>
        Right,
        /// <summary>
        /// 满行碰撞
        /// </summary>
        Note,
    }


    /// <summary>
    /// 显示模式
    /// </summary>
    public enum DisplayMode
    {
        /// <summary>
        /// 竖屏显示
        /// </summary>
        Vertical,
        /// <summary>
        /// 横屏显示
        /// </summary>
        Horizontal,
    }
}
