using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Ball : MonoBehaviour
{

    [SerializeField]
    Transform _targetTransform;

    [SerializeField]
    private List<Animation> _robots;

    [SerializeField]
    private Animator _player;
    
    private float _velocity;
    public float _shootStrengh;
    public float _maxStrengh;

    private float _mass = 1f;
    private bool _isShooting;
    private bool _scored = true;

    public Slider _strenghSlider;
    public Slider _effectSlider;
    
    Vector3 _gravity = new Vector3(0, -4.41f, 0);

    private Vector3 _vectorDirection;
    private Vector3 _initialPosition;

    private Rigidbody _rb;

    public Transform _arrow;
    
    

   
    

    // Start is called before the first frame update
    void Start()
    {
        _isShooting = false;

        _shootStrengh = 0f;
        _maxStrengh = 100f;
            
        _rb = GetComponent<Rigidbody>();
        _initialPosition = transform.position;

        

    }

    // Update is called once per frame
    void Update()
    {
        
        CanvasControl();
        _arrow.LookAt(_targetTransform);
        _strenghSlider.value = _shootStrengh;
        if((transform.position - (transform.position + _vectorDirection * _velocity * Time.deltaTime)).z > (transform.position - _targetTransform.position).z)
        {
            _isShooting = false;
            transform.position = _initialPosition;
        }
        
        if (_isShooting)
        {
            
            _rb.isKinematic = true;
            _vectorDirection = (_targetTransform.position - transform.position).normalized;
            _velocity = _shootStrengh / _mass;
            transform.position += (_vectorDirection * _velocity) * Time.deltaTime + _gravity * Time.deltaTime;
            StartCoroutine(ReturnGame());
        }
        else
        {
            _rb.isKinematic = false;
        }
    }


    public void CanvasControl()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            _shootStrengh++;
            if (_shootStrengh >= _maxStrengh)
            {
                _shootStrengh = 100f;
            }

        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            StartCoroutine(PlayerMovement());
            //StartCoroutine(ReturnGame());
        }
        else if (Input.GetKey(KeyCode.Z))
        {
            _effectSlider.value -= 0.05f;
        }
        else if (Input.GetKey(KeyCode.X))
        {
            _effectSlider.value += 0.05f;
        }

    }

    private IEnumerator ReturnGame()
    {
        for (int i = 0; i < _robots.Count; i++)
        {
            yield return new WaitForSeconds(0.5f);
            _robots[i].Play();
        }
        yield return new WaitForSeconds(1f);
        _arrow.gameObject.SetActive(true);
        _isShooting = false;
        _shootStrengh = 0f;
        transform.position = _initialPosition;
        
    }

    private IEnumerator PlayerMovement()
    {
        _player.SetTrigger("hasToKickBall");
        yield return new WaitForSeconds(5.8f);
        
        _isShooting = true;
        _arrow.gameObject.SetActive(false);
        

    }
}

   

