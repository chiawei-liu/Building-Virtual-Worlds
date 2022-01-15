using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Line
{
    public float audioLength;
    public string spriteName;
    public string bgmAssetName;
    public List<Voice> voices;

    public Line()
    {
        voices = new List<Voice>();
    }
}