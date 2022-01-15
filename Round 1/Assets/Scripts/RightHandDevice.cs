using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class RightHandDevice: MonoBehaviour
{
    private static RightHandDevice instance = null;
    public static RightHandDevice Instance => instance;

    private List<InputDevice> devices;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        } else {
            instance = this;
        }
    }

    void Start()
    {
        devices = new List<InputDevice>();
    }

    void Update()
    {
        if (devices.Count != 0) return;
        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Right, devices);
    }
    
    public IEnumerator WaitPrimaryButtonClicked()
    {
        while (true)
        {
            foreach(var device in devices)
            {
                if (!device.TryGetFeatureValue(CommonUsages.primaryButton, out var triggerValue) || !triggerValue) continue;
                Debug.Log("Trigger button is pressed.");
                yield break;
            }
            yield return null;
        }
    }

    public void SendHapticImpulse(float amplitude, float duration)
    {
        devices.ForEach(d => d.SendHapticImpulse(0, amplitude, duration));
    }

    public void StopHaptics()
    {
        devices.ForEach(d => d.StopHaptics());
    }
}
