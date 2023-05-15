using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowLine : MonoBehaviour
{
    public GameObject Top;
    public GameObject Mid;
    public GameObject Bottom;

    public LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = this.GetComponent<LineRenderer>();

        lineRenderer.positionCount = 3;
        
    }

    // Update is called once per frame
    void Update()
    {
        lineRenderer.SetPositions(new Vector3[] { Top.transform.position, Mid.transform.position, Bottom.transform.position });
    }
}
