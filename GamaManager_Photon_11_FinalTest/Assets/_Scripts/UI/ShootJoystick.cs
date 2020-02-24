using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Photon.Pun;

public class ShootJoystick : MonoBehaviourPun, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    // singleton
    public static ShootJoystick instance;

    [SerializeField] private RectTransform rect_Background;
    [SerializeField] private RectTransform rect_Joystick;

    public bool isTouch = false;

    private float radius;
    public Vector2 value { get; private set; }
    public float shootingRotation { get; private set; }
    public float distance;
    public Quaternion spin { get; private set; }

    public Image shootingCounterImage;

    public float shootingCounts = 3;
    public float countCoolTime = 0f;
    public float shootingCoolTime;
    public int maxBullets = 3;

    private float posibleCoolTime = 0f;
    public bool isPossibleShooting;
    private float shootingMinCoolTime = 0.5f;

    IsometricCamera mainCameraIsometricScript;

    void OnDisable()
    {
        rect_Joystick.localPosition = Vector3.zero;
        isTouch = false;
    }

    void Awake()
    {
        if (instance == null) instance = this;
        else { Destroy(this.gameObject); }
    }

    void Start()
    {
        // 백그라운드의 반지름
        radius = rect_Background.rect.width * 0.5f;

        shootingCoolTime = 1.75f;

        mainCameraIsometricScript = GameObject.Find("Main Camera").GetComponent<IsometricCamera>();
    }

    void Update()
    {
        // 로컬플레이어일 경우에만
        if(!GameManager.instance.mainPlayer.GetPhotonView().IsMine)
        {
            return;
        }

        shootingCoolTimeReset();
        ShootingCountReset();
    }

    public void ShootingCountReset()
    {
        if(shootingCounts < maxBullets && isPossibleShooting)
        {
            countCoolTime += Time.deltaTime;

            if (countCoolTime > shootingCoolTime)
            {
                ++shootingCounts;
                SoundManager.instance.PlaySFX("reload");
                countCoolTime = 0.0f;
            }
        }
    }

    public void shootingCoolTimeReset()
    {
        if (posibleCoolTime < shootingMinCoolTime)
        {
            isPossibleShooting = false;
            posibleCoolTime += Time.deltaTime;

            this.GetComponent<Image>().fillAmount = posibleCoolTime * 2.0f;
        }
        else
        {
            isPossibleShooting = true;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        value = eventData.position - (Vector2)rect_Background.position;

        // 원밖으로 나갈 경우 그 백그라운드에 가두어야 함
        value = Vector2.ClampMagnitude(value, radius);

        // 캐릭터 회전
        shootingRotation = Mathf.Atan2(value.x, value.y);
        shootingRotation *= Mathf.Rad2Deg;

        //부모 객체에 대한 상대적인 좌표
        rect_Joystick.localPosition = value;

        //카메라 바뀌었을때만 변경시켜 줌
        if (mainCameraIsometricScript.isRotateCamera)
        {
            shootingRotation -= 180.0f;
            value = -value;
        }

        spin = Quaternion.AngleAxis(shootingRotation, Vector3.up);
        distance = Vector2.Distance(Vector2.zero, value) / radius;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        distance = 0.0f;
        isTouch = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if ((isPossibleShooting == true) && (shootingCounts > 0))
        {
            //일정거리이상 조이스틱을 당겨야만 발사 됨
            if (distance > 0.2f)
            {
                posibleCoolTime = 0.0f;
                shootingCounts--;

                GameManager.instance.isAttack();
                MoveJoystick.instance.isShootingRotationControl = true;
            }
        }
        rect_Joystick.localPosition = Vector3.zero;
        isTouch = false;
    }
}
