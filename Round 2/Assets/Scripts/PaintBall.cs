using System.Collections;
using UnityEngine;

public class PaintBall : MonoBehaviour
{
    private AudioSource audioSource;
    public bool hit = false;
    [SerializeField] public Color32 color;
    [SerializeField] private GameObject splashVFX;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Paintable")) return;
        if (GameManager.Instance.hitTarget != other.gameObject) return;
        var contact = other.GetContact(0);
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<Collider>().enabled = false;
        StartCoroutine(SplashEffects(contact.point, contact.normal));
        StartCoroutine(SplashSound());
    }

    private IEnumerator SplashSound()
    {
        audioSource.Play();
        yield return new WaitForSeconds(audioSource.clip.length);
        Destroy(gameObject);
    }

    private IEnumerator SplashEffects(Vector3 pos, Vector3 normal)
    {
        GameManager.Instance.splashMat.color = new Color32(color.r, color.g, color.b, 255);
        var vfx = Instantiate(splashVFX, pos, new Quaternion(), transform);
        vfx.transform.up = -normal;
        yield return new WaitForSeconds(2);
        Destroy(vfx);
    }
}
