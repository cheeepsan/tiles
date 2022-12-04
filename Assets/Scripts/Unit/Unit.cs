using System;
using System.Collections;
using BuildingNS;
using ResourceNS;
using Signals.ResourceNS;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

namespace UnitNS
{
    public class Unit : MonoBehaviour
    {
        [Inject] private ResourceSignals _resourceSignals;

        private NavMeshAgent _myNavMeshAgent;
        private Resource _currentResource;
        private bool _resourceAtWork;
        private PlaceableBuilding _parentBuilding;

        public void Start()
        {
            _myNavMeshAgent = GetComponent<NavMeshAgent>();
            _resourceAtWork = false;
        }

        public void SetParentBuilding(PlaceableBuilding parent)
        {
            _parentBuilding = parent;
        }

        public void SetCurrentResource(Resource resource)
        {
            _currentResource = resource;
            _resourceAtWork = false;
        }

        public void WorkOnResource()
        {
            if (_currentResource != null && !_resourceAtWork)
            {
                _myNavMeshAgent.SetDestination(_currentResource.gameObject.transform.position);
                _resourceAtWork = true;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_currentResource != null)
            {
                if (other.gameObject == _currentResource.gameObject)
                {
                    Debug.Log("Trigger, resource");
                    _myNavMeshAgent.SetDestination(_parentBuilding.gameObject.transform.position);
                }
                else if (other.gameObject == _parentBuilding.gameObject)
                {
                    Debug.Log("Trigger, BUILDING");
                    _myNavMeshAgent.SetDestination(_currentResource.gameObject.transform.position);
                }
                else
                {
                    //Debug.Log("Trigger, OTHER");
                    //_myNavMeshAgent.SetDestination(_currentResource.gameObject.transform.position);
                }
            }
        }
    }
}