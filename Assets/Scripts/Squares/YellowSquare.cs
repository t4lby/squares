using UnityEngine;
using System.Collections;
using System;
using System.Threading;

public class YellowSquare : Square
{
    public float BulletForce { get; set; }

    public float BuildBulletDiff { get; set; }

    /// <summary>
    /// Tracks if triggered is held.
    /// </summary>
    private bool _TriggerHeld;
    private float _NextBuildBulletTime;

    protected override void SetSquareProperties()
    {
        this.Health = 1f;
        this.Durability = 7.5f;
        this.Color = SquareType.Yellow;
        this.MinimumTransparency = 0.25f;
        this.RegenerationSpeed = 0.2f;
        this.BulletForce = 1000f;
        this.BuildBulletDiff = 0.5f;
    }

    private void Start()
    {
        _TriggerHeld = false;
        _NextBuildBulletTime = Time.time;
    }

    protected override void UpdateSquare()
    {
        if (Triggered &&
            _TriggerHeld == false)
        {
            var angle = this.transform.rotation.eulerAngles.z * Mathf.PI / 180;
            var bulletDirection = new Vector3(-Mathf.Sin(angle),
                                             Mathf.Cos(angle),
                                              0).normalized;
            var bullet = Factory.SpawnBullet(transform.position + Game.SquareSize * bulletDirection,
                                bulletDirection * BulletForce, this.Color);
            if (IsBuildSquare)
            {
                Destroy(bullet.GetComponent<Collider>());
                _NextBuildBulletTime = Time.time + BuildBulletDiff;
            }
            else
            {
                this.GetComponent<Rigidbody>().AddForce(-bulletDirection * BulletForce);
            }
            _TriggerHeld = true;
        }
        if (IsBuildSquare &&
                Snapped &&
                _NextBuildBulletTime < Time.time
            || !Triggered)
        {
            _TriggerHeld = false;

        }
    }
}
