using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ObjectWithShadows : MonoBehaviour
{
    [SerializeField] private List<Trigger> triggers;
    [SerializeField] private List<GraduatedTrigger> gTriggers;

    [SerializeField] private float increaseSpeed;
    [SerializeField] private float decreaseSpeed;
    [SerializeField] private float hapticAmplitude;
    [SerializeField] private float hapticDuration;
    [SerializeField] private float acceptableAngleDiff;
    
    
    // Start is called before the first frame update
    void Start()
    {
        triggers.ForEach(t =>
        {
            t.parent = GetComponent<ObjectWithShadows>();
        });
    }

    // Update is called once per frame
    void Update()
    {
    //     Debug.Log("light: " + light.transform.forward);
    //     Debug.Log("This: " + transform.forward);
    //     var dif = transform.position - light.transform.position;
    //     Debug.Log("dif: " + dif);
    //
    //     if (dif.x > 0)
    //     {
    //         transparentObjects[0].SetActive(true);
    //         transparentObjects[1].SetActive(false);
    //     }
    //     else
    //     {
    //         transparentObjects[0].SetActive(false);
    //         transparentObjects[1].SetActive(true);
    //
    //     }
        // bool allNotTriggered = true;
        // triggers.ForEach(t =>
        // {
        //     if (t.triggered)
        //     {
        //         flashLight.GetComponent<Light>().shadowStrength = 1;
        //         allNotTriggered = false;
        //     }
        // });
        // if (allNotTriggered)
        // {
        //     flashLight.GetComponent<Light>().shadowStrength = originalShadowStrength;
        // } 
    }

    public void TriggerEnter(Collider other, Trigger trigger)
    {
        if (Vector3.Angle(other.gameObject.transform.forward,
            gameObject.transform.position - trigger.gameObject.transform.position) < acceptableAngleDiff)
        {
            other.GetComponent<FlashLight>().TryIncreaseShadow(increaseSpeed, hapticAmplitude, hapticDuration, this, trigger);
        }
        else
        {
            other.GetComponent<FlashLight>().TryDecreaseShadow(decreaseSpeed);
        }
    }
    
    public void TriggerExit(Collider other, Trigger trigger)
    {
        other.GetComponent<FlashLight>().TryDecreaseShadow(decreaseSpeed);
    }
    
    public void TriggerStay(Collider other, Trigger trigger)
    {
        if (Vector3.Angle(other.gameObject.transform.forward,
            gameObject.transform.position - trigger.gameObject.transform.position) < acceptableAngleDiff)
        {
            other.GetComponent<FlashLight>().TryIncreaseShadow(increaseSpeed,hapticAmplitude, hapticDuration, this, trigger);
        }
        else
        {
            other.GetComponent<FlashLight>().TryDecreaseShadow(decreaseSpeed);
        }
    }

    public void OnTriggerHoverEnter(HoverEnterEventArgs eventArgs)
    {
        Debug.Log("TriggerHoverEnter");
    }

    public void DisableGraduatedTriggers()
    {
        gTriggers.ForEach(t => t.gameObject.SetActive(false));
    }
    
    public void EnableGraduatedTriggers()
    {
        gTriggers.ForEach(t => t.gameObject.SetActive(true));
    }
    
}
