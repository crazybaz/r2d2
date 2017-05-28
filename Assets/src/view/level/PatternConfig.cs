using System.Collections.Generic;
using UnityEngine;

namespace Paladin.View
{
    [System.Serializable]
    public class Patterns
    {
        public List<MasterPattern> patterns;
    }

    public class PatternConfig : MonoBehaviour
    {
        public MasterPattern ZeroPattern;
        public List<Patterns> Patterns;
        public List<Patterns> Bosses;
    }
}