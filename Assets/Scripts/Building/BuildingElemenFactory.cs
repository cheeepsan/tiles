using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

namespace BuildingNS
{
    public class BuildingElementFactory : PlaceholderFactory<Object, BuildingElement> {
        readonly DiContainer _container;
        
        public BuildingElementFactory(DiContainer container)
        {
            _container = container;
        }
        
        public BuildingElement Create(Object prefab, Transform transform)
        {
            return _container.InstantiatePrefabForComponent<BuildingElement>(prefab, transform);
        }

    }
}