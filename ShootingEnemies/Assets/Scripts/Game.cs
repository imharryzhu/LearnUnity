using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField, Tooltip("游戏地面大小")]
    Vector2Int boardSize = new Vector2Int(11, 11);

    [SerializeField, Tooltip("游戏地面")]
    GameBoard board;

    [SerializeField, Tooltip("格子内容工厂")]
    GameTileContentFactory tileContentFactory;
    
    // 获取鼠标在屏幕中的射线
    private Ray mouseRay => Camera.main.ScreenPointToRay(Input.mousePosition);

    private void HandleTouch()
    {
        GameTile tile = board.GetTile(mouseRay);
        if (tile != null)
        {
            board.ToggleDestination(tile);
        }
    }

#region Unity Life Functions

    private void Awake()
    {
        // 初始化游戏地面
        board.Initialized(boardSize, tileContentFactory);
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            HandleTouch();
        }
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
