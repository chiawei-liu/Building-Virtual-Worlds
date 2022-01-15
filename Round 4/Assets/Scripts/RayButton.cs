using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class RayButton : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private Image baseImage;

    [SerializeField] private float holdTime;
    [SerializeField] private float cancelTime;
    [SerializeField] private AudioClip hoverSound;
    [SerializeField] private AudioClip holdSound;
    [SerializeField] private AudioClip activateSound;
    private bool activated;
    private Coroutine selection;
    private Coroutine cancelSelection;
    private Color32 baseColor;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        baseColor = baseImage.color;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnHoverEnter()
    {
        if (activated) return;
        audioSource.Stop();
        audioSource.PlayOneShot(hoverSound);
        baseImage.color = Color.white;
    }

    public void OnHoverExit()
    {
        if (activated) return;
        baseImage.color = baseColor;
        //LeaveButton();
    }

    public void HoldStartButton()
    {
        if (activated) return;

        if (cancelSelection != null)
        {
            StopCoroutine(cancelSelection);
            cancelSelection = null;
        }

        if (selection != null) return;
        audioSource.Stop();
        audioSource.PlayOneShot(holdSound);

        selection = StartCoroutine(StartButtonSelection(holdTime));
    }

    public void HoldExitButton()
    {
        if (activated) return;

        if (cancelSelection != null)
        {
            StopCoroutine(cancelSelection);
            cancelSelection = null;
        }

        if (selection != null) return;
        audioSource.Stop();
        audioSource.PlayOneShot(holdSound);
        selection = StartCoroutine(ExitButtonSelection(holdTime));
    }
    

    public void LeaveButton()
    {
        if (activated) return;

        if (selection != null)
        {
            StopCoroutine(selection);
            selection = null;
        }

        cancelSelection ??= StartCoroutine(CancelSelection(cancelTime));
    }

    private IEnumerator ExitButtonSelection(float time)
    {
        yield return Selection(time);
        activated = true;
        audioSource.Stop();
        audioSource.PlayOneShot(activateSound);
        fillImage.color = baseColor;

        Transition.Instance.SetSandStormActive(true);
        yield return new WaitForSeconds(2f);
        Transition.Instance.SceneFadeOut();
        StartCoroutine(FadeOutSound(2f));
        yield return new WaitForSeconds(1f);
        yield return new WaitForSeconds(2f);
        // exit game
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
    }
    
    private IEnumerator StartButtonSelection(float time)
    {
        yield return Selection(time);
        activated = true;
        
        // start game
        audioSource.Stop();
        audioSource.PlayOneShot(activateSound);
        fillImage.color = baseColor;
        Transition.Instance.SetSandStormActive(true);
        yield return new WaitForSeconds(2f);
        Transition.Instance.SceneFadeOut();
        StartCoroutine(FadeOutSound(2f));
        yield return new WaitForSeconds(1f);
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("Muru_Scene");
        // while (!asyncLoad.isDone)
        // {
        //     yield return null;
        // }
    }

    private IEnumerator FadeOutSound(float fadeTime)
    {
        var startVolume = audioSource.volume;
        while (audioSource.volume > 0) {
            audioSource.volume -= startVolume * Time.deltaTime / fadeTime;
            yield return null;
        }
    }

    private IEnumerator Selection(float holdTime)
    {
        var selectSpeed = 1 / holdTime;
        var amount = fillImage.fillAmount;
        while (amount < 1)
        {
            amount += selectSpeed * Time.deltaTime;
            fillImage.fillAmount = amount;
            yield return null;
        }
    }

    private IEnumerator CancelSelection(float cancelTime)
    {
        var cancelSpeed = 1 / cancelTime; 
        var amount = fillImage.fillAmount;
        while (amount > 0)
        {
            amount -= cancelSpeed * Time.deltaTime;
            fillImage.fillAmount = amount;
            yield return null;
        }
    }
}
