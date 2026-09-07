using UnityEngine;
using UnityEngine.Serialization;
using DG.Tweening;

namespace KW.UI.Animation
{
    public abstract class UIAnimator : MonoBehaviour
    {
        public enum AnimationType
        {
            None,
            Fade,
            Scale,
            ScaleBounce,
            SlideUp,
            SlideDown,
            SlideLeft,
            SlideRight,
            PopUp,
            Shake,
            Rotate,
            Pulse
        }

        [Header("Animation Settings")]
        [SerializeField, FormerlySerializedAs("_showAnimation")] protected AnimationType showAnimation = AnimationType.Fade;
        [SerializeField, FormerlySerializedAs("_hideAnimation")] protected AnimationType hideAnimation = AnimationType.Fade;
        [SerializeField, FormerlySerializedAs("_showDuration")] protected float showDuration = 0.3f;
        [SerializeField, FormerlySerializedAs("_hideDuration")] protected float hideDuration = 0.2f;
        [SerializeField, FormerlySerializedAs("_showDelay")] protected float showDelay = 0f;
        [SerializeField, FormerlySerializedAs("_showEase")] protected Ease showEase = Ease.OutCubic;
        [SerializeField, FormerlySerializedAs("_hideEase")] protected Ease hideEase = Ease.InCubic;

        [Header("Animation Customization")]
        [SerializeField, FormerlySerializedAs("_slideDistance")] protected float slideDistance = 500f;
        [SerializeField, FormerlySerializedAs("_scaleMultiplier")] protected float scaleMultiplier = 1f;
        [SerializeField, FormerlySerializedAs("_rotationAngle")] protected float rotationAngle = 360f;

        [Header("References")]
        [SerializeField, FormerlySerializedAs("_animationTarget")] protected RectTransform animationTarget;
        [SerializeField, FormerlySerializedAs("_canvasGroup")] protected CanvasGroup canvasGroup;

        protected Vector3 originalScale;
        protected Vector2 originalPosition;
        protected Quaternion originalRotation;

        private Sequence cachedShowSequence;
        private Sequence cachedHideSequence;
        private System.Action pendingShowCallback;
        private System.Action pendingHideCallback;

        // ─── Unity Lifecycle ─────────────────────────────────────────────────────

        protected virtual void Awake()
        {
            if (animationTarget == null) animationTarget = GetComponent<RectTransform>();
            if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();

            if (animationTarget != null)
            {
                originalScale = animationTarget.localScale;
                originalPosition = animationTarget.anchoredPosition;
                originalRotation = animationTarget.localRotation;
            }
            // 시퀀스는 첫 재생 시 빌드 (lazy init) — Awake에서 미리 빌드하면
            // 재생되지 않는 인스턴스도 DOTween 풀을 소비함
        }

        protected virtual void OnDestroy()
        {
            cachedShowSequence?.Kill();
            cachedHideSequence?.Kill();
        }

        // ─── Public API ───────────────────────────────────────────────────────────

        public virtual void PlayShowAnimation(System.Action onComplete = null)
        {
            KillCurrentAnimation();

            if (showAnimation == AnimationType.None)
            {
                ResetToInitialState();
                onComplete?.Invoke();
                return;
            }

            ApplyShowInitialState();
            pendingShowCallback = onComplete;

            if (cachedShowSequence == null) BuildShowSequence();
            cachedShowSequence?.Restart();
        }

        public virtual void PlayHideAnimation(System.Action onComplete = null)
        {
            KillCurrentAnimation();

            if (hideAnimation == AnimationType.None)
            {
                onComplete?.Invoke();
                return;
            }

            pendingHideCallback = onComplete;

            if (cachedHideSequence == null) BuildHideSequence();
            cachedHideSequence?.Restart();
        }

