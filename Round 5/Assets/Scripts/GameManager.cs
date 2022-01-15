using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private Score myScore;
    [SerializeField] private float timeRemaining = 60f;
    [SerializeField] private Text timerText1;
    [SerializeField] private Text timerText2;
    public bool isTimerRunning = false;
    private readonly bool[] audioPlayed = { false, false, false, false };
    [NonSerialized] public JamoDrum jamoDrum;
    [NonSerialized] public DashUI DashUI;
    [NonSerialized] public SoundManager SoundManager;
    [NonSerialized] public HeroManager HeroManager;
    [NonSerialized] public ScoreManager ScoreManager;
    [NonSerialized] public CountdownUI CountdownUI;
    [SerializeField] public List<Player> players;
    [SerializeField] public List<HeroData> heroDatas;
    public GameObject[] powerUps;
    private bool isLastTwentyFiveSec = false;
    [SerializeField] private float powerUpTimeDelay;
    [SerializeField] private GameObject SuddenDeathFire;
    [NonSerialized] public List<GameObject> powerUpInstances;
    private EventHandler powerUpSpawnEvent;
    
    // Start is called before the first frame update
    private void Awake()
    {
        Instance = this;
        jamoDrum = FindObjectOfType<JamoDrum>();
        DashUI = FindObjectOfType<DashUI>();
        SoundManager = FindObjectOfType<SoundManager>();
        HeroManager = FindObjectOfType<HeroManager>();
        CountdownUI = FindObjectOfType<CountdownUI>();
        // myScore = FindObjectOfType<Score>();
        ScoreManager = FindObjectOfType<ScoreManager>();
        powerUpInstances = new List<GameObject>();
    }

    void Start()
    {
        timerText1.gameObject.SetActive(true);
        timerText2.gameObject.SetActive(true);
        isTimerRunning = false;
        InvokeRepeating("PowerUpSpawning", 5f, powerUpTimeDelay);
        StartCoroutine(StartWholeGameFlow());
    }

    // Update is called once per frame
    void Update()
    {


    }

    private void DisplayTime(float timeToDisplay)
    {
        if (timeToDisplay <= 60f && !audioPlayed[0])
        {
            audioPlayed[0] = true;
            SoundManager.GetComponent<SoundManager>().OneMinuteLeft();
        }

        if (timeToDisplay <= 10f && !audioPlayed[1])
        {
            audioPlayed[1] = true;
            SoundManager.GetComponent<SoundManager>().TenSecLeft();
        }

        if(timeToDisplay <= 25f && !audioPlayed[3])
        {
            audioPlayed[3] = true;
            isLastTwentyFiveSec = true;
            StartCoroutine(LastTwentyFiveSec());
        }

        if (timeToDisplay <= 30f && !audioPlayed[2])
        {
            audioPlayed[2] = true;
            SoundManager.GetComponent<SoundManager>().ThirtySec();
        }

        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        var milliSeconds = (timeToDisplay % 1) * 1000;

        timerText1.text = $"{minutes:0}:{seconds:00}:{milliSeconds:00}";
        timerText2.text = $"{minutes:0}:{seconds:00}:{milliSeconds:00}";

        //timerText1.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        //timerText2.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private IEnumerator StartWholeGameFlow()
    {
        powerUpSpawnEvent += SoundManager.OnPowerUpSpawn;
        yield return StartCoroutine(StartMainGamePlay());
        
        // TODO clear balls
        
        var isDraw = ShowResult();
        if (!isDraw)
        {
            yield return StartCoroutine(WaitForRestart());
            yield break;
        }
        
        yield return new WaitForSeconds(1);
        CountdownUI.countdown.text = "Sudden Death";
        SoundManager.SuddenDeath();
        yield return new WaitForSeconds(1);
        CountdownUI.countdown.gameObject.SetActive(false);
        // TODO visual effects on
        yield return StartCoroutine(StartOverTimeGamePlay());
        
        ShowResult();
        yield return StartCoroutine(WaitForRestart());
    }


    private IEnumerator StartMainGamePlay()
    {
        ScoreManager.StartMainGameplay();
        SoundManager.ONGameStart();
        HeroManager.SpawnHeroes();
        StartCoroutine(CountdownUI.OnGameStartCountdown());
        yield return new WaitForSeconds(3);
        
        HeroManager.StartGamePlay();
        if (timeRemaining >= 30)
        {
            StartCoroutine(EnableFireDelay(timeRemaining - 30));
        }
        while (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            DisplayTime(timeRemaining);
            yield return null;
        }
        timerText1.gameObject.SetActive(false);
        timerText2.gameObject.SetActive(false);
        powerUpInstances.ForEach(Destroy);
        powerUpInstances.Clear();
        HeroManager.EndGamePlay();
        ScoreManager.EndGamePlay();
    }

    private IEnumerator EnableFireDelay(float time)
    {
        yield return new WaitForSeconds(time);
        SuddenDeathFire.SetActive(true);
        SoundManager.FireBGM();
    }

    private bool ShowResult()
    {
        var tempRobots = ScoreManager.scoreRobots.score;
        var tempMonsters = ScoreManager.scoreMonsters.score;
        //if (scoreRobots > scoreMonsters)
        
        if(tempRobots > tempMonsters)
        {
            CountdownUI.OnTeamWin("Robots Win!!");
            SoundManager.OnTeamWin(0);
            return false;
        }
        if(tempRobots < tempMonsters)
        {
            CountdownUI.OnTeamWin("Monsters win!!");
            SoundManager.OnTeamWin(1);
            return false;
        }
        CountdownUI.OnTeamWin("DRAW!!");
        SoundManager.OnTeamDraw();
        return true;
    }

    public void PowerUpSpawning()
    {
        if (isLastTwentyFiveSec) return;
        var powerIndex = UnityEngine.Random.Range(0, powerUps.Length);
        powerUpSpawnEvent?.Invoke(this, EventArgs.Empty);
        powerUpInstances.Add(Instantiate(powerUps[powerIndex], new Vector3((float)UnityEngine.Random.Range(-2.5f, 2.5f), 0.01f, (float)UnityEngine.Random.Range(-1.3f, 1.3f)), Quaternion.identity));

    }

    private IEnumerator LastTwentyFiveSec()
    {
        var powerIndex = UnityEngine.Random.Range(0, powerUps.Length);
        powerUpInstances.Add(Instantiate(powerUps[powerIndex], new Vector3(-3f, 0.1f, 0f), Quaternion.identity));
        powerIndex = UnityEngine.Random.Range(0, powerUps.Length);
        powerUpInstances.Add(Instantiate(powerUps[powerIndex], new Vector3(-1.5f, 0.1f, 0f), Quaternion.identity));
        powerIndex = UnityEngine.Random.Range(0, powerUps.Length);
        powerUpInstances.Add(Instantiate(powerUps[powerIndex], new Vector3(3f, 0.1f, 0f), Quaternion.identity));
        powerIndex = UnityEngine.Random.Range(0, powerUps.Length);
        powerUpInstances.Add(Instantiate(powerUps[powerIndex], new Vector3(1.5f, 0.1f, 0f), Quaternion.identity));

        yield return new WaitForSeconds(15f);

        powerIndex = UnityEngine.Random.Range(0, powerUps.Length);
        powerUpInstances.Add(Instantiate(powerUps[powerIndex], new Vector3(-3f, 0.1f, 0f), Quaternion.identity));
        powerIndex = UnityEngine.Random.Range(0, powerUps.Length);
        powerUpInstances.Add(Instantiate(powerUps[powerIndex], new Vector3(-1.5f, 0.1f, 0f), Quaternion.identity));
        powerIndex = UnityEngine.Random.Range(0, powerUps.Length);
        powerUpInstances.Add(Instantiate(powerUps[powerIndex], new Vector3(3f, 0.1f, 0f), Quaternion.identity));
        powerIndex = UnityEngine.Random.Range(0, powerUps.Length);
        powerUpInstances.Add(Instantiate(powerUps[powerIndex], new Vector3(1.5f, 0.1f, 0f), Quaternion.identity));
    }

    private IEnumerator StartOverTimeGamePlay()
    {
        HeroManager.GetComponent<HeroManager>().isSuddenDeath = true;
        
        SoundManager.ONGameStart();
        HeroManager.SpawnHeroes();
        StartCoroutine(CountdownUI.OnGameStartCountdown());
        var suddenDeath = StartCoroutine(ScoreManager.StartSuddenDeath());
        yield return new WaitForSeconds(3);
        HeroManager.StartGamePlay();
        yield return suddenDeath;
        HeroManager.EndGamePlay();
    }

    public void OnPowerUpPickedUp(object sender, PowerUpPickUpEventArgs e)
    {
        powerUpInstances.Remove(e.powerUp);
        Destroy(e.powerUp);
    }

    private IEnumerator WaitForRestart()
    {
        yield return new WaitForSeconds(2f);
        CountdownUI.SetCornerTexts("Press to Return", true);
        while (true)
        {
            if (jamoDrum.hit.Contains(1))
            {
                SceneManager.LoadScene(0);
            }
            yield return null;
        }
    }
}
