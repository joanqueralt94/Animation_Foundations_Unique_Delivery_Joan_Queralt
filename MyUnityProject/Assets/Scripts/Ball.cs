using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ball : MonoBehaviour
{
    private GameObject _ball;
    //private float _strenghSliderVelocity;
    public float _shootStrengh;
    public float _maxStrengh;
    
    

    public Slider _strenghSlider;

    public Transform _origin;
    public Transform _target;

    public DrawLine _lineRenderer;
    
    // Start is called before the first frame update
    void Start()
    {

        _ball = GameObject.Find("Ball");
        //_strenghSliderVelocity = 1f;
        _shootStrengh = 0f;
        _maxStrengh = 100f;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            forceIncrement();
            //Debug.Log("Spacebar being pressed and force value is: " + _shootStrengh);

            if (_shootStrengh >= _maxStrengh)
            {
                _shootStrengh = 100f;
            }
            
        }
        else
        {
            _shootStrengh = 0f;   
        }

        _strenghSlider.value = _shootStrengh;

        //_ball.transform.Translate(_lineRenderer.transform.forward * _shootStrengh);

    }

    public void forceIncrement()
    {
        _shootStrengh++;
    }
}
