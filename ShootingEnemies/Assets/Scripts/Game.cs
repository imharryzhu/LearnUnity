using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField, Tooltip("游戏地面大小")]
    Vector2Int boardSize = new Vector2Int(11, 11);

    [SerializeField, Tooltip("游戏地面")]
    GameBoard board;


#region Unity Life Functions

    private void Awake()
    {
        // 初始化游戏地面
        board.Initialized(boardSize);
    }

    /// <summary>
    /// 当Inspector中的值被修改时
    /// </summary>
    private void OnValidate()
    {
        if (boardSize.x < 2)
        {
            boardSize.x = 2;
        }
        if (boardSize.y < 2)
        {
            boardSize.y = 2;
        }
    }


    #endregion
}
