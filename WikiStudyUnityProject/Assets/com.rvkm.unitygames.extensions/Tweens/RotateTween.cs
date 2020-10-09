using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.rvkm.unitygames.extensions.tween
{
    public class RotateTween : MonoBehaviour
    {
        Transform _transform;
        [SerializeField] Vector3 rotateVector = new Vector3(0, 0, 1f);
        [SerializeField] float speed = 125f;
        private void Awake()
        {
            _transform = transform;
        }

        // Update is called once per frame
        void Update()
        {
            _transform.Rotate(rotateVector * speed * Time.deltaTime);
        }
    }
}