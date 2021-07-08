namespace Controller
{
    using System;
    using UnityEngine;

    public class CustomCameraController : MonoBehaviour
    {
        public GameObject Player;
        private Vector3 Distance = Vector3.back * 10;

        public void SearchPlayerForCum()
        {
            Player = GameObject.Find("Player");
        }
        
        void Update()
        {
            transform.position = Player.transform.position + Distance;
        }
    }
}