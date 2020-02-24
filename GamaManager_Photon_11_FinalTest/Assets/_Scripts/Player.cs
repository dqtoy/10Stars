using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using Photon.Pun;

public class Player : MonoBehaviourPun
{
    public float hp;
    public float maxHp;
    public float speed;
    public float Exp;
    public float maxExp;

    public bool playerIsDead;
    public GameObject attackPlayer;

    public int lv;
    private const int maxLv = 5;
    int[] hpArray = new int[maxLv] { 0, 100, 130, 150, 200 };

    public int projectileLv;

    public Animator hitUiAnim;
    public Text hitText;
    public Camera mainCamera;
    private Animator anim;

    public NavMeshAgent nv;
    public GameObject wp;
    public GameObject lvUpEffect;

    //AutoHeal
    public float autoHealthyCoolTime = 5.0f;
    public bool isNowAutoHealthy = false;
    private bool isHealCoroutineRunning = false;

    // 슬라이더 hp
    public Slider healthSlider;
    public GameObject healOnce;

    // Die
    public Text playerInfoText;
    public SpawnManager spawnManager;
    public SkinnedMeshRenderer[] playerMesh = new SkinnedMeshRenderer[4];
    public GameObject BulletImg;
    public Image SliderImg;

    // MagneticZone
    public bool isInCircle = true;
    public float inCircleCoolTime = 1.0f;
    public Light mainLight;

    void Awake()
    {
        lv = 1;
        Exp = 0;
        maxExp = 4;
        maxHp = 100f;
        speed = 10f;

        SoundManager.instance.PlaySFX("start" + Random.Range(1, 8).ToString());
    }

    void Start()
    {
        mainCamera = Camera.main;
        anim = this.GetComponent<Animator>();
        nv = this.GetComponent<NavMeshAgent>();
        SliderHpUpdate();

        playerInfoText = GameObject.Find("PlayerInfoText").GetComponent<Text>();
        spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        mainLight = GameObject.Find("Light").GetComponent<Light>();
        playerInfoText.enabled = false;

        SpawnedPlayer();
    }


    void SpawnedPlayer()
    {
        playerIsDead = false;
        hp = maxHp;

        anim.SetBool("Die", false);

        photonView.RPC("ApplyUpdateHealth", RpcTarget.Others, this.hp, this.maxHp, playerIsDead);

        if (!photonView.IsMine)
        {
            healthSlider.gameObject.SetActive(true);
            healthSlider.value = hp / maxHp;
        }

        MoveJoystick.instance.enabled = true;
        ShootJoystick.instance.enabled = true;
    }

    void Update()
    {
        //죽지 않았을 경우에만
        if (!playerIsDead && photonView.IsMine)
        {
            photonView.RPC("SliderHpUpdate", RpcTarget.Others);
            CheckInCircle();
            AutoHeal();
        } 
        else if (playerIsDead && photonView.IsMine)
        {
            photonView.RPC("SliderHpUpdate", RpcTarget.Others);
            MoveJoystick.instance.enabled = false;
            ShootJoystick.instance.enabled = false;
        }
    }

    #region 체력, 슬라이더, 경치
    [PunRPC]
    public void SliderHpUpdate()
    {
        if (photonView.IsMine)
        {
            healthSlider.gameObject.SetActive(false);
        }
        else
        {
            healthSlider.value = Mathf.Lerp(healthSlider.value, hp / maxHp, 0.1f);
        }
    }

    [PunRPC]
    public void ApplyUpdateHealth(float newHealth, float newMaxHp, bool newDead)
    {
        maxHp = newMaxHp;
        hp = newHealth;
        playerIsDead = newDead;
    }

    [PunRPC]
    public void ApplyUpdateExp(float newExp, int newLv)
    {
        Exp = newExp;
        lv = newLv;
    }

    public void PlusExp(float exp)
    {
        float temp = this.Exp + exp;

        if (temp >= maxExp)
        {
            this.lv++;
            SoundManager.instance.PlaySFX("lvUp");

            maxHp = hpArray[lv];
            TenPercentHpUp();

            photonView.RPC("LvUpParticleRPC", RpcTarget.All, null);

            if (this.lv > maxLv)
            {
                this.lv = maxLv;
                this.Exp = maxExp;
            }
            this.Exp = temp - maxExp;

            photonView.RPC("ApplyUpdateHealth", RpcTarget.Others, this.hp, this.maxHp, playerIsDead);
            photonView.RPC("ApplyUpdateExp", RpcTarget.Others, this.Exp, this.lv);
        }
        else
        {
            this.Exp += exp;
            photonView.RPC("ApplyUpdateExp", RpcTarget.Others, this.Exp, this.lv);
        }
    }

