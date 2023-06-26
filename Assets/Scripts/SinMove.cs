
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinMove : MonoBehaviour
{
    [SerializeField]
    Material[] _materialArray = new Material[2];

    [SerializeField]
    MeshRenderer _meshRenderer;

    [SerializeField]
    LineRenderer _lineRenderer;

    [SerializeField]
    GameObject _ball;

    [SerializeField]
    float _theta = 0;

    [SerializeField]
    float _sin = 0;

    [SerializeField]
    float _tan = 0;
    private void FixedUpdate()
    {
        _sin = Mathf.Sin(_theta);
        _tan = Mathf.Tan(_theta);

        _ball.transform.position = new Vector3(_theta - 10, 0.5f, _tan);

        _theta += 4 * Mathf.PI / 360;

        if(_sin > 0)
        {
            _meshRenderer.material = _materialArray[1];
        }
        else 
        {
            _meshRenderer.material = _materialArray[0];
        }
    }
}
