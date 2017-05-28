using System.Collections;
using UnityEngine;

namespace Paladin.Controller.Ability
{
    public class Rocket : AbilityBase
    {
        private GameObject rocketPrefab;
        private float reloadTime;
        private int amount;

        private float currentTime;

        public override void Prepare()
        {
            rocketPrefab = Resources.Load<GameObject>("prefabs/bullets/" + GetOption<string>("bullet"));
            reloadTime = GetOption<float>("reloadTime");
            amount = GetOption<int>("amount");
        }

        public override void Activate()
        {
            if (rocketPrefab.GetComponent<Bullet>() == null)
                throw new MissingComponentException("Bullet is missing");

            Paladin.View.StartCoroutine(Produce());
        }

        private IEnumerator Produce()
        {
            while (!Paladin.View.Disabled)
            {
                yield return new WaitForEndOfFrame();
                Update();
            }
        }

        private void Update()
        {
            currentTime += Time.deltaTime;
            if (currentTime >= reloadTime)
            {
                currentTime = 0;

                for (var i = 0; i < amount; i++)
                {
                    var position = Paladin.View.transform.position;
                    position.y = global::Config.I.PALADIN_HALF_HEIGHT;

                    var rotation = Random.rotation;
                    rotation.x = 0;
                    rotation.z = 0;

                    var rocket = GameObject.Instantiate(rocketPrefab, position, rotation) as GameObject;

                    var bullet = rocket.GetComponent<Bullet>();
                    bullet.WeaponOwnerType = WeaponOwnerType.Paladin;
                    bullet.MovingConfig = new MovingConfig
                    {
                        MovingType = MovingType.Aiming,
                        AngleSpeed = 360,
                        AimingAngle = 360f,
                        AimingCooldown = 0.1f,
                        DetectingRadius = 300
                    };
                }
            }
        }

        /*public override void Deactivate()
        {
            PaladinModel.BuffModule.UnregisterBuff(buffId);
        }*/
    }
}