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
        private GUILayoutOption[] workSpaceLayout = new GUILayoutOption[1];
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
            if (headerInfoTitleStyle == null)
                headerInfoTitleStyle = new GUIStyle(GUI.skin.label);

            headerInfoTitleStyle.alignment = TextAnchor.MiddleCenter;
            headerInfoTitleStyle.fontSize = 13;

            if (previewTitleStyle == null)
                previewTitleStyle = new GUIStyle(GUI.skin.label);

            previewTitleStyle.alignment = TextAnchor.MiddleCenter;
            previewTitleStyle.fontSize = 10;


            if (stateListContent == null)
            {
                int count = animator.states.Count;
                stateListContent = new string[count];

                for (int i = 0; i < count; i++)
                {
                    stateListContent[i] = animator.states[i].animation.name;
                }

                stateListSelectHeight = stateListItemSize * count;
            }

            EditorView.VerticalGroup(EditorKey.GroupBox, () =>
            {
                if (animator)
                    EditorGUILayout.LabelField(animator.name, headerInfoTitleStyle);
                else
                    EditorGUILayout.LabelField("Select your mesh animator info", headerInfoTitleStyle);
            });

            if (!animator)
                return;

            EditorView.HorizontalGroup(EditorKey.GroupBox,() =>
            {
                DrawAnimationList();

                DrawWorkSpace();

            });
        }

        private void DrawAnimationList()
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

            bool isValideToDraw = animator.states.Count > 0;

            EditorView.HorizontalGroup(EditorKey.Box, () =>
            {
                if (isValideToDraw)
                {
                    state = animator.states[selectingStateIndex];
                    EditorGUILayout.LabelField(state.animation.name, headerInfoTitleStyle);
                }
                else
                    EditorGUILayout.LabelField("State is empty", headerInfoTitleStyle);

                bool isDefaultState = animator.defaultStateIndex == selectingStateIndex;
                if (animator.defaultStateIndex == selectingStateIndex)
                    EditorGUILayout.LabelField("Default state", GUI.skin.button, GUILayout.Width(120f));
                else
                {
                    if(GUILayout.Button("Set Default State", GUILayout.Width(120f)))
                    {
                        animator.defaultStateIndex = selectingStateIndex;
                    }
                }
            });

            if (!isValideToDraw)
                return;

            state = animator.states[selectingStateIndex];

            int transitionCount = state.transitionInfos.Count;

            if (transitionCount > 0)
            {

                for (int i = 0; i < transitionCount; i++)
                {
                    if (!DrawTransitionElement(state.transitionInfos[i], i))
                        break;
                }
            }

            if (animator.states.Count > 1)
            {
                if (GUILayout.Button("+"))
                {
                    SetSelectingStateStatus(true);
                    TransitioninWindowEditor.Open(this);
                }
            }

            EditorGUILayout.LabelField("This state is no transition target.", previewTitleStyle);

            EditorGUILayout.EndVertical();

            Undo.RecordObject(animator, "Mesh animator controller");

            Repaint();
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