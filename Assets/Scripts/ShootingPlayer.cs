using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingPlayer : Player, IShooting
{
    [Header("Controls settings")]
    [SerializeField, Range(0f, 1f)] private float joystickShotMinDistance;
    [Header("Required Objects")]
    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform bulletSpawnPoint;

    private float shootingTimePassed = 0f;

    private bool activated = false;
    public bool Activated {
        [ClientRpc]
        set
        {
            activated = true;
            Camera.main.GetComponent<CameraController>().ChangeWaitingForPlayerValue(!value);
        }
    }

    #region SERVER
    
    [Command]
    private void CmdCreateBullet()
    {
        Bullet bullet = CreateBullet();
        NetworkServer.Spawn(bullet.gameObject);
    }
    
    #endregion
    
    #region CLIENT
    private Bullet CreateBullet()
    {
        Bullet _bullet = Instantiate(bullet, bulletSpawnPoint.position, bulletSpawnPoint.rotation).GetComponent<Bullet>();
        return _bullet;
    }

    [ClientCallback]
    public void Shoot()
    {
        shootingTimePassed += Time.deltaTime;
        if (shootingTimePassed >= 1f / bulletsPerSecond)
        {
            CmdCreateBullet();
            shootingTimePassed = 0f;
        }
    }
    #endregion

    protected override void Update()
    {
        if (isOwned)
        {
            if (Vector2.Distance(Vector2.zero, look.ReadValue<Vector2>()) >= joystickShotMinDistance)
            {
                Shoot();
            }
        }
        base.Update();
    }
}
