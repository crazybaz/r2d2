using System.Collections;
using Paladin.View;
using UnityEngine;

namespace Paladin.Controller.Ability
{
    public class VacuumBomb : AbilityBase
    {
        private int countDown = 3;
        private VacuumBombView view;
        private GameObject prefab;
        private GameObject bulletPrefab;

        public override void Prepare()
        {
            prefab = Resources.Load<GameObject>("prefabs/ability/VacuumBombAbility");
            bulletPrefab = Resources.Load<GameObject>("prefabs/bullets/" + GetOption<string>("bullet"));
        }

        public override void Activate()
        {
            var abilityObject =
                GameObject.Instantiate(prefab, new Vector3(0, 0, global::Config.I.GAMEPLAY_BOUNDS.zMax - 140),
                    Quaternion.identity) as GameObject;
            view = abilityObject.GetComponent<VacuumBombView>();

            Paladin.View.StartCoroutine(CountDown());
            Paladin.View.StartCoroutine(DropBomb());

            GameObject.Destroy(view.gameObject, 3f);
        }

        private IEnumerator CountDown()
        {
            while (countDown >= 0)
            {
                view.Counter.text = countDown.ToString();
                yield return new WaitForSeconds(0.6f);
                countDown -= 1;
            }
        }

        private IEnumerator DropBomb()
        {
            yield return new WaitForSeconds(0);

            var bulletObject = GameObject.Instantiate(bulletPrefab, view.Origin.transform.position, Quaternion.identity) as GameObject;
            bulletObject.transform.parent = view.Origin.transform;

            var bullet = bulletObject.GetComponent<Bullet>();
            if (bullet == null)
                throw new MissingComponentException("Bullet is missing");

            bullet.DestroyOutsideBounds = false;
            bullet.WeaponOwnerType = WeaponOwnerType.Paladin;
            bullet.MovingConfig = new MovingConfig
            {
                MovingType = MovingType.Aiming,
                AngleSpeed = 100,
                TargetObject = view.Target
            };
        }
    }
}