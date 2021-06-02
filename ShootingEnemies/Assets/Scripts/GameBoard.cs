using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    [SerializeField, Tooltip("地图")]
    Transform ground;

    [SerializeField, Tooltip("箭头的瓦片预制件")]
    GameTile tilePrefab;

    private Vector2Int size;
    private GameTile[] gameTiles;

    public void Initialized(Vector2Int size)
    {
        this.size = size;
        ground.localScale = new Vector3(size.x, size.y, 1f);

        // 由于中心点在箭头的中心，所以做个偏移
        Vector2 offset = new Vector2(
            (size.x - 1) / 2f, (size.y - 1) / 2f);

        // 初始化地面的箭头
        gameTiles = new GameTile[size.x * size.y];
        int i = 0;
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++, i++)
            {
                var tile = gameTiles[i] = Instantiate(tilePrefab);
                tile.transform.SetParent(transform, false);
                tile.transform.localPosition = new Vector3(
                    x - offset.x, 0f, y - offset.y);
            }
        }

    }
}
