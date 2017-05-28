﻿public enum GameEvent
{
    LOAD_DEFS_PROGRESS,
    LOAD_ASSETS_PROGRESS,
    LOAD_STATE_PROGRESS,
    LOAD_HANGAR_PROGRESS,
    LOAD_BATTLE_PROGRESS,
    PROVISOR_PROGRESS,

    ACTIVATE_HANGAR,
    ACTIVATE_BATTLE,

    COMPLETE,
    ADD_LOOT_ITEM, // подобрать предмет и закопить в инвентарь
    PAWN_DESTROY,
    FIRE_RATE_COLLECTED,

    START_SHOOTING,

    FINISH_BATTLE,
    CHANGE_FIELD_SPEED
}