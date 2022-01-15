using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] public Score scoreRobots;
    [SerializeField] public Score scoreMonsters;
    [SerializeField] private Text countdown;
    private float respawnWaitTime = 3f;
    private float countdownQ;
    [SerializeField] private GameObject collectablePrefab;
    private List<GameObject> collectables;
    private bool respawn = true;
    [SerializeField] private Transform spawnBallPos;
    [SerializeField] private GameObject screamVFXPrefab;
    private Coroutine respawnCoroutine;
    
    private void Awake()
    {
        collectables = new List<GameObject>();
    }

    // Start is called before the first frame update
    void Start()
    {
        scoreMonsters.scoreEvent += OnScoreRemoveBall;
        scoreMonsters.scoreEvent += OnScoreRespawnBall;
        scoreMonsters.scoreEvent += GameManager.Instance.SoundManager.OnScore;
        scoreMonsters.scoreEvent += GameManager.Instance.SoundManager.OnMonstersScore;
        scoreRobots.scoreEvent += OnScoreRemoveBall;
        scoreRobots.scoreEvent += OnScoreRespawnBall;
        scoreRobots.scoreEvent += GameManager.Instance.SoundManager.OnScore;
        scoreRobots.scoreEvent += GameManager.Instance.SoundManager.OnRobotsScore;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartMainGameplay()
    {
        var go = Instantiate(collectablePrefab, new Vector3(0, 0.2f, 0), Quaternion.identity);
        go.GetComponent<Collectable>().spawnedPos = go.transform.position;
        collectables.Add(go);
    }
    
    private IEnumerator RespawnDelay()
    {
        //Debug.Log("scoreMonster" + scoreMonsters + " scoreRobots" + scoreRobots);
        countdown.gameObject.SetActive(true);

        for (var i = respawnWaitTime; i > 0; i--)
        {
            countdownQ = i;
            countdown.text = countdownQ.ToString();
            yield return new WaitForSeconds(1f);
        }
        //yield return new WaitForSeconds(respawnWaitTime);
        countdown.gameObject.SetActive(false);
        
        var go = Instantiate(collectablePrefab, new Vector3(0, 0.2f, 0), Quaternion.identity);
        go.GetComponent<Collectable>().spawnedPos = go.transform.position;
        collectables.Add(go);
        //collectableObject.gameObject.SetActive(true);
        // Destroy(collectableObject.gameObject);
    }

    private void OnScoreRemoveBall(object sender, CollectableEventArgs e)
    {
        Instantiate(screamVFXPrefab, e.Collectable.gameObject.transform.position, Quaternion.identity);
        collectables.Remove(e.Collectable.gameObject);
        Destroy(e.Collectable.gameObject);
    }

    private void OnScoreRespawnBall(object sender, CollectableEventArgs e)
    {
        respawnCoroutine = StartCoroutine(RespawnDelay());
    }

    public IEnumerator StartSuddenDeath()
    {
        scoreMonsters.scoreEvent -= OnScoreRespawnBall;
        scoreRobots.scoreEvent -= OnScoreRespawnBall;

        var positions = new List<Vector3>
            {new Vector3(2.5f, 0.2f, 0), new Vector3(0, 0.2f, 0), new Vector3(-2.5f, 0.2f, 0)};
        positions.ForEach(p =>
        {
            var go = Instantiate(collectablePrefab, p, Quaternion.identity);
            go.GetComponent<Collectable>().spawnedPos = p;
        });
        var currentScore = scoreMonsters.score;
        yield return new WaitUntil(() => scoreMonsters.score >= currentScore + 2 || scoreRobots.score >= currentScore + 2);
    }

    private void ClearCollectables()
    {
        collectables.ForEach(c => Destroy(c));
        collectables = new List<GameObject>();
    }

    public void EndGamePlay()
    {
        ClearCollectables();
        if (respawnCoroutine != null)
        {
            StopCoroutine(respawnCoroutine);
        }
    }

    public void OnBallDropped(object sender, CollectableEventArgs e)
    {
        if (!e.Reset) return;
        e.Collectable.transform.position = e.Collectable.spawnedPos;
    }
}
