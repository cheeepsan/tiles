using UnityEngine;

namespace UnitNS
{
    public class Farmer : Unit
    {
        public override void Work()
        {
            if (_currentResource != null && !_atWork)
            {
                _myNavMeshAgent.SetDestination(_currentResource.gameObject.transform.position);
                _atWork = true;
            }
        }
        
        protected virtual void OnTriggerEnter(Collider other)
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