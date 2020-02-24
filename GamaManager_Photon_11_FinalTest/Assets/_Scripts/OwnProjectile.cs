using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class OwnProjectile : MonoBehaviourPun
{
    public GameObject ownPlayer;
    private ShootingScripts shootingScripts;

    public int damage;

    void Start()
    {
        ownPlayer = GameManager.instance.mainPlayer;
        shootingScripts = GameManager.instance.SC;
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((other.gameObject.tag != "Bullet") && (photonView.IsMine) && (other.gameObject != ownPlayer) && (other.gameObject.tag != "MagneticField"))
        {
            ownPlayer = GameManager.instance.mainPlayer;
            GameObject Explosion = PhotonNetwork.Instantiate(shootingScripts.Projectile[ownPlayer.GetComponent<Player>().projectileLv].Explosion.name, this.transform.position, Quaternion.identity);

            var otherPhotonView = other.gameObject.GetPhotonView();

            if (other.gameObject.tag == "Player" && !other.gameObject.GetComponent<Player>().playerIsDead)
            {
                otherPhotonView.RPC("PlayerDamage", otherPhotonView.Owner, damage);
            }
            else if(other.gameObject.tag == "Animals" && !other.gameObject.GetComponent<BaseAnimals>().BaseIsDead)
            {
                other.gameObject.GetComponent<Wolf>().AnimalsDamage(damage, ownPlayer.GetPhotonView().ViewID);
            }

            shootingScripts.TempObjectEX = Explosion;
            PhotonNetwork.Destroy(this.gameObject);
        }
    }
}
