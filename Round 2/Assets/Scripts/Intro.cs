using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class Intro : MonoBehaviour
{
    [SerializeField]
    private VideoPlayer vp;
    
    
    // Start is called before the first frame update
    void Start()
    {
        vp.loopPointReached += LoadMainScene;
    }

    // Update is called once per frame
    void Update()
    {
    }

    void LoadMainScene(VideoPlayer videoPlayer)
    {
        StartCoroutine(LoadSceneAsync());
    }

    private IEnumerator LoadSceneAsync()
    {
        var asyncLoad = SceneManager.LoadSceneAsync("FinalMain");
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
