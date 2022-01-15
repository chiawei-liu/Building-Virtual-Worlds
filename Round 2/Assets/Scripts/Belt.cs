using UnityEngine;

public class Belt : MonoBehaviour
{
    [SerializeField] private Material beltMat;

    [SerializeField] private float speed;

    private Vector2 offset;

    private bool playing = false;
    // Start is called before the first frame update
    void Start()
    {
        offset = new Vector2();
        //beltMat = GetComponent<MeshRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        if (!playing) return;
        offset.x += Time.deltaTime * speed;
        GetComponent<MeshRenderer>().material.mainTextureOffset = offset;
        if (offset.x > 0.2)
        {
            offset.x -= 0.2f;
        }
    }

    public void StartPlaying()
    {
        playing = true;
    }

    public void StopPlaying()
    {
        playing = false;
    }
}
