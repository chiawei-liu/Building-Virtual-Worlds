using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpringJoint))]
[RequireComponent(typeof(LineRenderer))]
public class SpringRenderer : MonoBehaviour
{
    private SpringJoint sj;
    private LineRenderer lr;
    [SerializeField] private GameObject attachedObject;
    
    // Start is called before the first frame update
    void Start()
    {
        sj = GetComponent<SpringJoint>();
        lr = GetComponent<LineRenderer>();
        lr.SetPosition(0, transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        lr.SetPosition(1, attachedObject.transform.position);
        lr.SetPosition(0, transform.position);
    }
}
