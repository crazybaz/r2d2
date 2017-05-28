using System.Collections;
using UnityEngine;

namespace Paladin.Controller.Ability
{
    public class Nanobots : AbilityBase
    {
        private float restoreValue; // Сколько восстанавливаем за тик
        private float restoreAmount; // Сколько всего надо восстановить

        private const float restoreRate = 1 / 60f; // частота тиков восстановления

        public override void Prepare()
        {
            restoreAmount = Paladin.Model.MaxHealth * GetOption<float>("restorePart");
            restoreValue =  restoreAmount * restoreRate / Defs.Config.NANOBOTS_DURATION;
        }

        public override void Activate()
        {
            Paladin.View.StartCoroutine(RestoreHealth());
        }

        private IEnumerator RestoreHealth()
        {
            while (restoreAmount > 0)
            {
                yield return new WaitForSeconds(restoreRate);

                float restored;
                if (restoreAmount > restoreValue)
                {
                    restoreAmount -= restoreValue;
                    restored = restoreValue;
                }
                else
                {
                    restored = restoreAmount;
                    restoreAmount = 0;
                }

                Paladin.View.dispatcher.Dispatch(ViewEvent.RESTORE_HEALTH, restored);
            }
        }
    }
}