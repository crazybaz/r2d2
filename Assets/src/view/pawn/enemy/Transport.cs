using System.Collections;
using System.Collections.Generic;
using SWS;
using UnityEngine;

public class Transport : BattlePawn
{
    [SerializeField] private List<GameObject> crewList;
    [SerializeField] private float landingPause = 1.0f;
    [SerializeField] private ushort landingCount = 1;
    [SerializeField] private List<Transform> wheels;

    private IEnumerator descentRoutine;

    protected override void Awake()
    {
        base.Awake();
        foreach (var crew in crewList)
            crew.SetActive(false);
    }

    public void ShotHandler()
    {
        Anim.Play(BattleStateMachine.STATE_ATTACK);
        Anim.Play(BattleStateMachine.STATE_SHOT2);
    }

    public void Shot4Handler()
    {
        Anim.Play(BattleStateMachine.STATE_SHOT4);
    }

    public void StartLanding()
    {
        if (landingCount <= 0) return;

        Mover.Pause();
        // Anim.Play(BattleStateMachine.STATE_LANDING);

        descentRoutine = DescentCrew();
        StartCoroutine(descentRoutine);
    }

    public override void DestroyAnim()
    {
        base.DestroyAnim();
        if (descentRoutine != null)
            StopCoroutine(descentRoutine);
    }

    private IEnumerator DescentCrew()
    {
        for (int i = 0; i < crewList.Count; i++)
        {
            if (Disabled) yield return null;

            var crew = crewList[i];
            crew.SetActive(true);
            var pathForUnits = crew.GetComponent<splineMove>().pathContainer as BezierPathManager;
            pathForUnits.CalculatePath();
            yield return new WaitForSeconds(landingPause);
        }

        // Anim.Play(BattleStateMachine.STATE_LANDING_END);
        Mover.Resume();
        landingCount--;
    }

    protected override void Update()
    {
        if (immovable == false)
        foreach (var wheel in wheels)
            wheel.transform.Rotate(Vector3.right * 10);

        base.Update();
    }

    protected override void OnDestroy()
    {
        descentRoutine = null;
        crewList.Clear();
        crewList = null;
        base.OnDestroy();
    }
}