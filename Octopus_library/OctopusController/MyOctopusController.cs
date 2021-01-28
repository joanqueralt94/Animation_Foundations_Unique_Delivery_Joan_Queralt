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

        #region Serialized Fields

        IKMode _myMode;

        #endregion

        #region Standard Attributes

        private float _twistMin;
        private float _twistMax;
        private float _swingMin;
        private float _swingMax;
        private Transform _currentRegion;
        Transform[] _randomTargets;
        private Transform _target;
        private Vector3[,] _jointsPos;
        private MyTentacleController[] _tentacles = new MyTentacleController[4];
        private float[,] distanceBetweenBones;
        private int idTentacle;
        private float _timer = 6;
        private Transform _container = null;
        private bool _resetRandoms = false;
        private Quaternion _finalRotation;
        private Quaternion _twist;


        private Vector3 _axisRight = Vector3.right;
        private Vector3 _axisUp = Vector3.up;
        private Vector3 _axisForward = Vector3.forward;

        //CCD
        private float _maxIterations = 1;
        private float _currentIterations = 0;
        private bool _done = false;
        private float _theta = 0;
        private Vector3 _vectorToEndEffector;
        private Vector3 _vectorToTarget;
        private Vector3 _rotationAxis;
        private Vector3[] _startTargetPositions;
        const float EPSILON = 0.1f;


        //GRADIENT
        float[,] _resultAngleContainer = null;
        Transform _targetTransforn;
        Vector3[,] _directorVectors = null;
        float _learningRate = 5f;
        public float _deltaGradient = 0.25f;
        #endregion

        #region Consultors and Modifiers

        public float TwistMin { set => _twistMin = value; }
        public float TwistMax { set => _twistMax = value; }
        public float SwingMin { set => _swingMin = value; }
        public float SwingMax { set => _swingMax = value; }

        #endregion

        #region public methods
        //DO NOT CHANGE THE PUBLIC METHODS!!
        public void TestLogging(string objectName)
        {
            //Debug.Log("hello, I am initializing my Octopus Controller in object "+objectName);
        }

        public void Init(IKMode name, Transform[] tentacleRoots, Transform[] randomTargets)
        {
            _myMode = name;
            _tentacles = new MyTentacleController[tentacleRoots.Length];
            for (int i = 0; i < tentacleRoots.Length; i++)
            {
                _tentacles[i] = new MyTentacleController();
                _tentacles[i].LoadTentacleJoints(tentacleRoots[i]);
            }
            _randomTargets = randomTargets;
            _jointsPos = new Vector3[_tentacles.Length, _tentacles[0].Bones.Length];
            InitJointsPositions();
            _startTargetPositions = new Vector3[_randomTargets.Length];
            InitStartTargetPositions();
            InitDistanceBetweenBones();
            _resultAngleContainer = new float[_tentacles.Length, _tentacles[0].Bones.Length];
            _directorVectors = new Vector3[_tentacles.Length, _tentacles[0].Bones.Length];
            for (int i = 0; i < _tentacles.Length; i++)
                for (int j = 0; j < _tentacles[i].Bones.Length - 1; j++)
                    _directorVectors[i, j] = DistanceBetweenTwoPoints(_tentacles[i].Bones[j].position, _tentacles[i].Bones[j + 1].position);
        }

        private void InitStartTargetPositions()
        {
            for (int i = 0; i < _randomTargets.Length; i++)
            {
                _startTargetPositions[i] = _randomTargets[i].position;
            }
        }

        private void InitJointsPositions()
        {
            for (int j = 0; j < _tentacles.Length; j++)
                for (int i = 0; i < _tentacles[0].Bones.Length; i++)
                    _jointsPos[j, i] = _tentacles[j].Bones[i].position;
        }

        public void NotifyTarget(Transform target, Transform region)
        {
            _currentRegion = region;
            _target = target;
        }

        public void NotifyShoot()
        {
            if (_timer < 5) return;
            
            _timer = 0;
            _resetRandoms = true;
            switch (_currentRegion.name)
            {
                case "region1":
                    Debug.Log("shoot a " + _currentRegion.name);
                    _container = _randomTargets[0];
                    _randomTargets[0].position = _target.position;
                    break;
                case "region2":
                   Debug.Log("shoot a " + _currentRegion.name);
                    _container = _randomTargets[1];
                    _randomTargets[1].position = _target.position;
                    break;
                case "region3":
                    Debug.Log("shoot a " + _currentRegion.name);
                    _container = _randomTargets[2];
                    _randomTargets[2].position = _target.position;
                    break;
                case "region4":
                    Debug.Log("shoot a " + _currentRegion.name);
                    _container = _randomTargets[3];
                    _randomTargets[3].position = _target.position;
                    break;
                default:
                    break;
            }
            //TODO. what happens here?

        }

        public void UpdateTentacles()
        {

            switch (_myMode)
            {
                case IKMode.CCD:
                    update_ccd();
                    break;
                case IKMode.FABRIK:
                    update_fabrik();
                    break;
                case IKMode.GRADIENT:
                    update_gradient();
                    break;
            }

            if (_timer < 5)
            {
                _timer += Time.deltaTime;

            }
            else if (_resetRandoms)
            {
                _resetRandoms = false;
                ReturnToRandom();
            }

        }

        #endregion

        #region private and internal methods
        //todo: add here anything that you need
        private void update_ccd()
        {
            for (int i = 0; i < _tentacles.Length; i++)
            {
                CCDAlgorithm(i);
            }
        }

        private void update_fabrik()
        {
            for (int i = 0; i < _tentacles.Length; i++)
            {
                FabrikAlgorithm(i);
                RepositionJoints(i);
            }
        }

        private void update_gradient()
        {
            for (int i = 0; i < _tentacles.Length; i++)
            {
                Gradient_AproachTarget(i);
            }
            SetAngle();
        }
        #endregion

        #region FABRIK FUNCTIONS
        private void FabrikAlgorithm(int idTentacleRegion)
        {
            for (int j = _tentacles[idTentacleRegion].Bones.Length - 1; j >= 0; j--)
            {
                if (j == _tentacles[idTentacleRegion].Bones.Length - 1)
                {
                    //Aqui es té que cambiar el Fabrik
                    _jointsPos[idTentacleRegion, j] = _randomTargets[idTentacleRegion].position;
                    continue;
                }
                Vector3 nextPos = _jointsPos[idTentacleRegion, j + 1];
                Vector3 ownPos = _jointsPos[idTentacleRegion, j];
                float distance = distanceBetweenBones[idTentacleRegion, j];
                _jointsPos[idTentacleRegion, j] = nextPos + ((ownPos - nextPos).normalized * distance);
            }

            for (int j = 0; j < _tentacles[idTentacleRegion].Bones.Length; j++)
            {
                if (j == 0)
                {
                    _jointsPos[idTentacleRegion, j] = _tentacles[idTentacleRegion].Bones[0].position;
                    continue;
                }
                Vector3 prevPos = _jointsPos[idTentacleRegion, j - 1];
                Vector3 myownPos = _jointsPos[idTentacleRegion, j];
                float distance = distanceBetweenBones[idTentacleRegion, j - 1];

                _jointsPos[idTentacleRegion, j] = prevPos + ((myownPos - prevPos).normalized * distance);
            }
        }

        private void RepositionJoints(int idTentacleRegion)
        {
            int it = 0;
            while (it < 20)
            {
                for (int j = 0; j < _tentacles[idTentacleRegion].Bones.Length - 1; j++)
                {
                    Vector3 currentDir = _tentacles[idTentacleRegion].Bones[j].TransformDirection(Vector3.up).normalized;
                    Vector3 destinyDir = (_jointsPos[idTentacleRegion, j + 1] - _jointsPos[idTentacleRegion, j]).normalized;

                    Vector3 axis = Vector3.Cross(currentDir, destinyDir);
                    float angle = Mathf.Acos((Vector3.Dot(currentDir, destinyDir) / (destinyDir.magnitude * currentDir.magnitude)));

                    angle *= Mathf.Rad2Deg;

                    _finalRotation = Quaternion.AngleAxis(angle * Time.deltaTime, axis);
                    _twist = new Quaternion(0, _finalRotation.y, 0, _finalRotation.w).normalized;
                    _finalRotation = Quaternion.Inverse(_twist) * _finalRotation;

                    _tentacles[idTentacleRegion].Bones[j].transform.rotation = _finalRotation * _tentacles[idTentacleRegion].Bones[j].transform.rotation;
                }
                it++;
            }
        }
        #endregion

        #region CCD FUNCTIONS
        private void CCDAlgorithm(int idTentacle)
        {
            if (!_done && _currentIterations < _maxIterations)
            {
                for (int i = _tentacles[idTentacle].Bones.Length - 2; i >= 0; i--)
                {
                    _vectorToEndEffector = _tentacles[idTentacle].Bones[i].position - _tentacles[idTentacle].Bones[_tentacles[idTentacle].Bones.Length - 1].position;
                    _vectorToTarget = _tentacles[idTentacle].Bones[i].position - _randomTargets[idTentacle].position;
                    _rotationAxis = Vector3.Cross(_vectorToEndEffector, _vectorToTarget).normalized;
                    _theta = Vector3.Angle(_vectorToEndEffector, _vectorToTarget);

                    if (_theta / 180 >= 1)
                    {
                        _theta -= 360;
                    }

                    _tentacles[idTentacle].Bones[i].rotation = Quaternion.AngleAxis(_theta, _rotationAxis) * _tentacles[idTentacle].Bones[i].rotation;
                }

                _currentIterations++;
            }

            float distance = (_tentacles[idTentacle].Bones[_tentacles[idTentacle].Bones.Length - 1].position - _randomTargets[idTentacle].position).magnitude;

            _done = distance < EPSILON;

            foreach (Vector3 _startPosition in _startTargetPositions)
            {
                if (_randomTargets[idTentacle].position != _startPosition)
                {
                    _currentIterations = 0;
                    InitStartTargetPositions();
                }
            }
        }

        private void InitDistanceBetweenBones()
        {
            distanceBetweenBones = new float[_tentacles.Length, _tentacles[0].Bones.Length - 1];
            for (int i = 0; i < _tentacles.Length; i++)
            {
                for (int j = 0; j < _tentacles[i].Bones.Length - 1; j++)
                {
                    distanceBetweenBones[i, j] = Vector3.Distance(_tentacles[i].Bones[j].position, _tentacles[i].Bones[j + 1].position);
                }
            }
        }

        private void ReturnToRandom()
        {
            switch (_currentRegion.name)
            {
                case "region1":
                    _randomTargets[0] = _container;
                    break;
                case "region2":
                    _randomTargets[1] = _container;
                    break;
                case "region3":
                    _randomTargets[2] = _container;
                    break;
                case "region4":
                    _randomTargets[3] = _container;
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region GRADIENT

        void Gradient_AproachTarget(int idTentacle)
        {
            for (int j = 0; j < _tentacles[idTentacle].Bones.Length; j++)
            {
                _resultAngleContainer[idTentacle, j] = _resultAngleContainer[idTentacle, j] - _learningRate * CalculateGradient(_randomTargets[idTentacle].position, _resultAngleContainer, idTentacle, j, _deltaGradient);
            }
        }

        void SetAngle()
        {
            for (int i = 0; i < _tentacles.Length; i++)
            {
                for (int j = 0; j < _tentacles[i].Bones.Length; j++)
                {
                    // _tentacles[i].Bones[j].localEulerAngles = new Vector3(_resultAngleContainer[i, j], _resultAngleContainer[i, j], _resultAngleContainer[i, j]);
                    switch (i % 2)
                    {
                        case 0:
                            _tentacles[i].Bones[j].localRotation = Quaternion.AngleAxis(_resultAngleContainer[i, j], _axisRight);
                            break;
                        case 1:
                            _tentacles[i].Bones[j].localRotation = Quaternion.AngleAxis(_resultAngleContainer[i, j], _axisForward);
                            break;
                    }
                }
            }
        }
        float CalculateGradient(Vector3 target, float[,] resultAngleContainer, int tentacle, int nodeTentacle, float delta)
        {
            float[,] incrementedSolution = new float[_tentacles.Length, _tentacles[0].Bones.Length];

            for (int k = 0; k < _tentacles[0].Bones.Length; k++)
            {
                incrementedSolution[tentacle, k] = resultAngleContainer[tentacle, k];
            }

            incrementedSolution[tentacle, nodeTentacle] += delta;

            return (DistanceFromTarget(target, incrementedSolution, tentacle) - DistanceFromTarget(target, resultAngleContainer, tentacle)) / delta;
        }

        float DistanceFromTarget(Vector3 target, float[,] Solution, int idTentacleRegion)
        {
            Vector3 point = ForwardKinematics(Solution, idTentacleRegion);
            return Vector3.Distance(point, target);
        }

        Vector3 ForwardKinematics(float[,] angleContainer, int idTentacleRegion)
        {
            Vector3 newEndDeffectorPosition = Vector3.zero;
            Quaternion quaternions = Quaternion.identity;
            Vector3 rotatedVector;
            newEndDeffectorPosition = _tentacles[idTentacleRegion].Bones[0].position;

            for (int i = 0; i < _tentacles[0].Bones.Length; i++)
            {
                float angleTemp = angleContainer[idTentacleRegion, i];
                switch (i % 2)
                {
                    case 0:
                        quaternions *= Quaternion.AngleAxis(angleTemp, _axisRight);
                        break;
                    case 1:
                        quaternions *= Quaternion.AngleAxis(angleTemp, _axisForward);
                        break;
                }
                rotatedVector = quaternions * _directorVectors[idTentacleRegion, i];
                Vector3 temp = newEndDeffectorPosition;
                newEndDeffectorPosition += rotatedVector;
                Debug.DrawLine(temp, newEndDeffectorPosition);

            }

            return newEndDeffectorPosition;
        }

        Vector3 DistanceBetweenTwoPoints(Vector3 initialPoint, Vector3 finalPoint)
        {
            return (finalPoint - initialPoint);
        }

        #endregion





    }
}
