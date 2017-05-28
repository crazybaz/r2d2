using System;
using System.Collections.Generic;
using System.Linq;
using strange.extensions.context.api;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Paladin.View
{
    public class LevelFactory
    {
        [Inject]
        public Logic Logic { get; set; }

        [Inject]
        public GameLogic GameLogic { get; set; }

        [Inject(ContextKeys.CONTEXT_VIEW)]
        public GameObject ContextView { get; set; }

        public LevelConfig LevelConfig;

        private List<MasterPattern> levelData;
        private PatternConfig patternConfig;

        public void Init()
        {
            levelData = new List<MasterPattern>();
            patternConfig = ContextView.GetComponent<PatternConfig>();
            LevelConfig = Logic.Defs.GetLevel(Logic.Level);

#if UNITY_EDITOR
            if (GameLogic.LocalConfig.FakeLevelGeneration)
            {
                foreach (var item in patternConfig.Patterns)
                {
                    levelData.AddRange(item.patterns);
                }
                return;
            }
#endif

            foreach (var patternLevel in LevelConfig.Patterns.Keys)
            {
                var patternCount = LevelConfig.Patterns[patternLevel];

                while (patternCount > 0)
                {
                    patternCount -= 1;
                    var patterns = patternConfig.Patterns[(int)patternLevel - 1].patterns;
                    var pattern = patterns[Random.Range(0, patterns.Count)];
                    pattern.Level = patternLevel;
                    levelData.Add(pattern);
                }
            }

            //Debug.Log("SORT BEFORE");
            //foreach (var p in level)
            //    Debug.Log(p.Level);

            var rnd = new System.Random();
            levelData = levelData.OrderBy(item => rnd.Next()).ToList();

            //Debug.Log("SORT AFTER");
            //foreach (var p in level)
            //    Debug.Log(p.Level);


            // Add boss pattern
            if (patternConfig.Bosses.Count < LevelConfig.BossLevel)
                throw new Exception("Error, boss patterns not found for level: " + LevelConfig.BossLevel);

            var bossPatterns = patternConfig.Bosses[(int)LevelConfig.BossLevel - 1].patterns;
            var bossPattern = bossPatterns[Random.Range(0, bossPatterns.Count)];
            bossPattern.Level = LevelConfig.BossLevel;
            levelData.Add(bossPattern);
        }

        public MasterPattern ZeroPattern
        {
            get { return patternConfig.ZeroPattern; }
        }

        public bool HasNext
        {
            get { return levelData.Count > 0; }
        }

        public MasterPattern Next()
        {
            var pattern = levelData[0];
            levelData.RemoveAt(0);

#if UNITY_EDITOR
            if (GameLogic.LocalConfig.FakeLevelGeneration)
                levelData.Add(pattern);
#endif

            return pattern;
        }
    }
}