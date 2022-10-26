using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Building;
using JetBrains.Annotations;
using Signals.UI;
using Unity.VisualScripting;
using UnityEngine;
using Util;
using Zenject;

namespace Game
{
    public class ObjectPlacement : MonoBehaviour
    {
        [Inject] private Configuration conf;
        [Inject] private UiSignals _uiSignals;
        
        private List<CfgBuilding> _prefabs;
        
        [SerializeField] public GameObject _house;

        private GameObject _currentPlaceableObject = null;
        private PlaceableBuilding _currentPlaceableBuilding = null;

        private bool _objectInPlacement = false;
        private Ray _ray;
        private RaycastHit _raycastHit;

        public static int TERRAIN_LAYER_MASK = 1 << 6; // todo, move to zenject


        
        void Start()
        {
            _prefabs = conf.GetCfgBuildingList();
            // foreach (var cfgBuilding in prefabs)
            // {
            //     Debug.Log(cfgBuilding.path);
            // }
            // Debug.Log("PREFABS " + prefabs.Count());

            SubscribeToUiSignals();
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
                var pos = new Vector3(
                    (currentPos.x),
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

        // Subscribes

        private void SubscribeToUiSignals()
        {
            //_uiSignals.Subscribe<BuildingButtonClickedSignal>(SetCurrentBuilding);
            
            _uiSignals.Subscribe3<BuildingButtonClickedSignal, List<CfgBuilding>, GameObject>(
                (x, b, o) => SetCurrentBuilding(x, b, o), _prefabs, _house);
        }

        private Action<BuildingButtonClickedSignal, List<CfgBuilding>, GameObject> SetCurrentBuilding = (s, l, o) =>
        {
            var id = s.id;
            var p = l.Find(x => x.id == id);
            var r = (GameObject)Resources.Load(p.path);
            Debug.Log("Found: " + p.id + ", name: " + p.name + ", path: " + p.path );
            Debug.Log(r);
            Debug.Log(o);
            // TODO GAME OBJECT TO WRAPPER?

        };

        /*
        private Action<BuildingButtonClickedSignal> SetCurrentBuilding = signal =>
        {
            
            Debug.Log("CLICKED ON: " + signal.id);
        };
        */
        
    }
}