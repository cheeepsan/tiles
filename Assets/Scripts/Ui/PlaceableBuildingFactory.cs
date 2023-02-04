using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace BuildingNS
{
    public class UiBuildingButtonFactory : PlaceholderFactory<Object, Button> {
        readonly DiContainer _container;
        
        public UiBuildingButtonFactory(DiContainer container)
        {
            _container = container;
        }
        
        public Button Create(Object prefab, Transform transform)
        {
            Button building =
                    _container.InstantiatePrefabForComponent<Button>(prefab, transform);

            return building;
        }
    }
}