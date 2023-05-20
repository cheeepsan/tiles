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

        private bool _isAtResource;
        private bool _isDisposingResources;

        private Tuple<ResourceType, float> _gatheredResourceAmount;

        public Farmer()
        {
            _targetTimeToGather = 10;
            _targetTimeToDisposeResources = 5;
            _currentTick = 0;
            _isAtResource = false;
            _isDisposingResources = false;
            TimeManager.OnTick += delegate(object sender, TimeManager.OnTickEventArgs args)
            {
                AtResource();
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
                    _isAtResource = true;
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

       
        private void AtResource()
        {
            if (_isAtResource)
            {
                if (_currentTick < _currentResource.TimeToFinish())
                {
                    // TODO Increment tick before or after
                    _currentResource.ResourceHandling(this, _currentTick++);
                    _currentTick++;
                }
                else
                {
                    _currentTick = 0f;
                    _isAtResource = false;
                    _gatheredResourceAmount =
                        Tuple.Create(_currentResource.GetResourceType(), _currentResource.GetYield());
                    _myNavMeshAgent.SetDestination(_parentBuilding.gameObject.transform.position);
                }
            }
        }
        
        private void AtFruitResource()
        {
        }

        private void AtFarmResource()
        {
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
                    // TODO remove not null
                    if (_gatheredResourceAmount != null)
                    {
                        _parentBuilding.DisposeResources(_gatheredResourceAmount);
                    }

                    _currentTick = 0f;
                    _isDisposingResources = false;
                    _currentResource.ZeroYield();
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

            if (_isAtResource || _isDisposingResources)
            {
                GUI.Box(new Rect(targetPos.x - xOffset, Screen.height - targetPos.y - yOffset, 60, 20),
                    _currentTick.ToString());
            }
        }
    }
}