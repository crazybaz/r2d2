using System;
using strange.extensions.dispatcher.eventdispatcher.api;
using UnityEngine;

namespace Paladin.View
{
    public class TornBuff : BuffView
    {
        public int FullValue;
        public float Multiplier;
        public int Distance;


        private GameObject fx;
        private float totalDamage;
        private float currentDamage;
        private CapsuleCollider damageCollider;
        private Collider[] otherColliders;


        public TornBuff()
        {
            Type = BuffType.Torn;
        }

        public override void Activate()
        {
            base.Activate();
            totalDamage = FullValue * Multiplier;
            fx = Resources.Load<GameObject>("prefabs/fx/fx_plazmoid_explosion");
            Paladin.View.dispatcher.AddListener(ViewEvent.BULLET_HIT, HitHandler);
        }

        public override void Deactivate()
        {
            base.Deactivate();
            Paladin.View.dispatcher.RemoveListener(ViewEvent.BULLET_HIT, HitHandler);
        }

        private void HitHandler(IEvent e)
        {
            currentDamage += (float)e.data;
            if (currentDamage >= FullValue)
            {
                var pos = gameObject.transform.position;
                pos.y = Config.I.PALADIN_HALF_HEIGHT;
                Instantiate(fx, pos, Quaternion.identity);

                // сперва отключим все другие коллайдеры
                otherColliders = gameObject.GetComponents<Collider>();
                Array.ForEach(otherColliders, c => c.enabled = false);

                // навешаем наш коллайдер
                damageCollider = gameObject.AddComponent<CapsuleCollider>();
                damageCollider.radius = Distance;
                damageCollider.enabled = false;
                damageCollider.isTrigger = true;

                currentDamage = currentDamage - FullValue;
            }
        }

        private void FixedUpdate()
        {
            if (damageCollider == null)
                return;

            if (damageCollider.enabled == false)
                damageCollider.enabled = true;
            else
            {
                Destroy(damageCollider);
                damageCollider = null;
                Array.ForEach(otherColliders, c => c.enabled = true);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (damageCollider == null)
                return;

            var pawn = other.gameObject.GetComponent<IPawn>();
            if (pawn != null && !pawn.Disabled)
                pawn.dispatcher.Dispatch(ViewEvent.BULLET_HIT, totalDamage);
        }

        protected override void OnDestroy()
        {
            if (damageCollider != null)
                Destroy(damageCollider);

            damageCollider = null;
            fx = null;

            base.OnDestroy();
        }
    }
}