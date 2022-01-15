using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityTemplateProjects;

public class FunkiPopPart : MonoBehaviour
{
    [NonSerialized] public Color32 Color;
    [NonSerialized] public Color32 CorrectColor;
    public List<IColorChangeEventReceiver> receivers;

    public FunkiPopPart()
    {
        receivers = new List<IColorChangeEventReceiver>();
    }
    // Start is called before the first frame update
    void Start()
    {
        Color = GetComponent<MeshRenderer>().material.color;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnCollisionEnter(Collision other)
    {
        var paintBall = other.gameObject.GetComponent<PaintBall>();
        if (paintBall == null) return;
        if (paintBall.hit) return;
        if (GameManager.Instance.hitTarget != gameObject) return;
        paintBall.hit = true;
        GetComponent<MeshRenderer>().material.color = paintBall.color;
        Color = paintBall.color;
        receivers.ForEach(r => r.ColorChangeEvent(this));
    }
}
