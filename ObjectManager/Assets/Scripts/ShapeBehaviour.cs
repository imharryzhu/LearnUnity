﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ShapeBehaviour : MonoBehaviour
{
    public abstract void GameUpdate(Shape shape);
    public abstract void Save(GameDataWriter writer);
    public abstract void Load(GameDataReader reader);
}
