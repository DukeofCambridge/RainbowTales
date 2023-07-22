using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Rainbow.Transition
{
    public class TransitionManager : MonoBehaviour
    {
        [SceneName]
        public string startSceneName = string.Empty;
        private CanvasGroup _fadeCanvasGroup;
        private bool _isFade;
        private void OnEnable()
        {
            EventHandler.TransitionEvent += OnTransitionEvent;
        }

        private void OnDisable()
        {
            EventHandler.TransitionEvent -= OnTransitionEvent;
        }



        private IEnumerator Start()
        {
            _fadeCanvasGroup = FindObjectOfType<CanvasGroup>();
            yield return LoadSceneSetActive(startSceneName);
            EventHandler.CallAfterSceneLoadedEvent();
        }


        private void OnTransitionEvent(string sceneToGo, Vector3 positionToGo)
        {
            //you cannot transit during a transition
            if (!_isFade)
            {
                StartCoroutine(Transition(sceneToGo, positionToGo));
            }
            
        }

        /// <summary>
        /// 场景切换
        /// </summary>
        /// <param name="sceneName">目标场景</param>
        /// <param name="targetPosition">目标位置</param>
        /// <returns></returns>
        private IEnumerator Transition(string sceneName, Vector3 targetPosition)
        {
            EventHandler.CallBeforeSceneUnloadEvent();
            yield return Fade(1);

            yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());

            yield return LoadSceneSetActive(sceneName);
            //移动人物坐标
            EventHandler.CallMoveToPosition(targetPosition);
            EventHandler.CallAfterSceneLoadedEvent();
            yield return Fade(0);
        }

        /// <summary>
        /// 加载场景并设置为激活
        /// </summary>
        /// <param name="sceneName">场景名</param>
        /// <returns></returns>
        private IEnumerator LoadSceneSetActive(string sceneName)
        {
            yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

            Scene newScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);

            SceneManager.SetActiveScene(newScene);
        }
        /// <summary>
        /// 淡入淡出场景
        /// </summary>
        /// <param name="targetAlpha">1是黑，0是透明</param>
        /// <returns></returns>
        private IEnumerator Fade(float targetAlpha)
        {
            _isFade = true;

            _fadeCanvasGroup.blocksRaycasts = true;

            //Mathf method can be replaced by DOFade()
            float speed = Mathf.Abs(_fadeCanvasGroup.alpha - targetAlpha) / Settings.MaskFadeDuration;
            while (!Mathf.Approximately(_fadeCanvasGroup.alpha, targetAlpha))
            {
                _fadeCanvasGroup.alpha = Mathf.MoveTowards(_fadeCanvasGroup.alpha, targetAlpha, speed * Time.deltaTime);
                yield return null;
            }

            _fadeCanvasGroup.blocksRaycasts = false;

            _isFade = false;
        }
    }
}