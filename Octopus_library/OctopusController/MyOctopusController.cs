using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace OctopusController
{

   

    public enum IKMode {GRADIENT, CCD, FABRIK };


    public class MyOctopusController 
    {
        
        MyTentacleController[] _tentacles =new  MyTentacleController[4];

        [SerializeField]
        IKMode _myMode;

        Transform _currentRegion;
        Transform _target;

        Transform[] _randomTargets;// = new Transform[4];


        float _twistMin, _twistMax;
        float _swingMin, _swingMax;

        #region public methods
        //DO NOT CHANGE THE PUBLIC METHODS!!

        public float TwistMin { set => _twistMin = value; }
        public float TwistMax { set => _twistMax = value; }
        public float SwingMin {  set => _swingMin = value; }
        public float SwingMax { set => _swingMax = value; }
        

        public void TestLogging(string objectName)
        {

           
            Debug.Log("hello, I am initializing my Octopus Controller in object "+objectName);

            
        }


        //        public void Init(IKMode name, Transform[] tentacleRoots, GameObject[] regions) {
        public void Init(IKMode name, Transform[] tentacleRoots, Transform[] randomTargets)
        {

            _myMode = name;

            _tentacles = new MyTentacleController[tentacleRoots.Length];

            // int i = 0;
            // foreach (Transform t in tentacleRoots)
            for(int i = 0;  i  < tentacleRoots.Length; i++)
            {

                _tentacles[i] = new MyTentacleController();
                _tentacles[i].LoadTentacleJoints(tentacleRoots[i]);
                i++;
            }

            _randomTargets = randomTargets;
            //TODO: use the regions however you need to make sure each tentacle stays in its region

        }

              
        public void NotifyTarget(Transform target, Transform region)
        {
            _currentRegion = region;
            _target = target;

        }

        public void NotifyShoot() {
            //TODO. what happens here?

        }


        public void UpdateTentacles()
        {
            switch (_myMode) {
                case IKMode.CCD:
                    //TODO one of the exercises
                    update_ccd();
                    break;
                case IKMode.FABRIK:
                    update_fabrik();
                    break;
                case IKMode.GRADIENT:
                    update_gradient();
                    break;
            }

        }




        #endregion


        #region private and internal methods
        //todo: add here anything that you need

        void update_ccd() {


        }

        void update_fabrik()
        {


        }

        void update_gradient()
        {


        }


        

        #endregion






    }
}
