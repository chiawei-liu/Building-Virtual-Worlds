using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public static HUD Instance;
    [SerializeField] private List<Sprite> instructions;
    // [SerializeField] private Image backgroundPanel;
    [SerializeField] private GameObject sandWarning;
    [SerializeField] private Animator animator;
    [SerializeField] private List<Sprite> batteries;
    [SerializeField] private Sprite errorHUD;
    [SerializeField] private Sprite errorHUD3Seconds;
    [SerializeField] private Image instructionImage;
    [SerializeField] private Image batteryImage;
    private int instructionIndex = 0, batteryIndex = 0;
    private AudioSource audioSource;
    
    // Start is called before the first frame update
    private void Awake()
    {
        Instance = this;
        audioSource = GetComponent<AudioSource>();
        // instructionImage = GetComponent<Image>();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NextInstruction()
    {
        instructionImage.enabled = true;
        instructionImage.sprite = instructions[++instructionIndex];
        Debug.Log("Instruction index: " + instructionIndex);
        animator.SetTrigger("Pop");
        audioSource.Play();
    }

    public void ClearInstruction()
    {
        instructionImage.enabled = false;
    }

    public void SetWarningActive(bool active)
    {
        sandWarning.SetActive(active);
    }

    public void ErrorGlitch()
    {
        batteryImage.enabled = false;
        instructionImage.enabled = false;
        GetComponent<Image>().sprite = errorHUD;
        animator.SetBool("Error", true);
    }

    public void ErrorGlitchPhase2()
    {
        GetComponent<Image>().sprite = errorHUD3Seconds;
    }

    public void ShutDown()
    {
        animator.SetTrigger("Shutdown");
    }

    public void NextBattery()
    {
        batteryIndex++;
        if (batteryIndex < batteries.Count - 1)
        {
            batteryImage.sprite = batteries[batteryIndex];
        }
    }
    
}
