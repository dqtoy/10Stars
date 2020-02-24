using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimalsHpBar : MonoBehaviour
{
    private float viewHp;

    public Image hpbar;
    private BaseAnimals baseAnimal;
    private Canvas thisCanvas;

    void Start()
    {
        baseAnimal = this.gameObject.GetComponentInParent<BaseAnimals>();
        viewHp = baseAnimal.BaseMaxHp;
        thisCanvas = this.GetComponent<Canvas>();
    }

    void Update()
    {
        viewHp = Mathf.Lerp(viewHp, baseAnimal.BaseHp, 0.2f);
        hpbar.fillAmount = viewHp / baseAnimal.BaseMaxHp;

        if(baseAnimal.BaseIsViewHpBar)
        {
            hpBarCoolTime();
        }
    }

    void hpBarCoolTime()
    {
        if (baseAnimal.BaseIsViewHpTime > 0)
        {
            baseAnimal.BaseIsViewHpTime -= Time.deltaTime;

            thisCanvas.enabled = true;

            if (baseAnimal.BaseIsViewHpTime <= 0)
            {
                thisCanvas.enabled = false;
                baseAnimal.BaseIsViewHpBar = false;
            }
        }
    }
}
