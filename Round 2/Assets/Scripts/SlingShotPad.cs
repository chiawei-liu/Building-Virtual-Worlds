using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SlingShotPad : MonoBehaviour
{
    [SerializeField] private GameObject ballPrefab;

    [SerializeField] private List<SpringJoint> springs;
    [SerializeField] private float springStrength;

    [SerializeField] private AudioClip onDragSound;
    [SerializeField] private AudioClip onLoadSound;
    [SerializeField] private AudioClip onFireSound;
    [SerializeField] private float ballLoadingOffset;
    
    private Vector3 localOrigin;
    // private bool ballReleased = false;
    private GameObject ball;
    private AudioSource audioSource;
    private Rigidbody rb;
    private bool dragged = false;
    private GameObject originalParent;
    private LineRenderer lr;

    // Start is called before the first frame update
    void Start()
    {
        lr = GetComponent<LineRenderer>();
        rb = GetComponent<Rigidbody>();
        localOrigin = transform.localPosition + Vector3.forward * 0.01f;
        audioSource = GetComponent<AudioSource>();
        originalParent = transform.parent.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        // Vector3 localVelocity = transform.InverseTransformDirection(rb.velocity);
        // localVelocity.y = 0;
        // rb.velocity = transform.TransformDirection(localVelocity);
        //transform.parent.TransformPoint(localOrigin)
        
        //if (ball == null) return;
        // if (ballReleased) return;
        if (ball == null) return;
        ball.transform.position = transform.position + transform.forward * ballLoadingOffset;
        if (!dragged)
        {
            transform.localPosition = localOrigin;
        }
        else
        {
            transform.up = originalParent.transform.parent.up;
            transform.forward = originalParent.transform.TransformPoint(localOrigin) - transform.position;
            // Does the ray intersect any objects excluding the player layer
            if (!Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out var hit, 20,
                ~8)) return;
            lr.SetPosition(0, transform.position);
            lr.SetPosition(1, hit.point);
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            Debug.Log("Did Hit");
            //GameManager.Instance.hitTarget = hit.collider.gameObject;
        }
    }

    public void GenerateBall(GameObject ball)
    {
        if (this.ball != null)
        {
            Destroy(this.ball);
        }
        this.ball = Instantiate(ball, transform.position + transform.forward * ballLoadingOffset, new Quaternion());
        // var color = this.ball.GetComponent<PaintBall>().color;
        // var newColor = new Color32(color.r, color.g, color,)
        // GameManager.Instance.splashMat.color = new Color32(color.r, color.g, color.b, 255);
        audioSource.PlayOneShot(onLoadSound);
        // ballReleased = false;
    }

    public void OnDrag()
    {
        transform.parent = transform.parent.parent;
        springs.ForEach(s => s.spring = 0);
        dragged = true;
        lr.enabled = true;
        audioSource.PlayOneShot(onDragSound);
    }

    public void ReleaseSpring()
    {
        StartCoroutine(ReleaseSpringCor());
    }

    private IEnumerator ResetPad(float second)
    {
        yield return new WaitForSeconds(second);
        transform.parent = originalParent.transform;
        transform.rotation = transform.parent.rotation;
        transform.localPosition = localOrigin;
        rb.velocity = new Vector3();
        // rb.isKinematic = true;
        springs.ForEach(s => s.spring = 0);
    }

    private IEnumerator ApplyBallGravity(float second)
    {
        yield return new WaitForSeconds(second);

        ball.GetComponent<Rigidbody>().useGravity = true;
        ball = null;
    }
    
    private IEnumerator DestroyAfterSeconds(float second, GameObject go)
    {
        yield return new WaitForSeconds(second);
        Destroy(go);
    }

    private IEnumerator ReleaseSpringCor()
    {
        lr.enabled = false;
        if (ball != null)
        {
            // Does the ray intersect any objects excluding the player layer
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out var hit, 20, ~8))
            {
                GameManager.Instance.hitTarget = hit.collider.gameObject;
            }
            ball.GetComponent<Collider>().enabled = true;
            ball.GetComponent<Rigidbody>().isKinematic = false;
            yield return null;
        }

        dragged = false;
        springs.ForEach(s => s.spring = springStrength);
        audioSource.Stop();
        audioSource.PlayOneShot(onFireSound);
        StartCoroutine(ResetPad(0.2f));
        
        if (ball == null) yield break;
        var dir = originalParent.transform.TransformPoint(localOrigin) - transform.position;
        const float speed = 1.5f;
        ball.GetComponent<Rigidbody>().velocity = dir.normalized * speed;
        StartCoroutine(DestroyAfterSeconds(3f, ball));
        ball = null;
    }
}
