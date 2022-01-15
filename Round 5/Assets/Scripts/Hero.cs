using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class Hero : MonoBehaviour
{
    [SerializeField] public float speed;
    [SerializeField] public float dashSpeed;
    [SerializeField] public SpriteRenderer image;
    [SerializeField] public Animator animator;
    [NonSerialized] public Player player;
    [NonSerialized] public Collectable carryingCollectable;

    [NonSerialized] public bool isContDash = false;
    [NonSerialized] public bool isSpeedIncrease = false;
    [NonSerialized] public bool isShield = false;
    [NonSerialized] public float chargingSpeed;
    // [NonSerialized] public bool dashing = false;
    [NonSerialized] public bool canCollect = true;
    // [NonSerialized] public bool stun = false;
    [NonSerialized] public float stunDuration;
    [NonSerialized] public AudioClip dashSound;

    //[NonSerialized] public GameManager gameManager;
    [SerializeField] private GameObject[] powerUpVisuals;
    [SerializeField] private GameObject stunVisuals;
    [SerializeField] public GameObject afterimageShadows;
    [SerializeField] public List<TrailRenderer> teamTrails;
    [SerializeField] public TrailRenderer rainbowTrail;
    [NonSerialized] public TrailRenderer currentTrail;
    
    public float Charge
    {
        get => charge;
        set
        {
            charge = value;
            chargeChangedEvent?.Invoke(this, EventArgs.Empty);
        }
    }

    private float charge;
    public event EventHandler chargeChangedEvent;
    public event EventHandler haveBallEvent;
    public event EventHandler<CollectableEventArgs> dropBallEvent;
    public event EventHandler stunEvent;
    public event EventHandler<PowerUpPickUpEventArgs> powerUpPickUpEvent;
    private Coroutine shieldDelay;
    private GameObject shield;
    
    public HeroState State
    {
        get => state;
        set
        {
            state = value;
            animator.SetInteger("State", (int)value);
        }
    }
    private HeroState state;
    
    // Start is called before the first frame update
    void Start()
    {
        //gameManager = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // private void OnTriggerEnter(Collider other)
    // {
    //     var collectable = other.gameObject.GetComponent<Collectable>();
    //     if (collectable != null && !collectable.collected && canCollect)
    //     {
    //         other.gameObject.transform.parent = transform;
    //         collectable.collected = true;
    //         carrying = collectable;
    //     }
    // }

    private void OnTriggerEnter(Collider other)
    {
        OnTriggerEnterOrStay(other);
    }

    private void OnTriggerStay(Collider other)
    {
        OnTriggerEnterOrStay(other);
    }

    private void OnTriggerEnterOrStay(Collider other)
    {
        // hit fire
        var fire = other.gameObject.GetComponent<Fire>();
        if (fire != null && State != HeroState.Stun && !isShield)
        {
            StartCoroutine(Stun(this, true));
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        OnCollisionEnterOrStay(other);

    }

    private void OnCollisionStay(Collision other)
    {
        OnCollisionEnterOrStay(other);
    }

    private void OnCollisionEnterOrStay(Collision other)
    {
        // pick up
        var collectable = other.gameObject.GetComponent<Collectable>();
        if (collectable != null && !collectable.collected && State != HeroState.Stun && carryingCollectable == null)
        {
            var dir3 = (other.transform.position - transform.position);
            dir3.y = 0;
            dir3 = dir3.normalized * 0.5f;
            other.transform.position = new Vector3(transform.position.x + dir3.x, other.transform.position.y, transform.position.z + dir3.z);
            other.gameObject.transform.parent = transform;
            
            collectable.collected = true;
            carryingCollectable = collectable;
            collectable.carriedBy = this;
            
            haveBallEvent?.Invoke(this, EventArgs.Empty);
        }
        
        // hit by other
        var theOtherHero = other.gameObject.GetComponent<Hero>();
        if (theOtherHero != null && theOtherHero.player.side != player.side && theOtherHero.state == HeroState.Dash)
        {
            if (isShield)
            {
                if (shieldDelay != null)
                {
                    StopCoroutine(shieldDelay);
                    Destroy(shield);
                    isShield = false;
                }
            }
            else
            {
                StartCoroutine(Stun(theOtherHero, false));
            }
        }

        // hit wall
        if (other.gameObject.CompareTag("Wall") && State == HeroState.Dash)
        {
            State = HeroState.Walk;
            // p.GetComponent<MeshRenderer>().material.color = Color.white;
            //image.color = Color.white;
        }
        
        

        switch (other.gameObject.tag)
        {
            case "ContinuousDash":
                powerUpPickUpEvent?.Invoke(this, new PowerUpPickUpEventArgs
                {
                    powerUp = other.gameObject
                });
                isContDash = true;
                GameManager.Instance.DashUI.SetPowerUp(player, true);
                StartCoroutine(ContinuousDash());
                break;
            case "SpeedIncrease":
                powerUpPickUpEvent?.Invoke(this, new PowerUpPickUpEventArgs
                {
                    powerUp = other.gameObject
                });
                isSpeedIncrease = true;
                StartCoroutine(SpeedIncrease());
                break;
            case "ShieldPower":
                powerUpPickUpEvent?.Invoke(this, new PowerUpPickUpEventArgs
                {
                    powerUp = other.gameObject
                });
                isShield = true;
                shieldDelay = StartCoroutine(ShieldPower());
                break;
        }
    }

    private IEnumerator Stun(Hero theOtherHero, bool resetPos)
    {
        yield return null;
        stunEvent?.Invoke(this, EventArgs.Empty);
        Collectable c = carryingCollectable;
        if (c != null)
        {
            Drop();
            var eventArgs = new CollectableEventArgs { Collectable = c, Reset = resetPos };
            dropBallEvent?.Invoke(this, eventArgs);
        }

        State = HeroState.Stun;
        if (resetPos)
        {
            animator.SetTrigger("Burnt");
        }
        //image.color = Color.red;
        gameObject.layer = LayerMask.NameToLayer("Stun");
        var vfx = Instantiate(stunVisuals, transform);
        currentTrail.Clear();
        yield return new WaitForSeconds(stunDuration / 2);
        
        //burnt
        if (resetPos)
        {
            GetComponent<Rigidbody>().constraints ^= RigidbodyConstraints.FreezePositionY;
            transform.position = player.transform.position;
            transform.rotation = player.transform.rotation;
            currentTrail.Clear();
        }        
        yield return new WaitForSeconds(stunDuration / 2);
        Destroy(vfx);
        GetComponent<Rigidbody>().constraints |= RigidbodyConstraints.FreezePositionY;
        State = HeroState.Walk;
        gameObject.layer = LayerMask.NameToLayer("Hero");
    }

    public void Drop()
    {
        carryingCollectable.gameObject.transform.parent = null;
        carryingCollectable.collected = false;
        carryingCollectable.carriedBy = null;
        carryingCollectable = null;
    }

    //Tanvitest
    /*private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "ContinuousDash")
        {
            ContinuousDash();
        }
    }*/

    /*public void ContinuousDash()
    {
        Debug.Log("Continuous Dash Started");
        if (!isTimeDone)
        {
            heroManager.GetComponent<HeroManager>().isDashPowerUp = true;
            StartCoroutine(powerUpCounter());
        }

        if (isTimeDone)
        {
            heroManager.GetComponent<HeroManager>().isDashPowerUp = false;
        }

    }*/

    public IEnumerator ShieldPower()
    {
        //Debug.Log("Shield Power Started");
        //Debug.Log("Timer on");
        Vector3 tempPos = this.transform.position;
        shield = Instantiate(powerUpVisuals[0], new Vector3(tempPos.x, tempPos.y, tempPos.z), Quaternion.identity);
        //var shield = Instantiate(powerUpVisuals[0], this.transform.GetChild(0).GetChild(0).transform);
        shield.transform.parent = this.transform;
        yield return new WaitForSeconds(5f);
        Destroy(shield.gameObject);
        isShield = false;
    }

    private IEnumerator ShieldDelay(float time)
    {
        yield return new WaitForSeconds(time);
    }

    private IEnumerator ContinuousDash()
    {
        Vector3 tempPos = this.transform.position;
        // var shield = Instantiate(powerUpVisuals[1], new Vector3(tempPos.x, tempPos.y, tempPos.z), Quaternion.identity);
        // //var shield = Instantiate(powerUpVisuals[0], this.transform.GetChild(0).GetChild(0).transform);
        // shield.transform.parent = this.transform;
        var emitting = currentTrail.emitting;
        currentTrail.emitting = false;
        currentTrail = rainbowTrail;
        currentTrail.emitting = emitting;
        yield return new WaitForSeconds(10f);
        emitting = currentTrail.emitting;
        currentTrail.emitting = false;
        currentTrail = teamTrails[player.side];
        currentTrail.emitting = emitting;
        
        Debug.Log("Timer off");
        // Destroy(shield.gameObject);
        isContDash = false;
        GameManager.Instance.DashUI.SetPowerUp(player, false);
    }

    public IEnumerator SpeedIncrease()
    {
        Vector3 tempPos = this.transform.position;
        afterimageShadows.SetActive(true);
        yield return new WaitForSeconds(10f);
        isSpeedIncrease = false;
        afterimageShadows.SetActive(false);
        // if (player.side == 0)
        // {
        //     var shield = Instantiate(powerUpVisuals[2], new Vector3(tempPos.x, tempPos.y + 1.5f, tempPos.z), Quaternion.identity);
        //     shield.transform.parent = this.transform;
        //     yield return new WaitForSeconds(10f);
        //     Destroy(shield.gameObject);
        // }
        //
        // else
        // {
        //     var shield = Instantiate(powerUpVisuals[3], new Vector3(tempPos.x, tempPos.y + 1.5f, tempPos.z), Quaternion.identity);
        //     shield.transform.parent = this.transform;
        //     yield return new WaitForSeconds(10f);
        //     Destroy(shield.gameObject);
        // }
        //var shield = Instantiate(powerUpVisuals[0], this.transform.GetChild(0).GetChild(0).transform);
    }
}




    /*public void SpeedIncrease()
    {
        Debug.Log("More Speed Started");
        if (!isTimeDone)
        {
            heroManager.GetComponent<HeroManager>().isSpeedPowerUp = true;
            StartCoroutine(powerUpCounter());
        }

        if (isTimeDone)
        {
            heroManager.GetComponent<HeroManager>().isSpeedPowerUp = false;
        }
    }*/


