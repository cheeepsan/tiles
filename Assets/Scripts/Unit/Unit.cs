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
        [Inject] protected ResourceSignals _resourceSignals;

        protected NavMeshAgent _myNavMeshAgent;
        protected Resource _currentResource;
        protected PlaceableBuilding _parentBuilding;
        protected Camera _camera;
        protected bool _atWork;

        public void Start()
        {
            _camera = Camera.main;
            _myNavMeshAgent = GetComponent<NavMeshAgent>();
            _atWork = false;
        }

        public virtual void SetParentBuilding(PlaceableBuilding parent)
        {
            _parentBuilding = parent;
        }

        public void SetCurrentResource(Resource resource)
        {
            _currentResource = resource;
            _atWork = false;
        }

        public virtual void Work()
        {
            //if (_currentResource != null && !_atWork)
            //{
            //    _myNavMeshAgent.SetDestination(_currentResource.gameObject.transform.position);
            //    _atWork = true;
            //}
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            //if (_currentResource != null)
            //{
            //    if (other.gameObject == _currentResource.gameObject)
            //    {
            //        Debug.Log("Trigger, resource");
            //        _myNavMeshAgent.SetDestination(_parentBuilding.gameObject.transform.position);
            //    }
            //    else if (other.gameObject == _parentBuilding.gameObject)
            //    {
            //        Debug.Log("Trigger, BUILDING");
            //        _myNavMeshAgent.SetDestination(_currentResource.gameObject.transform.position);
            //    }
            //    else
            //    {
            //        //Debug.Log("Trigger, OTHER");
            //        //_myNavMeshAgent.SetDestination(_currentResource.gameObject.transform.position);
            //    }
            //}
        }
    }
}