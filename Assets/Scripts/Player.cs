using Fusion;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Player : NetworkBehaviour
{
    [SerializeField]
    private Ball _prefabBall;

    [SerializeField]
    private PhysxBall _prefabPhysxBall;

    [Networked]
    private TickTimer Delay { get; set; }

    [Networked(OnChanged = nameof(OnBallSpawned))]
    public new NetworkBool Spawned { get; set; }

    private NetworkCharacterControllerPrototype _cc;
    private Vector3 _forward;
    private Vector3 _up;

    private Material _material;

    private Text _messages;

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    public void RPC_SendMessage(string message, RpcInfo info = default)
    {
        if (_messages == null)
            _messages = FindObjectOfType<Text>();
        if (info.IsInvokeLocal)
            message = $"You said: {message}\n";
        else
            message = $"Some other player said: {message}\n";
        _messages.text += message;
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

    private void Awake()
    {
        _cc = GetComponent<NetworkCharacterControllerPrototype>();
        _forward = transform.forward;
    }

    public void Update()
    {
        if (Object.HasInputAuthority && Input.GetKeyDown(KeyCode.R))
        {
            RPC_SendMessage("Hey Mate!");
        }

        if(Object.HasInputAuthority && Input.GetKeyDown(KeyCode.H))
        {
            RPC_SendMessage("Hello");
        }

        if(Object.HasInputAuthority && Input.GetKeyDown(KeyCode.B))
        {
            RPC_SendMessage("Bye");
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            data.direction.Normalize();
            _cc.Move(5 * data.direction * Runner.DeltaTime);

            if (data.direction.sqrMagnitude > 0)
                _forward = data.direction;

            if (Delay.ExpiredOrNotRunning(Runner))
            {
                /*if ((data.buttons & NetworkInputData.MOUSEBUTTON1) != 0)
                {
                    Delay = TickTimer.CreateFromSeconds(Runner, 0.2f);
                    Runner.Spawn(_prefabBall,
                    transform.position + _forward, Quaternion.LookRotation(_forward),
                    Object.InputAuthority, (runner, o) =>
                    {
                        o.GetComponent<Ball>().Init();
                        Spawned = !Spawned;
                        Debug.Log(_forward);
                    });
                }*/

                if ((data.buttons & NetworkInputData.MOUSEBUTTON2) != 0)
                {
                    Delay = TickTimer.CreateFromSeconds(Runner, 0.5f);
                    Runner.Spawn(_prefabPhysxBall,
                      transform.position + _forward,
                      Quaternion.LookRotation(_forward),
                      Object.InputAuthority,
                      (runner, o) =>
                      {
                          o.GetComponent<PhysxBall>().Init(10 * _forward);
                          Spawned = !Spawned;
                      });
                }
            }
        }
    }

    public static void OnBallSpawned(Changed<Player> changed)
    {
        changed.Behaviour.material.color = Color.white;
    }

    public override void Render()
    {
        material.color = Color.Lerp(material.color, Color.blue, Time.deltaTime);
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public void OnCollisionEnter(Collision collision)
    {
        Debug.Log("a");
        Runner.Despawn(Object);
        int count = 0; 
        if (count < 5)
        {
            count++;
            Debug.Log("hit");
        }
        else if (5 < count)
        {
            Runner.Despawn(Object);
        }
    }
}