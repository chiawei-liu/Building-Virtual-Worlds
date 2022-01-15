using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroManager : MonoBehaviour
{
    //[SerializeField] private List<Player> players;
    private List<Hero> heroes;
    // [SerializeField] private List<Player> players;
    [SerializeField] private float rotationScale;
    [SerializeField] private float startingDashSpeed;
    [SerializeField] private float startingSpeed;
    [SerializeField] private float startingStunDuration;
    [SerializeField] private float startingChargingSpeed;
    [SerializeField] private GameObject characterPrefab;
    private Coroutine gameplay;
    private AudioSource audioSource;

    [NonSerialized] public bool isDashPowerUp = false;
    [NonSerialized] public bool isSpeedPowerUp = false;
    [NonSerialized] public bool isSuddenDeath = false;
    
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        heroes = new List<Hero>();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        // for (int i = 0; i < heroes.Count; i++)
        // {
        //     heroes[i].playerNumber = i;
        // }
        //StartCoroutine(StartPlaying());
    }

    public void StartGamePlay()
    {
        gameplay = StartCoroutine(StartPlaying());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnHeroes()
    {
        heroes.ForEach(h => Destroy(h.gameObject));
        heroes = new List<Hero>();
        foreach (var player in GameManager.Instance.players)
        {
            var go = Instantiate(characterPrefab, player.transform.position, player.transform.rotation);
            var hero = go.GetComponent<Hero>();
            hero.player = player;
            hero.image.sprite = GameManager.Instance.heroDatas[player.selectedHero].sprite;
            hero.player = player;
            hero.afterimageShadows.GetComponent<ParticleSystem>().GetComponent<Renderer>().material = GameManager.Instance
                .heroDatas[player.selectedHero].afterimageMaterial;
            hero.currentTrail = hero.teamTrails[hero.player.side];
            player.hero = hero;
            heroes.Add(hero);
        }
    }

    private IEnumerator StartPlaying()
    {
        heroes.ForEach(hero =>
        {
            // var go = Instantiate(characterPrefab, GameManager.Instance.players[i].transform.position, GameManager.Instance.players[i].transform.rotation);
            // var hero = go.GetComponent<Hero>();
            hero.speed = startingSpeed;
            hero.dashSpeed = startingDashSpeed;
            hero.stunDuration = startingStunDuration;

            hero.chargingSpeed = startingChargingSpeed;
            hero.chargeChangedEvent += GameManager.Instance.DashUI.OnChargeChanged;
            // hero.haveBallEvent += GameManager.Instance.SoundManager.OnHaveBall;
            hero.dropBallEvent += GameManager.Instance.SoundManager.OnDropBall;
            hero.dropBallEvent += GameManager.Instance.ScoreManager.OnBallDropped;
            hero.stunEvent += GameManager.Instance.SoundManager.OnHeroStun;
            hero.powerUpPickUpEvent += GameManager.Instance.SoundManager.OnPowerUpPickedUp;
            hero.powerUpPickUpEvent += GameManager.Instance.OnPowerUpPickedUp;
            hero.dashSound = GameManager.Instance
                .heroDatas[hero.player.selectedHero].dashSound;
            hero.animator.runtimeAnimatorController = GameManager.Instance
                .heroDatas[hero.player.selectedHero].animatorController;
            hero.GetComponent<Rigidbody>().constraints |= RigidbodyConstraints.FreezePositionY;
        });
        
        while (true)
        {
            heroes.ForEach(hero =>
            {
                var pos = hero.transform.position;
                if (hero.State == HeroState.Walk)
                {
                    if (hero.carryingCollectable == null && GameManager.Instance.jamoDrum.hit[hero.player.playerNumber] == 1 && hero.Charge == 1)
                    {
                        StartCoroutine(SetHeroDashing(hero, true));
                    }
                    else
                    {
                        hero.transform.Rotate(0,
                            GameManager.Instance.jamoDrum.spinDelta[hero.player.playerNumber] * rotationScale, 0);

                        if (hero.isSpeedIncrease || isSuddenDeath)
                        {
                            hero.GetComponent<Rigidbody>().velocity = hero.transform.forward * hero.speed * 1.5f;
                        }
                        else
                            hero.GetComponent<Rigidbody>().velocity = hero.transform.forward * hero.speed;
                    }
                    hero.Charge = hero.isContDash ? Mathf.Clamp(hero.Charge + (hero.chargingSpeed *2) * Time.deltaTime, 0, 1) : Mathf.Clamp(hero.Charge + hero.chargingSpeed * Time.deltaTime, 0, 1);

                    if (hero.Charge == 1) hero.currentTrail.emitting = true;
                }

               
                // pos += p.transform.forward * p.speed * Time.deltaTime;
                // p.transform.position = pos;

                // var rot = p.transform.rotation.eulerAngles;
                // if (p.dashing && p.GetComponent<Rigidbody>().velocity.magnitude < 0.01)
                // {
                //     SetHeroDashing(p, false);
                // }
                
            });
            yield return null;
        }
    }

    private IEnumerator SetHeroDashing(Hero p, bool dashing)
    {
        if (dashing)
        {           
            p.Charge = 0;                   
            var rg = p.GetComponent<Rigidbody>();
            rg.velocity = p.transform.forward * p.dashSpeed;
            p.State = HeroState.Dash;
            audioSource.PlayOneShot(p.dashSound);
            //p.GetComponent<MeshRenderer>().material.color = Color.blue;
            //p.image.color = Color.blue;
            p.afterimageShadows.SetActive(true);
            yield return new WaitUntil(() => rg.velocity.magnitude < p.speed);
            if (!p.isSpeedIncrease)
            {
                p.afterimageShadows.SetActive(false);
            }
            StartCoroutine(SetHeroDashing(p, false));
        }
        else
        {
            p.currentTrail.Clear();
            p.currentTrail.emitting = false;
            if (p.State == HeroState.Stun) yield break;
            p.State = HeroState.Walk;
            // p.GetComponent<MeshRenderer>().material.color = Color.white;
            //p.image.color = Color.white;
        }
    }
    //
    // private IEnumerator dash(Player player)
    // {
    //     player.speed = dashConstant * player.speed;
    //     yield return null;
    // }

    public void EndGamePlay()
    {
        StopCoroutine(gameplay);
    }
}
