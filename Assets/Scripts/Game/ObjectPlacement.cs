using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BuildingNS;
using Constant;
using Signals.Building;
using JetBrains.Annotations;
using ModestTree;
using Signals.UI;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Splines;
using Util;
using Zenject;

namespace Game
{
    public class ObjectPlacement : MonoBehaviour
    {
        [Inject] private readonly Configuration conf;
        [Inject] private readonly UiSignals _uiSignals;
        [Inject] private readonly BuildingSignals _buildingSignals;
        [Inject] private readonly BuildingManager _buildingManager;
        
        [Inject] private readonly PlaceableBuildingFactory _placeableBuildingFactory;
        
        private List<CfgBuilding> _prefabs;

        private Building _selectedObject;

        private GameObject _currentPlaceableObject = null;
        private PlaceableBuilding _currentPlaceableBuilding = null;
        private PlaceableBuilding _currentPlaceableAnchor = null;

        private bool _objectInPlacement = false;
        private Ray _ray;
        private RaycastHit _raycastHit;

        public static int TERRAIN_LAYER_MASK = 1 << 6; // todo, move to zenject

        private GameObject _hierarchyParent;

        private SplineContainer _splineContainer;
        private LineRenderer _lineRenderer;
        private NavMeshPath _navMeshPathHelper;
        private bool _navMeshPathComplete = false;
        void Start()
        {
            _splineContainer = gameObject.AddComponent<SplineContainer>();
            _lineRenderer = gameObject.AddComponent<LineRenderer>();
            _lineRenderer.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
            _navMeshPathHelper = new NavMeshPath();
            
            _hierarchyParent = GameObject.Find("Objects"); // hack, move to own static class
            _prefabs = conf.GetCfgBuildingList();
            var firstBuildingConf = _prefabs.First();
            var fromResources = (GameObject)Resources.Load(firstBuildingConf.path);
            
            _selectedObject = new Building(fromResources, firstBuildingConf);
            SubscribeToUiSignals();
        }
        
        // todo: should be connected to timer?
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
            if (_currentPlaceableBuilding.GetBuildingConfig().id == BuildingConstants.PALACE_ID)
            {
                _navMeshPathComplete = true;
            }
            else
            {
                BuildBezier();
            }
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
                    
                    //Debug.Log(scale.magnitude);


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
                    //Debug.Log(scale.magnitude);
                    //Debug.Log(scale.normalized);
                    pos = new Vector3(
                        (currentPos.x),
                        ((currentPos.y) + 1) ,
                        (currentPos.z));
                }

                _currentPlaceableObject.transform.position = pos;
                
                // TODO in case of multiple palaces anchor should be recalculated
                if (_currentPlaceableAnchor != null)
                {
                    Curve(_currentPlaceableAnchor.transform, _currentPlaceableBuilding.transform);
                }
            }
            else
            {
                //Debug.Log("Out of raycast");
            }
            
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Destroy(_currentPlaceableObject);
                _objectInPlacement = false;
                _currentPlaceableAnchor = null;
                DisableLineRenderer();
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (_currentPlaceableBuilding.canBuild && _navMeshPathComplete)
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
                    _currentPlaceableAnchor = null;
                    DisableLineRenderer();
                }
            }
        }

        private void BuildBezier()
        {
                        
            if (!_buildingManager.GetAllBuildings().Values.IsEmpty())
            {
                Vector3 buildingPos = _currentPlaceableBuilding.transform.position;
                PlaceableBuilding[] anchors = 
                    _buildingManager.GetAllBuildings().Values.
                        Where(x => x.GetBuildingConfig().id == BuildingConstants.PALACE_ID).ToArray();
                float distance = 0;
                foreach (var anchor in anchors)
                {
                    float tempDistance = Vector3.Distance(buildingPos, anchor.transform.position);
                    if (distance == 0)
                    {
                        distance = tempDistance;
                        _currentPlaceableAnchor = anchor;
                    } else if (tempDistance < distance)
                    {
                        distance = tempDistance;
                        _currentPlaceableAnchor = anchor;
                    }
                }
            }

            if (_currentPlaceableAnchor != null)
            {
                EnableLineRenderer();
                Curve(_currentPlaceableAnchor.transform, _currentPlaceableBuilding.transform);
            }
        }

        private void Curve(Transform initPos, Transform buildingPos)
        {
            if (_lineRenderer.enabled)
            {
                // Create a new Spline on the SplineContainer.
                Spline spline = null; 
                if (_splineContainer.Splines.Count == 0)
                {
                    spline = _splineContainer.AddSpline();
                }
                else
                {
                    spline = _splineContainer.Spline;
                }
                

                // Set some knot values.
                var knots = new BezierKnot[2];
                knots[0] = new BezierKnot(new float3(initPos.position));
                knots[1] = new BezierKnot(new float3(buildingPos.position));

                knots[0].TangentOut = new float3(0f, 5f, 0f);
                knots[1].TangentIn = new float3(0f, 5f, 0f);
                spline.Knots = knots;

                List<Vector3> p = new List<Vector3>();

                var curve = spline.GetCurve(0);

                for (float i = 0; i < 1f; i += 0.05f)
                {
                    var ab = Vector3.Lerp(curve.P0, curve.P1, i);
                    var bc = Vector3.Lerp(curve.P1, curve.P2, i);
                    var cd = Vector3.Lerp(curve.P2, curve.P3, i);

                    var abbc = Vector3.Lerp(ab, bc, i);
                    var bccd = Vector3.Lerp(bc, cd, i);

                    var abcd = Vector3.Lerp(abbc, bccd, i);
                    p.Add(abcd);
                }
                
                spline.Clear();
                
                _lineRenderer.positionCount = p.Count;
                _lineRenderer.startWidth = 0.1f;
                _lineRenderer.endWidth = 0.1f;
                _lineRenderer.numCornerVertices = 40;
                _lineRenderer.SetPositions(p.ToArray());
                _lineRenderer.loop = false;


                // MASK == 1 == WALKABLE
                NavMesh.CalculatePath(initPos.position, buildingPos.position, 1, _navMeshPathHelper);
                _navMeshPathComplete = _navMeshPathHelper.status == NavMeshPathStatus.PathComplete;


                
                if (_currentPlaceableBuilding.canBuild && _navMeshPathComplete)
                {
                    _lineRenderer.startColor = Color.green;
                    _lineRenderer.endColor = Color.green;
                }
                else
                {
                    _lineRenderer.startColor = Color.red;
                    _lineRenderer.endColor = Color.red;
                }
            }
        }
        
        private void DisableLineRenderer()
        {
            _lineRenderer.enabled = false;
        }

        private void EnableLineRenderer()
        {
            _lineRenderer.enabled = true;
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