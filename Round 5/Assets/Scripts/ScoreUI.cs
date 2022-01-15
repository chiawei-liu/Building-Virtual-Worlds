using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] private GameObject scoreIndicator;
    [SerializeField] private Text scoreText;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddOneScore()
    {
        
    }

    public void ResetScores()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void OnScoreChanged(int score)
    {
        scoreText.text = score.ToString();
        // if (transform.childCount < score)
        // {
        //     for (int i = transform.childCount; i < score; i++)
        //     {
        //         Instantiate(scoreIndicator, transform);
        //     }
        // }
        // else
        // {
        //     var c = transform.childCount;
        //     for (int i = score; i < c; i++)
        //     {
        //         Destroy(transform.GetChild(0));
        //     }
        // }
    }
}
