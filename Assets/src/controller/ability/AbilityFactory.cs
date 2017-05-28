using System;
using Paladin.Controller.Ability;

namespace Paladin.Controller
{
    public class AbilityFactory
    {
        [Inject]
        public Logic Logic { get; set; }

        [Inject]
        public GameLogic GameLogic { get; set; }

        public AbilityBase Create(AbilityType type, uint level)
        {
            var ability = GetAbility(type);

            ability.Type = type;
            ability.Level = level;
            ability.Config = Logic.Defs.GetAbility(type, level);
            ability.Paladin = GameLogic.Paladin;
            ability.Defs = Logic.Defs;
            ability.Prepare();
            return ability;
        }

        private AbilityBase GetAbility(AbilityType type)
        {
            switch (type)
            {
                // active
                case AbilityType.ClusterGrenades:
                    return new ClusterGrenades();
                case AbilityType.Invulnerability:
                    return new Invulnerability();
                case AbilityType.Nanobots:
                    return new Nanobots();
                case AbilityType.ShockShells:
                    return new ShockShells();
                case AbilityType.Phantom:
                    return new Phantom();
                case AbilityType.ShockWaves:
                    return null; // post-release
                case AbilityType.VacuumBomb:
                    return new VacuumBomb();
                case AbilityType.Artillery:
                    return new Artillery();

                // passive
                case AbilityType.Rocket:
                    return new Rocket();
                case AbilityType.HiScores:
                    return new HiScores();
                case AbilityType.Bankir:
                    return new Bankir();
                case AbilityType.HealthBonus:
                    return new HealthBonus();
                case AbilityType.SpeedBonus:
                    return new SpeedBonus();
                case AbilityType.FireRate:
                    return new FireRate();
                case AbilityType.DamageBonus:
                    return new DamageBonus();
                case AbilityType.ArmorBonus:
                    return new ArmorBonus();
                case AbilityType.DropRate:
                    return new DropRateBonus();
                case AbilityType.Vampirism:
                    return new Vampirism();
                case AbilityType.Torn:
                    return new Torn();
                case AbilityType.Magnet:
                    return new Magnet();
                case AbilityType.DroneX:
                case AbilityType.DroneY:
                    return null; // post-release
                default:
                    throw new ArgumentOutOfRangeException("There is no implementation for ability " + type);
            }
        }
    }
}