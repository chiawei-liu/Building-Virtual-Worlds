using System;
using System.Collections;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    [NonSerialized] public bool collected = false;
    public Hero carriedBy;
    public Vector3 spawnedPos;
    
    void Start()
    {

    }

    void Update()
    {
        
    }

    // public void Respawn()
    // {
    //     //StartCoroutine(RespawnCollectable());
    //     collectable = Instantiate(collectableToSpawn, new Vector3(0, 0.2f, 0), Quaternion.identity);
    //     collectable.SetActive(true);
    // }

     public IEnumerator RespawnCollectable()
    {
        Debug.Log("Respawning");
        yield return new WaitForSeconds(2f);
        Debug.Log("Delay done");
    }
}
