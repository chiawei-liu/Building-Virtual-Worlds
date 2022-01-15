using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Collider))]
public class GrabableItem : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField] private AudioClip collisionSound, pickUpSound;
    
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnCollisionEnter(Collision other)
    {
        //audioSource.PlayOneShot(collisionSound);
    }

    public void OnPickUp()
    {
        audioSource.PlayOneShot(pickUpSound);
    }
}
