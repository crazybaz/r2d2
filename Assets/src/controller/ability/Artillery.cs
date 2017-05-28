using System.Collections;
using System.Collections.Generic;
using Paladin.View;
using UnityEngine;

namespace Paladin.Controller.Ability
{
    public class Artillery : AbilityBase
    {
        private List<ArtilleryView> views;
        private GameObject prefab;
        private GameObject bulletPrefab;
        private float fireCooldown = 0.2f;

        public override void Prepare()
        {
            prefab = Resources.Load<GameObject>("prefabs/ability/ArtilleryAbility");
            bulletPrefab = Resources.Load<GameObject>("prefabs/bullets/" + GetOption<string>("bullet"));
        }

        public override void Activate()
        {
            views = new List<ArtilleryView>();
            for (var i = 0; i < GetOption<int>("amount"); i++)
            {
                var xOrigin = Random.Range(global::Config.I.GAMEPLAY_BOUNDS.xMin + 10,
                    global::Config.I.GAMEPLAY_BOUNDS.xMax - 10);
                var zOrigin = Random.Range(global::Config.I.GAMEPLAY_BOUNDS.zMax - 140,
                    global::Config.I.GAMEPLAY_BOUNDS.zMax - 100);

                var abilityObject = GameObject.Instantiate(prefab, new Vector3(xOrigin, 0, zOrigin), Quaternion.identity) as GameObject;
                views.Add(abilityObject.GetComponent<ArtilleryView>());
            }

            Paladin.View.StartCoroutine(LounchBombs());
        }

        IEnumerator LounchBombs()
        {
            foreach (var view in views)
            {
                DropBomb(view);
                yield return new WaitForSeconds(fireCooldown);
            }

            foreach (var view in views)
                GameObject.Destroy(view.gameObject, 2.5f);
        }

        private void DropBomb(ArtilleryView view)
        {
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