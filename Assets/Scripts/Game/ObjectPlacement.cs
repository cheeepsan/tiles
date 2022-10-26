using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Building;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using Util;
using Zenject;

namespace Game
{
    public class ObjectPlacement : MonoBehaviour
    {
        [Inject] private Configuration conf;
        private List<CfgBuilding> prefabs;
        
        [SerializeField] public GameObject _house;

        private GameObject _currentPlaceableObject = null;
        private PlaceableBuilding _currentPlaceableBuilding = null;

        private bool _objectInPlacement = false;
        private Ray _ray;
        private RaycastHit _raycastHit;

        public static int TERRAIN_LAYER_MASK = 1 << 6; // todo, move to zenject
        
        void Start()
        {
            prefabs = conf.GetCfgBuildingList();
            foreach (var cfgBuilding in prefabs)
            {
                Debug.Log(cfgBuilding.path);
            }
            Debug.Log("PREFABS " + prefabs.Count());
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F) && !_objectInPlacement) // todo move to subscribe?
            {
                SpawnPlaceableObject();
            }

            if (_objectInPlacement && _currentPlaceableObject != null)
            {
                Placement();
            }
        }

        private void SpawnPlaceableObject()
        {
            _objectInPlacement = true;
            if (_currentPlaceableObject != null) // not used
            {
                Destroy(_currentPlaceableObject);
            }

            GameObject gb = Instantiate(_house);

            _currentPlaceableObject = gb;
            _currentPlaceableBuilding = gb.GetComponentInChildren<PlaceableBuilding>();

            var position = _currentPlaceableObject.transform.position;
            position.Set(position.x, 1f, position.z);
            _currentPlaceableObject.transform.position = position;
        }

        private void Placement()
        {
            _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(
                    _ray,
                    out _raycastHit,
                    1000f,
                    TERRAIN_LAYER_MASK
                ))
            {
                var currentPos = _raycastHit.transform.position;
                var pos = new Vector3((currentPos.x),
                    (currentPos.y) + 1,
                    (currentPos.z));
                _currentPlaceableObject.transform.position = pos;
            }
            else
            {
                Debug.Log("Out of raycast");
            }
            
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Destroy(_currentPlaceableObject);
                _objectInPlacement = false;
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (_currentPlaceableBuilding.canBuild)
                {
                    _objectInPlacement = false;
                    _currentPlaceableObject = null;
                    _currentPlaceableBuilding = null;
                }
            }
        }
    }
}