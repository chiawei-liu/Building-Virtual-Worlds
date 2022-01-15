using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class Metronome : MonoBehaviour
{
    [SerializeField] private List<Sprite> numbers;
    private Image image;
    // Start is called before the first frame update
    private float pos, startTime;
    
    void Start()
    {
        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Play(int nBeats, float audioLength)
    {
        image.enabled = true;
        startTime = (float) AudioSettings.dspTime;
        pos = 0;
        StartCoroutine(PlayMetroNome(nBeats, audioLength));
    }

    private IEnumerator PlayMetroNome(int nBeats, float audioLength)
    {
        var index = 0;
        var beatLength = audioLength / nBeats;
        while (pos < audioLength)
        {
            if (pos > index * beatLength)
            {
                image.sprite = numbers[index++];
            }
            pos = (float) AudioSettings.dspTime - startTime;
            yield return null;
        }
        image.enabled = false;
    }
}
