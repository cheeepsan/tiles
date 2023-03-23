using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BuildingNS;
using Signals.Building;
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
        
        [Inject] private readonly PlaceableBuildingFactory _placeableBuildingFactory;
        
        private List<CfgBuilding> _prefabs;

        private Building _selectedObject;

        private GameObject _currentPlaceableObject = null;
        private PlaceableBuilding _currentPlaceableBuilding = null;

        private bool _objectInPlacement = false;
        private Ray _ray;
        private RaycastHit _raycastHit;

        public static int TERRAIN_LAYER_MASK = 1 << 6; // todo, move to zenject

        private GameObject _hierarchyParent;

        
        void Start()
        {
            _hierarchyParent = GameObject.Find("Objects"); // hack, move to own static class
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

            PlaceableBuilding instantiatedBuilding = _placeableBuildingFactory.Create(_selectedObject.GetGameObject(), _hierarchyParent.transform);
            
            _currentPlaceableObject = instantiatedBuilding.gameObject;
            _currentPlaceableBuilding = instantiatedBuilding;
            _currentPlaceableBuilding.SetBuildingConfig(_selectedObject.GetBuildingConfiguration()); // TODO: Unify Building and PlaceableBuilding?    
            
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
                var scale = _currentPlaceableObject.transform.localScale;

                var currentPos = _raycastHit.transform.position;
                
                Vector3 pos;
                if (scale.magnitude < 1 ) // ??
                {
                    var point = _raycastHit.point;
                    var pointX = point.x;
                    var pointZ = point.z;
                    
                    Debug.Log(scale.magnitude);


                    int offsetX = (int)((pointX - currentPos.x) / scale.x);
                    int offsetZ = (int)((pointZ - currentPos.z) / scale.z);
                    
                    float offsetY = scale.y / 2;
                    
                    pos = new Vector3(
                        (currentPos.x + (scale.x * offsetX)),
                        ((currentPos.y) + (scale.y * 2) + offsetY), // ??
                        (currentPos.z + (scale.z * offsetZ))
                    );
                }
                else
                {
                    Debug.Log(scale.magnitude);
                    Debug.Log(scale.normalized);
                    pos = new Vector3(
                        (currentPos.x),
                        ((currentPos.y) + 1) ,
                        (currentPos.z));
                }

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
                        placeableBuilding = _currentPlaceableBuilding
                    };
                    
                    _currentPlaceableBuilding.Place(); 
                    
                    _buildingSignals.FireBuildingPlacedEvent(buildingPlacedSignal); // TODO FIRE ONLY IF ACTUALLY PLACED
                    
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