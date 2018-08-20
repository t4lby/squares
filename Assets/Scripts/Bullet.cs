using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
    public float Lifetime { get; set; }

    private float _InstantiatedAt;

    private void Awake()
    {
        Lifetime = 2f;
    }

    private void Start()
    {
        _InstantiatedAt = Time.time;
    }

    private void Update()
    {
        if (_InstantiatedAt + Lifetime < Time.time)
        {
            Destroy(this.gameObject);
        }
    }
}
