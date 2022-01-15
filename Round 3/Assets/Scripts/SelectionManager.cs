using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionManager : MonoBehaviour
{
    [SerializeField] private List<Selectable> selectables;

    [SerializeField] private float selectionTime;
    [SerializeField] private float cancelSpeed;
    [SerializeField] private Canvas canvas;
    [SerializeField] private Image progressBar;
    [SerializeField] private List<SelectionEvent> selectionEvents;
    private bool selecting = false, canceling = false;
    private Coroutine selectionCoroutine, cancelCoroutine;
    private Selectable selectingObject;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var gazedObject = TobiiAPIOrMouse.Instance.GetFocusedObject();
        Debug.Log(gazedObject);
        if (gazedObject == null || gazedObject.GetComponent<Selectable>() == null || !gazedObject.GetComponent<Selectable>().selectable)
        {
            if (!selecting || canceling) return;
            StopCoroutine(selectionCoroutine);
            cancelCoroutine = StartCoroutine(CancelSelection());
            return;
        }
        var selectable = gazedObject.GetComponent<Selectable>();
        if (selecting && selectingObject == selectable || !selectable.selectable) return;
        selectionCoroutine =  StartCoroutine(StartSelection(selectable));
    }

    private IEnumerator StartSelection(Selectable selectable)
    {
        if (canceling)
        {
            StopCoroutine(cancelCoroutine);
            canceling = false;
            if (selectable != selectingObject)
            {
                progressBar.fillAmount = 0;
                selectingObject.SetSelectingProgress(0);
            }
        }
        
        selectingObject = selectable;
        selecting = true;
        var pos = selectable.transform.position - selectable.transform.forward * selectable.GetComponent<BoxCollider>().size.z / 2;
        pos =WorldToCanvas(pos);
        progressBar.GetComponent<RectTransform>().anchoredPosition = pos;
        progressBar.enabled = true;
        
        while (progressBar.fillAmount < 1)
        {
            progressBar.fillAmount +=  Time.deltaTime / selectionTime;
            selectable.SetSelectingProgress(progressBar.fillAmount);
            yield return null;
        }

        selectingObject = null;
        selectionEvents.ForEach(e => e.Invoke(selectable));
        selectable.onSelects.ForEach(e => e.Invoke());
        progressBar.enabled = false;
        selecting = false;
        progressBar.fillAmount = 0;
        yield return null;
    }

    private Vector2 WorldToCanvas(Vector3 pos)
    {
        var viewportPos = GameManager.Instance.cam.WorldToViewportPoint(pos);
        viewportPos.x -= 0.5f;
        viewportPos.y -= 0.5f;
        var canvasRect = canvas.GetComponent<RectTransform>();
        return new Vector2(viewportPos.x * canvasRect.sizeDelta.x, viewportPos.y * canvasRect.sizeDelta.y);
    }


    private IEnumerator CancelSelection()
    {
        selecting = false;
        canceling = true;
        while (progressBar.fillAmount > 0)
        {
            progressBar.fillAmount -= cancelSpeed * Time.deltaTime;
            selectingObject.SetSelectingProgress(progressBar.fillAmount);
            yield return null;
        }
        progressBar.enabled = false;
        selectingObject.SetSelectingProgress(0);
        selectingObject = null;
        selecting = false;
        canceling = false;
    }
}
