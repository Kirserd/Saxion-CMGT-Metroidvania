using UnityEngine;
using System.Collections;

public enum CameraState
{
    PLAYER,
    TARGET,
    FREE
};

public enum CameraMovementType
{
    INTERPOLATION,
    LINEAR,
    TELEPORT
};

public class CameraMovement : MonoBehaviour
{

    #region MainFields
    public static CameraMovement Current { get; private set; }

    public CameraState State;
    public CameraMovementType MovementType;

    [Header("Params")]
    [SerializeField] private float _zoomMultiplier = 1;
    [SerializeField] private float _linearSpeed = 1;
    [SerializeField] private float _interpolateSpeed = 8f;
    [SerializeField] private Vector2 _offset = Vector2.zero;

    [Header("Optional")]
    [SerializeField] private Transform _target;
    [SerializeField] private Transform _player;

    private const sbyte CONSTANT_RANGE = -10;

    public Transform Target
    {
        get => _target;
        set
        {
            if (value != null)
                _target = value;
        }
    }
    public Transform Player
    {
        get => _player;
        set
        {
            if (value != null)
                _player = value;
        }
    }

    public float ZoomMultiplier
    {
        get => _zoomMultiplier;
        set
        {
            if (value > 0)
                _zoomMultiplier = value;
            else
                Debug.LogAssertion("Zoom multiplier can not be less or equal to 0");
        }
    }
    public float LinearSpeed
    {
        get => _linearSpeed;
        set
        {
            if (value > 0)
                _linearSpeed = value;
            else
                Debug.LogAssertion("Linear speed can not be less equal to 0");
        }
    }
    public float InterpolateSpeed
    {
        get
        {
            return _interpolateSpeed;
        }
        set
        {
            if (value > 0)
                _interpolateSpeed = value;
            else
                Debug.LogAssertion("Interpolation speed can not be less or equal to 0");
        }
    }

    #endregion

    #region Subscriptions
    private void Awake() => Current = this;
    #endregion

    private void Start() => RefreshPlayerLink();
    private void RefreshPlayerLink() => Player = GameObject.FindGameObjectWithTag("Player").transform;

    private void LateUpdate()
    {
        switch (State)
        {
            case CameraState.PLAYER:
                Move(Player.position);
                break;
            case CameraState.TARGET:
                Move(Target.position);
                break;
            default:
                break;
        }
    }
    private void Move(Vector2 Destination)
    {
        switch (MovementType)
        {
            case CameraMovementType.INTERPOLATION:
                Interpolate();
                break;
            case CameraMovementType.LINEAR:
                Linear();
                break;
            default:
                Teleport();
                break;
        }

        void Interpolate()
        {
            Vector2 InterpolatedPosition = Vector2.Lerp(transform.position, Destination + _offset, InterpolateSpeed * Time.deltaTime);
            Vector3 NewPosition = new(InterpolatedPosition.x, InterpolatedPosition.y, CONSTANT_RANGE * _zoomMultiplier);
            transform.position = NewPosition;
        }

        void Linear() => transform.position += LinearSpeed *
            new Vector3
            (
                Destination.x + _offset.x - transform.position.x,
                Destination.y + _offset.y - transform.position.y,
                0
            ).normalized;

        void Teleport() => transform.position = new Vector3
            (
                Destination.x + _offset.x,
                Destination.y + _offset.y,
                CONSTANT_RANGE * _zoomMultiplier
            );
    }

    public void Shake(float amount = 0.5f, float time = 0.5f, float frequency = 0.04f) => StartCoroutine(Shaker(amount, time, frequency));
    private IEnumerator Shaker(float amount, float time, float frequency)
    {
        float timer = 0;
        while (time != 0 ? timer < time : amount > 0.01f)
        {
            transform.position += new Vector3(Random.Range(-amount, amount), Random.Range(-amount, amount), 0);
            yield return new WaitForSeconds(frequency);
            amount *= 0.8f;
            timer += frequency;
        }
    }
}
