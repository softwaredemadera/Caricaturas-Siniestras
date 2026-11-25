using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CameraDamageEffect : MonoBehaviour
{
    [Header("Volume (post-procesado)")]
    [SerializeField] private Volume volume;

    [Header("Vida")]
    [Range(0f, 1f)][SerializeField] private float health01 = 1f;
    [SerializeField] private float smoothTime = 0.35f;

    [Header("Balanceo de cámara en mano")]
    [SerializeField] private float swayRotation = 0.35f;     // grados con vida completa
    [SerializeField] private float swayPosition = 0.004f;    // metros con vida completa
    [SerializeField] private float swaySpeed = 0.8f;
    [SerializeField] private float damagedSwayMultiplier = 5f;

    [Header("Glitches (con poca vida)")]
    [SerializeField] private float glitchStartsBelow = 0.5f; // empiezan por debajo de este % de vida
    [SerializeField] private Vector2 glitchInterval = new Vector2(1.5f, 5f);
    [SerializeField] private float glitchDuration = 0.25f;

    private Vignette vignette;
    private FilmGrain filmGrain;
    private ChromaticAberration chromatic;
    private LensDistortion lensDistortion;
    private ColorAdjustments colorAdjustments;

    private float damage01;          // 0 = sano, 1 = casi muerto (suavizado)
    private float damageVelocity;
    private float hitPulse;          // pulso al recibir un golpe
    private float glitchTimer;
    private float nextGlitchIn;
    private float seed;

    private void Awake()
    {
        seed = Random.value * 100f;
        nextGlitchIn = Random.Range(glitchInterval.x, glitchInterval.y);

        if (volume == null) volume = FindFirstObjectByType<Volume>();

        // volume.profile crea una copia en runtime (no ensucia el asset original)
        if (volume != null)
        {
            volume.profile.TryGet(out vignette);
            volume.profile.TryGet(out filmGrain);
            volume.profile.TryGet(out chromatic);
            volume.profile.TryGet(out lensDistortion);
            volume.profile.TryGet(out colorAdjustments);
        }
    }

    public float GetHealth() => health01;

    public void SetHealth(float current, float max)
    {
        health01 = Mathf.Clamp01(current / max);
    }

    public void Hit(float strength = 1f)
    {
        hitPulse = Mathf.Clamp01(strength);
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.hKey.wasPressedThisFrame)
        {
            //daño provisional para probar el efecto
            Hit();
            SetHealth(health01 - 0.45f, 1f);
        }
            
        
        damage01 = Mathf.SmoothDamp(damage01, 1f - health01, ref damageVelocity, smoothTime);

        hitPulse = Mathf.MoveTowards(hitPulse, 0f, Time.deltaTime * 2.5f);
        float glitch = UpdateGlitch();
        float spike = Mathf.Max(hitPulse, glitch);

        ApplyPostProcessing(spike);
        ApplyHandheldSway(spike);
    }

    private float UpdateGlitch()
    {
        float lowHealth = Mathf.InverseLerp(glitchStartsBelow, 0f, health01); 
        if (lowHealth <= 0f) return 0f;

        glitchTimer += Time.deltaTime;

        if (glitchTimer >= nextGlitchIn)
        {
            glitchTimer = 0f;
            // Con menos vida, los glitches son más frecuentes
            nextGlitchIn = Random.Range(glitchInterval.x, glitchInterval.y) * Mathf.Lerp(1f, 0.3f, lowHealth);
        }

        // Ráfaga corta al inicio de cada ciclo
        float burst = glitchTimer < glitchDuration ? 1f - (glitchTimer / glitchDuration) : 0f;
        return burst * lowHealth;
    }

    private void ApplyPostProcessing(float spike)
    {
        if (vignette != null)
            vignette.intensity.value = Mathf.Lerp(0.25f, 0.6f, damage01);

        if (filmGrain != null)
            filmGrain.intensity.value = Mathf.Clamp01(Mathf.Lerp(0.35f, 1f, damage01) + spike * 0.2f);

        if (chromatic != null)
            chromatic.intensity.value = Mathf.Clamp01(Mathf.Lerp(0.1f, 0.5f, damage01) + spike * 0.8f);

        if (lensDistortion != null)
            lensDistortion.intensity.value = Mathf.Lerp(-0.1f, -0.35f, damage01) - spike * 0.3f;

        if (colorAdjustments != null)
            colorAdjustments.saturation.value = Mathf.Lerp(-10f, -70f, damage01);
    }

    private void ApplyHandheldSway(float spike)
    {
        float amount = 1f + damage01 * damagedSwayMultiplier + spike * 4f;
        float t = Time.time * swaySpeed * (1f + damage01);

        // Perlin devuelve 0..1; lo llevamos a -1..1
        float nx = Mathf.PerlinNoise(seed, t) * 2f - 1f;
        float ny = Mathf.PerlinNoise(seed + 10f, t) * 2f - 1f;
        float nz = Mathf.PerlinNoise(seed + 20f, t) * 2f - 1f;

        // Se aplica sobre la cámara, que es hija del pivot: no pelea con el mouse look.
        transform.localRotation = Quaternion.Euler(
            ny * swayRotation * amount,
            nx * swayRotation * amount,
            nz * swayRotation * 0.6f * amount);

        transform.localPosition = new Vector3(
            nx * swayPosition * amount,
            ny * swayPosition * amount,
            0f);
    }
}