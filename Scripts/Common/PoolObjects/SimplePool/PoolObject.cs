using UnityEngine;

namespace Gamla.Logic
{
    public class PoolObject : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _rootParticleSystem;
        
        private Transform _root;
        public void Spawn(Transform parent, Vector3 position, Quaternion rotation)
        {
            _root = transform.parent;
            gameObject.SetActive(true);
            transform.SetParent(parent);
            transform.localPosition = position;
            transform.localRotation = rotation;

            if (_rootParticleSystem != null) {
                 // _rootParticleSystem.Stop(true,
                 //   ParticleSystemStopBehavior.StopEmittingAndClear); // мгновенно но не дешево
                 //_rootParticleSystem.Stop(true,
                 // ParticleSystemStopBehavior.StopEmittingAndClear); // не мгновенно но дешево
            }
            gameObject.SetActive(true);
        }
        public void Release()
        {
            gameObject.SetActive(false);
            transform.localScale = Vector3.one;
            transform.SetParent(_root);
            SimplePoolManager.Release(this);
        }
        public void OnDestroy()
        {
            Debug.Log("Object destroy, please check release on parent");
        }
    }
}