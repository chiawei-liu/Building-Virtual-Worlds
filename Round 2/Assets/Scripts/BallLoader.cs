using UnityEngine;

public class BallLoader : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        var ballGenerator = other.gameObject.GetComponent<GrabablePaintBall>();
        if (ballGenerator == null) return;
        GetComponentInChildren<SlingShotPad>().GenerateBall(ballGenerator.ballPrefab);
        Destroy(other.gameObject);
    }
}