    [PunRPC]
    public void LvUpParticleRPC()
    {
        GameObject lvUpParticle = Instantiate(lvUpEffect, this.transform.position, Quaternion.LookRotation(this.transform.up));
        Destroy(lvUpParticle, 1.0f);
    }

    public void CheckInCircle()
    {
        if (isInCircle)
        {
            inCircleCoolTime = 1.0f;
            mainLight.color = Color.Lerp(mainLight.color, Color.white, 0.1f);
        }
        else
        {
            inCircleCoolTime -= Time.deltaTime;
            mainLight.color = Color.Lerp(mainLight.color, Color.red, 0.1f);

            if (inCircleCoolTime <= 0)
            {
                inCircleCoolTime = 1.0f;
                hp -= (int)maxHp / 10;
                SoundManager.instance.PlaySFX("hurt" + Random.Range(1, 6).ToString());

                if (photonView.IsMine)
                {
                    mainCamera.GetComponent<IsometricCamera>().SoftShakeCam();
                    mainCamera.GetComponent<IsometricCamera>().startBloodScreen();
                }

                autoHealtReset();

                if (this.hp <= 0)
                {
                    SoundManager.instance.PlaySFX("death" + Random.Range(1, 5).ToString());
                    this.hp = 0;
                    playerIsDead = true;
                    anim.SetBool("Die", true);
                    this.gameObject.GetComponent<CapsuleCollider>().enabled = false;

                    if (GameManager.instance.startMagneticFieldShrink && photonView.IsMine)
                    {
                        photonView.RPC("YouWinOtherRPC", RpcTarget.Others, null);

                        playerInfoText.enabled = true;
                        playerInfoText.color = Color.red;
                        playerInfoText.text = "패배!";

                        StartCoroutine("DelayResult");
                    }
                    else if (photonView.IsMine)
                    {
                        StartCoroutine("PlayerRespawn");
                    }
                }

                photonView.RPC("ApplyUpdateHealth", RpcTarget.Others, this.hp, this.maxHp, playerIsDead);
                photonView.RPC("Hurt", RpcTarget.All, (int)maxHp / 10);
            }
        }
    }
    #endregion

    //데미지 처리 즉 Hurt를 전체 실행시키기 위한 함수
    [PunRPC]
    public void PlayerDamage(int inDamage)
    {
        autoHealtReset();

        this.hp -= inDamage;
        SoundManager.instance.PlaySFX("hurt" + Random.Range(1, 6).ToString());

        if (photonView.IsMine)
        {
            mainCamera.GetComponent<IsometricCamera>().SoftShakeCam();
            mainCamera.GetComponent<IsometricCamera>().startBloodScreen();
        }

        if (this.hp <= 0)
        {
            SoundManager.instance.PlaySFX("death" + Random.Range(1, 5).ToString());
            this.hp = 0;
            playerIsDead = true;
            anim.SetBool("Die", true);
            this.gameObject.GetComponent<CapsuleCollider>().enabled = false;

            if (GameManager.instance.startMagneticFieldShrink && photonView.IsMine)
            {
                photonView.RPC("YouWinOtherRPC", RpcTarget.Others, null);

                playerInfoText.enabled = true;
                playerInfoText.color = Color.red;
                playerInfoText.text = "패배!";

                StartCoroutine("DelayResult");
            }
            else if (photonView.IsMine) 
            {
                StartCoroutine("PlayerRespawn");
            }
        }

        photonView.RPC("ApplyUpdateHealth", RpcTarget.Others, this.hp, this.maxHp, playerIsDead);
        photonView.RPC("Hurt", RpcTarget.All, inDamage);
    }

    IEnumerator DelayResult()
    {
        yield return new WaitForSeconds(2.0f);
        GameManager.instance.FadeIn();

        yield return new WaitForSeconds(3.0f);
        GameManager.instance.textRunning = false;
        playerInfoText.enabled = false;
        photonView.RPC("NowChangeRPC", RpcTarget.All, null);
    }

    [PunRPC]
    public void YouWinOtherRPC()
    {
        GameManager.instance.isWin = true;
    }

    [PunRPC]
    public void NowChangeRPC()
    {
        GameManager.instance.NowChangeScene = true;
    }

