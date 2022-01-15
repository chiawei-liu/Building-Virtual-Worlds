using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public Camera cam;
    public Song song;
    public List<Selectable> voiceGroups;
    public SheetScreen sheetScreen;
    public SongPlayer songPlayer;
    public Animator pianoAnimator;

    private List<Line> voiceRecordLines;

    public AudioClip metronome;
    [SerializeField] private Image tutorial;
    [SerializeField] private Image startImage;
    [SerializeField] private StartButton startButton;

    [SerializeField] private GameObject classroom;
    [SerializeField] private GameObject stage; 
    [SerializeField] private GameObject screen;
    [SerializeField] private Animator curtains;
    public AudioClip crowdSound;
    public AudioClip applause;
    [SerializeField] private GameObject credits;
    [SerializeField] private GameObject directionalLight;
    [SerializeField] private GameObject replay;
    public bool startPressed = false; 
    
    public static GameManager Instance
    {
        get
        {
            if (instance != null) return instance;
            instance = FindObjectOfType<GameManager>();
            if (instance != null) return instance;
            GameObject newObject = new GameObject();
            instance = newObject.AddComponent<GameManager>();
            return instance;
        }
    }
    
    protected void Awake()
    {
        instance = this;
        var jsonString = (TextAsset)Resources.Load("song");
        song = Song.CreateFromJSON(jsonString.text);
        for (var i = 0; i < voiceGroups.Count; i++)
        {
            voiceGroups[i].index = i;
        }
        voiceRecordLines = new List<Line>();
        // cam = GetComponent<Camera>();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GameFlow());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator GameFlow()
    {
        // var previousLineLength = 0f;
        yield return new WaitForSeconds(3);
        startButton.gameObject.SetActive(true);
        while (startImage.color.a < 1)
        {
            startImage.color = new Color(startImage.color.r, startImage.color.g, startImage.color.b,
                startImage.color.a + Time.deltaTime * 2);
            yield return null;
        }

        yield return new WaitUntil((() => startPressed));
        Destroy(startImage.gameObject);
        Destroy(tutorial.gameObject);
        Destroy(startButton.gameObject);
        
        for (var i = 0; i < song.lines.Count; i++)
        {
            yield return StartCoroutine(songPlayer.PlayLine(song.lines[i]));
            foreach (var t in voiceGroups)
            {
                voiceRecordLines.Add(new Line());
                voiceRecordLines[i].voices.Add(t.playerVoice);
                t.playerVoice = new Voice();
            }
            yield return new WaitForSeconds(1);
        }
        
        curtains.SetBool("Close", true);
        
        yield return new WaitForSeconds(1.5f);
        directionalLight.SetActive(false);
        classroom.SetActive(false);
        stage.SetActive(true);
        screen.SetActive(false);
        var audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(applause);
        yield return new WaitForSeconds(1f);
        curtains.SetBool("Close", false);
        replay.SetActive(true);
        yield return new WaitUntil(() => !audioSource.isPlaying);
        voiceRecordLines.RemoveRange(0, song.lines.Count / 2);
        song.lines.RemoveRange(0, song.lines.Count / 2);
        
        yield return StartCoroutine(songPlayer.Replay(song.lines, voiceRecordLines));
        yield return new WaitForSeconds(1.5f);
        replay.SetActive(false);
        curtains.SetBool("Close", true);
        GetComponent<AudioSource>().PlayOneShot(applause);
        yield return new WaitForSeconds(2f);

        credits.SetActive(true);
    }

    private void Replay()
    {
    }

    public void StartButtonPressed()
    {
        startPressed = true;
    }
}
