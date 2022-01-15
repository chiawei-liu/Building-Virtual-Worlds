using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;
using UnityEngine;

[Serializable]
public class Song
{
    public List<Line> lines;

    public static Song CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<Song>(jsonString);
    }
}
