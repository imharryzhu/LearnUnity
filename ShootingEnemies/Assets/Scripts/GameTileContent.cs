using UnityEngine;

public enum GameTileContentType
{
    Empty, // 空格子
    Destination, // 目标格子
    Wall // 墙
}

public class GameTileContent : MonoBehaviour
{
    [SerializeField, Tooltip("格子的内容类型")]
    GameTileContentType type;
    public GameTileContentType Type => type;

    /// <summary>
    /// 格子创建工厂
    /// </summary>
    private GameTileContentFactory originFactory;
    public GameTileContentFactory OriginFactory
    {
        get => originFactory;
        set
        {
            Debug.Assert(originFactory == null, "不可以二次赋值创建工厂！");
            originFactory = value;
        }
    }

    public void Recycle()
    {
        originFactory.Reclaim(this);
    }
}
