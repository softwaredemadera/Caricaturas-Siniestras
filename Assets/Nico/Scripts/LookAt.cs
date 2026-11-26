using UnityEngine;

public class LookAt : MonoBehaviour
{
    [Header("Objetivo")]
    [SerializeField] private Transform target;

    [Header("Comportamiento")]
    [Tooltip("Gira solo en horizontal (eje Y). Solo para sprites y personajes que no deben inclinarse.")]
    [SerializeField] private bool lockYAxis = true;

    [Tooltip("Activar para sprites/quads, que se ven desde su cara trasera (-Z). " + "Déjarlo apagado para modelos 3D cuyo frente es +Z.")]
    [SerializeField] private bool invertFacing = true;

    [Tooltip("0 = giro instantáneo. Valores más altos = giro más rápido pero suave.")]
    [SerializeField] private float turnSpeed = 0f;

    [Tooltip("Corrige la orientación base del modelo (grados en Y) si no mira bien de frente.")]
    [SerializeField] private float yawOffset = 0f;

    private void Start()
    {
        if (target != null) return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            target = player.transform;
        else if (Camera.main != null)
            target = Camera.main.transform;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 direction = target.position - transform.position;

        if (lockYAxis)
            direction.y = 0f;

        if (direction.sqrMagnitude < 0.0001f) return;

        if (invertFacing)
            direction = -direction;

        Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up)
                                    * Quaternion.Euler(0f, yawOffset, 0f);

        if (turnSpeed <= 0f)
        {
            transform.rotation = targetRotation;
        }
        else
        {
            float t = 1f - Mathf.Exp(-turnSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, t);
        }
    }
}
