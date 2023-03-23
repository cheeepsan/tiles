using UnityEngine;
using Zenject;

namespace TideNS
{
    public class TideMeshFactory : PlaceholderFactory<Object, TideMesh>
    {
        readonly DiContainer _container;

        public TideMeshFactory(DiContainer container)
        {
            _container = container;
        }

        public override TideMesh Create(Object prefab)
        {
            return _container.InstantiatePrefabForComponent<TideMesh>(prefab);
        }
    }
}