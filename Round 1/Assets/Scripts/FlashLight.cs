using System.Collections;
using UnityEngine;


enum FlashLightState
{
    Idle,
    Increasing,
    Decreasing,
    Activated
}

[RequireComponent(typeof(Light))]
public class FlashLight : MonoBehaviour
{
    private ScaredSibling scaredSibling;
    private FlashLightState state = FlashLightState.Idle;
    private float originalShadowStrength;
    private Light lightComponent;
    private Coroutine shadowStrengthIncreaseCoroutine, shadowStrengthDecreaseCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        originalShadowStrength = GetComponent<Light>().shadowStrength;
        lightComponent = GetComponent<Light>();
        // devices = new List<InputDevice>();
        // InputDevices.GetDevicesWithRole(InputDeviceRole.RightHanded, devices);
        scaredSibling = GameManager.Instance?.SiblingCharacter.GetComponent<ScaredSibling>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TryIncreaseShadow(float speed, float hapticAmplitude, float hapticDuration, ObjectWithShadows obj, Trigger trigger)
    {
        if (state == FlashLightState.Increasing || state == FlashLightState.Activated) return;
        if (state == FlashLightState.Decreasing) StopCoroutine(shadowStrengthDecreaseCoroutine);
        state = FlashLightState.Increasing;
        obj.DisableGraduatedTriggers();
        RightHandDevice.Instance.StopHaptics();
        RightHandDevice.Instance.SendHapticImpulse(hapticAmplitude, hapticDuration);
        shadowStrengthIncreaseCoroutine = StartCoroutine(IncreaseShadowStrength(speed, obj, trigger));
    }
    
    public void TryDecreaseShadow(float speed)
    {
        if (state == FlashLightState.Decreasing || state == FlashLightState.Idle) return;
        if (state == FlashLightState.Increasing) StopCoroutine(shadowStrengthIncreaseCoroutine);
        state = FlashLightState.Decreasing;
        RightHandDevice.Instance.StopHaptics();
        shadowStrengthDecreaseCoroutine = StartCoroutine(DecreaseShadowStrength(speed));
    }
    
    private IEnumerator IncreaseShadowStrength(float speed, ObjectWithShadows obj, Trigger trigger)
    {
        
        while (lightComponent.shadowStrength < 1)
        {
            lightComponent.shadowStrength += speed * Time.deltaTime;
            yield return null;
        }
        RightHandDevice.Instance.StopHaptics();
        GetComponent<Light>().shadowStrength = 1;
        if (trigger.isRightAnswer)
        {
            scaredSibling?.SeeSolution();
        }
        else
        {
            obj.EnableGraduatedTriggers();
        }
        
        state = FlashLightState.Activated;
    }
    
    private IEnumerator DecreaseShadowStrength(float speed)
    {
        while (lightComponent.shadowStrength > originalShadowStrength)
        {
            lightComponent.shadowStrength -= speed * Time.deltaTime;
            yield return null;
        }
        lightComponent.shadowStrength = originalShadowStrength;
        state = FlashLightState.Idle;
    }

    /*private void StartImpulse(float hapticAmplitude, float hapticDuration)
    {
        Debug.Log("Start Impulse");
        foreach (var device in devices)
        {
            HapticCapabilities capabilities;
            if (device.TryGetHapticCapabilities(out capabilities))
            {
                if (capabilities.supportsImpulse)
                {
                    uint channel = 0;
                    device.SendHapticImpulse(channel, hapticAmplitude, hapticDuration);
                }
            }
        }
    }

    private void StopImpulses()
    {
        Debug.Log("Stop Impulse");
        foreach (var device in devices)
        {
            if (!device.TryGetHapticCapabilities(out var capabilities)) continue;
            if (capabilities.supportsImpulse)
            {
                device.StopHaptics();
            }
        }
    }*/
}