    [PunRPC]
    public void Hurt(int damage)
    {
        hitText.text = "-" + damage.ToString();
        hitUiAnim.SetTrigger("Hit");
    }

    IEnumerator PlayerRespawn()
    {
        playerInfoText.enabled = true;

        playerInfoText.text = "플레이어 사망!";
        yield return new WaitForSeconds(2.0f);
        GameManager.instance.FadeIn();

        playerInfoText.text = "잠시 후 살아납니다!";
        yield return new WaitForSeconds(3.5f);

        photonView.RPC("PlayerSetActiveFalse", RpcTarget.All);

        // PositionSetting
        if (Random.value >= 0.5f) this.gameObject.transform.position = spawnManager.wayPoint1[Random.Range(0, 2)].transform.position;
        else this.gameObject.transform.position = spawnManager.wayPoint2[Random.Range(0, 2)].transform.position;

        playerInfoText.text = "3!";
        yield return new WaitForSeconds(1.0f);
        playerInfoText.text = "2!";
        yield return new WaitForSeconds(1.0f);

        SoundManager.instance.PlaySFX("Respawn");

        playerInfoText.text = "1!";
        yield return new WaitForSeconds(1.0f);

        photonView.RPC("PlayerSetActiveTrue", RpcTarget.All);


        SpawnedPlayer();
        GameManager.instance.FadeOut();
        playerInfoText.enabled = false;
    }

    #region 잡동사니

    [PunRPC]
    public void PlayerSetActiveTrue()
    {
        BulletImg.SetActive(true);
        healthSlider.enabled = true;
        SliderImg.enabled = true;
        this.gameObject.GetComponent<CapsuleCollider>().enabled = true;

        for (int i = 0; i < playerMesh.Length; i++)
        {
            playerMesh[i].enabled = true;
        }
    }

    [PunRPC]
    public void PlayerSetActiveFalse()
    {
        BulletImg.SetActive(false);
        healthSlider.enabled = false;
        SliderImg.enabled = false;

        for (int i = 0; i < playerMesh.Length; i++)
        {
            playerMesh[i].enabled = false;
        }
    }

    public void TenPercentHpUp()
    {
        hp += Mathf.Round(maxHp / 10);

        if (hp > maxHp)
        {
            hp = maxHp;
        }
    }

    public void ChangeProjcetileLv()
    {
        projectileLv = lv - 1;
    }

    public void RegionControlRotation()
    {
        MoveJoystick.instance.isShootingRotationControl = false;
    }

    public void AutoHeal()
    {
        if (hp != maxHp)
        {
            autoHealthyCoolTime -= Time.deltaTime;
        }
        else
        {
            autoHealthyCoolTime = 5.0f;
            return;
        }

        if (autoHealthyCoolTime < 0) autoHealthyCoolTime = 0;

        if (GameManager.instance.isNowAttack)
        {
            autoHealtReset();
        }

        if ((hp != maxHp) && (!isHealCoroutineRunning))
        {
            isNowAutoHealthy = false;
        }

        if ((autoHealthyCoolTime == 0) && (isNowAutoHealthy == false))
        {
            isNowAutoHealthy = true;
            autoHealthyCoolTime = 0;
            StartCoroutine("AutoHealty");
        }
    }

    //autoHealthyCoolTime Reset
    private void autoHealtReset()
    {
        autoHealthyCoolTime = 5.0f;
        StopCoroutine("AutoHealty");
        isHealCoroutineRunning = false;
    }

    IEnumerator AutoHealty()
    {
        isHealCoroutineRunning = true;

        while (true)
        {
            SoundManager.instance.PlaySFX("autoHeal");
            photonView.RPC("HealParticleRPC", RpcTarget.All, null);
            hp += Mathf.Round(maxHp / 20);

            // 체력올라갈떄 동기화
            photonView.RPC("ApplyUpdateHealth", RpcTarget.Others, this.hp, this.maxHp, playerIsDead);

            if (hp >= maxHp)
            {
                hp = maxHp;
                break;
            }
            yield return new WaitForSeconds(1.0f);
        }
        isHealCoroutineRunning = false;
    }

    [PunRPC]
    public void HealParticleRPC()
    {
        GameObject healParticle = Instantiate(healOnce, this.transform.position, Quaternion.Euler(-90, 0, 0));
        Destroy(healParticle, 0.7f);
    }
    #endregion
}
