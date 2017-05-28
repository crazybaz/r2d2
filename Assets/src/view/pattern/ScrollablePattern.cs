using System;
using System.Collections;
using SWS;
using UnityEngine;

namespace Paladin.View
{
    public class ScrollablePattern : Pattern
    {
        private GameConfig config;
        private BoxCollider boundsCollider;
        private float boundsTotalSize;

        private const string SURFACE_TAG = "Surface";

        public void Init(GameConfig config)
        {
            this.config = config;
        }

        protected override void Awake()
        {
            base.Awake();

#if UNITY_EDITOR
            try
            {
                Utils.FindInParent<MasterPattern>(transform);
            }
            catch
            {
                return;
            }
#endif

            Transform[] allChildren = Utils.FindInParent<MasterPattern>(transform).GetComponentsInChildren<Transform>();
            Transform child;
            for (int i = 0; i < allChildren.Length; i++)
            {
                child = allChildren[i];
                if (child.gameObject.tag == SURFACE_TAG)
                {
                    boundsCollider = child.gameObject.GetComponent<BoxCollider>();
                    break;
                }
            }

            if (boundsCollider == null)
                throw new Exception("There is no child with tag '" + SURFACE_TAG + "' in the pattern");
            else
                boundsTotalSize = boundsCollider.bounds.extents.z + boundsCollider.bounds.size.z * boundsCollider.center.y / boundsCollider.size.y;
        }

        protected override void Start()
        {
            base.Start();

            if (Type == PatternType.dp)
            {

#if UNITY_EDITOR
                try
                {
                    Utils.FindInParent<MasterPattern>(transform);
                }
                catch
                {
                    return;
                }
#endif

                var staticPattern = Utils.FindInParent<MasterPattern>(transform).StaticPattern;

                var movers = GetComponentsInChildren<splineMove>(true);
                foreach (var mover in movers)
                {
                    for (var i = 0; i < staticPattern.transform.childCount; i++)
                    {
                        var child = staticPattern.transform.GetChild(i);
                        if (child.tag == mover.tag)
                        {
                            mover.SetPath(child.GetComponent<PathManager>());
                            (mover.pathContainer as BezierPathManager).CalculatePath();
                        }
                    }
                }
            }
            StartCoroutine(DetectPatternIsOutOfBounds());
        }

        private IEnumerator DetectPatternIsOutOfBounds()
        {

#if UNITY_EDITOR
            if (boundsCollider == null)
                yield break;
#endif
            while (true)
            {
                if (boundsCollider.transform.position.z + boundsTotalSize < Config.I.GAMEPLAY_BOUNDS.xMin)
                {
                    //Debug.Log(">> SP DESTROYED");
                    Destroyed = true;
                }
                yield return new WaitForSecondsRealtime(1);
            }
        }

        protected override void OnDestroy()
        {
            config = null;
            boundsCollider = null;
            base.OnDestroy();
        }
    }
}