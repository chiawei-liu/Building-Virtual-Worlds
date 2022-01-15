using System;
using System.Collections;
using System.Collections.Generic;
using FSM.State;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class LiftInteractable : MonoBehaviour
{
    [SerializeField] private GameObject tip;

    [SerializeField] private GameObject progressBar;
    [SerializeField] private Sprite handHint;
    [SerializeField] private Sprite liftHint;
    [SerializeField] private Image filling;
    [SerializeField] private float step = 0.1f;
    [SerializeField] private GameObject rotationAnchor;
    [SerializeField] private float maxAngle = 5;
    [SerializeField] private GameObject sandGround;
    private float originalAngle;
    [SerializeField] private Color32 tipColor;
    [SerializeField] private Color32 tipColorHighlight;
    [SerializeField] private List<LiftTrigger> triggers;
    [SerializeField] private AudioClip liftSound;
    private bool halfLiftTriggered = false;
    private int triggerCount = 0;
    public static LiftInteractable Instance;
    private bool fullyTriggered = false;
    private AudioSource audioSource;
    private bool isComplete = false;

    private void Awake()
    {
        Instance = this;
        triggers.ForEach(t => t.parent = this);
        audioSource = GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        progressBar.SetActive(false);
        tip.SetActive(false);
        triggers.ForEach(t => t.SetActive(false));
        // GetComponent<Outline>().enabled = false;
        originalAngle = transform.rotation.x;
        // GetComponent<XRSimpleInteractable>().enabled = false;
        // Activate();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Activate()
    {
        // tip.SetActive(true);
        // tip.GetComponent<Text>().text = "Put hand here";

        progressBar.SetActive(true);
        //handHint.SetActive(true);
        filling.fillAmount = 0;
        triggers.ForEach(t => t.SetActive(true));
    }

    public void Deactivate()
    {
        // tip.SetActive(false);
    }

    public void OnHoverEntered()
    {
        // tip.SetActive(true);
        tip.GetComponent<Text>().text = "Press A";
    }

    public void OnHoverExited()
    {
        // tip.SetActive(false);
        tip.GetComponent<Text>().text = "Put hand here";
    }
    

    public void OnSelectEntered()
    {
        // if (!fullyTriggered) return;
        if (isComplete) return;
        filling.fillAmount += step;
        var rot = rotationAnchor.transform.rotation;
        rot.x = originalAngle + filling.fillAmount * maxAngle * Mathf.Deg2Rad;
        rotationAnchor.transform.rotation = rot;
        tip.GetComponent<Text>().color = tipColorHighlight;
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(liftSound);
        }

        if (!halfLiftTriggered && filling.fillAmount >= 0.5)
        {
            halfLiftTriggered = true;
            GameplayFlow.Instance.StateEvent("halfLiftCar", new StateData());
        }
        if (filling.fillAmount >= 1)
        {
            Complete();
        }
    }
    
    public void OnSelectedExited()
    {
        // if (!fullyTriggered) return;
        tip.GetComponent<Text>().color = tipColor;
    }

    private void Complete()
    {
        // tip.SetActive(false);
        isComplete = true;
        progressBar.SetActive(false);
        rotationAnchor.GetComponent<Rigidbody>().isKinematic = false;
        StartCoroutine(SetIsKinematicTure(0.5f));
        sandGround.layer = LayerMask.NameToLayer("Default");
        GameplayFlow.Instance.StateEvent("doneLiftCar", new StateData());
        progressBar.SetActive(false);
        triggers.ForEach(t =>
        {
            t.GetComponent<Collider>().enabled = false;
            t.SetActive(false);
        });
    }

    private IEnumerator SetIsKinematicTure(float time)
    {
        yield return new WaitForSeconds(time);
        rotationAnchor.GetComponent<Rigidbody>().isKinematic = true;
    }

    public void TriggerEnters(LiftTrigger trigger)
    {
        triggerCount += 1;
        //progressBar.SetActive(true);
        progressBar.GetComponent<Image>().sprite = liftHint;
        // handHint.SetActive(false);
        // if (triggerCount == 2)
        // {
        //     progressBar.SetActive(true);
        //     fullyTriggered = true;
        // }
    }

    public void TriggerExits(LiftTrigger trigger)
    {
        triggerCount -= 1;
        // progressBar.SetActive(false);
        fullyTriggered = false;
        if (triggerCount == 0)
        {
            //progressBar.SetActive(false);
            //filling.fillAmount = 0;
            progressBar.GetComponent<Image>().sprite = handHint;
        }
    }
}
