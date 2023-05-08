using UnityEngine;
using UnityEngine.Tilemaps;

/*
 * Helper object to ease organization of tilesets
 */
namespace GridNS
{
    public class Grid : MonoBehaviour
    {
        [SerializeField] public Tilemap ground;
        [SerializeField] public Tilemap objects;
        [SerializeField] public Tilemap water;
        [SerializeField] public Tilemap shallowWater;
    }
}