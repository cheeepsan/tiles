using System;
using System.Collections;
using BuildingNS;
using Game;
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

        protected bool _isAtResource;
        protected bool _isDisposingResources;
        protected bool _creatingResource;

        protected float _currentTick;
        public void Start()
        {
            _camera = Camera.main;
            _myNavMeshAgent = GetComponent<NavMeshAgent>();
            _atWork = false;
            SubscribeOnPauseToggle();
        }

        private void SubscribeOnPauseToggle()
        {
            TimeManager.OnPauseToggle += delegate(object sender, TimeManager.OnPauseToggleEventArgs args)
            {
                bool paused = args.paused;
                if (paused)
                {
                    _myNavMeshAgent.isStopped = true;
                }
                else
                {
                    _myNavMeshAgent.isStopped = false;
                }
            };
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

        public virtual void NextStep()
        {
            
        }
        
    }
}