using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class LiftTrigger : MonoBehaviour
{
    [SerializeField] private GameObject tip;
    [SerializeField] private GameObject tipActiveFill;
    // [SerializeField] private GameObject tip;
    [NonSerialized] public LiftInteractable parent;
    
    private bool active;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnHoverEnter()
    {
        if (active)
        {
            parent.TriggerEnters(this);
            // tipActiveFill.SetActive(true);
        }
    }
    
    public void OnHoverExit()
    {
        if (active)
        {
            parent.TriggerExits(this);
            // tipActiveFill.SetActive(false);
        }
    }

    public void SetActive(bool active)
    {
        this.active = active;
        GetComponent<Outline>().enabled = active;
        GetComponent<XRSimpleInteractable>().enabled = active;
        // tip.SetActive(active);
    }
    
    public void OnSelectEntered()
    {
        parent.OnSelectEntered();
    }
    
    public void OnSelectedExited()
    {
        parent.OnSelectedExited();
    }
}
