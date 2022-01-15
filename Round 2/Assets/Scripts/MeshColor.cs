using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MeshColor : MonoBehaviour
{
    private Mesh mesh;
    private Vector3[] verts;
    private Color[] colors;
    private AudioSource audioSource;
    [SerializeField] private GameObject splashVFX;
    
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        
        mesh = GetComponent<MeshFilter>().mesh;
        verts = mesh.vertices;

        // create new colors array where the colors will be created.
        colors = new Color[verts.Length];

        for (var i = 0; i < verts.Length; i++)
        {
            colors[i] = Color.white;
        }

        // assign the array of colors to the Mesh.
        mesh.colors = colors;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision other)
    {
        var paintBall = other.gameObject.GetComponent<PaintBall>();
        if (paintBall == null) return;
        var contact = other.GetContact(0);
        var contactPoint = contact.point;

        for (var i = 0; i < verts.Length; i++)
        {
            const float maxDis = 0.05f;
            var dis = Vector3.Distance(transform.TransformPoint(verts[i]), contactPoint);
            if (dis < maxDis)
            {
                colors[i] = paintBall.color;
            }
        }
        
        mesh.colors = colors;
        StartCoroutine(SplashEffects(contact.point, contact.normal));
        audioSource.Play();
        Destroy(other.gameObject);
    }

    private IEnumerator SplashEffects(Vector3 pos, Vector3 normal)
    {
        var vfx = Instantiate(splashVFX, pos, new Quaternion(), transform);
        vfx.transform.up = -normal;
        yield return new WaitForSeconds(2);
        Destroy(vfx);
    }
}
