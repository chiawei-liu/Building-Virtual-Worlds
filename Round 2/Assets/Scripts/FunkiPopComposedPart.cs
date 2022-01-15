using System.Collections.Generic;
using UnityEngine;
using UnityTemplateProjects;

public class FunkiPopComposedPart : FunkiPopPart, IColorChangeEventReceiver
{
    [SerializeField] private List<GameObject> parts;
    private List<FunkiPopPart> funkiPopParts;
    
    public FunkiPopComposedPart()
    {
        // receivers = new List<IColorChangeEventReceiver>();
        // parts.ForEach(p => p.receivers.Add(this));
    }
    
    // Start is called before the first frame update
    void Start()
    {
        Color = parts[0].GetComponent<MeshRenderer>().material.color;
        funkiPopParts = new List<FunkiPopPart>();
        for (int i = 0; i < parts.Count; i++)
        {
            var comp = parts[i].GetComponent<FunkiPopPart>();
            if (comp == null)
            {
                comp = parts[i].AddComponent<FunkiPopPart>();
            }
            funkiPopParts.Add(comp);funkiPopParts[i].receivers.Add(this);
            funkiPopParts[i].CorrectColor = CorrectColor;
        }
        // parts.ForEach(p => p.GetComponent<FunkiPopPart>().receivers.Add(this));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnCollisionEnter(Collision other)
    {
        
    }

    public void ColorChangeEvent(FunkiPopPart part)
    {
        parts.ForEach(p =>
        {
            p.GetComponent<MeshRenderer>().material.color = part.Color;
            p.GetComponent<FunkiPopPart>().Color = part.Color;
        });
        Color = part.Color;
        receivers.ForEach(r => r.ColorChangeEvent(this));
    }
}
