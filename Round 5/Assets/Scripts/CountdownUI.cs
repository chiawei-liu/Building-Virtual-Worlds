using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CountdownUI : MonoBehaviour
{
    [SerializeField] private Text[] countdownText;
    [SerializeField] public Text countdown;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator OnGameStartCountdown()
    {
        foreach (var text in countdownText)
        {
            text.gameObject.SetActive(true);
            text.text = "Ready";
        }

        yield return new WaitForSeconds(1f);
        foreach (var text in countdownText)
        {
            text.text = "Set";
        }

        yield return new WaitForSeconds(1f);
        foreach (var text in countdownText)
        {
            text.text = "Go";
        }

        yield return new WaitForSeconds(1f);
        foreach (var text in countdownText)
        {
            text.gameObject.SetActive(false);
        }
    }

    public void OnTeamWin(string s)
    {
        countdown.text = s;
        countdown.gameObject.SetActive(true);
    }

    public void SetCornerTexts(string s, bool active)
    {
        foreach (var text in countdownText)
        {
            text.text = s;
            text.gameObject.SetActive(active);
        }
    } 
}
