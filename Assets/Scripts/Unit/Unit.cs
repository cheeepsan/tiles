using System;
using UnityEngine;
using UnityEngine.AI;

namespace UnitNS
{
    public class Unit : MonoBehaviour
    {
        
        NavMeshAgent _myNavMeshAgent;
        public void Start()
        {
            _myNavMeshAgent = GetComponent<NavMeshAgent>();
            GoTo();
        }

        public void GoTo()
        {
            _myNavMeshAgent.SetDestination(new Vector3(9.5f, 0f, 17.5f));
        }
        
    }
}