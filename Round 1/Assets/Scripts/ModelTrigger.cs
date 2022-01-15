using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelTrigger : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter()
    {
        GetComponent<Animator>().Play("Active");
        GetComponent<AudioSource>().Play();
    }
}
