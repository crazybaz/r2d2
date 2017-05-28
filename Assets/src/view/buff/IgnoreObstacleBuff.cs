using UnityEngine;

namespace Paladin.View
{
    public class IgnoreObstacleBuff : BuffView
    {
        private Renderer paladinRenderer;
        private Material origMaterial;
        private Hologram_Controller holoController;

        public IgnoreObstacleBuff()
        {
            Type = BuffType.IgnoreObstacle;
        }

        public override void Activate()
        {
            base.Activate();

            paladinRenderer = Paladin.View.GetComponentInChildren<Renderer>();
            origMaterial = paladinRenderer.material;

            Paladin.View.IgnoreObstacleCollisions = true;
            paladinRenderer.material = Paladin.View.HologramMaterial;
            holoController = paladinRenderer.gameObject.AddComponent<Hologram_Controller>();
        }

        public override void Deactivate()
        {
            base.Deactivate();

            Paladin.View.IgnoreObstacleCollisions = false;
            paladinRenderer.material = origMaterial;

            Destroy(holoController);
        }
    }
}