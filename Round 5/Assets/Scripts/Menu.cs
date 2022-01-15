using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Menu : MonoBehaviour
{
    [SerializeField] private JamoDrum jamoDrumMenu;
    [SerializeField] private GameObject[] ready;
    [SerializeField] private List<GameObject> lights;
    private DashUI dashUI;
    private bool[] isReady = { false, false, false, false };
    
    void Start()
    {
        dashUI = FindObjectOfType<DashUI>();
        foreach(GameObject textThing in ready)
        {
            textThing.SetActive(false);
        }
        jamoDrumMenu = FindObjectOfType<JamoDrum>();
        //StartCoroutine(GetAllInputs());
    }
    
    void Update()
    {

        for (int i = 0; i < 4; i++)
        {
            if (jamoDrumMenu.hit[i] == 1)
            {   
                ready[i].SetActive(true);
                isReady[i] = true;
                dashUI.SetCharge(i, 1);
                lights[i].SetActive(true);
            }
        }

        if (isReady[0] && isReady[1] && isReady[2] && isReady[3])
        {
            Debug.Log("Ready!");
            SceneManager.LoadScene(1);
        }
}
}
