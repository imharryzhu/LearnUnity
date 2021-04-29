using UnityEngine;
using System.Collections;

public class GameLevel : MonoBehaviour
{
    [SerializeField, Tooltip("空间生成器")]
    SpawnZone spawnZone;

    void Start()
    {
        Game.Instance.spawnZoneOfLevel = spawnZone;
    }
}
