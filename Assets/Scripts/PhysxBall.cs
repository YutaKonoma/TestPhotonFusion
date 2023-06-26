using UnityEngine;
using Fusion;

public class PhysxBall : NetworkBehaviour
{
    [Networked] 
    private TickTimer life { get; set; }

    [Networked]
    private int _fireTick { get; set; }
    [Networked]
    private UnityEngine.Vector3 _firePosition { get; set; }
    [Networked]
    private UnityEngine.Vector3 _fireVelocity { get; set; }

    private LayerMask _hitMask;
    private UnityEngine.Vector3 _hitPosition;

    [Networked(OnChanged = nameof(OnDestroyChanged))]
    private NetworkBool _isDestroyed { get; set; }

    [SerializeField]
    private float _hitImpulse = 50f;

    [SerializeField]
    private float _lifeTimeAfterHit = 2f;

    public void Init(Vector3 forward)
    {
        life = TickTimer.CreateFromSeconds(Runner, 5.0f);
        GetComponent<Rigidbody>().velocity = forward;
    }

    public override void FixedUpdateNetwork()
    {
        if (life.Expired(Runner))
            Runner.Despawn(Object);

        if (_isDestroyed == true)
            return;
        var previousPosition = GetMovePosition(Runner.Tick - 1);
        var nextPosition = GetMovePosition(Runner.Tick);
        var direction = nextPosition - previousPosition;
        float distance = direction.magnitude;
        direction /= distance;
        var hitOptions = HitOptions.IncludePhysX | HitOptions.IgnoreInputAuthority;
        if (Runner.LagCompensation.Raycast(previousPosition, direction, distance, Object.InputAuthority, out var hit, _hitMask, hitOptions) == true)
        {
            _isDestroyed = true;
 
            _hitPosition = hit.Point;

            if (hit.Collider != null && hit.Collider.attachedRigidbody != null)
            {
                hit.Collider.attachedRigidbody.AddForce(direction * _hitImpulse, ForceMode.Impulse);
                float renderTime = Object.IsProxy == true ? Runner.InterpolationRenderTime : Runner.SimulationRenderTime;
                float floatTick = renderTime / Runner.DeltaTime;
                transform.position = GetMovePosition(floatTick);
            }
        }
    }
     private Vector3 GetMovePosition(float currentTick)
    {
        float time = (currentTick - _fireTick) * Runner.DeltaTime;

        if (time <= 0f)
            return _firePosition;

        return _firePosition + _fireVelocity * time;
    }

    public static void OnDestroyChanged(Changed<PhysxBall> changed)
    {
    }
}