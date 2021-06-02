using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTile : MonoBehaviour
{
    [SerializeField]
    Transform arrow;

    // 记录当前格子上下左右的邻格
    public GameTile north, east, south, west;

#region 建立格子关系
    public static void MakeEastWestNeighbors(GameTile east, GameTile west)
    {
        if (!east.west || !west.east)
        {
            Debug.LogError("错误！不允许重复定义左右格子");
        }
        west.east = east;
        east.west = east;
    }

    public static void MakeNorthSouthNeighbors(GameTile north, GameTile south)
    {
        if (!north.south || !south.north)
        {
            Debug.LogError("错误！不允许重复定义上下格子");
        }
        north.south = south;
        south.north = north;
    }
#endregion
}
