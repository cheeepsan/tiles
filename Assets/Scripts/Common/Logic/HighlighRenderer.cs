using System.Linq;
using UnityEngine;

namespace Common.Logic
{
    public class HighlightRenderer : MonoBehaviour
    {
        
        public LineRenderer _lineRenderer;


        // Start is called before the first frame update
        private void Start()
        {
            Debug.Log("Highlight renderer created");
            _lineRenderer = gameObject.AddComponent<LineRenderer>();
            _lineRenderer.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
            _lineRenderer.startWidth = 0.1f;
            _lineRenderer.endWidth = 0.1f;
            _lineRenderer.startColor = Color.green;
            _lineRenderer.endColor = Color.green;
        }

        public void SpawnLine(Vector3 center, Vector3 size)
        {
            Vector3 startingPoint = new Vector3(center.x, center.y + 2f, center.z);
            Vector3 endingPoint = new Vector3(center.x, center.y + 3f, center.z);
            Vector3[] points = new[]
            {
                startingPoint,
                endingPoint
            };
            _lineRenderer.SetPositions(points);
          
        }
        

    }
}