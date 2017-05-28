using System.Collections;
using strange.extensions.dispatcher.eventdispatcher.api;
using UnityEngine;

namespace Paladin.View
{
    public class StunBuff : BuffView
    {
        public float StunTime;

        public StunBuff()
        {
            Type = BuffType.Stun;
        }

        public override void Activate()
        {
            base.Activate();
            Paladin.View.dispatcher.AddListener(ViewEvent.BULLET_CREATED, BulletCreatedHandler);
        }

        public override void Deactivate()
        {
            base.Deactivate();
            Paladin.View.dispatcher.RemoveListener(ViewEvent.BULLET_CREATED, BulletCreatedHandler);
        }

        private void BulletCreatedHandler(IEvent evt)
        {
            var bullet = (Bullet)evt.data;
            bullet.EnemyHitEvent += AddStun;
        }

        private void AddStun(IPawn pawn)
        {
            pawn.dispatcher.Dispatch(ViewEvent.STUN_SPEED, true);
            StartCoroutine(RemoveStun(pawn));
        }

        private IEnumerator RemoveStun(IPawn pawn)
        {
            yield return new WaitForSeconds(StunTime);
            pawn.dispatcher.Dispatch(ViewEvent.STUN_SPEED, false);
        }
    }
}