using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.XR;

public class InputTest : MonoBehaviour
{
    // public Action action;
    private List<InputDevice> inputDevices;
    // Start is called before the first frame update
    void Start()
    {
        inputDevices = new List<UnityEngine.XR.InputDevice>();
        UnityEngine.XR.InputDevices.GetDevices(inputDevices);

        
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var device in inputDevices)
        {
            bool primaryButton;
            if (device.TryGetFeatureValue(CommonUsages.primaryButton, out primaryButton) && primaryButton)
            {
                Debug.Log("Primary button pressed");
            }
        }
    }
    
    
}
