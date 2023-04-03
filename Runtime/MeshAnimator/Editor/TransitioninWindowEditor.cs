#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Services.Optimization.MeshAnimationSystem
{
    public class TransitioninWindowEditor : EditorWindow
    {
        private MeshAniamtorControllerEditorWindow animatorEditor;
        private MeshAnimationState selectingState;

        public static void Open(MeshAniamtorControllerEditorWindow aniamtorEditor)
        {
            TransitioninWindowEditor window = GetWindow<TransitioninWindowEditor>("Transition Selection");
            window.animatorEditor = aniamtorEditor;
        }

        private void OnGUI()
        {
            int stateCount = animatorEditor.animator.states.Count;
            int selectingStateIndex = animatorEditor.currentSelectingStateIndex;

            if (stateCount < 1)
            {
                EditorGUILayout.LabelField("Other state is not validate to add state");
                return;
            }

            selectingState = animatorEditor.selectingState;

            for (int i = 0; i < stateCount; i++)
            {
                if (i == selectingStateIndex)
                    continue;

                if (IsTargetStateTransitionExited(i, selectingState))
                    continue;

                string stateAnimationName = animatorEditor.animator.states[i].animation.name;

                if (GUILayout.Button(stateAnimationName))
                {
                    animatorEditor.AddTransitionToSelectingState(i);
                    animatorEditor.SetSelectingStateStatus(false);
                    Close();
                    return;
                }
            }
        }

        private bool IsTargetStateTransitionExited(int targetIndex, MeshAnimationState selectingState)
        {
            int transitionCount = selectingState.transitionInfos.Count;
            for (int i = 0; i < transitionCount; i++)
            {
                if (targetIndex == selectingState.transitionInfos[i].targetStateIndex)
                    return true;
            }

            return false;
        }

        private void OnDisable()
        {
            animatorEditor.SetSelectingStateStatus(false);
        }
    }
}
#endif