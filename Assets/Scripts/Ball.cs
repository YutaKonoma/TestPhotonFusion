using Fusion;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;

public class Ball : NetworkBehaviour
{
    [Networked] private TickTimer Life { get; set; }

    private Material _material;

    [Networked]
    private int _fireTick { get; set; }
    [Networked]
    private UnityEngine.Vector3 _firePosition { get; set; }
    [Networked]
    private UnityEngine.Vector3 _fireVelocity { get; set; }

    private LayerMask _hitMask;
    private UnityEngine.Vector3 _hitPosition;

    [SerializeField]
    private float _hitImpulse = 50f;

    [SerializeField]
    private float _lifeTimeAfterHit = 2f;

    [Networked]
    private TickTimer _lifeCooldown { get; set; }

    [Networked]
    private NetworkBool _isDestroyed { get; set; }
    public void Init()
    {
        Life = TickTimer.CreateFromSeconds(Runner, 2.0f);
    }
    Material material
    {
        get
        {
            if (_material == null)
                _material = GetComponentInChildren<MeshRenderer>().material;
            return _material;
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (Life.Expired(Runner))
            Runner.Despawn(Object);
        else
            transform.position += 5 * transform.forward * Runner.DeltaTime;

        var previousPosition = GetMovePosition(Runner.Tick - 1);
        var nextPosition = GetMovePosition(Runner.Tick);
        var direction = nextPosition - previousPosition;
        float distance = direction.magnitude;
        direction /= distance;
        var hitOptions = HitOptions.IncludePhysX | HitOptions.IgnoreInputAuthority;
        if (Runner.LagCompensation.Raycast(previousPosition, direction, distance,
                    Object.InputAuthority, out var hit, _hitMask, hitOptions) == true)
        {
            _isDestroyed = true;
            _lifeCooldown = TickTimer.CreateFromSeconds(Runner, _lifeTimeAfterHit);

            // Save hit position so hit effects are at correct position on proxies
            _hitPosition = hit.Point;

            if (hit.Collider != null && hit.Collider.attachedRigidbody != null)
            {
                hit.Collider.attachedRigidbody.AddForce(direction * _hitImpulse, ForceMode.Impulse);
                float renderTime = Object.IsProxy == true ? Runner.InterpolationRenderTime : Runner.SimulationRenderTime;
                float floatTick = renderTime / Runner.DeltaTime;
                transform.position = GetMovePosition(floatTick);
            }
        }

    UnityEngine.Vector3 GetMovePosition(float currentTick)
    {
        float time = (currentTick - _fireTick) * Runner.DeltaTime;

        if (time <= 0f)
            return _firePosition;

        return _firePosition + _fireVelocity * time;
    }
}
}
