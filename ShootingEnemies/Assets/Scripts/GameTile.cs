using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTile : MonoBehaviour
{
    [SerializeField]
    Transform arrow;

    // 记录当前格子上下左右的邻格
    private GameTile north, east, south, west;
    private GameTile nextOnPath;
    private int distance;

    /// <summary>
    /// 否是一个可选的
    /// </summary>
    public bool IsAlternative { get; set; }

    #region 寻路相关

    /// <summary>
    /// 箭头方向
    /// </summary>
    private static Quaternion
        northRotation = Quaternion.Euler(90f, 0f, 0f),
        eastRotation = Quaternion.Euler(90f, 90f, 0f),
        southRotation = Quaternion.Euler(90f, 180f, 0f),
        westRotation = Quaternion.Euler(90f, 270f, 0f);

    /// <summary>
    /// 判断是否对目标点有路径
    /// </summary>
    public bool HasPath => distance != int.MaxValue;

    /// <summary>
    /// 格子内容对象
    /// </summary>
    private GameTileContent content;
    public GameTileContent Content
    {
        get => content;
        set
        {
            Debug.Assert(value != null, "Null assigned to content!");
            if (content != null)
            {
                content.Recycle();
            }
            content = value;
            content.transform.localPosition = transform.localPosition;
        }
    }

    /// <summary>
    /// 重置寻路状态
    /// </summary>
    public void ClearPath()
    {
        distance = int.MaxValue;
        nextOnPath = null;
    }

    public void BecomeDestination()
    {
        distance = 0;
        nextOnPath = null;
    }

    /// <summary>
    /// 根据路径的下一个块，设置旋转方向
    /// </summary>
    public void ShowPath()
    {
        if (distance == 0)
        {
            arrow.gameObject.SetActive(false);
            return;
        }
        arrow.gameObject.SetActive(true);
        arrow.localRotation =
            nextOnPath == north ? northRotation :
            nextOnPath == south ? southRotation :
            nextOnPath == east ? eastRotation :
            westRotation;
    }

    public GameTile GrowPathNorth() => GrowPathTo(north);
    public GameTile GrowPathSouth() => GrowPathTo(south);
    public GameTile GrowPathEast() => GrowPathTo(east);
    public GameTile GrowPathWest() => GrowPathTo(west);

    private GameTile GrowPathTo(GameTile neighbor)
    {
        if (neighbor == null || neighbor.HasPath)
            return null;
        neighbor.distance = distance + 1;
        neighbor.nextOnPath = this;
        return neighbor;
    }
    #endregion

    #region 建立格子关系
    public static void MakeEastWestNeighbors(GameTile east, GameTile west)
    {
        if (east.west != null || west.east != null)
        {
            Debug.LogError("错误！不允许重复定义左右格子");
        }
        west.east = east;
        east.west = west;
    }

    public static void MakeNorthSouthNeighbors(GameTile north, GameTile south)
    {
        if (north.south != null || south.north != null)
        {
            Debug.LogError("错误！不允许重复定义上下格子");
        }
        north.south = south;
        south.north = north;
    }
    #endregion
}
