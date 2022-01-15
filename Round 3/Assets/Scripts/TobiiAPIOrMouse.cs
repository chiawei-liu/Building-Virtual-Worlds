using System;
using System.IO.Compression;
using System.Runtime.InteropServices.WindowsRuntime;
using Tobii.Gaming;
using UnityEngine;

public class TobiiAPIOrMouse : MonoBehaviour
{
    public bool UseMouse = false;
    public Camera cam;
    private static TobiiAPIOrMouse instance;
    [NonSerialized] public GameObject MouseFocusedObject;
    public static TobiiAPIOrMouse Instance
    {
        get
        {
            if (instance != null) return instance;
            instance = FindObjectOfType<TobiiAPIOrMouse>();
            if (instance != null) return instance;
            var newObject = new GameObject();
            instance = newObject.AddComponent<TobiiAPIOrMouse>();
            return instance;
        }
    }
    

    protected void Awake()
    {
        instance = this;
        TobiiAPI.Start();
        // cam = GetComponent<Camera>();
    }

    private void Start()
    {
        // cam = GetComponent<Camera>();
    }

    public GameObject GetFocusedObject()
    {
        return !UseMouse ? TobiiAPI.GetFocusedObject() : MouseFocusedObject;
    }

    private void Update()
    {
        if (!UseMouse) return;
        
        MouseFocusedObject = null;
        var ray = GameManager.Instance.cam.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out var hit)) return;
        if (hit.collider == null) return;
        MouseFocusedObject = hit.collider.gameObject;
        var gazeAwareOrMouse = MouseFocusedObject.GetComponent<GazeAwareOrMouse>();
        if (gazeAwareOrMouse != null)
        {
            gazeAwareOrMouse.HasMouseFocus = true;
        }

        // Debug.Log(cam.pixelWidth + " " + cam.pixelHeight);
        // Debug.Log(Input.mousePosition.x + " " + Input.mousePosition.y);
        // Debug.Log("-----------------------------");
    }
}
        
