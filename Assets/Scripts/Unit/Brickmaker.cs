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
        private float _currentTick;
        private float _targetTimeToCreateSubresource;
        
        protected bool _isAtResource;
        protected bool _isDisposingResources;
        protected bool _creatingResource;

        private Tuple<ResourceType, float> _gatheredResourceAmount;

        public Brickmaker()
        {
            _targetTimeToGather = 10;
            _targetTimeToDisposeResources = 5;
            _currentTick = 0;
            _isAtResource = false;
            _isDisposingResources = false;
            _creatingResource = false;

            _targetTimeToCreateSubresource = 15;
            
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
                        _parentBuilding.DisposeResources(_gatheredResourceAmount);
                    }

                    _isDisposingResources = false;
                    _currentResource.ZeroYield();
                    
                    _currentTick = 0f;
                    _gatheredResourceAmount = Tuple.Create(ResourceType.Unknown, 0f);
                    
                    float currentAmount = _parentBuilding.GetReservedResourceAmount();
                    if (_parentBuilding.GetToProduceResourceAmount() <= currentAmount)
                    {
                        _parentBuilding.DecreaseReservedResourceAmount(_parentBuilding.GetToProduceResourceAmount());
                        _creatingResource = true;
                    }

                    if (!_creatingResource)
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
                    _creatingResource = false;
                    _resourceSignals.FireAddResourceToQueue(new AddResourceToQueueSignal()
                    {
                        resource = Tuple.Create(ResourceType.Brick, 2f)
                    });
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
        }

    }
}