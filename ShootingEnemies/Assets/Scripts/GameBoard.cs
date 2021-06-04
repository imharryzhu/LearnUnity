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
    private GameTile[] tiles;
    // 搜索路径的队列
    private Queue<GameTile> searchFrontier = new Queue<GameTile>();

    public void Initialized(Vector2Int size)
    {
        this.size = size;
        ground.localScale = new Vector3(size.x, size.y, 1f);

        // 由于中心点在箭头的中心，所以做个偏移
        Vector2 offset = new Vector2(
            (size.x - 1) / 2f, (size.y - 1) / 2f);

        // 初始化地面的箭头
        tiles = new GameTile[size.x * size.y];
        int i = 0;
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++, i++)
            {
                var tile = tiles[i] = Instantiate(tilePrefab);
                tile.transform.SetParent(transform, false);
                tile.transform.localPosition = new Vector3(
                    x - offset.x, 0f, y - offset.y);

                if (x > 0)
                {
                    GameTile.MakeEastWestNeighbors(tile, tiles[i - size.y]);
                }
                if (y > 0)
                {
                    GameTile.MakeNorthSouthNeighbors(tile, tiles[i - 1]);
                }

                tile.name = string.Format("Tile<{0}>({1}, {2})", i, x, y);
                
                // x坐标为偶数，设置为IsAlternative
                tile.IsAlternative = (x & 1) == 0;
                if ((y & 1) == 0)
                {
                    tile.IsAlternative = !tile.IsAlternative;
                }
            }
        }
        FindPaths();
    }

    private void FindPaths()
    {
        // 先重置所有格子的寻路数据
        foreach (var tile in tiles)
        {
            tile.ClearPath();
        }

        // 从0开始寻路
        tiles[tiles.Length / 2].BecomeDestination();
        searchFrontier.Enqueue(tiles[tiles.Length / 2]);

        while(searchFrontier.Count > 0)
        {
            GameTile tile = searchFrontier.Dequeue();
            if(tile != null)
            {
                if (tile.IsAlternative)
                {
                    searchFrontier.Enqueue(tile.GrowPathNorth());
                    searchFrontier.Enqueue(tile.GrowPathSouth());
                    searchFrontier.Enqueue(tile.GrowPathEast());
                    searchFrontier.Enqueue(tile.GrowPathWest());
                }
                else
                {
                    searchFrontier.Enqueue(tile.GrowPathWest());
                    searchFrontier.Enqueue(tile.GrowPathEast());
                    searchFrontier.Enqueue(tile.GrowPathSouth());
                    searchFrontier.Enqueue(tile.GrowPathNorth());
                }
            }
        }
        foreach (var tile in tiles)
        {
            tile.ShowPath();
        }
    }
}
