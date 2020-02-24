using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraInfo : MonoBehaviour
{
    public Image bloodScreen;

    public void SoftShakeCam()
    {
        StartCoroutine(shake());
    }

    public void startBloodScreen()
    {
        StartCoroutine(ShowBloodScreen());
    }

    IEnumerator shake()
    {
        float randomFloat = Random.Range(0.02f, 0.03f);

        this.transform.position = new Vector3(this.transform.position.x + randomFloat, this.transform.position.y + randomFloat, this.transform.position.z + randomFloat);
        yield return new WaitForSeconds(0.05f);

        this.transform.position = new Vector3(this.transform.position.x + randomFloat, this.transform.position.y + randomFloat, this.transform.position.z + randomFloat);
        yield return new WaitForSeconds(0.05f);
    }

    IEnumerator ShowBloodScreen()
    {
        float i = 0.3f;

        bloodScreen.color = new Color(1, 0, 0, i);
        yield return new WaitForSeconds(0.05f);

        while(true)
        {
            bloodScreen.color = new Color(1, 0, 0, i);

            i -= 0.03f;

            if(i < 0)
            {
                break;
            }

            yield return null;
        }
        bloodScreen.color = Color.clear;
    }
}
