﻿// Внутренние события view <-> mediator для GUI
public enum ViewEvent
{
    COLLECT_DROP_ITEM,

    PALADIN_COLLISION,
    ENEMY_COLLISION,

    BULLET_CREATED,
    BULLET_HIT,

    RESTORE_HEALTH,

    //PAWN_COLLISION, // pawn to pawn collision

    STUN_SPEED,
    BOOST_SPEED,

    // BUTTONS
    ADD_RESOURCES,
    START_BUTTON,
    FACEBOOK_BUTTON,
    GUEST_BUTTON,
    CONTINUE_BUTTON,
    CLOSE_BUTTON,
    FULL_ENERGY_BUTTON,
    REQUEST_ENERGY_BUTTON,
    UNLIM_ENERGY_BUTTON,
    SPEEDUP_ENERGY_BUTTON,
    COLLECT_BUTTON,
    ON_UPGRADE,
    ON_SELECT,

    // ETC
    START,
    FINISH,
    COMPLETE,

    // BOSSES
    BOSS_INIT_FIGHT,
    BOSS_PATTERN_ARRIVED
}