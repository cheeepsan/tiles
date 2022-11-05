using System;
using UnityEngine;

namespace BuildingNS
{
    public class PlaceableBuilding : MonoBehaviour // to interface
    {
        public bool canBuild;
        
        private void Start()
        {
            canBuild = true;
            Debug.Log("Building created");
        }


        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer != 6)
            {
                canBuild = false;
                Debug.Log("An object entered. " + other.name);
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.layer != 6)
            {
                if (canBuild) canBuild = false;
                
                Debug.Log("An object is still inside of the trigger " + other.name);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.layer != 6)
            {
                canBuild = true;
                Debug.Log("An object left.");
            }
        }
    }
}