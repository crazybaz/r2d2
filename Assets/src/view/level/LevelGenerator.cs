using strange.extensions.mediation.impl;
using UnityEngine;

namespace Paladin.View
{
    public class LevelGenerator : EventView
    {
        private float startDelay;

        private MasterPattern lastPattern;
        private MasterPattern zeroPattern;

        private GameConfig config;
        private LevelFactory levelProvider;

        public void Init(GameConfig config, LevelFactory provider)
        {
            levelProvider = provider;
            this.config = config;
            startDelay = config.PATTERN_START_DELAY;

            // add zero pattern
            var pattern = HierarchicalPrefabUtility.Instantiate(levelProvider.ZeroPattern.gameObject, new Vector3(0, 0, 0), Quaternion.identity);
            zeroPattern = pattern.GetComponent<MasterPattern>();
            zeroPattern.IsZeroPattern = true;
            zeroPattern.transform.parent = transform;
        }

        private void FixedUpdate()
        {
            if (startDelay < 0)
                if (levelProvider.HasNext)
                    ProcessPatterns();
                else
                    ProcessBossPattern();
            else
                startDelay -= Time.fixedDeltaTime;
        }

        private void ProcessPatterns()
        {
            if (lastPattern == null || lastPattern.Bounds.transform.position.z + lastPattern.Bounds.bounds.size.z < config.PATTERN_APPEAR_Z)
                AddNewPattern();
        }

        private void ProcessBossPattern()
        {
            if (lastPattern.Bounds.transform.position.z <= Config.I.GAMEPLAY_BOUNDS.zMin)
            {
                dispatcher.Dispatch(ViewEvent.BOSS_PATTERN_ARRIVED);
                enabled = false;
            }
        }

        private void AddNewPattern()
        {
            var pattern = levelProvider.Next();

            var newPattern = HierarchicalPrefabUtility
                .Instantiate(pattern.gameObject, new Vector3(0, 0, 0), Quaternion.identity)
                .GetComponent<MasterPattern>();

            newPattern.LastPattern = lastPattern == null ? zeroPattern.gameObject : lastPattern.gameObject;

            lastPattern = newPattern;

            lastPattern.transform.parent = transform;
        }
    }
}