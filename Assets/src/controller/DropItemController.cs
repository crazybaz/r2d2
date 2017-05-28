using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class DropItemController : MonoBehaviour
{
    [Header("Радиус возникновения дропа")]
    [Tooltip("Радиус на плоскости XZ в игровых единицах. Сейчас весь дроп возникают на одинаковой высоте над землей")]
    [Range(1, 20)]
    public float Radius = 10.0f;

    [Header("Сила разлета")]
    [Tooltip("К примеру 0 - не разлетаются совсем, 2 - разлетаются вдвое сильнее обычного")]
    [Range(0.0f, 5.0f)]
    public float ForceModifier = 1.5f;

    [Header("Изначальный размер дропа")]
    [Tooltip("К примеру 0.1 - дроп появляются размером 10% от обычного, 0.3 - 30%")]
    [Range(0.0f, 1.0f)]
    [SerializeField] private float initSize = 0.3f;

    [Header("Время вырастания")]
    [Tooltip("Время до полного вырастания дропа из изначального размера")]
    [SerializeField] private float timeScaling = 0.5f;

    [Header("Минимальная скорость вращения")]
    [Range(0, 500)]
    [Tooltip("Скорость вращения дропа после выпадания. Устанавливается случайно для каждого дропа в диапазоне от мин до макс")]
    [SerializeField] private int rotationSpeedMin = 30;

    [Header("Максимальная скорость вращения")]
    [Range(0, 500)]
    [Tooltip("Скорость вращения дропа после выпадания. Устанавливается случайно для каждого дропа в диапазоне от мин до макс")]
    [SerializeField] private int rotationSpeedMax = 60;

    [Header("Ось вращения")]
    [Tooltip("Выберите ось, вокруг которой будет вращаться дроп")]
    [SerializeField] private Axis rotationAxis = Axis.Y;

    private Vector3 rotationDirection;
    private int rotationSpeed;
    private Vector3 scale;
    private float time = 0.0f;

    private void Start()
    {
        switch (rotationAxis)
        {
            case Axis.Y:
                rotationDirection = Random.value > 0.5f ? Vector3.up : -Vector3.up;
                break;
            case Axis.X:
                rotationDirection = Random.value > 0.5f ? Vector3.left : -Vector3.left;
                break;
            case Axis.Z:
                rotationDirection = Random.value > 0.5f ? Vector3.forward : -Vector3.forward;
                break;
        }

        rotationSpeed = Random.Range(rotationSpeedMin, rotationSpeedMax);
        scale = transform.localScale;
    }

    private void Update()
    {
        // rotation
        transform.RotateAround(transform.position, rotationDirection, rotationSpeed * Time.deltaTime);

        // расширение при появлении
        if (time < 1)
        {
            time += Time.deltaTime;
            if (time > 1)
                time = 1;

            transform.localScale = Vector3.Lerp(scale * initSize, scale, time / timeScaling);
        }

        if (Config.I.GAMEPLAY_BOUNDS.IsOutside(gameObject.transform.position))
        {
            Destroy(gameObject);
        }
    }
}