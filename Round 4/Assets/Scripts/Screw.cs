using System;
using System.Collections;
using System.Collections.Generic;
using FSM.State;
using UnityEngine;
using UnityEngine.UI;

public class Screw : MonoBehaviour
{
    private Coroutine c;
    private Vector3 constraintWorldAxis;
    [SerializeField] private Vector3 constraintLocalAxis;
    [SerializeField] private bool clockwise;
    [NonSerialized] public float angleRotated = 0;
    private float progress = 0;
    [SerializeField] private GameObject progressBar;
    [SerializeField] private Image progressBarFilling;
    [SerializeField] private float progressSpeed = 0.5f;
    [SerializeField] private Camera cam;
    [SerializeField] private float offset = 0;
    [SerializeField] private AudioClip screwingSound;
    [SerializeField] private Outline outline;
    private bool activated = false;
    private AudioSource audioSource;
    public static Screw Instance = null;
    [SerializeField] private GameObject smoke;
    [SerializeField] private List<Outline> driversOutlines;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }

        audioSource = GetComponent<AudioSource>();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        progressBar.SetActive(false);
        driversOutlines.ForEach(d => d.enabled = false);
        outline.enabled = false;
        constraintWorldAxis = transform.TransformVector(constraintLocalAxis).normalized;
        // ActivateFixingTire();
    }

    // Update is called once per frame
    void Update()
    {
        // var target = gameObject.transform.position;
        // target.y += offset;
        // progressBar.transform.position = cam.WorldToScreenPoint(target);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (activated && other.gameObject.CompareTag("Player"))
        {
            // GetComponent<FixedJoint>().connectedBody = other.gameObject.GetComponent<Rigidbody>();
            c = StartCoroutine(Screwing(other.gameObject));
            // Debug.Log("enter");
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (!activated || !other.gameObject.CompareTag("Player")) return;
        // GetComponent<FixedJoint>().connectedBody = null;
        if (c == null) return;
        StopCoroutine(c);
        c = null;
        // Debug.Log("exit");
    }

    public void ActivateFixingTire()
    {
        progressBar.SetActive(true);
        outline.enabled = true;
        activated = true;
        driversOutlines.ForEach(d => d.enabled = true);
    }

    private IEnumerator Screwing(GameObject go)
    {
        // var lastRot = go.transform.rotation.eulerAngles;
        var x1 = go.transform.forward - constraintWorldAxis * (Vector3.Dot(go.transform.forward, constraintWorldAxis));
        var lastRot = 0;
        while (true)
        {
            var x2 = go.transform.forward - constraintWorldAxis * (Vector3.Dot(go.transform.forward, constraintWorldAxis));
            float dif = 0;
            // Debug.Log(Vector3.Dot(x1, transform.forward));
            // Debug.Log(Vector3.Dot(x2, transform.forward));
            // Debug.Log(Vector3.Cross(x1, x2));
            if (Vector3.Cross(x1, x2).normalized == constraintWorldAxis)
            {
                dif = Vector3.Angle(x1, x2);
            }
            else
            {
                dif = -Vector3.Angle(x1, x2);
            }
            // Debug.Log(dif);
            // var dif = go.transform.eulerAngles.z - lastRot.z;
            if (/*clockwise && dif > 0 || !clockwise && dif < 0*/    dif * (clockwise?1:-1) > 1.5)
            {
                //var rot = gameObject.transform.rotation.eulerAngles;
                //rot.z += dif;
                gameObject.transform.RotateAround(transform.position, constraintWorldAxis, dif);
                angleRotated += clockwise ? dif : -dif;
                progressBarFilling.fillAmount += progressSpeed * Time.deltaTime;
                if (!audioSource.isPlaying)
                {
                    audioSource.PlayOneShot(screwingSound);
                }
                if (progressBarFilling.fillAmount >= 1)
                {
                    GameplayFlow.Instance.StateEvent("doneTakeOffTire", new StateData());
                    GameplayFlow.Instance.StateEvent("doneFixTire", new StateData());

                    activated = false;
                    progressBar.SetActive(false);
                    SetSmokeActive(false);
                    outline.enabled = false;
                    driversOutlines.ForEach(d => d.enabled = false);

                    yield break;
                }
            }
            x1 = x2;
            // lastRot = go.transform.eulerAngles;
            yield return null;
        }
    }

    public void SetSmokeActive(bool active)
    {
        smoke.SetActive(active);
    }
}
