using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guruguru : MonoBehaviour
{
    [SerializeField]
    GameObject _ball;

    [SerializeField]
    float _theta;

    [SerializeField]
    float _phi;
    [SerializeField]
    float _r;

    void Update()
    {
        float x = _r * Mathf.Sin(_theta) * Mathf.Cos(_phi);
        float y = _r * Mathf.Cos(_theta);
        float z = _r * Mathf.Sin(_theta) * Mathf.Sin(_phi);
        
        _ball.transform.position = new Vector3(x, y, z);

        _theta += 0.05f * (2 * Mathf.PI / 360);
        _phi += 3 * (2 * Mathf.PI / 360);
    }
}
