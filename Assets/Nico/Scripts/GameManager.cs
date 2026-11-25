using JetBrains.Annotations;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    CameraDamageEffect cameraDamageEffect;
    SoundManager soundManager;
    void Start()
    {
        soundManager = FindFirstObjectByType<SoundManager>();
        cameraDamageEffect = FindFirstObjectByType<CameraDamageEffect>();   
    }

    public void SetHealth(float health, float maxHealth)
    {
        if(cameraDamageEffect.SetHealth(health, maxHealth))
        {
            soundManager.PlaySound("hit");
            soundManager.IncreaseNoiseSound();
        }
    }

    public void Walk(bool isWalking, float speed)
    {
        if (isWalking)
        {
            soundManager.StartSteps(speed);
        }
        else
        {
            soundManager.StopSteps();
        }
    }
}
