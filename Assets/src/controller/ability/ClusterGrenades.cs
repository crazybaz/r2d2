using UnityEngine;
using Random = UnityEngine.Random;

namespace Paladin.Controller.Ability
{
    public class ClusterGrenades : AbilityBase
    {
        // values from def
        private int amount;
        private int minDistance;
        private int maxDistance;
        private int targetAngle;
        private GameObject bullet;

        public override void Prepare()
        {
            amount = GetOption<int>("amount");
            minDistance = GetOption<int>("minDistance");
            maxDistance = GetOption<int>("maxDistance");
            targetAngle = GetOption<int>("targetAngle");
            bullet = Resources.Load<GameObject>("prefabs/bullets/" + GetOption<string>("bullet"));
        }

        public override void Activate()
        {
            var rotationDelta = targetAngle / amount;
            var rotation = Quaternion.Euler(new Vector3(0, -targetAngle * .5f, 0));
            var position = Paladin.View.transform.position;
            position.y = global::Config.I.PALADIN_HALF_HEIGHT;

            for (var i = 0; i < amount; i++)
            {
                rotation = Quaternion.Euler(0, rotation.eulerAngles.y + rotationDelta, 0);
                var bulletObject = GameObject.Instantiate(this.bullet, position, rotation) as GameObject;

                var bullet = bulletObject.GetComponent<Bullet>();
                if (bullet == null)
                    throw new MissingComponentException("Bullet is missing");

                bullet.WeaponOwnerType = WeaponOwnerType.Paladin;
                bullet.WeaponMultiplier = 1;
                bullet.MovingConfig = new MovingConfig
                {
                    MovingType = MovingType.Parabolic,
                    Distance = (uint) Random.Range(minDistance, maxDistance)
                };
            }
        }
    }
}