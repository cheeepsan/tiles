using System;
using BuildingNS;
using Game;
using ResourceNS.Enum;
using Signals.ResourceNS;
using UnityEngine;

namespace UnitNS
{
    public class Brickmaker : Unit
    {
        // TODO move to conf. How?
        private float _targetTimeToGather;
        private float _targetTimeToDisposeResources;
        private float _targetTimeToCreateSubresource;
        
    
        private Tuple<ResourceType, float> _gatheredResourceAmount;

        public Brickmaker()
        {
            _targetTimeToGather = 10;
            _targetTimeToDisposeResources = 5;
            _currentTick = 0;
            _isAtResource = false;
            _isDisposingResources = false;
            _creatingResource = false;

            _targetTimeToCreateSubresource = 20;
            
            TimeManager.OnTick += delegate(object sender, TimeManager.OnTickEventArgs args)
            {
                AtResource();
                DisposingResources();
                CreateTargetResource();
            };
        }

        public override void Work()
        {
            if (_currentResource != null && _currentResource.gameObject != null && !_atWork)
            {
                Debug.Log("START work: Resource pos: " + _currentResource.gameObject.transform.position);
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
                    _isAtResource = true;
                }
                else if (other.gameObject == _parentBuilding.gameObject)
                {
                    _isDisposingResources = true;
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
                        _parentBuilding.DisposeResources(_gatheredResourceAmount, this);
                    }
                    
                    _currentResource.ZeroYield();
                    
                    _currentTick = 0f;
                    _gatheredResourceAmount = Tuple.Create(ResourceType.Unknown, 0f);
                    
                    float currentAmount = _parentBuilding.GetReservedResourceAmount();
                    if (_parentBuilding.GetToProduceResourceAmount() <= currentAmount)
                    {
                        _parentBuilding.DecreaseReservedResourceAmount(_parentBuilding.GetToProduceResourceAmount());
                        _creatingResource = true;
                        
                    }
                    
                    _isDisposingResources = false;
                }
            }
        }

        private void CreateTargetResource()
        {
            if (_creatingResource)
            {
                if (_currentTick < _targetTimeToCreateSubresource)
                {
                    _currentTick++;
                }
                else
                {

                    _currentTick = 0f;
                    Debug.Log("Added brick");

                    _parentBuilding.DisposeResources(Tuple.Create(ResourceType.Brick, 2f), this);

                    _creatingResource = false;

                }
            }
        }

        public override void NextStep()
        {
            if (_creatingResource == false && _isDisposingResources == false && _atWork == false)
            {
                if (_currentResource != null && _currentResource.gameObject != null)
                {
                    _myNavMeshAgent.SetDestination(_currentResource.gameObject.transform.position);
                }
                else
                {
                    _myNavMeshAgent.SetDestination(_parentBuilding.gameObject.transform.position);
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
            } else if (_creatingResource)
            {
                GUI.Box(new Rect(targetPos.x - xOffset, Screen.height - targetPos.y - yOffset, 60, 20),"Making brick");
            }
            else
            {
                
            }
        }
    }
}