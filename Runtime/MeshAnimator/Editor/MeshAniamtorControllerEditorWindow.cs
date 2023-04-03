#if UNITY_EDITOR
using Services.Utility;
using UnityEditor;
using UnityEngine;

namespace Services.Optimization.MeshAnimationSystem
{
    public class MeshAniamtorControllerEditorWindow : EditorWindow
    {
        public MeshAnimatorController animator { get; private set; }
        public MeshAnimationState selectingState 
        { 
            get => animator.states[selectingStateIndex]; 
        }

        public int currentSelectingStateIndex
        {
            get => selectingStateIndex;
        }

        private MeshAnimationState state;
        private GUIStyle headerInfoTitleStyle;
        private GUIStyle previewTitleStyle;
        private GUILayoutOption[] listLayout = new GUILayoutOption[2];
        private int selectingStateIndex = 0;
        private float stateListItemSize = 22f;
        private float stateListSelectHeight;
        private string[] stateListContent;

        private bool isSelectingState = false;

        public static void Open(MeshAnimatorController target)
        {
            MeshAniamtorControllerEditorWindow window = GetWindow<MeshAniamtorControllerEditorWindow>("Mesh Animator Editor");
            window.SetDispalyInfo(target);
        }

        public void SetDispalyInfo(MeshAnimatorController target)
        {
            animator = target;

            selectingStateIndex = 0;
        }

        private void OnGUI()
        {
            headerInfoTitleStyle = SetupTextGUIStyle(headerInfoTitleStyle, 13);
            previewTitleStyle = SetupTextGUIStyle(previewTitleStyle, 10);

            if (stateListContent == null)
            {
                int count = animator.states.Count;
                stateListContent = new string[count];

                for (int i = 0; i < count; i++)
                    stateListContent[i] = animator.states[i].animation.name;

                stateListSelectHeight = stateListItemSize * count;
            }

            DrawWindowTitle();

            if (!animator)
                return;

            EditorView.HorizontalGroup(EditorKey.GroupBox, () =>
            {
                DrawStateList();

                DrawWorkSpace();

            });
        }

        private GUIStyle SetupTextGUIStyle(GUIStyle gUIStyle, int fontSize)
        {
            if (gUIStyle == null)
                gUIStyle = new GUIStyle(GUI.skin.label);

            gUIStyle.alignment = TextAnchor.MiddleCenter;
            gUIStyle.fontSize = fontSize;
            return gUIStyle;
        }

        private void DrawWindowTitle()
        {
            EditorView.VerticalGroup(EditorKey.GroupBox, () =>
            {
                if (animator)
                    EditorGUILayout.LabelField(animator.name, headerInfoTitleStyle);
                else
                    EditorGUILayout.LabelField("Select your mesh animator info", headerInfoTitleStyle);
            });
        }

        private void DrawStateList()
        {
            listLayout[0] = GUILayout.Width(200);
            listLayout[1] = GUILayout.ExpandHeight(true);

            EditorView.VerticalGroup(EditorKey.GroupBox, listLayout, () =>
            {
                selectingStateIndex = GUILayout.SelectionGrid(selectingStateIndex, stateListContent, 1, GUILayout.Height(stateListSelectHeight));
            });
        }

        private void DrawWorkSpace()
        {
            EditorGUILayout.BeginVertical(EditorKey.GroupBox, GUILayout.ExpandHeight(true));

            int stateCount = animator.states.Count;
            bool isStateIsNotEmpty = stateCount > 0;

            EditorView.HorizontalGroup(EditorKey.Box, () =>
            {
                DrawWorkSpaceTitle(isStateIsNotEmpty);
                DrawSelectingStateDefaultStatus();
            });

            if (!isStateIsNotEmpty)
                return;

            state = animator.states[selectingStateIndex];

            int transitionCount = state.transitionInfos.Count;

            if (transitionCount > 0)
                DrawTransitionEelement(transitionCount);

            if (stateCount > 1)
                DrawAddTransitionButton();

            EditorGUILayout.LabelField("This state is no transition target.", previewTitleStyle);

            EditorGUILayout.EndVertical();

            Undo.RecordObject(animator, "Mesh animator controller");

            Repaint();
        }

        private void DrawTransitionEelement(int transitionCount)
        {
            for (int i = 0; i < transitionCount; i++)
            {
                if (!DrawTransitionElement(state.transitionInfos[i], i))
                    break;
            }
        }

        private void DrawAddTransitionButton()
        {
            if (GUILayout.Button("+"))
            {
                if (isSelectingState)
                    return;

                SetSelectingStateStatus(true);
                TransitioninWindowEditor.Open(this);
            }
        }

        private void DrawSelectingStateDefaultStatus()
        {
            if (animator.defaultStateIndex == selectingStateIndex)
                EditorGUILayout.LabelField("Default state", GUI.skin.button, GUILayout.Width(120f));
            else
            {
                if (GUILayout.Button("Set Default State", GUILayout.Width(120f)))
                {
                    animator.defaultStateIndex = selectingStateIndex;
                }
            }
        }

        private void DrawWorkSpaceTitle(bool isStateIsNotEmpty)
        {
            if (isStateIsNotEmpty)
            {
                state = animator.states[selectingStateIndex];
                EditorGUILayout.LabelField(state.animation.name, headerInfoTitleStyle);
            }
            else
                EditorGUILayout.LabelField("State is empty", headerInfoTitleStyle);
        }

        private bool DrawTransitionElement(TransitionInfo transition, int index)
        {
            bool status = true;
            EditorView.HorizontalGroup(() =>
            {
                string targetStateName = animator.states[transition.targetStateIndex].animation.name;
                EditorGUILayout.LabelField($" : => {targetStateName}", GUI.skin.textField);

                if (GUILayout.Button("-", GUILayout.Width(20f)))
                {
                    status = false;
                    state.transitionInfos.RemoveAt(index);
                }
            });

            return status;
        }

        public void AddTransitionToSelectingState(int targetIndex)
        {
            TransitionInfo transition = new TransitionInfo();
            transition.targetStateIndex = targetIndex;
            state.transitionInfos.Add(transition);

            Undo.RecordObject(animator, "Mesh animator controller");

            Save();
        }

        public void SetSelectingStateStatus(bool isSelecting)
        {
            isSelectingState = isSelecting;
        }

        private void OnDisable()
        {
            Save();
        }

        public void Save()
        {
            EditorUtility.SetDirty(animator);
            AssetDatabase.SaveAssetIfDirty(animator);
        }
    }
}
#endif