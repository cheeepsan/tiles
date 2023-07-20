using Constant;
using Signals.ResourceNS;
using UnityEngine;
using Zenject;

namespace TideNS
{
    public class Tide : MonoBehaviour
    {
        [Inject] private ResourceSignals _resourceSignals;

        private void OnTriggerEnter(Collider other)
        {
            GameObject otherGb = other.gameObject;

            /*
             * REASON FOR: otherGb.transform.parent.gameObject.name is that coordinates are stored in parents,
             * while logic is within child. TODO maybe mimic transform in children? How?
             */
            if (otherGb.layer == 6 &&
                (otherGb.transform.parent != null && otherGb.transform.parent.gameObject.name == NameConstants.GROUND_TILE))
            {
                _resourceSignals.FireAddAvailableFarmPlot(
                    new AddAvailableFarmPlotSignal()
                        { farmPlotTransform = otherGb.transform });
            }
        }
    }
}