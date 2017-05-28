using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Paladin.View;
using strange.extensions.mediation.impl;
using UnityEngine;
using Random = UnityEngine.Random;

public class MasterPattern : EventView
{
    [Inject]
    public GameLogic GameLogic { get; set; }

    [NonSerialized] public uint Level = 1;
    [NonSerialized] public BoxCollider Bounds;
    [NonSerialized] public bool IsZeroPattern;
    [NonSerialized] public GameObject LastPattern;

    private GameConfig config;
    private List<Pawn> pawns;
    private List<Pattern> patterns = new List<Pattern>();
    private Dictionary<Pawn, uint> pawnCoins;

    [NonSerialized] public Pattern StaticPattern;

    private const string SURFACE_TAG = "Surface";
    private const uint GURANTEED_SAFE_DISTANCE = 500;

    public IEnumerator Init(GameConfig config)
    {
        this.config = config;
        pawns = new List<Pawn>();
        pawnCoins = new Dictionary<Pawn, uint>();

        ScrollableObject scrollableObject = GetComponentInChildren<ScrollableObject>();
        if (scrollableObject != null)
        {
            Bounds = scrollableObject.GetComponent<BoxCollider>();
        }
        else
        {
            ImmobilizePattern immobilizePattern = GetComponentInChildren<ImmobilizePattern>();
            if (immobilizePattern != null)
                Bounds = immobilizePattern.GetComponent<BoxCollider>();
        }

        if (Bounds == null)
            throw new Exception("No bounds from a child with tag '" + SURFACE_TAG + "' are detected in MasterPattern hierarchy");

        if (!IsZeroPattern)
            Bounds.transform.position = new Vector3(0, 0, GURANTEED_SAFE_DISTANCE);

        yield return new WaitForEndOfFrame();

        if (!IsZeroPattern)
        {
            GameObject closestSurface = null;
            Transform[] allChildren = LastPattern.GetComponentsInChildren<Transform>();
            Transform child;
            for (int i = 0; i < allChildren.Length; i++)
            {
                child = allChildren[i];
                if (child.gameObject.tag == SURFACE_TAG)
                {
                    closestSurface = child.gameObject;
                    break;
                }
            }
            if (closestSurface != null)
            {
                Bounds.transform.position = new Vector3(0, 0,
                    closestSurface.transform.position.z
                    + closestSurface.GetComponent<BoxCollider>().bounds.extents.z
                    + closestSurface.GetComponent<BoxCollider>().bounds.size.z *
                    closestSurface.GetComponent<BoxCollider>().center.y /
                    closestSurface.GetComponent<BoxCollider>().size.y - 1);
            }
            LastPattern = null;
        }
        ShareCoins();
    }

    public void AddPattern(Pattern pattern)
    {
        patterns.Add(pattern);
        if (pattern.Type == PatternType.sp)
            StaticPattern = pattern;
    }

    private void Update()
    {
        foreach (var pattern in patterns)
        {
            if (!pattern.Destroyed)
                return;
        }
        Destroy(gameObject);
    }

    private void ShareCoins()
    {
        foreach (var pattern in patterns)
            pawns.AddRange(pattern.PawnList);

        if (pawns.Count <= 0)
            return;

        // Общее число монет на шаблон
        var coinPriceData = config.COIN_PRICE;
        var coinPrice = Level > coinPriceData.Length ? coinPriceData.Last() : coinPriceData[Level - 1];
        var coinCount = Mathf.CeilToInt(Random.Range(coinPrice.x, coinPrice.y) * GameLogic.BuffModule.CoinMultiplier);

        // Распределяем монеты
        for (int i = 0; i < coinCount; i++)
        {
            var pawn = pawns[Random.Range(0, pawns.Count)];

            if (pawnCoins.ContainsKey(pawn))
                pawnCoins[pawn] += 1;
            else
                pawnCoins.Add(pawn, 1);
        }

        // Расфасовываем ранжированные лут-монеты
        foreach (var pawn in pawnCoins.Keys)
        {
            uint value = pawnCoins[pawn];

            var value5 = value / 5;
            var value3 = (value % 5) / 3;
            var value1 = value - value5 * 5 - value3 * 3;

            for (int i = 0; i < value5; i++)
                pawn.AddDropItem(Resources.Load<GameObject>("prefabs/drop/loot/Coin5"));

            for (int i = 0; i < value3; i++)
                pawn.AddDropItem(Resources.Load<GameObject>("prefabs/drop/loot/Coin3"));

            for (int i = 0; i < value1; i++)
                pawn.AddDropItem(Resources.Load<GameObject>("prefabs/drop/loot/Coin1"));
        }
    }
}