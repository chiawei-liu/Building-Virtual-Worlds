using System.Collections;
using System.Collections.Generic;
using Tobii.Gaming;
using UnityEngine;

public class StartButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Pressed()
    {
        GameManager.Instance.startPressed = true;
    }
}
