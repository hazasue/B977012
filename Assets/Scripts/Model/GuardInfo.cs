using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GuardInfo
{
    public string code;
    public string guardType;
    public float width;
    public float height;
    public int count;
    public float spawnDelay;
    public float spawnDuration;
    public bool loop;

    public GuardInfo(string code, string guardType, float width, float height, int count, float spawnDelay, float spawnDuration, bool loop)
    {
        this.code = code;
        this.guardType = guardType;
        this.width = width;
        this.height = height;
        this.count = count;
        this.spawnDelay = spawnDelay;
        this.spawnDuration = spawnDuration;
        this.loop = loop;
    }
}
