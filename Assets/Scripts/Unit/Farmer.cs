using System;
using BuildingNS;
using Game;
using ResourceNS.Enum;
using UnityEngine;

namespace UnitNS
{
    public class Farmer : Unit
    {
        // TODO move to conf. How?
        private float _targetTimeToGather;
        private float _targetTimeToDisposeResources;
        private float _currentTick;
        
        private bool _isGathering;
        private bool _isDisposingResources;

        private Tuple<ResourceType, float> _gatheredResourceAmount;
        public Farmer()
        {
            _targetTimeToGather = 10;
            _targetTimeToDisposeResources = 5;
            _currentTick = 0;
            _isGathering = false;
            _isDisposingResources = false;
            TimeManager.OnTick += delegate(object sender, TimeManager.OnTickEventArgs args)
            {
                Gathering();
                DisposingResources();
            };
        }
        
        public override void Work()
        {
            if (_currentResource != null && !_atWork)
            {
                Debug.Log("Resource pos: " + _currentResource.gameObject.transform.position);
                _myNavMeshAgent.SetDestination(_currentResource.gameObject.transform.position);

                _atWork = true;
            }
        }

        protected override void OnTriggerEnter(Collider other)
        {
            if (_currentResource != null)
            {
                if (other.gameObject == _currentResource.gameObject)
                {
                    //Debug.Log("Trigger, resource");
                    _isGathering = true;
                }
                else if (other.gameObject == _parentBuilding.gameObject)
                {
                    //Debug.Log("Trigger, BUILDING");
                    _isDisposingResources = true;
                }
                else
                {
                    //Debug.Log("Trigger, OTHER");
                    //_myNavMeshAgent.SetDestination(_currentResource.gameObject.transform.position);
                }
            }
        }

        private void Gathering()
        {
            if (_isGathering)
            {
                if (_currentTick < _targetTimeToGather)
                {
                    _currentTick++;
                }
                else
                {
                    _currentTick = 0f;
                    _isGathering = false;
                    _gatheredResourceAmount = Tuple.Create(_currentResource.GetResourceType() ,_currentResource.GetYield());
                    _myNavMeshAgent.SetDestination(_parentBuilding.gameObject.transform.position);
                }
            }
        }
        
        private void DisposingResources()
        {
            if (_isDisposingResources)
            {
                if (_currentTick < _targetTimeToDisposeResources)
                {
                    _currentTick++;
                }
                else
                {
                    _parentBuilding.DisposeResources(_gatheredResourceAmount);
                    _currentTick = 0f;
                    _isDisposingResources = false;
                    _gatheredResourceAmount = Tuple.Create(ResourceType.Unknown, 0f);
                    _myNavMeshAgent.SetDestination(_currentResource.gameObject.transform.position);
                }
            }
        }
        
        public void OnGUI()
        {

            Vector2 targetPos;
            targetPos = _camera.WorldToScreenPoint(transform.position);

            float yOffset = 60; // not fixed? 
            float xOffset = 30;

            if (_isGathering || _isDisposingResources)
            {
                GUI.Box(new Rect(targetPos.x - xOffset, Screen.height - targetPos.y - yOffset, 60, 20), _currentTick.ToString()); 
            }
        }
    }
}