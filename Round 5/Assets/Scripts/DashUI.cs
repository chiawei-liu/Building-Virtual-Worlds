using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DashUI : MonoBehaviour
{
    [SerializeField] private List<Image> chargingBar;
    [SerializeField] private Sprite normalBarSprite;
    [SerializeField] private Sprite constantDashSprite;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnChargeChanged(object sender, EventArgs e)
    {
        var hero = (Hero) sender;
        chargingBar[hero.player.playerNumber].fillAmount = hero.Charge;
    }
    
    public void SetCharge(int index, float amount)
    {
        chargingBar[index].fillAmount = amount;
    }
    
    public void SetPowerUp(Player player, bool active)
    {
        chargingBar[player.playerNumber].sprite = active ? constantDashSprite : normalBarSprite;
    }
}
