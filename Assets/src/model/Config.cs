using System;
using UnityEngine;

public class Config : MonoBehaviour
{
    [Header("Ограничение движения игрока")]
    public Boundary PLAYER_MOVEMENT_BOUNDS = new Boundary();

    [Header("Ограничение движения камеры")]
    public Boundary CAMERA_MOVEMENT_BOUNDS = new Boundary();

    [Header("Режим отпускания пальца")]
    public bool TOUCH_RELEASE_MODE = true;

    [Header("Коэффициент замедления")]
    [Tooltip("Во сколько раз тормозим игру на время, когда палец отпущен")]
    [Range(1.0f,100.0f)]
    public float SLOW_MODE_MULTIPLIER = 30.0f;

    [Header("Время затухания экрана")]
    [Tooltip("За какое время (в секундах) экран затемняется, когда палец отлепляется от экрана. За это же время все возвращается назад")]
    [Range(0.1f, 2.0f)]
    public float FADE_INTERVAL = 0.5f;

    [Header("Целевая непрозрачность")]
    [Tooltip("Какой непрозрачности будет цвет полностью зафеженого экрана. Пример если цвет выбран черный, а TARGET_ALPHA = 1, то экран станет полностью черным, игровое поле будет практически не видно")]
    [Range(0.1f, 1.0f)]
    public float TARGET_ALPHA = 0.5f;

    [Header("Примерная половина роста паладина")]
    public uint PALADIN_HALF_HEIGHT = 15;

    [NonSerialized]
    public Vector3 PALADIN_HALF_HEIGHT_VECTOR;

    [Header("Цвет исчезновения юнитов")]
    public Color32 UNITS_FADE_OUT_COLOR = new Color32(0x85, 0xDB, 0xFF, 0xff);

    [NonSerialized] public int FIELD_MASK;
    [NonSerialized] public int PAWN_MASK;
    public Boundary GAMEPLAY_BOUNDS = new Boundary();

    private static Config Instance;
    public static Config I
    {
        get
        {
            if (Instance == null)
                Instance = FindObjectOfType(typeof (Config)) as Config;
            return Instance;
        }
    }

    private void Awake()
    {
        FIELD_MASK = 1 << LayerMask.NameToLayer("Field");
        PAWN_MASK = 1 << LayerMask.NameToLayer("Pawn");
        PALADIN_HALF_HEIGHT_VECTOR = new Vector3(0, I.PALADIN_HALF_HEIGHT, 0);
    }
}