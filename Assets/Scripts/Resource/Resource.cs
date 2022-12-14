using System;
using ResourceNS.Enum;
using Signals.ResourceNS;
using UnityEngine;
using Zenject;


namespace ResourceNS
{
    public class Resource : MonoBehaviour
    {
        [Inject] private ResourceSignals _resourceSignals;

        private string _resourceUuid;
        private bool _isAvailable;
        
        protected ResourceType resourceType;
        
        public virtual void Start()
        {
            _resourceUuid = Guid.NewGuid().ToString();
            _isAvailable = true;
            _resourceSignals.FireResourceAvailableSignal(new ResourceAvailableSignal()
            {
                resourceId = _resourceUuid,
                resource = this // todo not sending monobehaviour?
            });
        }

        public void SetAvailable(bool isAvailable)
        {
            _isAvailable = isAvailable;
        }

        public bool IsAvailable()
        {
            return _isAvailable;
        }
    }
}