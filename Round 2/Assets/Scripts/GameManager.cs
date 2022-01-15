using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class GameManager : MonoBehaviour
{
    public ScorePanel scorePanel;
    public Material splashMat;
    [SerializeField] private AudioClip endAudioClip;
    [SerializeField] private AudioClip introAudioClip;
    private static GameManager _instance;
    private AudioSource audioSource;
    public GameObject hitTarget;
    public Image poster;
    public Image infront;
    [SerializeField] private Sprite credits;
    [SerializeField] private List<Sprite> intros;
    [SerializeField] private List<float> introDurations;
    public Belt belt;
    public static GameManager Instance
    {
        get
        {
            if (_instance != null) return _instance;
            _instance = FindObjectOfType<GameManager>();
            if (_instance != null) return _instance;
            var newObject = new GameObject();
            _instance = newObject.AddComponent<GameManager>();
            return _instance;
        }
    }
    // Start is called before the first frame update
    protected virtual void Awake()
    {
        _instance = this;
    }
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        belt = FindObjectOfType<Belt>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator Intro()
    {
        yield return null;
        infront.enabled = true;
        audioSource.PlayOneShot(introAudioClip);
        for (var i = 0; i < intros.Count; i++)
        {
            infront.sprite = intros[i];
            yield return new WaitForSeconds(introDurations[i]);
        }
        infront.enabled = false;
    }

    public void EndGame()
    {
        audioSource.PlayOneShot(endAudioClip);
        infront.enabled = true;
        infront.sprite = credits;
    }
    
}
