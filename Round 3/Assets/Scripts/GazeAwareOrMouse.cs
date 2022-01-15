using System;
using Tobii.Gaming;

public class GazeAwareOrMouse : GazeAware
{
    [NonSerialized] public bool HasMouseFocus;
    public bool HasGazeFocused()
    {
        return !TobiiAPIOrMouse.Instance.UseMouse ? HasGazeFocus : HasMouseFocus;
    }
}
 