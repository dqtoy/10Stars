using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerEXp : MonoBehaviour
{
    private Player mainPlayerScript;

    void Start()
    {
        mainPlayerScript = GameManager.instance.GetComponent<Player>();
    }

    void Update()
    {
        this.GetComponent<Text>().text = mainPlayerScript.Exp.ToString();
    }
}
