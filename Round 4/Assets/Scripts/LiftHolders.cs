using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiftHolders : MonoBehaviour
{
    public static LiftHolders Instance;
    [SerializeField] private List<GameObject> holders;
    
    // Start is called before the first frame update
    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetHoldersActive(bool active)
    {
        holders.ForEach(go => go.SetActive(active));
    }
}
