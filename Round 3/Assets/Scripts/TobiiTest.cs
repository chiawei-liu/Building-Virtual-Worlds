using System.Collections;
using System.Collections.Generic;
using Tobii.Gaming;
using UnityEngine;

public class TobiiTest : MonoBehaviour
{

    private GazeAwareOrMouse gazeAware;
    
    // Start is called before the first frame update
    void Start()
    {
        gazeAware = GetComponent<GazeAwareOrMouse>();
    }

    // Update is called once per frame
    void Update()
    {
        if (TobiiAPIOrMouse.Instance.GetFocusedObject() == gameObject)
        {
            gameObject.transform.localScale = new Vector3(2,2,2);
        }
        // if (gazeAware.HasGazeFocused())
        // {
        //     gameObject.transform.localScale = new Vector3(2,2,2);
        // }
        var point = TobiiAPI.GetGazePoint();
    }
}
