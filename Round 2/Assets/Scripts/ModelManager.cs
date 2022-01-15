using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ModelManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> funkiPops;
    [SerializeField] private GameObject funkiPopFree;
    [SerializeField] private List<GameObject> anchors;
    [SerializeField] private float speed;
    // private GameObject curFunki;
    private bool done = false;
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StartCor());
    }

    private IEnumerator StartCor()
    {
        yield return null;
        //yield return StartCoroutine(GameManager.Instance.Intro());
        foreach (var curFunki in funkiPops.Select(funkiPop => Instantiate(funkiPop, anchors[0].transform.position, anchors[1].transform.rotation, transform)))
        {
            yield return MoveTo(curFunki, anchors[1].transform.position);
            GameManager.Instance.scorePanel.Reset(curFunki.GetComponent<FunkiPop>().parts.Count);
            GameManager.Instance.scorePanel.gameObject.SetActive(true);
            GameManager.Instance.poster.enabled = true;
            GameManager.Instance.poster.sprite = curFunki.GetComponent<FunkiPop>().poster;
            curFunki.GetComponent<FunkiPop>().UpdateScores();
            done = false;
            yield return new WaitUntil(() => done);
            
            GameManager.Instance.scorePanel.PlayDoneAudio();
            yield return new WaitForSeconds(1);
            yield return MoveTo(curFunki, anchors[2].transform.position);
            GameManager.Instance.poster.enabled = false;
            GameManager.Instance.scorePanel.gameObject.SetActive(false);
            yield return new WaitForSeconds(1);
            Destroy(curFunki);
        }
        GameManager.Instance.EndGame();
        // var freeFunki = Instantiate(funkiPopFree, anchors[0].transform.position, anchors[1].transform.rotation, transform);
        // yield return MoveTo(freeFunki, anchors[1].transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B)) done = true;
    }

    // private IEnumerator NewPop(GameObject funkiPop)
    // {
    //     curFunki = Instantiate(funkiPop, anchors[0].transform.position, anchors[1].transform.rotation, transform);
    //     yield return MoveTo(curFunki, anchors[1].transform.position);
    //     
    // }

    private IEnumerator MoveTo(GameObject go, Vector3 dest)
    {
        GameManager.Instance.belt.StartPlaying();
        while (Vector3.Distance(go.transform.position, dest) > 0.01)
        {
            go.transform.position = Vector3.MoveTowards(go.transform.position, dest, speed * Time.deltaTime);
            yield return null;
        }
        GameManager.Instance.belt.StopPlaying();
    }

    public void Done()
    {
        done = true;
    }
}
