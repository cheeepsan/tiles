using UnityEngine;
using Zenject;

namespace Common.Logic
{
    public class OnClickHighlightLogic
    {

        [Inject] private HighlightRenderer _highlightRenderer;
        
        
        public void Highlight(GameObject gb)
        {
            Bounds b = gb.GetComponent<Renderer>().bounds;
            var center = b.center;
            var size = b.size;

            _highlightRenderer.SpawnLine(center, size);
        }
        
    }
}