using System.Collections;
using UnityEngine;

namespace Util
{
    public class CoroutineRunner : MonoBehaviour
    {
        private IEnumerator _coroutine;

        public void RunCoroutine()
        {
            if (_coroutine != null)
                StartCoroutine(_coroutine);
        }

        public void SetCoroutine(IEnumerator coroutine)
        {
            _coroutine = coroutine;
        }
    }
}