        public virtual void KillCurrentAnimation()
        {
            if (cachedShowSequence != null && cachedShowSequence.IsActive() && cachedShowSequence.IsPlaying())
                cachedShowSequence.Pause();

            if (cachedHideSequence != null && cachedHideSequence.IsActive() && cachedHideSequence.IsPlaying())
                cachedHideSequence.Pause();
        }

        public virtual void ResetToInitialState()
        {
            KillCurrentAnimation();

            if (animationTarget != null)
            {
                animationTarget.localScale = originalScale;
                animationTarget.anchoredPosition = originalPosition;
                animationTarget.localRotation = originalRotation;
            }

            if (canvasGroup != null) canvasGroup.alpha = 1f;
        }

        // ─── Sequence Building ────────────────────────────────────────────────────

        private void BuildShowSequence()
        {
            cachedShowSequence?.Kill();
            cachedShowSequence = null;
            if (showAnimation == AnimationType.None) return;

            cachedShowSequence = DOTween.Sequence().SetAutoKill(false).Pause();
            if (showDelay > 0f) cachedShowSequence.PrependInterval(showDelay);

            AppendShowTweens(cachedShowSequence);
            cachedShowSequence.OnComplete(() => pendingShowCallback?.Invoke());
        }

        private void BuildHideSequence()
        {
            cachedHideSequence?.Kill();
            cachedHideSequence = null;
            if (hideAnimation == AnimationType.None) return;

            cachedHideSequence = DOTween.Sequence().SetAutoKill(false).Pause();
            AppendHideTweens(cachedHideSequence);
            cachedHideSequence.OnComplete(() => pendingHideCallback?.Invoke());
        }

        // ─── Initial State ────────────────────────────────────────────────────────
        // PrependCallback은 Restart() 시 재실행이 보장되지 않으므로 직접 호출

        private void ApplyShowInitialState()
        {
            switch (showAnimation)
            {
                case AnimationType.Fade:
                case AnimationType.Shake:
                    if (canvasGroup) canvasGroup.alpha = 0f;
                    break;

                case AnimationType.Scale:
                case AnimationType.ScaleBounce:
                    if (animationTarget) animationTarget.localScale = Vector3.zero;
                    break;

                case AnimationType.SlideUp:
                case AnimationType.SlideDown:
                case AnimationType.SlideLeft:
                case AnimationType.SlideRight:
                    if (animationTarget) animationTarget.anchoredPosition = originalPosition + GetSlideShowDir(showAnimation) * slideDistance;
                    if (canvasGroup) canvasGroup.alpha = 0f;
                    break;

                case AnimationType.PopUp:
                    if (animationTarget) animationTarget.localScale = Vector3.zero;
                    if (canvasGroup) canvasGroup.alpha = 0f;
                    break;

                case AnimationType.Rotate:
                    if (animationTarget)
                    {
                        animationTarget.localRotation = Quaternion.Euler(0f, 0f, rotationAngle);
                        animationTarget.localScale = Vector3.zero;
                    }
                    if (canvasGroup) canvasGroup.alpha = 0f;
                    break;
            }
        }

        // ─── Tween Builders ───────────────────────────────────────────────────────

