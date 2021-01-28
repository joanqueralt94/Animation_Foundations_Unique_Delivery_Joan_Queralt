using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingTarget: MonoBehaviour
{
    [SerializeField]
    IK_tentacles _myOctopus;


    //random will create an object moving randomly within a box.
    enum MovingMode {RANDOM, USERTARGET };

    
    [SerializeField]
    MovingMode _mode;

    //movement speed in units per second
    [Range(0.0f,1.0f)]
    [SerializeField]
    private float _movementSpeed = 5f;


    [SerializeField]
    GameObject _region;
    float _xMin, _xMax, _yMin, _yMax;

    public Ball _ball;

    Vector3 _dir;


    //variable added just to control whether we are 
   

    // Start is called before the first frame update
    void Start()
    {
        if (_mode == MovingMode.RANDOM) {
            if (_region == null)
            {
                Debug.LogError("moving targets in random mode need to have a region assigned to");
            }
            else {
                _xMin = _region.transform.position.x - _region.transform.localScale.x / 2;
                _xMax = _region.transform.position.x + _region.transform.localScale.x / 2;
                _yMin = _region.transform.position.y - _region.transform.localScale.y / 2;
                _yMax = _region.transform.position.y + _region.transform.localScale.y / 2;
                float a = Random.Range(0.0f,1.0f);
                _dir = new Vector3(a, 1 - a, 0);
            }



        }
    }


    void Update()
    {
        
        transform.rotation = Quaternion.identity;
        
        if (_mode == MovingMode.USERTARGET)
        {
            //get the Input from Horizontal axis
            float horizontalInput = Input.GetAxis("Horizontal");
            //get the Input from Vertical axis
            float verticalInput = Input.GetAxis("Vertical");

            //update the position
            transform.position = transform.position + new Vector3(-horizontalInput * _movementSpeed * Time.deltaTime, verticalInput * _movementSpeed * Time.deltaTime, 0);
            
            

            if (Input.GetButtonDown("Fire1"))
            {
                //_mode = MovingMode.USERTARGET;
                //Debug.Log("I am object " + name + "  and i am notifying a shooting");
                _myOctopus.NotifyShoot();
                //StartCoroutine(TentacleStay());
                
            }
            
            
        }
        else if (_mode == MovingMode.RANDOM) {

            transform.position += _dir * _movementSpeed;

          //  StartCoroutine(ChangeDir());

        }
        
        else if (_ball._parar)
        {
            this._mode = MovingMode.USERTARGET;
        }
        else if (!_ball._parar)
        {
            this._mode = MovingMode.RANDOM;
        }
        
    }

    IEnumerator ChangeDir()
    {
            //we add a minimum distance because sometimes when the direction change is too near from the border, changing dear makes the ball get lost
           float wallDistance = 0.05f;


        if ((transform.position.x + wallDistance < _xMax) && (transform.position.x - wallDistance > _xMin) &&
             (transform.position.y + wallDistance < _yMax) && (transform.position.y - wallDistance > _yMin))
        {
            _dir = getNewDirection(_dir);
            yield return new WaitForSeconds(2);
        }
        else {
            //Debug.Log("I cannot change direction: " + transform.position + " " + _xMax + " " + _xMin + " " + _yMax + " " + _yMin);
            yield return new WaitForSeconds(0.1f);
        }
       
       

    }

    private void OnCollisionEnter(Collision collision)
    {
        if(_mode == MovingMode.USERTARGET)
            _myOctopus.NotifyTarget(transform, collision.collider.transform);

        //Debug.Log("I am object " + name + "  and i enter collision with " + collision.collider.name);
    }


    private void OnCollisionStay(Collision collision)
    {
        if (_mode == MovingMode.USERTARGET)
            _myOctopus.NotifyTarget(transform, collision.collider.transform);
        else if(_mode == MovingMode.RANDOM)
        {
            


        }
        //Debug.Log("I am object " + name + "  and i stay colliding with " + collision.collider.name);
    }
    private void OnCollisionExit(Collision collision)
    {
        if(_mode == MovingMode.RANDOM) { 

            if (collision.transform.Equals(_region.transform))
            {

                if (transform.position.x > _xMax)
                    _dir.x = -_dir.x;
                else if (transform.position.x < _xMin)
                    _dir.x = -_dir.x;


                if (transform.position.y > _yMax)
                    _dir.y = -_dir.y;
                else if (transform.position.y < _yMin)
                    _dir.y = -_dir.y;


              
            }
            else
            {
                // Debug.Log("STRANGE CASE!! check your layers for collisions");

            }
        }

    }


    //returns a new direction with components of same sign than current direction;
    private static Vector3 getNewDirection(Vector3 thedir) {
        float a = Random.Range(0.0f, 1.0f);
        Vector3 newdir = new Vector3(Mathf.Sign(thedir.x) *(  a), Mathf.Sign(thedir.y) * (1 - a), 0);
        //Debug.Log("my new dir is: " + newdir + " while my old one was: " + thedir);
        return newdir;
    }

    private IEnumerator TentacleStay()
    {
        yield return new WaitForSeconds(8f);
    }

}

