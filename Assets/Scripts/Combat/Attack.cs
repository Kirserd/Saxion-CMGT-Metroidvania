using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    private HashSet<GameObject> _damageMask = new();
    public delegate void OnAttackLandedHandler(float zAxisRotation);
    public OnAttackLandedHandler OnAttackLanded;

    [SerializeField]
    private float _lifeTime = 0.4f;
    private float _lifeTimeCounter = 0f;

    [SerializeField]
    private float _damageAmount = 1f;
    public float DamageAmount 
    { 
        get => _damageAmount;
        set => _damageAmount = value;
    }
    public void AddDamageMask(GameObject other) => _damageMask.Add(other);
    public void AddDamageMasks(GameObject[] others)
    {
        foreach (var other in others)
            _damageMask.Add(other);
    }
    public bool IsDealingDamageTo(GameObject other) => !_damageMask.Contains(other);

    private void Update()
    {
        _lifeTimeCounter += Time.deltaTime;
        if (_lifeTimeCounter >= _lifeTime)
            Destroy(gameObject);
    }
}
