using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch1 : MonoBehaviour
{
    [SerializeField]
    Material[] _materialArray = new Material[2];

    [SerializeField]
    MeshRenderer _meshRenderer;

    [SerializeField]
    float _rad = 0;

    [SerializeField]    
    float _sin = 0;

    private void FixedUpdate()
    {
        _sin = Mathf.Sin(_rad);
        //if‚Ì’†‚ÉðŒ‚ð‰Á‚¦‚é‚Æ“®‚­‘¬“x‚ª•Ï‚í‚é
        if(_sin > 0.7f && _sin < 0.9f) 
        {
            _meshRenderer.material = _materialArray[1];
        }
        else
        {
            _meshRenderer.material = _materialArray[0];
        }
        //‘¬“x‚É‚©‚©‚í‚é
        _rad += 2 * (2 * Mathf.PI / 360);
    }
}
