using System.Collections;
using Gamla.Scripts.Common;
using UnityEngine;

namespace Gamla.Scripts.Test
{
    public class TestCoroutineManager : MonoBehaviour
    {
        public void Start()
        {
            HomeThreadHelper.homeThread.ExecuteCoroutine(GlobalCoroutine());
        }

        IEnumerator GlobalCoroutine()
        {
            yield return new WaitForSecondsRealtime(10f); 
            //Debug.LogError("Coroutine");
        }
    }
}