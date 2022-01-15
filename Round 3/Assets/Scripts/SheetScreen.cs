using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SheetScreen : MonoBehaviour
{
    [SerializeField] private GameObject progressIndicator;
    [SerializeField] private Transform indicatorStartPos, indicatorEndPos;
    private RectTransform rt;
    private float curDspTime, dspTime;
    private float rectLength;
    [SerializeField] private List<GameObject> voiceLines;
    [SerializeField] private GameObject bar;
    [SerializeField] private Image sheetBackground;
    
    private void Awake()
    {
        rt = GetComponent<RectTransform>();
        rectLength = rt.rect.width;
    }

    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(Play());
    }

    // Update is called once per frame
    void Update()
    {
        
       
    }
    
    public void SetIndicator(float pos)
    {
        progressIndicator.GetComponent<RectTransform>().anchoredPosition =
            new Vector2(rectLength * pos, 0);
    }

    public void SetupBackgroundBars(Line line)
    {
        sheetBackground.sprite = Resources.Load<Sprite>(line.spriteName);
        voiceLines.ForEach(v => v.SetActive(false));
        for (var i = 0; i < line.voices.Count; i++)
        {
            voiceLines[i].SetActive(true);
            var go = voiceLines[i].transform.GetChild(0);
            for (var j = 0; j < line.voices[i].startPoints.Count; j++)
            {
                var x1 = line.voices[i].startPoints[j];
                var x2 = line.voices[i].endPoints[j];
                x1 = x1 / line.audioLength * rectLength;
                x2 = x2 / line.audioLength * rectLength;
                //x2 = ;
                var bar = Instantiate(go, voiceLines[i].transform);
                bar.GetComponent<RectTransform>().offsetMin =
                    new Vector2(x1, 0);
                bar.GetComponent<RectTransform>().offsetMax =
                    new Vector2(x2 - rectLength, 0);
                bar.gameObject.SetActive(x1 != rectLength);
            }
        }
    }

    public GameObject StartBar(int index, float pos)
    {
        var x1 = pos * rectLength;
        var go = voiceLines[index].transform.GetChild(0);
        var bar = Instantiate(go, voiceLines[index].transform).gameObject;
        var color = bar.GetComponent<Image>().color;
        bar.GetComponent<Image>().color = new Color(color.r, color.g, color.b, 1);
        bar.GetComponent<RectTransform>().offsetMin =
            new Vector2(x1, 0);
        bar.GetComponent<RectTransform>().offsetMax =
            new Vector2(x1 - rectLength, 0);
        bar.SetActive(true);
        return bar;
    }

    public void UpdateBar(int index, float pos, GameObject bar)
    {
        var x2 = pos * rectLength;
        bar.GetComponent<RectTransform>().offsetMax =
            new Vector2(x2 - rectLength, 0);
    }

    public void Clean()
    {
        voiceLines.ForEach(v =>
        {
            for (int i = 0; i < v.transform.childCount; i++)
            {
                var bar =  v.transform.GetChild(i).gameObject;
                if (bar.activeSelf == true)
                {
                    Destroy(bar);
                }
            }
        });
    }
}
