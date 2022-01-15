using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[Serializable]
public class Voice
{
    public string assetName;
    public List<float> startPoints;
    public List<float> endPoints;

    public Voice()
    {
        startPoints = new List<float>();
        endPoints = new List<float>();
    }
}