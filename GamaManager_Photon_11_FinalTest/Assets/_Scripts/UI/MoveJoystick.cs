using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.EventSystems;

public class MoveJoystick : MonoBehaviourPun, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    // singleton
    public static MoveJoystick instance;

    [SerializeField] private RectTransform rect_Background;
    [SerializeField] private RectTransform rect_Joystick;
    public GameObject go_Player;

    // 플레이어 스크립트를 가져옴
    Player PlayerScript;
    IsometricCamera PlayerCamera;

    private float radius;
    private bool isTouch = false;
    public bool isShootingRotationControl = false;

    // 이동관련 변수
    public Vector2 value { get; private set; }
    private Vector3 movePosition;
    private float distance;

    // 회전관련 변수
    public float moveLookRotation { get; private set; }
    private Quaternion spin;

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

        PlayerScript = GameManager.instance.mainPlayer.GetComponent<Player>();
        PlayerCamera = GameObject.Find("Main Camera").GetComponent<IsometricCamera>();
        go_Player = GameManager.instance.mainPlayer;
    }

    void FixedUpdate()
    {
        // 로컬플레이어일 경우에만
        if (!GameManager.instance.mainPlayer.GetPhotonView().IsMine)
        {
            return;
        }


        if (isTouch)
        {
            go_Player.transform.position += movePosition;
        }

        if (!isShootingRotationControl)
        {
            go_Player.transform.rotation = Quaternion.Slerp(go_Player.transform.rotation, spin, 0.2f);

            go_Player.GetComponent<Animator>().SetFloat("Speed", distance);

            movePosition = new Vector3(value.x * PlayerScript.speed * distance * 
                Time.deltaTime, 0f, value.y * PlayerScript.speed * distance * Time.deltaTime);
        }
        else
        {
            go_Player.transform.rotation = Quaternion.Slerp(go_Player.transform.rotation, ShootJoystick.instance.spin, 0.2f);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        value = eventData.position - (Vector2)rect_Background.position;

        // 원밖으로 나갈 경우 그 백그라운드에 가두어야 함
        value = Vector2.ClampMagnitude(value, radius);
        
        // 캐릭터 회전
        moveLookRotation = Mathf.Atan2(value.x, value.y);
        moveLookRotation *= Mathf.Rad2Deg;

        // 부모 객체에 대한 상대적인 좌표
        rect_Joystick.localPosition = value;

        distance = Vector2.Distance(Vector2.zero, value) / radius;

        if(PlayerCamera.isRotateCamera)
        {
            spin = Quaternion.AngleAxis(moveLookRotation - 180.0f, Vector3.up);
            value = -value.normalized;
        }
        else
        {
            spin = Quaternion.AngleAxis(moveLookRotation, Vector3.up);
            value = value.normalized;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isTouch = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isTouch = false;
        rect_Joystick.localPosition = Vector3.zero;
        movePosition = Vector3.zero;

        distance = 0;
    }
}
