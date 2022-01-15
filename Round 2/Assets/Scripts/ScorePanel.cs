using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class ScorePanel : MonoBehaviour
{
    [SerializeField] private GameObject star;
    [SerializeField] private Sprite scoredSprite, notScoredSprite;
    [SerializeField] private List<AudioClip> scoreAudioClips;
    private List<GameObject> stars;
    private Material glowMat;
    private int preScore = 0;
    private AudioSource audioSource;
    [SerializeField] private AudioClip gainScoreSound;
    [SerializeField] private AudioClip loseScoreSound;
    private int maxScore;
    
    // Start is called before the first frame update
    void Start()
    {
        glowMat = Resources.Load<Material>("Shaders/Star");
        stars = new List<GameObject>();
        audioSource = GetComponent<AudioSource>();
        var a = 0;
        // stars = new List<GameObject>();
        // stars.Add(star);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Reset(int maxScore)
    {
        stars.ForEach(Destroy);
        stars = new List<GameObject>();
        for (int i = 0; i < maxScore; i++)
        {
            stars.Add(Instantiate(star, transform));
        }
        preScore = 0;
        this.maxScore = maxScore;
    }

    public void SetScore(int score)
    {
        Image image;
        for (var i = 0; i < score; i++)
        {
            image = stars[i].GetComponent<Image>();
            image.sprite = scoredSprite;
            // image.material = null;
        }

        for (var i = score; i < maxScore; i++)
        {
            image = stars[i].GetComponent<Image>();
            image.sprite = notScoredSprite;
            // image.material = null;
        }
        audioSource.Stop();
        if (score > preScore)
        {
            audioSource.PlayOneShot(gainScoreSound);
        } else if (score < preScore)
        {
            audioSource.PlayOneShot(loseScoreSound);
        }
        preScore = score;
    }

    public void PlayDoneAudio()
    {
        GetComponent<AudioSource>().PlayOneShot(scoreAudioClips[preScore]);
    }

    public void CleanPanel()
    {
        stars.ForEach(Destroy);
    }
}
