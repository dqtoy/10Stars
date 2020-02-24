using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class IsometricCamera : MonoBehaviourPun
{
    private float offsetX = 0f;
    private float offsetY = 16f;
    private float offsetZ = -13f;
    public GameObject player;

    public bool isRotateCamera;

    public Image bloodScreen;
    private Coroutine bloodCoroutine = null;

    private void Awake()
    {
        isRotateCamera = false;
    }

    void Start()
    {
        player = GameManager.instance.mainPlayer;
    }

    void Update()
    {
        if (GameManager.instance.mainPlayer.GetPhotonView().IsMine)
        {
            ChaseCameraToMainPlayer();
        }
    }

    public void ChaseCameraToMainPlayer()
    {
        if (!isRotateCamera)
        {
            this.transform.rotation = Quaternion.Euler(45f, 0f, 0f);
            this.transform.position = new Vector3
                (player.transform.position.x + offsetX, player.transform.position.y + offsetY, player.transform.position.z + offsetZ);
        }
        else
        {
            this.transform.rotation = Quaternion.Euler(135f, 0f, 180f);
            this.transform.position = new Vector3
                (player.transform.position.x, player.transform.position.y + offsetY, player.transform.position.z + 13f);
        }
    }

    public void IsRotateCamera()
    {
        isRotateCamera = !isRotateCamera;
    }

    public void startBloodScreen()
    {
        if (bloodCoroutine != null)
        {
            StopCoroutine(bloodCoroutine);
            bloodCoroutine = null;
        }
        bloodCoroutine = StartCoroutine("ShowBloodScreen");
    }

    public void SoftShakeCam()
    {
        StartCoroutine("shake");
    }

    IEnumerator ShowBloodScreen()
    {
        float alphaInfo = 0.4f;
        bloodScreen.color = new Color(1, 0, 0, alphaInfo);
        yield return new WaitForSeconds(0.05f);

        while (true)
        {
            bloodScreen.color = new Color(1, 0, 0, alphaInfo);
            alphaInfo -= 0.03f;

            if (alphaInfo < 0) break;
            yield return new WaitForSeconds(0.05f);
        }
        bloodScreen.color = Color.clear;
        bloodCoroutine = null;
    }

    IEnumerator shake()
    {
        float randomFloat = Random.Range(0.05f, 0.07f);

        this.transform.position = new Vector3(this.transform.position.x + randomFloat, this.transform.position.y + randomFloat, this.transform.position.z + randomFloat);
        yield return new WaitForSeconds(0.05f);

        this.transform.position = new Vector3(this.transform.position.x + randomFloat, this.transform.position.y + randomFloat, this.transform.position.z + randomFloat);
        yield return new WaitForSeconds(0.05f);
    }
}
