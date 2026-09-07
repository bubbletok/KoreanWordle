using UnityEngine;
using UnityEngine.Serialization;
using TMPro;
using KW.Core.Settings;

namespace KW.UI
{
    [RequireComponent(typeof(TMP_Text))]
    [ExecuteInEditMode]
    public class FontApplier : MonoBehaviour
    {
        [SerializeField, FormerlySerializedAs("_applyOnAwake")] private bool applyOnAwake = true;

        private TMP_Text textComponent;

        private void Awake()
        {
            if (applyOnAwake && Application.isPlaying)
            {
                ApplyFont();
            }
        }

        [ContextMenu("Apply Font")]
        public void ApplyFont()
        {
            if (textComponent == null)
            {
                textComponent = GetComponent<TMP_Text>();
            }

            if (textComponent == null || UILayoutSettings.Instance == null)
            {
                return;
            }

            var settings = UILayoutSettings.Instance;

            if (settings.defaultFont != null)
            {
                textComponent.font = settings.defaultFont;
            }
        }
    }
}
