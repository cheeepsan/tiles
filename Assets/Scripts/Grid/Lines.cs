using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Lines : MonoBehaviour
{

    private LineRenderer _lineRenderer;
    private Grid _grid;

    private Vector3[] points = new[]
    {
        new Vector3(-0.5f, 1f, 0.5f),
        new Vector3(0.5f, 1f, 0.5f),
        new Vector3(0.5f, 1f, -0.5f),
        new Vector3(-0.5f, 1f, -0.5f)
    };
    
    // Start is called before the first frame update
    void Start()
    {
        //_grid = GetComponent<Grid>();
        _lineRenderer = GetComponent<LineRenderer>();
        
        var offset = gameObject.transform.position;

        var updatedPoints = points.Select(p => new Vector3(p.x + offset.x, p.y, p.z + offset.z)).ToArray();
        _lineRenderer.SetPositions(updatedPoints);
        _lineRenderer.loop = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
