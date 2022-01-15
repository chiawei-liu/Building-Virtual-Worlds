using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    [NonSerialized] public HeroManager heroManager;
    [NonSerialized] public GameManager gameManager;
    [NonSerialized] public SoundManager soundManager;
    [SerializeField] private Text BR;
    [SerializeField] private Text YG;
    private float respawnWaitTime = 3f;
    [NonSerialized] public int scoreRobots;
    [NonSerialized] public int scoreMonsters;
    private static bool triggered = false;
    [SerializeField] private Text countdown;
    private float countdownQ;
    private GameObject collectable;
    [SerializeField] private Text[] countdownText;
    [SerializeField] private ScoreUI scoreUI;
    public event EventHandler<CollectableEventArgs> scoreEvent;
    
    public int score
    {
        get => _score;
        set
        {
            _score = value;
            scoreUI.OnScoreChanged(_score);
        }
    }
    private int _score;
    
    private void Start()
    {
        scoreRobots = 0;
        scoreMonsters = 0;
        soundManager = FindObjectOfType<SoundManager>();
        gameManager = FindObjectOfType<GameManager>();
        heroManager = FindObjectOfType<HeroManager>();
        if (!triggered)
        {
            // StartCoroutine(startCountdownTimer());
            triggered = true;
        }
        //countdown.gameObject.SetActive(false);
    }

    // public IEnumerator startCountdownTimer()
    // {
    //     soundManager.GetComponent<SoundManager>().onGameStart();
    //     heroManager.GetComponent<HeroManager>().SpawnHeroes();
    //     //foreach(Text startCountdown in countdownText)
    //     
    //
    //
    //             
    //     
    //     gameManager.GetComponent<GameManager>().isTimerRunning = true;
    //     heroManager.GetComponent<HeroManager>().StartGamePlay();
    //
    //
    // }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "collectable") return;
        other.GetComponent<Collider>().enabled = false;
        other.GetComponent<Collectable>().carriedBy.Drop();
        Destroy(other.gameObject);
        score++;
        var collectableEventArgs = new CollectableEventArgs();
        collectableEventArgs.Collectable = other.GetComponent<Collectable>();
        scoreEvent?.Invoke(this, collectableEventArgs);
    }
    
   public void OnGameOver()
    {
        var tempRobots = int.Parse(BR.text);
        var tempMonsters = int.Parse(YG.text);
        //if (scoreRobots > scoreMonsters)
        if(tempRobots > tempMonsters)
        {          
            //Debug.Log("scoreMonster" + scoreMonsters + " scoreRobots" + scoreRobots);
            countdown.text = "Robots Win!!";
            countdown.gameObject.SetActive(true);
        }
        else if(tempRobots < tempMonsters)
        {
            //Debug.Log("scoreMonster" + scoreMonsters + " scoreRobots" + scoreRobots);
            countdown.text = "Monsters win!!";
            countdown.gameObject.SetActive(true);
        }
        else
        {
            //Debug.Log("scoreMonster" + scoreMonsters + " scoreRobots" + scoreRobots);
            countdown.text = "Draw";
            countdown.gameObject.SetActive(true);
        }
        Time.timeScale = 0;
    }
   
}
