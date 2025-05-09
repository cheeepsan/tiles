using System;
using Common.Logic;
using ResourceNS.Enum;
using Signals.ResourceNS;
using Signals.UI;
using Ui.Common;
using UnitNS;
using UnityEngine;
using Zenject;


namespace ResourceNS
{
    public class Resource : MonoBehaviour
    {
        [Inject] protected ResourceSignals _resourceSignals;
        [Inject] protected UiSignals _uiSignals;
        [Inject] protected OnClickHighlightLogic _onClickHighlight;
        
        protected string resourceUuid; // TODO: Should be just id
        private bool _isAvailable;
        
        protected ResourceType? resourceType; // TODO: MOVE TO CONF
        protected float? yield;

        public virtual void Start()
        {
            resourceUuid = Guid.NewGuid().ToString();
            _isAvailable = true;
            _resourceSignals.FireResourceAvailableSignal(new ResourceAvailableSignal()
            {
                resourceId = resourceUuid,
                resource = this // todo not sending monobehaviour?
            });
        }

        public virtual void ResourceHandling(Unit unit, float tick)
        {
           Debug.LogWarning("Calling unimplemented overload");
        }

        public virtual float TimeToFinish()
        {
            throw new Exception("Not overloaded");
        } 

        public virtual float GetYield()
        {
            if (yield != null)
            {
                return yield.Value;
            }
            else
            {
                return 0f;   
            }
        }
        
        public virtual ResourceType GetResourceType()
        {
            if (resourceType != null)
            {
                return resourceType.Value;
            }
            else
            {
                return ResourceType.Unknown;
            }
        }

        public virtual void ZeroYield()
        {
            yield = 0f;
        }
        public void SetAvailable(bool isAvailable)
        {
            _isAvailable = isAvailable;
        }

        public bool IsAvailable()
        {
            return _isAvailable;
        }

        public string GetId()
        {
            return this.resourceUuid;
        }
    }
}