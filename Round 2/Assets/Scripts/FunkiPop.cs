using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityTemplateProjects;

public class FunkiPop : MonoBehaviour, IColorChangeEventReceiver
{
    [SerializeField] public List<GameObject> parts;
    private List<FunkiPopPart> funkiPopParts;
    [SerializeField] private List<Color32> correctColors;
    private int correctCount = 0;
    [SerializeField] public Sprite poster;
    
    // Start is called before the first frame update
    void Start()
    {
        funkiPopParts = new List<FunkiPopPart>();
        for (var i = 0; i < parts.Count; i++)
        {
            var comp = parts[i].GetComponent<FunkiPopPart>();
            if (comp == null)
            {
                comp = parts[i].AddComponent<FunkiPopPart>();
            }
            funkiPopParts.Add(comp);
            funkiPopParts[i].receivers.Add(this);
            funkiPopParts[i].CorrectColor = correctColors[i];
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ColorChangeEvent(FunkiPopPart part)
    {
        var count = 0;
        funkiPopParts.ForEach(p => count += p.Color.Compare(p.CorrectColor)? 1 : 0);
        correctCount = count;
        UpdateScores();
    }
    

    public void UpdateScores()
    {
        GameManager.Instance.scorePanel.SetScore(correctCount);
        Debug.Log("correct counts: " + correctCount);
    }
}
