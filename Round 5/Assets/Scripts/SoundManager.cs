using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioClip monstersHaveBall;
    [SerializeField] private AudioClip robotsHaveBall;
    [SerializeField] private AudioClip monstersDropBall;
    [SerializeField] private AudioClip robotsDropBall;
    [SerializeField] private AudioClip ballDropped;
    [SerializeField] private AudioClip go;
    [SerializeField] private AudioClip tenCountdown;
    [SerializeField] private AudioClip oneMinute;
    [SerializeField] private AudioClip thirtySeconds;
    [SerializeField] private AudioClip robotsScoreSound;
    [SerializeField] private AudioClip monstersScoreSound;
    [SerializeField] private AudioClip robotsWinSound;
    [SerializeField] private AudioClip monstersWinSound;
    [SerializeField] private AudioClip drawSound;
    [SerializeField] private AudioClip suddenDeath;
    [SerializeField] private AudioClip bloodBurst;
    [SerializeField] private AudioClip powerUpSpawn;
    [SerializeField] private AudioClip powerUpPickUp;
    [SerializeField] private AudioClip fireBGM;
    [SerializeField] private List<AudioClip> screams;

    [SerializeField] private AudioSource audioSource, screamSource, countdownSource, sfxSource, bgmSource;

    private void Awake()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void OnHaveBall(object sender, EventArgs e)
    {
        var hero = (Hero) sender;
        if (hero.player.side == 0)
        {
            audioSource.Stop();
            audioSource.PlayOneShot(robotsHaveBall);
        }
        else
        {
            audioSource.Stop();
            audioSource.PlayOneShot(monstersHaveBall);
        }
    }
    
    public void OnDropBall(object sender, EventArgs e)
    {
        // var hero = (Hero) sender;
        // if (hero.player.side == 0)
        // {
        //     audioSource.Stop();
        //     audioSource.PlayOneShot(robotsDropBall);
        // }
        // else
        // {
        //     audioSource.Stop();
        //     audioSource.PlayOneShot(monstersDropBall);
        // }
        sfxSource.PlayOneShot(ballDropped);
    }

    public void OnTeamWin(int side)
    {
        if (side == 0)
        {
            audioSource.Stop();
            audioSource.PlayOneShot(robotsWinSound);
        }
        else
        {
            audioSource.Stop();
            audioSource.PlayOneShot(monstersWinSound);
        }
    }

    public void OnTeamScore(int side)
    {
        if (side == 0)
        {
            audioSource.Stop();
            audioSource.PlayOneShot(robotsScoreSound);
        }
        else
        {
            audioSource.Stop();
            audioSource.PlayOneShot(monstersScoreSound);
        }
    }

    public void OnTeamDraw()
    {
        audioSource.PlayOneShot(drawSound);
    }

    public void SuddenDeath()
    {
        audioSource.Stop();
        audioSource.PlayOneShot(suddenDeath);
    }

    public void FireBGM()
    {
        bgmSource.PlayOneShot(fireBGM);
    }

    public void OnMonstersScore(object o, EventArgs e)
    {
        audioSource.Stop();
        audioSource.PlayOneShot(monstersScoreSound);
    }
    
    public void OnRobotsScore(object o, EventArgs e)
    {
        audioSource.Stop();
        audioSource.PlayOneShot(robotsScoreSound);
    }

    public void OnScore(object o, EventArgs e)
    {
        screamSource.Stop();
        screamSource.PlayOneShot(screams[Random.Range(0, screams.Count)]);
    }

    public void OnPowerUpPickedUp(object o, EventArgs e)
    {
        sfxSource.PlayOneShot(powerUpPickUp);
    }

    public void OnPowerUpSpawn(object o, EventArgs e)
    {
        sfxSource.PlayOneShot(powerUpSpawn);
    }

    public void OnHeroStun(object o, EventArgs e)
    {
        sfxSource.PlayOneShot(bloodBurst);

    }

    public void ONGameStart()
    {
        countdownSource.PlayOneShot(go);
    }

    public void OneMinuteLeft()
    {
        countdownSource.PlayOneShot(oneMinute);
    }

    public void TenSecLeft()
    {
        countdownSource.PlayOneShot(tenCountdown);
    }

    public void ThirtySec()
    {
        countdownSource.PlayOneShot(thirtySeconds);
    }
}
