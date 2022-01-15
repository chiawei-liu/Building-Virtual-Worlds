using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PowerUps : MonoBehaviour
{
    [NonSerialized] public HeroManager heroManager;
    private bool isTimeDone;
    void Start()
    {
        heroManager = FindObjectOfType<HeroManager>();   
    }

    private void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.tag == "ContinuousDash")
        {
            ContinuousDash();
        }
    }
    public void ContinuousDash()
    {
        if (!isTimeDone)
        {
            heroManager.GetComponent<HeroManager>().isDashPowerUp = true;
            StartCoroutine(powerUpCounter());
        }

        if (isTimeDone)
        {
            heroManager.GetComponent<HeroManager>().isDashPowerUp = false;
        }
             
    }

    public IEnumerator powerUpCounter()
    {
        yield return new WaitForSeconds(10f);
        isTimeDone = true;
        ContinuousDash();
    }
    

   
}
