using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.rvkm.unitygames.extensions.tween
{
    public class FlickTween : MonoBehaviour
    {
        [SerializeField] Vector3 from, to;
        [SerializeField] bool isRepeating = false;
        [SerializeField] float speed = 0.2f;
        [SerializeField] bool unscaledTime = false;

        Transform _transform;
        private void OnEnable()
        {
            if (_transform == null) { _transform = transform; }
            _transform.localRotation = Quaternion.Euler(from);
            target = Quaternion.Euler(to);
            isTween = true;
        }

        Quaternion target;
        bool isTween = false;

        // Update is called once per frame
        void Update()
        {
            if (isTween == false) { return; }

            _transform.localRotation = Quaternion.Slerp(_transform.localRotation, target, speed * (unscaledTime ? Time.unscaledDeltaTime : Time.deltaTime));
            if (Quaternion.Angle(_transform.localRotation, target) < 0.01f)
            {
                if (isRepeating)
                {
                    //decide new target
                    target = Quaternion.Angle(target, Quaternion.Euler(from)) > Quaternion.Angle(target, Quaternion.Euler(to)) ?
                        Quaternion.Euler(from) : Quaternion.Euler(to);
                }
                else
                {
                    isTween = false;
                    _transform.localRotation = Quaternion.Euler(to);
                }
            }
        }
    }

}