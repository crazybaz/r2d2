using System.Linq;
using Paladin.View;
using SWS;
using UnityEngine;
using UnityEngine.Events;

public class PawnCommandProxy : MonoBehaviour
{
    public uint WaitInterval;
    public UnityEvent[] Events = new UnityEvent[10];

    private IBattlePawn battlePawn;
    private IMeleePawn meleePawn;
    private Boss boss;
    private Transport transport;
    private Pawn pawn;

    private splineMove splineMover;

    private void Start()
    {
        splineMover = GetComponent<splineMove>();
        splineMover.events = Events.ToList();
    }

    public IBattlePawn BattlePawn
    {
        get
        {
            if (battlePawn == null)
            {
                battlePawn = GetComponentInChildren<IBattlePawn>();
                if (battlePawn == null)
                    throw new MissingComponentException("IBattlePawn not found in child component");
            }
            return battlePawn;
        }
    }

    public IMeleePawn MeleePawn
    {
        get
        {
            if (meleePawn == null)
            {
                meleePawn = GetComponentInChildren<IMeleePawn>();
                if (meleePawn == null)
                    throw new MissingComponentException("IMeleePawn not found in child component");
            }
            return meleePawn;
        }
    }

    public Transport Transport // TODO: возможно в будущем здесь тоже следует сделать интерфейс
    {
        get
        {
            if (transport == null)
            {
                transport = GetComponentInChildren<Transport>();
                if (transport == null)
                    throw new MissingComponentException("Transport not found in child component");
            }
            return transport;
        }
    }

    private Pawn Pawn
    {
        get
        {
            if (pawn == null)
                pawn = GetComponentInChildren<Pawn>();

            return pawn;
        }
    }

    public Boss Boss
    {
        get
        {
            if (boss == null)
            {
                boss = GetComponentInChildren<Boss>();
                if (boss == null)
                    throw new MissingComponentException("Boss not found in child component");
            }
            return boss;
        }
    }

    public void FinishArriveAnimation()
    {
        Boss.FinishArriveAnimation();
    }

    public void PauseMove()
    {
        if (Pawn != null)
            Pawn.PauseMove();
    }

    public void ResumeMove()
    {
        if (Pawn != null)
            Pawn.ResumeMove();
    }

    public void WaitForSeconds()
    {
        splineMover.Pause(WaitInterval);
    }

    public void StartLanding()
    {
        Transport.StartLanding();
    }

    public void StartAttack()
    {
        BattlePawn.StartAttack();
    }

    public void DiggIn()
    {
        MeleePawn.DiggIn();
    }

    public void DiggOut()
    {
        MeleePawn.DiggOut();
    }

    public void StartRoll()
    {
        MeleePawn.StartRoll();
    }

    public void EndRoll()
    {
        MeleePawn.EndRoll();
    }

    private void OnDestroy()
    {
        Events = null;
        splineMover = null;
        battlePawn = null;
        meleePawn = null;
        transport = null;
        pawn = null;
    }
}
