using Signals.ResourceNS;
using TileNS;
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

            if (otherGb.layer == 6 && otherGb.TryGetComponent(out Tile tile))
            {
                Debug.Log(tile.GetGuid());
                _resourceSignals.FireAddAvailableFarmPlot(
                    new AddAvailableFarmPlotSignal()
                        { farmPlot = other.gameObject, farmPlotGuid = tile.GetGuid()});
            }
        }
    }
}