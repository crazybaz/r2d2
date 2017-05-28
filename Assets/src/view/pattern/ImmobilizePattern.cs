using System.Collections;
using UnityEngine;

public class ImmobilizePattern : Pattern
{
    public float TimeOut;
    private bool isTemporal;
    [SerializeField] private float bossPatternDelay = -1;

    protected override void Awake()
    {
        base.Awake();

        if (TimeOut > 0)
            isTemporal = true;
    }

    private void Update()
    {
        if (isTemporal) // timeout condition
        {
            TimeOut -= Time.deltaTime;
            if (TimeOut <= 0)
                DestroyPattern();
        }
        else // all pawns destroyed condition
        {
            foreach (var pawn in PawnList)
                if (!pawn.Disabled)
                    return;

            DestroyPattern();
        }
    }

    // can be called form waypoint
    public void DestroyPattern()
    {
        // Debug.Log(">> AP DESTROYED");
        if (bossPatternDelay > 0)
            StartCoroutine(WaitForBossDelay());
        else
            Destroyed = true;
    }

    private IEnumerator WaitForBossDelay()
    {
        yield return new WaitForSeconds(bossPatternDelay);
        Destroyed = true;
    }
}