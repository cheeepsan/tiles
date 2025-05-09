using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using GridNS;
using NetTopologySuite.Geometries;
using Signals.ResourceNS;
using TideNS;
using UnityEngine;
using Util;
using Zenject;

namespace Game
{
    public class TideManager : IInitializable
    {
        [Inject] private GroundTileset _groundTileset;
        [Inject] private WaterTileset _waterTileset;
        [Inject] private TideMeshFactory _tideMeshFactory;
        [Inject] private Configuration _configuration;
        
        private readonly ResourceSignals _tideSignals;
        private Dictionary<String, Transform> _farmPlots;
        private List<TideMesh> _createdMeshes;
        private bool _floodActive;
        private List<Vector3> _waterAnchors;
        public TideManager(ResourceSignals tideSignals)
        {
            _tideSignals = tideSignals;
            _farmPlots = new Dictionary<String, Transform>();
            _createdMeshes = new List<TideMesh>();
            _floodActive = false;
            _waterAnchors = new List<Vector3>();
            TimeManager.OnMonthChange += delegate(object sender, TimeManager.OnMonthChangeEventArgs args)
            {
                YearlyCycle(args);
            };

            if (_floodActive)
            {
                GenerateTide();
            }

            
            SubscribeToSignals();
        }
        
        
        
        /*
         * 0 - 3 flood
         * 4 - 7 field works
         * 8 - 11 gather
         */
        private void YearlyCycle(TimeManager.OnMonthChangeEventArgs args)
        {
            List<int> floodMonth = new List<int>() { 0, 1, 2 };
            
            int month = args.month;
            if (floodMonth.Contains(month))
            {
                // flood
                if (month == 1)
                {
                    if (!_floodActive)
                    {            
                        GenerateTide();
                        _floodActive = true;
                    }

                }
            }

            if (month == 4)
            {
                // remove flood
                // create farms
                EndTide();
            }
        }

        private void EndTide()
        {
            foreach (var mesh in _createdMeshes)
            {
                if (mesh != null)
                {
                    GameObject.Destroy(mesh.gameObject);
                }

            }
            
            _floodActive = false;
        }
        private void GenerateTide()
        {
            var list = _farmPlots.Values.ToList(); 
            ConcaveAlgo a = new ConcaveAlgo(list);
            List<MeshData> meshes = a.CalculateMeshes();
            foreach (var meshData in meshes)
            {
                Mesh m = new Mesh
                {
                    name = "test mesh " + meshData.name
                };

                GameObject gb = (GameObject)Resources.Load(_configuration.GetSettings().tideMeshPath);
                TideMesh mesh = _tideMeshFactory.Create(gb);
                
                mesh.name = meshData.name;
            
            
                m.vertices = meshData.triangleVertices;
                m.triangles = meshData.triagnles;
                m.normals = meshData.normals;
                m.tangents = meshData.tangents;
                m.uv = meshData.uuvs;
                var center = meshData.centroid;
                mesh.transform.position = new Vector3((float)center.X, 0.66f, (float)center.Y);
                m.RecalculateNormals();
                mesh.GetComponent<MeshFilter>().sharedMesh = m;
                m.RecalculateBounds();
                
                _createdMeshes.Add(mesh);
            }
        }

        private void CreateWater()
        {
            var list = _waterTileset.GetAllWaterPos().Select(x =>
            {
                GameObject gb = new GameObject();
                gb.transform.position = x;
                return gb.transform;
            }).ToList();

            ConcaveAlgo a = new ConcaveAlgo(list);
            List<MeshData> meshes = a.CalculateMeshes();
            
            // hack, quick cleanup
            list.ForEach(x => GameObject.Destroy(x.gameObject));
            
            foreach (var meshData in meshes)
            {
                Point center = meshData.centroid;
                Vector3 meshCenter = new Vector3((float)center.X, 0f, (float)center.Y);
               _waterAnchors.Add(meshCenter);
                
                Mesh m = new Mesh();

                GameObject gb = (GameObject)Resources.Load(_configuration.GetSettings().tideMeshPath);
                TideMesh mesh = _tideMeshFactory.Create(gb);
                
                mesh.name = "Water " + meshData.name;

                m.vertices = meshData.triangleVertices;
                m.triangles = meshData.triagnles;
                m.normals = meshData.normals;
                m.tangents = meshData.tangents;
                m.uv = meshData.uuvs;
      
                mesh.transform.position = new Vector3((float)center.X, 0.4f, (float)center.Y);
                m.RecalculateNormals();
                mesh.GetComponent<MeshFilter>().sharedMesh = m;
                m.RecalculateBounds();
       
            }
        }

        private void SubscribeToSignals()
        {
            SubscribeToAddAvailableFarmPlot();
        }
        
        private void AddFarmPlot(Transform farmPlotCoord)
        {
            String farmPlotGuid = _groundTileset.GetGroundTileByPosition(farmPlotCoord.position);
            
            if (!_farmPlots.ContainsKey(farmPlotGuid))
            {
                // parent game object has actual pos
                _farmPlots.Add(farmPlotGuid, farmPlotCoord.parent.transform);
            }
        }
        
        private void SubscribeToAddAvailableFarmPlot()
        {
            _tideSignals.Subscribe<AddAvailableFarmPlotSignal>
                ((x) => { AddFarmPlot(x.farmPlotTransform); });
        }

        public void Initialize()
        {
            CreateWater();
        }
    }
}