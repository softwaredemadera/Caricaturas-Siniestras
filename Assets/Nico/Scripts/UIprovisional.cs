using UnityEngine;
using UnityEngine.UI;

public class UIprovisional : MonoBehaviour
{

    Scrollbar barra;
    CameraDamageEffect cameraDamageEffect;
    void Start()
    {
        barra = GetComponent<Scrollbar>();
        cameraDamageEffect = FindFirstObjectByType<CameraDamageEffect>();
    }

    // Update is called once per frame
    void Update()
    {
        if (cameraDamageEffect != null)
        {
            barra.size = cameraDamageEffect.GetHealth();
            if(barra.size <= 0.05f)
            {
                barra.targetGraphic.gameObject.SetActive(false);
            }
            else
            {
                barra.targetGraphic.gameObject.SetActive(true);
            }   
        }
    }
}
