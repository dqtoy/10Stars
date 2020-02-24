using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager instance;

    public GameObject mainPlayer;
    public GameObject enemyPlayer;
    public GameObject[] players;

    public ShootingScripts SC;

    public float coolTime = 0.1f;
    public float NowTime = 0f;
    //autoHealCheck 용도
    public bool isNowAttack = false;

    //EndMainScene
    public int animalSpawnCounter;
    public Image fadeInOut;
    private Coroutine FadeInOutCoroutine = null;
    public GameObject magneticField;
    public Text InfoText;
    public bool textRunning = false;
    public float shrinkingSize;
    public float minimumShrinkingSize;
    public bool startMagneticFieldShrink = false;
    public bool isWin;
    public bool NowChangeScene;

    void Awake()
    {
        if (instance == null) instance = this;
        else { Destroy(this.gameObject); }

        //Screen.SetResolution(1920, 1080, true);
        PhotonNetwork.SendRate = 30;
        PhotonNetwork.SerializationRate = 20;
        PhotonNetwork.AutomaticallySyncScene = false;
    }

    void Start()
    {
        SoundManager.instance.PlayBGM("Fight");
        SoundManager.instance.PlaySFX("FightIntro");

        SC = mainPlayer.GetComponentInChildren<ShootingScripts>();
        FadeOut();
        photonView.RPC("MagneticFieldSetActiveRPC", RpcTarget.All, false);

        StartCoroutine("CheckPlayerCount");
        animalSpawnCounter = GameObject.FindGameObjectsWithTag("Animals").Length;
        InfoText.enabled = false;

        if(PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("ShrinkInfoTextStart", RpcTarget.All, null);
        }
    }

    [PunRPC]
    public void EnemyPlayerSetting()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            enemyPlayer = GameObject.Find("Player2(Clone)");
        }
        else
        {
            enemyPlayer = GameObject.Find("Player(Clone)");
        }
    }

    [PunRPC]
    public void ShrinkInfoTextStart()
    {
        StartCoroutine("ShrinkInfoText");
    }

    IEnumerator ShrinkInfoText()
    {
        textRunning = true;
        InfoText.enabled = true;
        InfoText.color = Color.blue;

        for (int i = 90; i >= 0; i--)
        {
            InfoText.text = "파밍시간 : " + i.ToString() + "초";
            yield return new WaitForSeconds(1.0f);
        }
        photonView.RPC("MagneticFieldSetActiveRPC", RpcTarget.All, true);
        InfoText.enabled = false;

        mainPlayer.GetComponent<Player>().playerInfoText.enabled = true;
        mainPlayer.GetComponent<Player>().playerInfoText.color = Color.cyan;

        mainPlayer.GetComponent<Player>().playerInfoText.text = "자기장 생성!";
        SoundManager.instance.PlayBGM("OverTime");
        SoundManager.instance.PlaySFX("overTime");
        yield return new WaitForSeconds(2.0f);
        mainPlayer.GetComponent<Player>().playerInfoText.text = "사망 시 패배!";
        yield return new WaitForSeconds(2.0f);
        mainPlayer.GetComponent<Player>().playerInfoText.enabled = false;

        startMagneticFieldShrink = true;
        textRunning = false;
    }

    void Update()
    {
        //적 플레이어가 세팅 되어 있지 않을때
        if (!enemyPlayer) EnemyPlayerSetting();

        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Main"))
        {
            if (PhotonNetwork.IsMasterClient && magneticField.transform.localScale.x >= minimumShrinkingSize && photonView.IsMine && startMagneticFieldShrink)
            {
                magneticField.transform.localScale -= new Vector3(shrinkingSize * Time.deltaTime, shrinkingSize * Time.deltaTime, shrinkingSize * Time.deltaTime);
            }
        }

        if(isWin && !textRunning)
        {
            StartCoroutine("WinInfoText");
        }

        if(NowChangeScene)
        {
            if (isWin)
            {
                SceneManager.LoadScene("Win");
            }
            else
            {
                SceneManager.LoadScene("Lose");
            }
        }
    }

    IEnumerator WinInfoText()
    {
        textRunning = true;
        mainPlayer.GetComponent<Player>().playerInfoText.enabled = true;
        mainPlayer.GetComponent<Player>().playerInfoText.color = Color.green;

        SoundManager.instance.PlaySFX("kill" + Random.Range(1, 5).ToString());

        mainPlayer.GetComponent<Player>().playerInfoText.text = "상대방 사망!";
        yield return new WaitForSeconds(2.5f);
        mainPlayer.GetComponent<Player>().playerInfoText.text = "승리!";
        yield return new WaitForSeconds(2.0f);
        mainPlayer.GetComponent<Player>().playerInfoText.enabled = false;
        //textRunning = false;
    }

    [PunRPC]
    public void MagneticFieldSetActiveRPC(bool QA)
    {
        magneticField.SetActive(QA);
    }

    [PunRPC]
    public void PathRPC(int x, int y, int z)
    {
        mainPlayer.GetComponent<NavMeshAgent>().SetDestination(new Vector3(x, y, z));
    }

    public void isDie()
    {
        mainPlayer.GetComponent<Animator>().SetTrigger("Die");
    }

    public void isAttack()
    {
        // 로컬플레이어일 경우에만
        if (!mainPlayer.GetPhotonView().IsMine) return;

        if(isNowAttack) SoundManager.instance.PlaySFX("noAmmo");

        isNowAttack = true;
        mainPlayer.GetComponent<Animator>().SetTrigger("Attack");

        Invoke("NowShooting", 0.2f);

        NowTime = coolTime;
    }

    void NowShooting()
    {
        SC.Shooting();
        isNowAttack = false;
    }

    public void FadeOut()
    {
        if (FadeInOutCoroutine == null) FadeInOutCoroutine = StartCoroutine("FadeOutCoroutine");
    }

    public void FadeIn()
    {
        if (FadeInOutCoroutine == null) FadeInOutCoroutine = StartCoroutine("FadeInCoroutine");
    }

    IEnumerator FadeOutCoroutine()
    {
        fadeInOut.enabled = true;
        fadeInOut.color = Color.black;

        yield return null;
        while (true)
        {
            fadeInOut.color = new Color(fadeInOut.color.r, fadeInOut.color.g, fadeInOut.color.b, fadeInOut.color.a - 0.03f);

            if (fadeInOut.color.a <= 0.0f)
            {
                fadeInOut.color = Color.black;
                break;
            }
            yield return new WaitForSeconds(0.05f);
        }
        FadeInOutCoroutine = null;
        fadeInOut.enabled = false;
    }

    IEnumerator FadeInCoroutine()
    {
        fadeInOut.enabled = true;
        fadeInOut.color = new Color(Color.black.r, Color.black.g, Color.black.b, 0);

        yield return null;

        while (true)
        {
            fadeInOut.color = new Color(fadeInOut.color.r, fadeInOut.color.g, fadeInOut.color.b, fadeInOut.color.a + 0.03f);

            Debug.Log(fadeInOut.color.a);
            if (fadeInOut.color.a >= 1.0f)
            {
                break;
            }
            yield return new WaitForSeconds(0.1f);
        }
        FadeInOutCoroutine = null;
        //fadeInOut.enabled = false;
    }

    IEnumerator CheckPlayerCount()
    {
        while (true)
        {
            if(PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {
                Debug.Log("상대방이 나갔습니다.");
                magneticField.SetActive(true);
                break;  
            }
            yield return new WaitForSeconds(1.0f);
        }
    }
}
