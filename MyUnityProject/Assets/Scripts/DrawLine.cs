using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawLine : MonoBehaviour
{

    private LineRenderer _lineRenderer;
    private float _dist;
    private float _counter;
    
    private Vector3 _pointA;
    private Vector3 _pointB;


    public float _lineDrawSpeed = 12f;
    public Transform _origin;
    public Transform _destination;
    
    // Start is called before the first frame update
    void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.SetPosition(0, _origin.position);
        _lineRenderer.SetWidth(.45f, .45f);

        _pointA = new Vector3(0, 0, 0);
        _pointB = new Vector3(0, 0, 0);

        _dist = Vector3.Distance(_origin.position, _destination.position);



    }

    // Update is called once per frame
    void Update()
    {
        if (_counter < _dist)
        {
            _counter += .1f / _lineDrawSpeed;

            float x = Mathf.Lerp(0, _dist, _counter);
            
            _pointA = _origin.position;
            _pointB = _destination.position;

            Vector3 pointLine = x*Vector3.Normalize(_pointB - _pointA) + _pointA;
        
            _lineRenderer.SetPosition(1, pointLine);  
        }
        
    }
}
