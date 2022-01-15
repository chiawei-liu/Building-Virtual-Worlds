using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

[RequireComponent(typeof(Collider))] 
public class Trigger : MonoBehaviour
{
    [NonSerialized] public ObjectWithShadows parent;
    [SerializeField] public bool isRightAnswer;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        parent.TriggerEnter(other, this);
    }
    
    private void OnTriggerExit(Collider other)
    {
        parent.TriggerExit(other, this);
    }
    
    private void OnTriggerStay(Collider other)
    {
        parent.TriggerStay(other, this);
    }
}
