using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BuildingNS;
using Game.BuildingNS;
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
        [Inject] private readonly Configuration conf;
        [Inject] private readonly UiSignals _uiSignals;
        [Inject] private readonly BuildingSignals _buildingSignals;
        
        private List<CfgBuilding> _prefabs;

        private Building _selectedObject;

        private GameObject _currentPlaceableObject = null;
        private PlaceableBuilding _currentPlaceableBuilding = null;

        private bool _objectInPlacement = false;
        private Ray _ray;
        private RaycastHit _raycastHit;

        public static int TERRAIN_LAYER_MASK = 1 << 6; // todo, move to zenject


        
        void Start()
        {
            _prefabs = conf.GetCfgBuildingList();
            var firstBuildingConf = _prefabs.First();
            var fromResources = (GameObject)Resources.Load(firstBuildingConf.path);
            
            _selectedObject = new Building(fromResources, firstBuildingConf);
            SubscribeToUiSignals();
        }

        void Update()
        {
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

            GameObject gb = Instantiate(_selectedObject.GetGameObject());

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
                    BuildingPlacedSignal buildingPlacedSignal = new BuildingPlacedSignal
                    {
                        placedTimestamp = DateTime.Now,
                        buildingConfig = _selectedObject.GetBuildingConfiguration(),
                        pos = _currentPlaceableObject.transform.position
                    };
                    
                    _currentPlaceableBuilding.Place();
                    _buildingSignals.FireBuildingPlacedEvent(buildingPlacedSignal);
                    
                    _objectInPlacement = false;
                    _currentPlaceableObject = null;
                    _currentPlaceableBuilding = null;
                }
            }
        }

        // Subscribes

        private void SubscribeToUiSignals()
        {
            Action afterClick = SpawnPlaceableObject;
            
            _uiSignals.Subscribe4<BuildingButtonClickedSignal, List<CfgBuilding>, Building, Action>(
                (x, b, o, a) => 
                    SetCurrentBuilding(x, b, o, a), _prefabs, _selectedObject, afterClick);
        }

        private Action<BuildingButtonClickedSignal, List<CfgBuilding>, Building, Action> SetCurrentBuilding = (s, l, o, a) =>
        {
            var rGameObject = (GameObject)Resources.Load(s.buildingConf.path);
            o.SetBuildingConfiguration(s.buildingConf);
            o.SetGameObject(rGameObject);
            
            a.Invoke();
        };
        
    }
}