using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(AudioSource))]
public class GraduatedTrigger : MonoBehaviour
{
    [SerializeField] private float minPeriod;
    [SerializeField] private float maxPeriod;
    [SerializeField] private float maxAmplitude;
    [SerializeField] private float minAmplitude;
    [SerializeField] private float minAudioVolume;
    [SerializeField] private float maxAudioVolume;
    
    private AudioSource audioSource;
    private Coroutine heartBeat;
    private float period, amplitude;
    private float radius;
    
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        radius = GetComponent<SphereCollider>().radius;
        audioSource.volume = minAudioVolume;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        heartBeat ??= StartCoroutine(HeartBeat());
        audioSource.Play();
    }

    private void OnTriggerStay(Collider other)
    {
        var d = Vector3.Distance(transform.position, other.gameObject.transform.position);
        var t = Vector3.Distance(transform.position, other.gameObject.transform.position) / radius;
        amplitude = Mathf.Lerp(maxAmplitude, minAmplitude, (float)Math.Pow(t, 2));
        period = Mathf.Lerp(minPeriod, maxPeriod, (float)Math.Pow(t, 2));
        audioSource.volume = Mathf.Lerp(maxAudioVolume, minAudioVolume, (float)Math.Pow(t, 2));
    }

    private void OnTriggerExit(Collider other)
    {
        if (heartBeat != null)
        {
            StopCoroutine(heartBeat);
            heartBeat = null;
        }
        audioSource.Stop();
    }

    private IEnumerator HeartBeat()
    {
        while (true)
        {
            RightHandDevice.Instance.SendHapticImpulse(amplitude, period / 4 / 4 * 3);
            yield return new WaitForSeconds(period / 4 / 4 * 3);
            RightHandDevice.Instance.StopHaptics();
            yield return new WaitForSeconds(period / 4 / 4);
            RightHandDevice.Instance.SendHapticImpulse(amplitude / 3, period / 4);
            yield return new WaitForSeconds(period / 3 / 4 * 3);
            RightHandDevice.Instance.StopHaptics();
            yield return new WaitForSeconds(period / 3 / 4);

            var time = 0f;
            while (time < period / 2)
            {
                time += Time.deltaTime;
                yield return null;
            }
        }
    }

    private IEnumerator timeBetweenHeartBeat(float second)
    {
        yield return new WaitForSeconds(second);
    }

    /*private IEnumerator findDevices()
    {
        hapticDevices = new List<InputDevice>();
        while (hapticDevices.Count == 0)
        {
            var devices = new List<InputDevice>();
            InputDevices.GetDevicesWithRole(InputDeviceRole.RightHanded, devices);
            foreach (var device in devices)
            {
                HapticCapabilities capabilities;
                if (device.TryGetHapticCapabilities(out capabilities))
                {
                    if (capabilities.supportsImpulse)
                    {
                        hapticDevices.Add(device);
                    }
                }
            }
            yield return null;
        }
    }*/
}