        private void AppendShowTweens(Sequence s)
        {
            switch (showAnimation)
            {
                case AnimationType.Fade:
                    if (canvasGroup) s.Append(canvasGroup.DOFade(1f, showDuration).SetEase(showEase));
                    break;

                case AnimationType.Scale:
                    if (animationTarget) s.Append(animationTarget.DOScale(originalScale * scaleMultiplier, showDuration).SetEase(showEase));
                    break;

                case AnimationType.ScaleBounce:
                    if (animationTarget) s.Append(animationTarget.DOScale(originalScale * scaleMultiplier, showDuration).SetEase(Ease.OutBack));
                    break;

                case AnimationType.SlideUp:
                case AnimationType.SlideDown:
                case AnimationType.SlideLeft:
                case AnimationType.SlideRight:
                    if (animationTarget)
                    {
                        s.Append(animationTarget.DOAnchorPos(originalPosition, showDuration).SetEase(showEase));
                        if (canvasGroup) s.Join(canvasGroup.DOFade(1f, showDuration * 0.5f));
                    }
                    break;

                case AnimationType.PopUp:
                    if (animationTarget)
                    {
                        s.Append(animationTarget.DOScale(originalScale * scaleMultiplier, showDuration).SetEase(Ease.OutBack));
                        if (canvasGroup) s.Join(canvasGroup.DOFade(1f, showDuration * 0.7f));
                    }
                    break;

                case AnimationType.Shake:
                    if (canvasGroup) s.Append(canvasGroup.DOFade(1f, showDuration * 0.3f));
                    if (animationTarget) s.Append(animationTarget.DOShakePosition(showDuration * 0.5f, strength: 10f, vibrato: 20));
                    break;

                case AnimationType.Rotate:
                    if (animationTarget)
                    {
                        s.Append(animationTarget.DORotateQuaternion(originalRotation, showDuration).SetEase(showEase));
                        s.Join(animationTarget.DOScale(originalScale * scaleMultiplier, showDuration).SetEase(showEase));
                        if (canvasGroup) s.Join(canvasGroup.DOFade(1f, showDuration * 0.5f));
                    }
                    break;

                case AnimationType.Pulse:
                    if (animationTarget)
                    {
                        float pulseScale = scaleMultiplier * 1.1f;
                        s.Append(animationTarget.DOScale(originalScale * pulseScale, showDuration * 0.5f).SetEase(Ease.OutQuad));
                        s.Append(animationTarget.DOScale(originalScale, showDuration * 0.5f).SetEase(Ease.InQuad));
                    }
                    break;
            }
        }

        private void AppendHideTweens(Sequence s)
        {
            switch (hideAnimation)
            {
                case AnimationType.Fade:
                case AnimationType.Shake:
                case AnimationType.Pulse:
                    if (canvasGroup) s.Append(canvasGroup.DOFade(0f, hideDuration).SetEase(hideEase));
                    break;

                case AnimationType.Scale:
                case AnimationType.ScaleBounce:
                case AnimationType.PopUp:
                    if (animationTarget) s.Append(animationTarget.DOScale(Vector3.zero, hideDuration).SetEase(hideEase));
                    break;

                case AnimationType.SlideUp:
                case AnimationType.SlideDown:
                case AnimationType.SlideLeft:
                case AnimationType.SlideRight:
                    if (animationTarget)
                    {
                        Vector2 endPos = originalPosition + GetSlideHideDir(hideAnimation) * slideDistance;
                        s.Append(animationTarget.DOAnchorPos(endPos, hideDuration).SetEase(hideEase));
                        if (canvasGroup) s.Join(canvasGroup.DOFade(0f, hideDuration));
                    }
                    break;

                case AnimationType.Rotate:
                    if (animationTarget)
                    {
                        s.Append(animationTarget.DORotate(new Vector3(0f, 0f, rotationAngle), hideDuration).SetEase(hideEase));
                        s.Join(animationTarget.DOScale(Vector3.zero, hideDuration).SetEase(hideEase));
                        if (canvasGroup) s.Join(canvasGroup.DOFade(0f, hideDuration));
                    }
                    break;
            }
        }

        // ─── Helpers ──────────────────────────────────────────────────────────────

        private static Vector2 GetSlideShowDir(AnimationType type) => type switch
        {
            AnimationType.SlideUp => Vector2.down,
            AnimationType.SlideDown => Vector2.up,
            AnimationType.SlideLeft => Vector2.right,
            AnimationType.SlideRight => Vector2.left,
            _ => Vector2.zero
        };

        private static Vector2 GetSlideHideDir(AnimationType type) => type switch
        {
            AnimationType.SlideUp => Vector2.up,
            AnimationType.SlideDown => Vector2.down,
            AnimationType.SlideLeft => Vector2.left,
            AnimationType.SlideRight => Vector2.right,
            _ => Vector2.zero
        };
    }
}
