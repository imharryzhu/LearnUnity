using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTileContentFactory : ScriptableObject
{
    public GameTileContent Get(GameTileContent prefab)
    {
        var instance = Instantiate(prefab);
        instance.OriginFactory = this;
        return instance;
    }

    /// <summary>
    /// 回收对象
    /// </summary>
    /// <param name="content"></param>
    public void Reclaim(GameTileContent content)
    {
        Debug.Assert(content.OriginFactory == this, "错误的回收工厂！");
        Destroy(content.gameObject);
    }
}
