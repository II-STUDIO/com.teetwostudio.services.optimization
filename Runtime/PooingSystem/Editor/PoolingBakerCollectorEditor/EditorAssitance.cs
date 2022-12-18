#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using UnityEditor.AnimatedValues;

namespace Services
{
    /// <summary>
    /// This calss contain key fo editor GUI style.
    /// </summary>
    public static class EditorGUIKey
    {
        public const string GroupBox = "GroupBox";
        public const string Box = "Box";
        public const string Window = "Window";
        public const string Label = "Label";
        public const string TextField = "TextField";

    }

    /// <summary>
    /// This class use eazy to draw editor gui inspector or window.
    /// </summary>
    public static class EditorAssitance
    {
        /// <summary>
        /// Draw selector taps horizontal bar.
        /// </summary>
        /// <param name="menu"></param>
        /// <param name="index"></param>
        /// <returns>Selecting index</returns>
        public static int SelectTaps(string[] menu, int index) => GUILayout.Toolbar(index, menu);

        #region Horizontal
        /// <summary>
        /// Draw horizontal group.
        /// </summary>
        /// <param name="body"></param>
        public static void HorizontalGroup(Action body)
        {
            EditorGUILayout.BeginHorizontal();
            body.Invoke();
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        ///  Draw horizontal group.
        /// </summary>
        /// <param name="option"></param>
        /// <param name="body"></param>
        public static void HorizontalGroup(GUILayoutOption[] option, Action body)
        {
            EditorGUILayout.BeginHorizontal(option);
            body.Invoke();
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        ///  Draw horizontal group.
        /// </summary>
        /// <param name="style"></param>
        /// <param name="body"></param>
        public static void HorizontalGroup(string style, Action body)
        {
            EditorGUILayout.BeginHorizontal(style);
            body.Invoke();
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Draw horizontal group.
        /// </summary>
        /// <param name="body"></param>
        public static void HorizontalGroup(string style, GUILayoutOption[] option, Action body)
        {
            EditorGUILayout.BeginHorizontal(style, option);
            body.Invoke();
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        ///  Draw horizontal group.
        /// </summary>
        /// <param name="style"></param>
        /// <param name="body"></param>
        public static void HorizontalGroup(GUIStyle style, Action body)
        {
            EditorGUILayout.BeginHorizontal(style);
            body.Invoke();
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Draw horizontal group.
        /// </summary>
        /// <param name="style"></param>
        /// <param name="option"></param>
        /// <param name="body"></param>
        public static void HorizontalGroup(GUIStyle style, GUILayoutOption[] option, Action body)
        {
            EditorGUILayout.BeginHorizontal(style, option);
            body.Invoke();
            EditorGUILayout.EndHorizontal();
        }
        #endregion

        #region Vertical
        /// <summary>
        /// Draw vertical group
        /// </summary>
        /// <param name="body"></param>
        public static void VerticalGroup(Action body)
        {
            EditorGUILayout.BeginVertical();
            body.Invoke();
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// Draw vertical group
        /// </summary>
        /// <param name="option">options list</param>
        /// <param name="body">body</param>
        public static void VerticalGroup(GUILayoutOption[] option, Action body)
        {
            EditorGUILayout.BeginVertical(option);
            body.Invoke();
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// Draw vertical group
        /// </summary>
        /// <param name="style"></param>
        /// <param name="body"></param>
        public static void VerticalGroup(string style, Action body)
        {
            EditorGUILayout.BeginVertical(style);
            body.Invoke();
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// Draw vertical group
        /// </summary>
        /// <param name="style"></param>
        /// <param name="option"></param>
        /// <param name="body"></param>
        public static void VerticalGroup(string style, GUILayoutOption[] option, Action body)
        {
            EditorGUILayout.BeginVertical(style, option);
            body.Invoke();
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// Draw vertical group
        /// </summary>
        /// <param name="style"></param>
        /// <param name="body"></param>
        public static void VerticalGroup(GUIStyle style, Action body)
        {
            EditorGUILayout.BeginVertical(style);
            body.Invoke();
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// Draw vertical group
        /// </summary>
        /// <param name="style"></param>
        /// <param name="option"></param>
        /// <param name="body"></param>
        public static void VerticalGroup(GUIStyle style, GUILayoutOption[] option, Action body)
        {
            EditorGUILayout.BeginVertical(style, option);
            body.Invoke();
            EditorGUILayout.EndVertical();
        }
        #endregion

        #region ScrollView
        /// <summary>
        /// Create scroll view scope.
        /// </summary>
        /// <param name="body"></param>
        /// <param name="pos"></param>
        /// <returns>Current scope position x = horizontal bar, y = vertical bar</returns>
        public static Vector2 ScrollView(Action body, Vector2 pos)
        {
            var outPos = EditorGUILayout.BeginScrollView(pos);
            body.Invoke();
            EditorGUILayout.EndScrollView();
            return outPos;
        }

        /// <summary>
        /// Create scroll view scope.
        /// </summary>
        /// <param name="body"></param>
        /// <param name="pos"></param>
        /// <returns>Current scope position x = horizontal bar, y = vertical bar</returns>
        public static Vector2 ScrollView(Action body, Vector2 pos, GUILayoutOption[] options)
        {
            var outPos = EditorGUILayout.BeginScrollView(pos, options);
            body.Invoke();
            EditorGUILayout.EndScrollView();
            return outPos;
        }

        /// <summary>
        /// Create scroll view scope.
        /// </summary>
        /// <param name="body"></param>
        /// <param name="pos"></param>
        /// <returns>Current scope position x = horizontal bar, y = vertical bar</returns>
        public static Vector2 ScrollView(string style ,Action body, Vector2 pos)
        {
            var outPos = EditorGUILayout.BeginScrollView(pos, style);
            body.Invoke();
            EditorGUILayout.EndScrollView();
            return outPos;
        }

        /// <summary>
        /// Create scroll view scope.
        /// </summary>
        /// <param name="body"></param>
        /// <param name="pos"></param>
        /// <returns>Current scope position x = horizontal bar, y = vertical bar</returns>
        public static Vector2 ScrollView(GUIStyle style, Action body, Vector2 pos)
        {
            var outPos = EditorGUILayout.BeginScrollView(pos, style);
            body.Invoke();
            EditorGUILayout.EndScrollView();
            return outPos;
        }
        #endregion

        #region TogleGroup
        /// <summary>
        /// Create toggle group.
        /// </summary>
        /// <param name="label"></param>
        /// <param name="toggle"></param>
        /// <param name="body"></param>
        /// <returns>Toggle triggering status</returns>
        public static bool TogleGroup(string label,bool toggle,Action body)
        {
            var outPut = EditorGUILayout.BeginToggleGroup(label, toggle);
            body.Invoke();
            EditorGUILayout.EndToggleGroup();
            return outPut;
        }
        #endregion

        #region FoldOutGroup

        struct FoldoutFadeScope : IDisposable
        {
            private readonly bool wasIndent;

            public FoldoutFadeScope(AnimBool value, out bool shouldDraw, string label, Action horizontalAddon, bool indent = true)
            {
                value.target = Foldout(value.target, label, horizontalAddon);

                shouldDraw = EditorGUILayout.BeginFadeGroup(value.faded);
                if (shouldDraw && indent)
                {
                    Indent();
                    wasIndent = true;
                }
                else
                {
                    wasIndent = false;
                }
            }

            public void Dispose()
            {
                if (wasIndent)
                    EndIndent();
                EditorGUILayout.EndFadeGroup();
            }
        }

        static bool Foldout(bool value, string label, Action addon)
        {
            bool _value;
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.BeginHorizontal();

            _value = EditorGUILayout.Toggle(value, EditorStyles.foldout);
            addon?.Invoke();

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            var rect = GUILayoutUtility.GetLastRect();
            rect.x += 20;
            rect.width -= 20;

            EditorGUI.LabelField(rect, label, EditorStyles.boldLabel);

            return _value;
        }

        static void Indent()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(16);
            EditorGUILayout.BeginVertical();
        }

        static void EndIndent()
        {
            GUILayout.Space(16);
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Create fold out trasition group width anim bool bleading field width toggle trigger action.
        /// </summary>
        /// <param name="visible"></param>
        /// <param name="label"></param>
        /// <param name="body"></param>
        /// <param name="horizontalHeadAddon"></param>
        public static void FoldoutGroup(AnimBool visible, string label, Action body, Action horizontalHeadAddon = null)
        {
            using (new FoldoutFadeScope(visible, out var shouldDraw, label, horizontalHeadAddon))
            {
                if (shouldDraw) body.Invoke();
            }
        }

        /// <summary>
        /// Create fold out group width header and toggle trigger action.
        /// </summary>
        /// <param name="label"></param>
        /// <param name="toggle"></param>
        /// <param name="body"></param>
        /// <returns>Toggle triggering status</returns>
        public static bool FoldoutGroup(string label, bool toggle, Action body)
        {
            var outPut = EditorGUILayout.BeginFoldoutHeaderGroup(toggle, label);
            body.Invoke();
            EditorGUILayout.EndFoldoutHeaderGroup();
            return outPut;
        }

        /// <summary>
        /// Create fold out group width header and toggle trigger action.
        /// </summary>
        /// <param name="label"></param>
        /// <param name="toggle"></param>
        /// <param name="body"></param>
        public static bool FoldoutGroup(string style, string label, bool toggle, Action body)
        {
            var outPut = EditorGUILayout.BeginFoldoutHeaderGroup(toggle, label, style);
            body.Invoke();
            EditorGUILayout.EndFoldoutHeaderGroup();
            return outPut;
        }

        /// <summary>
        /// Create fold out group width header and toggle trigger action.
        /// </summary>
        /// <param name="label"></param>
        /// <param name="toggle"></param>
        /// <param name="body"></param>
        public static bool FoldoutGroup(GUIStyle style, string label, bool toggle, Action body)
        {
            var outPut = EditorGUILayout.BeginFoldoutHeaderGroup(toggle, label, style);
            body.Invoke();
            EditorGUILayout.EndFoldoutHeaderGroup();
            return outPut;
        }
        #endregion

        #region FadeGroup
        
        #endregion

        #region Texture2DField
        /// <summary>
        /// Draw horizontal texture field box GUI explanded.
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="size"></param>
        /// <returns>Texture that had set on field</returns>
        public static Texture2D HorizontalTexture2DField(Texture2D texture, int size = 50)
        {
            return (Texture2D)EditorGUILayout.ObjectField(texture, typeof(Texture2D), true, GUILayout.Width(size), GUILayout.Height(size));
        }

        /// <summary>
        /// Draw horizontal texture field box GUI explanded.
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="size"></param>
        /// <returns>Texture that had set on field</returns>
        public static Texture2D HorizontalTexture2DField(string label, Texture2D texture, int size = 50)
        {
            return (Texture2D)EditorGUILayout.ObjectField(label, texture, typeof(Texture2D), true, GUILayout.Width(size), GUILayout.Height(size));
        }

        /// <summary>
        /// Draw vertical texture field box GUI explanded.
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="size"></param>
        /// <returns>Texture that had set on field</returns>
        public static Texture2D VerticalTexture2DField(string label, Texture2D texture, int size = 50)
        {
            Texture2D outPut = texture;
            VerticalGroup(() =>
            {
                EditorGUILayout.LabelField(label);
                outPut = HorizontalTexture2DField(texture, size);
            });

            return outPut;
        }
        #endregion

        #region SpriteField
        /// <summary>
        /// Draw horizontal sprite field on GUI explanded.
        /// </summary>
        /// <param name="sprite"></param>
        /// <param name="size"></param>
        /// <returns>Sprite that had set on field</returns>
        public static Sprite HorizontalSpriteField(Sprite sprite, int size = 50)
        {
            return (Sprite)EditorGUILayout.ObjectField(sprite, typeof(Sprite), true, GUILayout.Width(size), GUILayout.Height(size));
        }

        /// <summary>
        /// Draw horizontal sprite field on GUI explanded.
        /// </summary>
        /// <param name="sprite"></param>
        /// <param name="size"></param>
        /// <returns>Sprite that had set on field</returns>
        public static Sprite HorizontalSpriteField(string label, Sprite sprite, int size = 50)
        {
            return (Sprite)EditorGUILayout.ObjectField(label, sprite, typeof(Sprite), true, GUILayout.Width(size), GUILayout.Height(size));
        }

        /// <summary>
        /// Draw vertical sprite field on GUI explanded.
        /// </summary>
        /// <param name="sprite"></param>
        /// <param name="size"></param>
        /// <returns>Sprite that had set on field</returns>
        public static Sprite VerticalSpriteField(string label, Sprite sprite, int size = 50)
        {
            Sprite outPut = sprite;
            VerticalGroup(() =>
            {
                EditorGUILayout.LabelField(label);
                outPut = HorizontalSpriteField(sprite, size);
            });

            return outPut;
        }
        #endregion
        /// <summary>
        /// Draw horizontal line on GUI explanded.
        /// </summary>
        public static void HorizontalLine()
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        }

        /// <summary>
        /// Draw horizontal line on GUI explanded width scaler.
        /// </summary>
        /// <param name="height"></param>
        /// <param name="width"></param>
        /// <param name="margin"></param>
        public static void HorizontalLine(float height = 1, float width = -1, Vector2 margin = new Vector2())
        {
            GUILayout.Space(margin.x);

            var rect = EditorGUILayout.GetControlRect(false, height);
            if (width > -1)
            {
                var centerX = rect.width / 2;
                rect.width = width;
                rect.x += centerX - width / 2;
            }

            Color color = EditorStyles.label.active.textColor;
            color.a = 0.5f;
            EditorGUI.DrawRect(rect, color);

            GUILayout.Space(margin.y);
        }

        /// <summary>
        /// Draw button on GUI explanded.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="onClick"></param>
        /// <param name="interactable"></param>
        public static void Button(string title, Action onClick, bool interactable = true)
        {
            if (!interactable)
            {
                var style = new GUIStyle(GUI.skin.textField);
                style.alignment = TextAnchor.MiddleCenter;
                GUILayout.Label(title, style);
                return;
            }

            if (!GUILayout.Button(title))
                return;

            onClick?.Invoke();
        }

        /// <summary>
        /// Draw button on GUI explanded.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="onClick"></param>
        /// <param name="interactable"></param>
        public static void Button(string title, Action onClick, bool interactable = true, params GUILayoutOption[] options)
        {
            if (!interactable)
            {
                var style = new GUIStyle(GUI.skin.textField);
                style.alignment = TextAnchor.MiddleCenter;
                GUILayout.Label(title, style);
                return;
            }

            if (!GUILayout.Button(title, options))
                return;

            onClick?.Invoke();
        }

        #region GameObjectField
        /// <summary>
        /// Draw GameObject field on GUI explanded.
        /// </summary>
        /// <param name="gameObject"></param>
        /// <returns>GameObject that set on field</returns>
        public static GameObject GameObjectField(GameObject gameObject)
        {
            return (GameObject)EditorGUILayout.ObjectField(gameObject, typeof(GameObject), true);
        }

        /// <summary>
        /// Draw GameObject field on GUI explanded width label.
        /// </summary>
        /// <param name="label"></param>
        /// <param name="gameObject"></param>
        /// <returns>GameObject that set on field</returns>
        public static GameObject GameObjectField(string label, GameObject gameObject)
        {
            return (GameObject)EditorGUILayout.ObjectField(label, gameObject, typeof(GameObject), true);
        }

        /// <summary>
        /// Draw GameObject width box field on GUI explanded.
        /// </summary>
        /// <param name="gameObject"></param>
        /// <returns>GameObject that set on field</returns>
        public static GameObject GameObjectBoxField(GameObject gameObject, int size = 50)
        {
            return (GameObject)EditorGUILayout.ObjectField(gameObject, typeof(GameObject), true, GUILayout.Width(size), GUILayout.Height(size));
        }

        /// <summary>
        /// Draw GameObject width box field on GUI explanded width label.
        /// </summary>
        /// <param name="gameObject"></param>
        /// <returns>GameObject that set on field</returns>
        public static GameObject GameObjectBoxField(string label, GameObject gameObject, int size = 50)
        {
            return (GameObject)EditorGUILayout.ObjectField(label, gameObject, typeof(GameObject), true, GUILayout.Width(size), GUILayout.Height(size));
        }
        #endregion

        #region Preview Field
        public static void DrawPreviewTexture(Texture2D texture,int size = 50)
        {
            EditorGUILayout.ObjectField(texture, typeof(Texture2D), false, GUILayout.Width(size), GUILayout.Height(size));
        }

        public static void DrawPrefviewObject(GameObject gameObject, int size = 50)
        {
            EditorGUILayout.ObjectField(AssetPreview.GetAssetPreview(gameObject), typeof(Texture2D), false, GUILayout.Width(size), GUILayout.Height(size));
        }
        #endregion

        #region TransformField
        /// <summary>
        /// Draw Transform field width field on GUI explanded.
        /// </summary>
        /// <param name="transform"></param>
        /// <returns>Transform that set on field</returns>
        public static Transform TransformField(Transform transform)
        {
            return (Transform)EditorGUILayout.ObjectField(transform, typeof(Transform), true);
        }

        /// <summary>
        /// Draw Transform field width field on GUI explanded.
        /// </summary>
        /// <param name="transform"></param>
        /// <returns>Transform that set on field</returns>
        public static Transform TransformField(string label, Transform transform)
        {
            return (Transform)EditorGUILayout.ObjectField(label, transform, typeof(Transform), true);
        }
        #endregion

        #region MaterialsField
        /// <summary>
        /// Draw material field on GUI explanded.
        /// </summary>
        /// <param name="material"></param>
        /// <returns>Material that set on field</returns>
        public static Material MaterialField(Material material)
        {
            return (Material)EditorGUILayout.ObjectField(material, typeof(Material), true);
        }

        /// <summary>
        /// Draw material field on GUI explanded width label.
        /// </summary>
        /// <param name="material"></param>
        /// <returns>Material that set on field</returns>
        public static Material MaterialField(string label, Material material)
        {
            return (Material)EditorGUILayout.ObjectField(label, material, typeof(Material), true);
        }

        /// <summary>
        /// Draw material field on GUI explanded.
        /// </summary>
        /// <param name="material"></param>
        /// <param name="size"></param>
        /// <returns>Material that set on field</returns>
        public static Material MaterialtBoxField(Material material, int size = 50)
        {
            return (Material)EditorGUILayout.ObjectField(material, typeof(Material), true, GUILayout.Width(size), GUILayout.Height(size));
        }

        /// <summary>
        /// Draw material field on GUI explanded width label.
        /// </summary>
        /// <param name="material"></param>
        /// <returns>Material that set on field</returns>
        public static Material MaterialtBoxField(string label, Material material, int size = 50)
        {
            return (Material)EditorGUILayout.ObjectField(label, material, typeof(Material), true, GUILayout.Width(size), GUILayout.Height(size));
        }
        #endregion
    }
}
#endif
