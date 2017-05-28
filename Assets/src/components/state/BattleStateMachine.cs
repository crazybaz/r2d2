using System;
using System.Collections.Generic;
using UnityEngine;

public class BattleStateMachine
{
    // стейты
    public const string STATE_APPEAR = "appear";
    public const string STATE_ATTACK = "attack";
    public const string STATE_DEATH = "death";
    public const string STATE_DIG_IN = "digIn";
    public const string STATE_DIG_OUT = "digOut";
    public const string STATE_LANDING = "landing";
    public const string STATE_LANDING_END = "landingEnd";
    public const string STATE_MINE = "mine";
    public const string STATE_PREPARE = "prepare";
    public const string STATE_PREPARE_SHOT3 = "prepare3";
    public const string STATE_PREPARE_SHOT4 = "prepare4";
    public const string STATE_PREPARE_SPECIAL_LEFT = "prepareSpecialLeft";
    public const string STATE_PREPARE_SPECIAL_RIGHT = "prepareSpecialRight";
    public const string STATE_RELOAD = "reload";
    public const string STATE_ROLL = "roll";
    public const string STATE_ROLL_END = "rollEnd";
    public const string STATE_RUN = "run";
    public const string STATE_SHOT1 = "shot1";
    public const string STATE_SHOT13LEFT = "shot13Left";
    public const string STATE_SHOT13RIGHT = "shot13Right";
    public const string STATE_SHOT2 = "shot2";
    public const string STATE_SHOT22 = "shot22";
    public const string STATE_SHOT23 = "shot23";
    public const string STATE_SHOT24 = "shot24";
    public const string STATE_SHOT3 = "shot3";
    public const string STATE_SHOT32 = "shot32";
    public const string STATE_SHOT33 = "shot33";
    public const string STATE_SHOT34 = "shot34";
    public const string STATE_SHOT4 = "shot4";
    public const string STATE_SHOT_LEFT = "shotLeft";
    public const string STATE_SHOT_LEFT1 = "shotLeft1";
    public const string STATE_SHOT_LEFT2 = "shotLeft2";
    public const string STATE_SHOT_RIGHT = "shotRight";
    public const string STATE_SHOT_SPECIAL_LEFT = "shotSpecialLeft";
    public const string STATE_SHOT_SPECIAL_RIGHT = "shotSpecialRight";
    public const string STATE_STAY = "stay";

    // стейты боссов
    public const string STATE_MACHINEGUN = "StartMachinegun";
    public const string STATE_MACHINEGUN2 = "StartMachinegun2";

    // тригеры
    public const string TRIGGER_RUN_STAY = "run_stay";
    public const string TRIGGER_STAY_RUN = "stay_run";
    public const string TRIGGER_STAY_ATTACK = "stay_attack";
    public const string TRIGGER_ANY_RUN = "any_run";
    public const string TRIGGER_ANY_RUN_REVERCE = "any_runReverce";
    public const string TRIGGER_ANY_STAY = "any_stay";
    public const string TRIGGER_ANY_DEATH = "any_death";
    public const string TRIGGER_RUN_DIG = "run_dig";
    public const string TRIGGER_RUN_ROLL = "run_roll";
    public const string TRIGGER_ROLL_END = "roll_end";
    public const string TRIGGER_ATTACK_RELOAD = "attack_reload";
    public const string TRIGGER_RUN_PREPARE = "run_prepare";
    public const string TRIGGER_RUN_PREPARE3 = "run_prepareShot3";
    public const string TRIGGER_PREPARE_SHOT3 = "prepareShot3_shot3";

    private static readonly KeyValuePair<string, BattleState>[] relations;

    static BattleStateMachine()
    {
        relations = new[]
        {
            new KeyValuePair<string, BattleState>(STATE_RUN, BattleState.Run),
            new KeyValuePair<string, BattleState>(STATE_STAY, BattleState.Stay),
            new KeyValuePair<string, BattleState>(STATE_ATTACK, BattleState.Fire),
            new KeyValuePair<string, BattleState>(STATE_DEATH, BattleState.Death),
            new KeyValuePair<string, BattleState>(STATE_DIG_IN, BattleState.DigIn),
            new KeyValuePair<string, BattleState>(STATE_DIG_OUT, BattleState.DigOut),
            new KeyValuePair<string, BattleState>(STATE_ROLL, BattleState.Roll),
            new KeyValuePair<string, BattleState>(STATE_ROLL_END, BattleState.RollEnd),
            new KeyValuePair<string, BattleState>(STATE_APPEAR, BattleState.Appear),
            new KeyValuePair<string, BattleState>(STATE_LANDING, BattleState.Landing),
            new KeyValuePair<string, BattleState>(STATE_LANDING_END, BattleState.LandingEnd),
            new KeyValuePair<string, BattleState>(STATE_PREPARE_SHOT3, BattleState.PrepareShot3),
            new KeyValuePair<string, BattleState>(STATE_PREPARE_SHOT4, BattleState.PrepareShot4),
            new KeyValuePair<string, BattleState>(STATE_PREPARE_SPECIAL_LEFT, BattleState.PrepareSpecialLeft),
            new KeyValuePair<string, BattleState>(STATE_PREPARE_SPECIAL_RIGHT, BattleState.PrepareSpecialRight),
            new KeyValuePair<string, BattleState>(STATE_PREPARE, BattleState.Prepare),
            new KeyValuePair<string, BattleState>(STATE_SHOT1, BattleState.Shot1),
            new KeyValuePair<string, BattleState>(STATE_SHOT3, BattleState.Shot3),
            new KeyValuePair<string, BattleState>(STATE_SHOT4, BattleState.Shot4),
            new KeyValuePair<string, BattleState>(STATE_SHOT_SPECIAL_LEFT, BattleState.ShotSpecialLeft),
            new KeyValuePair<string, BattleState>(STATE_SHOT_SPECIAL_RIGHT, BattleState.ShotSpecialRight),
            new KeyValuePair<string, BattleState>(STATE_RELOAD, BattleState.Reload)
        };
    }

    public static BattleState AnimToBattleState(AnimatorStateInfo animState)
    {
        foreach (var relation in relations)
        {
            if (animState.IsName(relation.Key))
                return relation.Value;
        }

        throw new Exception("Animation state not handled error");
    }

    public static BattleState Update(Animator anim, BattleState nextState, List<BattleStateTransition> transitions)
    {
        if (anim == null || transitions == null)
            return BattleState.None;

        var state = AnimToBattleState(anim.GetCurrentAnimatorStateInfo(0));

        foreach (var transition in transitions)
        {
            if (transition.Match(state, nextState))
                break;
        }

        return state;
    }
}