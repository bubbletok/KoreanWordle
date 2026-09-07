using UnityEditor;
using UnityEngine;

namespace KW.Editor
{
    public class KWEditor<T> : UnityEditor.Editor where T : MonoBehaviour
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
        }
    }
}