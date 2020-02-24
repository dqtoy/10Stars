using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Wolf : BaseAnimals
{
    public enum AnimalState
    {
        idle,
        trace,
        attack,
        regionHome,
        die
    };

    //FSM
    public AnimalState animalState = AnimalState.idle;
    public AnimalState originState;
    private NavMeshAgent nvAgent;
    private float dist;

    private float traceDist = 20.0f;
    private float attackDist = 5.0f;
    private float RegionDist = 40.0f;

    private Vector3 Home;
    private bool nowHomeRegion = false;
    private bool isDamaged = false;

    public GameObject player;
    private Animator anim;
    private AudioSource audiosource;

    public bool isInCircle = true;

    void Awake()
    {
        //BaseHp = 100;
        //test Hp
        BaseHp = 100;

        BaseMaxHp = BaseHp;
        BaseDamage = 1;
        BaseExp = 5;
        BaseIsDead = false;
    }

    void OnDisable()
    {
        GameManager.instance.animalSpawnCounter--;
    }

    void Start()
    {
        anim = this.GetComponent<Animator>();
        audiosource = this.GetComponent<AudioSource>();
        nvAgent = this.gameObject.GetComponent<NavMeshAgent>();
        player = GameManager.instance.mainPlayer;

        Home = this.transform.position;

        StartCoroutine(CheckAnimalState());
        StartCoroutine(AnimalAction());
    }

    void Update()
    {
        //죽으면 업데이트 하지 않음.
        if (BaseIsDead)
        {
            return;
        }

        if(!isInCircle)
        {
            this.GetComponent<Animator>().Play("Death");
            Destroy(this.gameObject, 1.3f);
        }
    }

    [PunRPC]
    public void ApplyUpdateState(AnimalState newState)
    {
        this.animalState = newState;
    }

    [PunRPC]
    public void ApplyUpdateHealth(float newHp, bool newIsDead)
    {
        BaseHp = newHp;
        BaseIsDead = newIsDead;
    }

    IEnumerator CheckAnimalState()
    {
        while (!BaseIsDead)
        {
            yield return new WaitForSeconds(0.2f);

            dist = Vector3.Distance(player.transform.position, this.transform.position);

            originState = animalState;

            if ((Vector3.Distance(this.transform.position, Home) > RegionDist) || (nowHomeRegion))
            {
                animalState = AnimalState.regionHome;
            }
            else if ((dist <= attackDist) && (isDamaged))
            {
                animalState = AnimalState.attack;
            }
            else if (isDamaged)
            {
                animalState = AnimalState.trace;
            }
            else if (!nowHomeRegion)
            {
                animalState = AnimalState.idle;
            }

            //위에서 저장했던 기존 상태와 지금상태가 다르다면 상태가 변경 된 것으로 Rpc함수 호출
            if (originState != animalState)
            {
                photonView.RPC("ApplyUpdateState", RpcTarget.All, animalState);
            }
        }
    }

    IEnumerator AnimalAction()
    {
        while (!BaseIsDead)
        {
            switch (animalState)
            {
                case AnimalState.idle:
                    anim.SetBool("isAttack", false);
                    anim.SetBool("isRunning", false);

                    break;

                case AnimalState.trace:
                    anim.SetBool("isAttack", false);
                    anim.SetBool("isRunning", true);

                    LookAt();

                    nvAgent.SetDestination(player.transform.position);
                    break;

                case AnimalState.regionHome:
                    anim.SetBool("isAttack", false);
                    anim.SetBool("isRunning", true);

                    isDamaged = false;
                    nowHomeRegion = true;

                    nvAgent.SetDestination(Home);
                    if (Vector3.Distance(this.transform.position, Home) <= nvAgent.stoppingDistance)
                    {
                        isDamaged = false;
                        nowHomeRegion = false;
                    }
                    break;

                case AnimalState.attack:
                    LookAt();

                    anim.SetBool("isRunning", false);
                    anim.SetBool("isAttack", true);
                    break;
            }
            yield return null;
        }
    }

    public void Attack()
    {
        if (photonView.IsMine)
        {
            //10 ~ 15 랜덤 데미지 주기
            BaseDamage = Random.Range(10, 16);
            //player.GetComponent<Player>().PlayerDamage(Mathf.RoundToInt(BaseDamage));
            player.GetPhotonView().RPC("PlayerDamage",RpcTarget.All , BaseDamage);

            //플레이어 사망시 다시 제자리로 돌아감
            if (player.GetComponent<Player>().hp <= 0)
            {
                nowHomeRegion = true;
            }
        }
    }

    void SoundHowl()
    {
        audiosource.Play();
    }

    public void AnimalsDamage(float inDamage, int attakPlayerViewId)
    {
        photonView.RPC("AnimalsHurt", RpcTarget.AllBuffered, inDamage, attakPlayerViewId);
    }

    [PunRPC]
    public void AnimalsHurt(float damage, int attakPlayerViewId)
    {
        BaseHp -= damage;
        player = PhotonView.Find(attakPlayerViewId).gameObject;

        Debug.Log(player + " + " + player.GetPhotonView().ViewID);

        BaseIsViewHpBar = true;
        BaseIsViewHpTime = 7.0f;
        isDamaged = true;   

        if (BaseHp <= 0)
        {
            BaseHp = 0;
            AnimalDie();
            BaseIsViewHpBar = false;
        }

        photonView.RPC("ApplyUpdateHealth", RpcTarget.All, BaseHp, BaseIsDead);
    }

    void AnimalDie()
    {
        StopAllCoroutines();
        Destroy(this.gameObject, 2.2f);
        BaseIsDead = true;



        SoundHowl();
        animalState = AnimalState.die;
        nvAgent.enabled = false;
        this.GetComponent<Animator>().Play("Death");
        player.GetComponent<Player>().PlusExp(BaseExp);
    }

    void LookAt()
    {
        Vector3 re = player.transform.position - this.transform.position;
        Quaternion rot = Quaternion.LookRotation(re);
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, rot, Time.deltaTime * 5f);
    }
}
