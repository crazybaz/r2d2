using System;
using System.Collections;
using Paladin.UI;
using strange.extensions.context.impl;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace Assets.src.core
{
    public class SceneHelper
    {
        public static int ROOT_SCENE = 0;
        public static int HANGAR_SCENE = 1;
        public static int BATTLE_SCENE = 2;

        public static ContextView CurrentContext
        {
            get
            {
                return (Context.firstContext.GetContextView() as GameObject).GetComponent<ContextView>();
            }
        }

        public static void Load(int sceneId, Action<float> onLoadProgress)
        {
            CurrentContext.StartCoroutine(ALoad(sceneId, onLoadProgress));
        }

        public static void Activate()
        {
            CurrentContext.GetComponentInChildren<EventSystem>().gameObject.SetActive(false);
            Loader.allowSceneActivation = true;
        }

        private static IEnumerator SetActive(int sceneId, Action onComplete)
        {
            yield return new WaitForFixedUpdate();

            var lastActiveScene = SceneManager.GetActiveScene();
            var lastSceneName = lastActiveScene.name;

            var scene = GetSceneById(sceneId);
            SceneManager.SetActiveScene(scene);

            if (lastActiveScene.buildIndex != ROOT_SCENE)
            {
                SceneManager.UnloadScene(lastActiveScene);
                Debug.Log(">> SCENE UNLOADED :: " + lastSceneName);
            }

            foreach (var gameObject in scene.GetRootGameObjects())
            {
                var contextView = gameObject.GetComponent<ContextView>();
                if (contextView != null)
                {
                    Context.firstContext = contextView.context;
                    break;
                }
            }

            UIMediator.ClearAll();
            GameController.ClearALL();
            onComplete.Invoke();
        }

        private static AsyncOperation Loader;

        private static IEnumerator ALoad(int sceneId, Action<float> onLoadProgress)
        {
            yield return null;

            Loader = SceneManager.LoadSceneAsync(sceneId, LoadSceneMode.Additive);
            Loader.allowSceneActivation = false;

            // first phase - loading [0 - 0.9]
            while (Loader.progress < 0.9f)
            {
                onLoadProgress(Loader.progress);
                yield return null;
            }

            onLoadProgress(0.9f);

            while (!Loader.isDone)
                yield return null;

            yield return SetActive(sceneId, () =>
            {
                onLoadProgress(1);
            });
        }

        private static Scene GetSceneById(int sceneId)
        {
            for (var i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                if (scene.buildIndex == sceneId)
                    return scene;
            }
            throw new Exception("Scene with build index " + sceneId + " not found");
        }
    }
}