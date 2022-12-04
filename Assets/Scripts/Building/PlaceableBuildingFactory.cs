using UnityEngine;
using Zenject;

namespace BuildingNS
{
    public class PlaceableBuildingFactory : PlaceholderFactory<Object, PlaceableBuilding> {
        readonly DiContainer _container;
        
        public PlaceableBuildingFactory(DiContainer container)
        {
            _container = container;
        }
        
        public PlaceableBuilding Create(Object prefab, Transform transform)
        {
            return _container.InstantiatePrefabForComponent<PlaceableBuilding>(prefab, transform);
        }

    }
}