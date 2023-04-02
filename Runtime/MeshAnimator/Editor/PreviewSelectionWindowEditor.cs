#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Services.Optimization.MeshAnimationSystem
{

    public class PreviewSelectionWindowEditor : EditorWindow
    {
        private MeshAnimatorEditor animatorEditor;
        public static void Open(MeshAnimatorEditor animatorEditor)
        {
            PreviewSelectionWindowEditor window = GetWindow<PreviewSelectionWindowEditor>("Animation Preview Selection");
            window.animatorEditor = animatorEditor;
        }

        private void OnGUI()
        {
            int count = animatorEditor.animator.controller.states.Count;
            for (int i = 0; i < count; i++)
            {
                string menuName = animatorEditor.animator.controller.states[i].animation.name;
                if (GUILayout.Button(menuName))
                {
                    animatorEditor.SelectPreviewIndex(i);
                    Close();
                    return;
                }
            }
        }
    }
}
#endif