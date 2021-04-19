using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PersistentStorage : MonoBehaviour
{
    // 存储路径
    string savePath;

    void Awake()
    {
        savePath = Path.Combine(Application.persistentDataPath, "saveFile");
    }

    public void Save(PersistableObject obj)
    {
        using (
            var writer = new BinaryWriter(File.Open(savePath, FileMode.Create))
        )
        {
            obj.Save(new GameDataWriter(writer));
        }
    }

    public void Load(PersistableObject obj)
    {
        using (
            var reader = new BinaryReader(File.Open(savePath, FileMode.Open))
        )
        {
            obj.Load(new GameDataReader(reader));
        }
    }
}
