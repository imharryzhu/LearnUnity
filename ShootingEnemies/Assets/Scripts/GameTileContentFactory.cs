using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu]
public class GameTileContentFactory : ScriptableObject
{
    #region 成员变量
    [SerializeField, Tooltip("平面")]
    GameTileContent destionationPrefab;

    [SerializeField, Tooltip("空的")]
    GameTileContent emptyPrefab;

    private Scene contentScene;
    #endregion


    public GameTileContent Get(GameTileContent prefab)
    {
        var instance = Instantiate(prefab);
        instance.OriginFactory = this;
        MoveToFactoryScene(instance.gameObject);
        return instance;
    }

    public GameTileContent Get(GameTileContentType type)
    {
        switch (type)
        {
            case GameTileContentType.Destination:
                return Get(destionationPrefab);
            case GameTileContentType.Empty:
                return Get(emptyPrefab);
        }
        Debug.Assert(false, "未定义的格子类型: " + type);
        return null;
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

    private void MoveToFactoryScene(GameObject o)
    {
        if(!contentScene.isLoaded)
        {
            if(Application.isEditor)
            {
                contentScene = SceneManager.GetSceneByName(name);
                if(!contentScene.isLoaded)
                {
                    contentScene = SceneManager.CreateScene(name);
                }
            }
            else
            {
                contentScene = SceneManager.CreateScene(name);
            }
        }
        SceneManager.MoveGameObjectToScene(o, contentScene);
    }
}
