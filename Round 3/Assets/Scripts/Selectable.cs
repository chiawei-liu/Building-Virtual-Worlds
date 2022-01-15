using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
public class Selectable : MonoBehaviour
{
    //public AudioClip AudioClip;

    public AudioSource audioSource;
    public int index;
    public Voice playerVoice, expectedVoice;
    public bool selectable = false;
    public List<Animator> individualAnimators;
    public List<SkinnedMeshRenderer> skinnedMeshRenderers;
    public List<GameObject> sleepingParticleObjects;
    public List<GameObject> wrongSongParticleObjects;
    public List<GameObject> songParticleObjects;
    [NonSerialized] public List<ParticleSystem> sleepingParticles;
    [NonSerialized] public List<ParticleSystem> wrongSongParticles;
    [NonSerialized] public List<ParticleSystem> songParticles;
    [SerializeField] public List<UnityEvent> onSelects;
    
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        playerVoice = new Voice();
        expectedVoice = new Voice();
        skinnedMeshRenderers = new List<SkinnedMeshRenderer>(GetComponentsInChildren<SkinnedMeshRenderer>());
        sleepingParticles = new List<ParticleSystem>();
        sleepingParticleObjects.ForEach(o => { sleepingParticles.Concat(new List<ParticleSystem>(o.GetComponentsInChildren<ParticleSystem>(true))); });
        songParticles = new List<ParticleSystem>();
        songParticles.ForEach(o => { songParticles.Concat(new List<ParticleSystem>(o.GetComponentsInChildren<ParticleSystem>(true))); });
        wrongSongParticles = new List<ParticleSystem>();
        wrongSongParticles.ForEach(o => { wrongSongParticles.Concat(new List<ParticleSystem>(o.GetComponentsInChildren<ParticleSystem>(true))); });
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
            
    }

    public void SetSelectingProgress(float progress)
    {
        skinnedMeshRenderers.ForEach(meshRenderer =>
        {
            meshRenderer.SetBlendShapeWeight(0, progress * 100);
        });
    }
    
    public void StartAnimation()
    {
        individualAnimators.ForEach(animator =>
        {
            animator.SetBool("Kid", true);
        });
    }
    
    public void StopAnimation()
    {
        individualAnimators.ForEach(animator =>
        {
            animator.SetBool("Kid", false);
        });
    }

    public void Sleeping()
    {
        sleepingParticleObjects.ForEach(p => p.gameObject.SetActive(true));
        wrongSongParticleObjects.ForEach(p => p.gameObject.SetActive(false));
        songParticleObjects.ForEach(p => p.gameObject.SetActive(false));
    }

    public void WrongSong()
    {
        sleepingParticleObjects.ForEach(p => p.gameObject.SetActive(false));
        wrongSongParticleObjects.ForEach(p => p.gameObject.SetActive(true));
        songParticleObjects.ForEach(p => p.gameObject.SetActive(false));
    }

    public void Song()
    {
        sleepingParticleObjects.ForEach(p => p.gameObject.SetActive(false));
        wrongSongParticleObjects.ForEach(p => p.gameObject.SetActive(false));
        songParticleObjects.ForEach(p => p.gameObject.SetActive(true));
    }

    public void TriggerSound()
    {
        GameManager.Instance.songPlayer.TurnOnVoice(this);
    }
}
