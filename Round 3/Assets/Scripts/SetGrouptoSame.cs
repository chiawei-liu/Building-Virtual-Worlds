using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetGrouptoSame : MonoBehaviour
{
    public bool mainval;
    public bool kid1;
    public bool kid2;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        kid1 = mainval;
        kid2 = mainval;
    }
}
