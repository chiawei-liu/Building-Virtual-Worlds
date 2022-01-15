using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class GrabablePaintBall : MonoBehaviour
{
    private AudioSource audioSource;

    public GameObject ballPrefab;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnGrab()
    {
        CloneBall();
        PlayPickUpSound();
        GetComponent<Collider>().isTrigger = true;
    }

    public void OnDrop()
    {
        GetComponent<Collider>().isTrigger = false;
    }

    private void CloneBall()
    {
        Instantiate(gameObject, new Vector3(transform.position.x, transform.position.y, transform.position.z), transform.rotation, transform.parent);
    }

    private void PlayPickUpSound()
    {
        audioSource.Play();
    }
}
