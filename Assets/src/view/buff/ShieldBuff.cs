using UnityEngine;

namespace Paladin.View
{
    public class ShieldBuff : BuffView
    {
        public int MaxHealth;
        public GameObject AppearEffect;
        public GameObject KickassEffect;
        public float AppearAnimationTime = 0.3f;
        public uint ColliderRadius = 15;
        public uint ColliderHeight = 60;

        private float health;
        private GameObject shieldEffect;
        private CapsuleCollider defenceCollider;

        public ShieldBuff()
        {
            Type = BuffType.Shield;
        }

        protected override void Awake()
        {
            base.Awake();

            if (AppearEffect == null)
                AppearEffect = Resources.Load<GameObject>("prefabs/fx/fx_shield_01");

            if (KickassEffect == null)
                KickassEffect = Resources.Load<GameObject>("prefabs/fx/fx_shield_explosion");
        }

        public override void Activate()
        {
            base.Activate();

            // fx
            shieldEffect = Instantiate(AppearEffect, transform, false) as GameObject;
            var pos = shieldEffect.transform.position;
            pos.y = Config.I.PALADIN_HALF_HEIGHT;
            shieldEffect.transform.position = pos;

            // коллайдер
            defenceCollider = gameObject.AddComponent<CapsuleCollider>();
            defenceCollider.isTrigger = true;
            defenceCollider.radius = ColliderRadius;
            defenceCollider.height = ColliderHeight;
            pos = defenceCollider.center;
            pos.y = ColliderHeight * 0.5f;
            defenceCollider.center = pos;

            // выставляем здоровье
            health = MaxHealth;

            // анимация появления
            shieldEffect.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            iTween.ScaleTo(shieldEffect, Vector3.one, AppearAnimationTime);

            for (int i = 0; i < shieldEffect.transform.childCount; i++)
            {
                var shieldEffectChild = shieldEffect.transform.GetChild(i);
                shieldEffectChild.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                iTween.ScaleTo(shieldEffectChild.gameObject, Vector3.one, AppearAnimationTime);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            // событие столкновения с пулей
            var bullet = other.GetComponent<Bullet>();
            if (bullet != null && bullet.WeaponOwnerType == WeaponOwnerType.Enemy)
            {
                var fx = Instantiate(KickassEffect, bullet.transform.position, Quaternion.identity) as GameObject;
                fx.transform.parent = transform;
                Destroy(fx, 5f);

                health -= bullet.Damage;
                if (health <= 0)
                    Deactivate();
            }
        }

        public override void Deactivate()
        {
            base.Deactivate();

            Destroy(defenceCollider);
            defenceCollider = null;

            // анимация исчезновения
            iTween.ScaleTo(shieldEffect, Vector3.zero, AppearAnimationTime);

            for (int i = 0; i < shieldEffect.transform.childCount; i++)
                iTween.ScaleTo(shieldEffect.transform.GetChild(i).gameObject, Vector3.zero, AppearAnimationTime);
        }

        protected override void OnDestroy()
        {
            Destroy(shieldEffect);
            shieldEffect = null;
            AppearEffect = null;
            KickassEffect = null;
            base.Awake();
        }
    }
}