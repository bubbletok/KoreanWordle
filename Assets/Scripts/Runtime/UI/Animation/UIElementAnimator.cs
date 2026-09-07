using UnityEngine;
using UnityEngine.Serialization;
using DG.Tweening;

namespace KW.UI.Animation
{
    public class UIElementAnimator : UIAnimator
    {
        [Header("Auto Play Settings")]
        [SerializeField, FormerlySerializedAs("_playOnEnable")] private bool playOnEnable = true;
        [SerializeField, FormerlySerializedAs("_resetOnDisable")] private bool resetOnDisable = false;

        private Tween delayedCallTween;

        private void OnEnable()
        {
            if (playOnEnable)
                PlayShowAnimation();
        }

        private void OnDisable()
        {
            delayedCallTween?.Kill();
            delayedCallTween = null;

            if (resetOnDisable)
                ResetToInitialState();
            else
                KillCurrentAnimation();
        }

        protected override void OnDestroy()
        {
            delayedCallTween?.Kill();
            base.OnDestroy();
        }

        public void Play()
        {
            PlayShowAnimation();
        }

        public void PlayDelayed(float delay)
        {
            delayedCallTween?.Kill();
            KillCurrentAnimation();
            delayedCallTween = DOVirtual.DelayedCall(delay, Play, ignoreTimeScale: false);
        }

        // Loops by chaining callbacks on the cached sequence — no new allocations per iteration
        public void PlayLoop(int loopCount = -1)
        {
            KillCurrentAnimation();
            ScheduleLoop(loopCount);
        }

        public void StopLoop()
        {
            KillCurrentAnimation();
        }

        private void ScheduleLoop(int remaining)
        {
            PlayShowAnimation(() =>
            {
                if (remaining == 0) return;
                ScheduleLoop(remaining > 0 ? remaining - 1 : -1);
            });
        }
    }
}
