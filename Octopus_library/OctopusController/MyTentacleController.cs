using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;




namespace OctopusController
{

    
    internal class MyTentacleController

    //MAINTAIN THIS CLASS AS INTERNAL
    {


        Transform[] _bones;
        Transform _endEffectorSphere;



        public Transform[] Bones { get => _bones; }
        

        //Exercise 1.
        public Transform[] LoadTentacleJoints(Transform root)
        {
            //TODO: add here whatever is needed to find the bones forming the tentacle
            //you may want to use a list, and then convert it to an array
            List<Transform> boneList = new List<Transform>();

            for (int i = 0; i < 51; i++)
            {
                boneList.Add(root);
                root = root.GetChild(0);
            }

            //TODO: in _endEffectorphere you  keep a reference to the sphere with a collider attached to the endEffector


            _endEffectorSphere = root;
            _bones = new Transform[boneList.Count];
            _bones = boneList.ToArray();
            return _bones;
        }

        void MoveRandomly(IKMode mode) {


        }








}
}
