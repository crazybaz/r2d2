using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Paladin.Controller.Ability
{
    public class Magnet : AbilityBase
    {
        private int force;
        private int distance;

        public override void Prepare()
        {
            force = GetOption<int>("force");
            distance = GetOption<int>("distance");
        }

        public override void Activate()
        {
            Paladin.View.StartCoroutine(Magnetize());
        }

        /*public override void Deactivate()
        {
            PaladinModel.BuffModule.UnregisterBuff(buffId);
        }*/

        private IEnumerator Magnetize()
        {
            while (!Paladin.View.Disabled)
            {
                yield return new WaitForSeconds(0.1f);
                Update();
            }
        }

        private void Update()
        {
            var paladinPosition = Paladin.View.transform.position;

            // TODO: использовать общий пул всех дроп айтемов которые сейчас валяются на поле
            var items = GameObject.FindObjectsOfType<DropItemController>();
            items = items.Where(i => (i.transform.position - paladinPosition).magnitude <= distance).ToArray();

            if (items.Length > 0)
                Array.ForEach(items, i => i.GetComponent<Rigidbody>().AddForce((paladinPosition - i.transform.position) * force, ForceMode.Force));
        }
    }
}