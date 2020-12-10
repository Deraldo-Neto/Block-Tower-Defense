using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour {

    [SerializeField]
    private GameObject StormBtn;
    [SerializeField]
    private GameObject FrostBtn;
    [SerializeField]
    private GameObject FireBtn;
    [SerializeField]
    private GameObject PoisonBtn;
    [SerializeField]
    private GameObject Base2Btn;

	void Update () {
        /*if (Existe Torre de Storm))
        {
            StormBtn.SetActive(true);
        }
        if (Existe Torre de Frost))
        {
            FrostBtn.SetActive(true);
        }
        if (Existe Torre de Fire))
        {
            FireBtn.SetActive(true);
        }
        if (Existe Torre de Poison))
        {
            PoisonBtn.SetActive(true);
        }
        */
    }
    //Primeiramente vai ser possivel só ter 1 de cada, depois vai poder colocar a quantidade do mesmo elemento que quiser, mas no momento não sei como fazer isso
    //Talvez eu tenha que mexer no canvas, e adicionar por script os botões.
}
