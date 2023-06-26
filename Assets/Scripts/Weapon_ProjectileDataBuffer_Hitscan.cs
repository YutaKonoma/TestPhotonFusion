using Fusion;
using Projectiles;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Weapon_ProjectileDataBuffer_Hitscan : WeaponBase
{
    [SerializeField]
    [Header("íeÇ™ìñÇΩÇÈÉåÉCÉÑÅ[")]
    LayerMask _hitMask;

    [SerializeField]
    [Header("íeÇ™ìñÇΩÇ¡ÇΩéûÇÃà–óÕ")]
    float _hitImpulse;

    private GameObject _hit;
    [SerializeField]
    Collider _collider;

    private NetworkObject _networkObject;

    [Networked]
    private int _fireCount { get; set; }

    private void Awake()
    {
        _networkObject = GetComponent<NetworkObject>();
        _collider = _hit.GetComponent<Collider>();
    }

    public override void Fire()
    {
        var hitPosition = Vector3.zero;

        var hitOptions = HitOptions.IncludePhysX | HitOptions.IgnoreInputAuthority;

        if (Runner.LagCompensation.Raycast(FireTransform.position, FireTransform.forward, 100f,
            _networkObject.InputAuthority, out var hit, _hitMask, hitOptions) == true)
        {
            if (hit.Collider != null && hit.Collider.attachedRigidbody != null)
            {
                hit.Collider.attachedRigidbody.AddForce(FireTransform.forward * _hitImpulse, ForceMode.Impulse);
            }
            _fireCount++;
        }
    }

    private bool RaycastHit(Vector3 position, Vector3 forward, float v, PlayerRef inputAuthority, out GameObject hit, LayerMask hitMask, HitOptions hitOptions)
    {
        throw new NotImplementedException();
    }

    private bool RaycastHit(Vector3 position, Vector3 forward, float v, PlayerRef inputAuthority, out Collider hit, LayerMask hitMask, HitOptions hitOptions)
    {
        throw new NotImplementedException();
    }
}

