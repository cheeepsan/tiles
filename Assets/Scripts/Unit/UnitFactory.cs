using UnityEngine;
using Zenject;

namespace UnitNS
{
    public class UnitFactory : PlaceholderFactory<Object, Unit> {
        readonly DiContainer _container;
        
        public UnitFactory(DiContainer container)
        {
            _container = container;
        }
        
        public Unit Create(Object prefab, Transform transform)
        {
            return _container.InstantiatePrefabForComponent<Unit>(prefab, transform);
        }

    }
}