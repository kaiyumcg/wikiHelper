using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.rvkm.unitygames.extensions.tween
{
    public class ScaleTween : MonoBehaviour
    {
        [SerializeField] Vector3 from, to;
        [SerializeField] bool isRepeating = false;
        [SerializeField] float speed = 0.2f;
        [SerializeField] bool unscaledTime = false;

        Transform _transform;
        private void OnEnable()
        {
            if (_transform == null) { _transform = transform; }
            _transform.localScale = from;
            target = to;
            isTween = true;
        }

        Vector3 target;
        bool isTween = false;
        // Update is called once per frame
        void Update()
        {
            if (isTween == false) { return; }
            _transform.localScale = Vector3.MoveTowards(_transform.localScale, target, speed * (unscaledTime ? Time.unscaledDeltaTime : Time.deltaTime));
            if (Vector3.Distance(_transform.localScale, target) < 0.001f)
            {
                if (isRepeating)
                {
                    //decide new target
                    target = Vector3.Distance(target, from) > Vector3.Distance(target, to) ? from : to;
                }
                else
                {
                    isTween = false;
                    _transform.localScale = to;
                }
            }
        }
    }
}