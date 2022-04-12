using UnityEngine;

namespace Gamla.UI
{
    public class SpinnerAnim : MonoBehaviour
    {
        //private bool _inAnim = false;
        public void Start()
        {
            //transform.DOLocalRotate(new Vector3(0, 0, -360), 0.7f, RotateMode.Fast).SetLoops(-1);
        }

        void LateUpdate()
        {
            transform.Rotate(Vector3.back * 30 * Time.deltaTime, Space.Self);
        }
        
    }
}