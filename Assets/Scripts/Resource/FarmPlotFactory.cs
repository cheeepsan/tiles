using BuildingNS;
using UnitNS;
using UnityEngine;
using Zenject;

namespace ResourceNS
{
    public class FarmPlotFactory : PlaceholderFactory<Object, FarmPlot> {
        readonly DiContainer _container;
        
        public FarmPlotFactory(DiContainer container)
        {
            _container = container;
        }
        
        public FarmPlot Create(Object prefab, Transform transform)
        {
            return _container.InstantiatePrefabForComponent<FarmPlot>(prefab, transform);
        }

    }
}