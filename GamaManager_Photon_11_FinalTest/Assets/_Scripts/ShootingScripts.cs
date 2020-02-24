using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ShootingScripts : MonoBehaviourPun
{
    public List<DetaileProjectile> Projectile = new List<DetaileProjectile>();
    public GameObject TempObjectEX = null;
    private Player player;
    float bulletSpeed = 40f;

    void Start()
    {
        player = GameManager.instance.mainPlayer.GetComponent<Player>();
    }

    void Update()
    {
        if(TempObjectEX)
        {
            StartCoroutine(DestroyAfter(TempObjectEX, 1.5f));
            TempObjectEX = null;
        }
    }

    public void Shooting()
    {
        player.ChangeProjcetileLv();

        GameObject Muzzle = PhotonNetwork.Instantiate(Projectile[player.projectileLv].Muzzle.name, new Vector3(this.transform.position.x, this.transform.position.y - 0.5f, this.transform.position.z), Quaternion.Euler(0f, ShootJoystick.instance.shootingRotation, 0f));
        StartCoroutine(DestroyAfter(Muzzle, 1.5f));
        //GameObject Muzzle = Instantiate(Projectile[player.projectileLv].Muzzle, new Vector3(this.transform.position.x, this.transform.position.y - 0.5f, this.transform.position.z), Quaternion.Euler(0f, ShootJoystick.instance.shootingRotation, 0f));

        //GameObject Missile = Instantiate(Projectile[player.projectileLv].Missile, new Vector3(this.transform.position.x, this.transform.position.y - 0.5f, this.transform.position.z), Quaternion.Euler(0f, ShootJoystick.instance.shootingRotation, 0f));
        GameObject Missile = PhotonNetwork.Instantiate(Projectile[player.projectileLv].Missile.name, new Vector3(this.transform.position.x, this.transform.position.y - 0.5f, this.transform.position.z), Quaternion.Euler(0f, ShootJoystick.instance.shootingRotation, 0f));
        Missile.transform.GetComponent<Rigidbody>().AddForce(Missile.transform.forward * bulletSpeed, ForceMode.Impulse);
        StartCoroutine(DestroyAfter(Missile, 1.5f));
    }

    IEnumerator DestroyAfter(GameObject target, float delay)
    {
        yield return new WaitForSeconds(delay);

        if(target != null)
        {
            PhotonNetwork.Destroy(target);
        }
    }
}

[System.Serializable]
public class DetaileProjectile
{
    public GameObject Explosion;
    public GameObject Missile;
    public GameObject Muzzle;
}
