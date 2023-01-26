using Game;
using UnityEngine;
using Zenject;

namespace UnitNS
{
    public class Scribe : Unit
    {
        [Inject] private BuildingManager _buildingManager;
        public override void Work()
        {
            Debug.Log(_buildingManager.GetStateInfo());
        }
        
        protected virtual void OnTriggerEnter(Collider other)
        {

        }
    }
}