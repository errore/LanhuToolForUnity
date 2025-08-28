using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;
using Image = UnityEngine.UI.Image;
using Toggle = UnityEngine.UIElements.Toggle;

namespace Editor.LanhuTool {
    public class LanHu : EditorWindow {
        [MenuItem("Tools/Lan Hu")]
        private static void Open() {
            var window = GetWindow<LanHu>("蓝湖");
            window.minSize = new Vector2(200, 105);
            window.Show();
        }

        private VisualElement gui;
        private VisualElement noSelected;
        private VisualElement editor;
        private Toggle setColor;
        private RadioButton leftUp;
        private RadioButton center;

        private RectTransform selectedTransform;

        private string colorStr = "#FFFFFFFF";

        private enum Mode {
            All,
            Pos,
            Size
        }

        public void CreateGUI() {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;

            // Import UXML
            var visualTree =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/Editor/LanHuTool/Lanhu.uxml");
            gui = visualTree.Instantiate();
            root.Add(gui);

            noSelected = root.Q<VisualElement>("NoSelected");
            editor = root.Q<VisualElement>("EditorRoot");
            setColor = root.Q<Toggle>("SetColor");
            leftUp = root.Q<RadioButton>("RadioLeftUp");
            center = root.Q<RadioButton>("RadioCenter");

            gui.Q<Button>("BtnApply").clickable.clicked += () => { SetRect(false); };

            gui.Q<Button>("BtnSetAll").clickable.clicked += () => { SetRect(true); };
            gui.Q<Button>("BtnSetPos").clickable.clicked += () => { SetRect(true, Mode.Pos); };
            gui.Q<Button>("BtnSetSize").clickable.clicked += () => { SetRect(true, Mode.Size); };
        }

        private void Update() {
            if (Selection.activeTransform == null) {
                editor.visible = false;
                noSelected.visible = true;
            } else {
                editor.visible = true;
                noSelected.visible = false;
                selectedTransform = Selection.activeTransform.GetComponent<RectTransform>();
            }
        }

        // private void OnGUI() {
        //     //如果未选中任何物体 return
        //     if (Selection.activeTransform == null) return;
        //     RectTransform rt = Selection.activeTransform.GetComponent<RectTransform>();
        //     //如果选中的物体不是UI元素 return
        //     if (rt == null) return;
        //
        //     var canvas = rt.GetComponentInParent<Canvas>();
        //     //获取父节点
        //     RectTransform parent = canvas.rootCanvas.GetComponent<RectTransform>();
        //
        //     float screenHeight = parent.rect.height;
        //     float screenWeight = parent.rect.width;
        //
        //     GUILayout.Label($"分辨率：{screenHeight}*{screenWeight}");
        //
        //     EditorGUILayout.Space();
        //
        //     GUILayout.Label("样式信息", "BoldLabel");
        //
        //     GUILayout.BeginHorizontal();
        //     GUILayout.Label("图层", GUILayout.Width(labelWidth));
        //     var image = rt.GetComponent<Image>();
        //     if (image != null && image.sprite != null) EditorGUILayout.TextField(image.sprite.name);
        //     else EditorGUILayout.HelpBox("未发现任何图层", MessageType.Warning);
        //     GUILayout.EndHorizontal();
        //
        //     GUILayout.BeginHorizontal();
        //     GUILayout.Label("位置", GUILayout.Width(labelWidth));
        //     x = EditorGUILayout.TextField(x);
        //     GUILayout.Label("px");
        //     y = EditorGUILayout.TextField(y);
        //     GUILayout.Label("px");
        //     GUILayout.EndHorizontal();
        //
        //     GUILayout.BeginHorizontal();
        //     GUILayout.Label("大小", GUILayout.Width(labelWidth));
        //     width = EditorGUILayout.TextField(width);
        //     GUILayout.Label("px");
        //     height = EditorGUILayout.TextField(height);
        //     GUILayout.Label("px");
        //     GUILayout.EndHorizontal();
        //
        //     GUILayout.BeginHorizontal();
        //     colorStr = EditorGUILayout.TextField("颜色", colorStr);
        //     GUILayout.EndHorizontal();
        //
        //     if (GUILayout.Button("应用")) {
        //         SetRect(rt, false);
        //     }
        //
        //     GUILayout.BeginHorizontal();
        //     if (GUILayout.Button("粘贴并应用")) {
        //         SetRect(rt, true);
        //     }
        //     GUILayout.EndHorizontal();
        // }

        private void SetRect(bool useClipboard, Mode mode = Mode.All) {
            //调整位置及大小
            var rt = selectedTransform;

            Undo.RecordObject(rt, "SetRect");
            float widthval, heightval, xVal, yVal;
            if (useClipboard) {
                var clipboard = GUIUtility.systemCopyBuffer;
                Debug.Log(clipboard);
                var data = clipboard.Split(",").ToList();
                widthval = Convert.ToSingle(data[2]);
                heightval = Convert.ToSingle(data[3]);
                xVal = Convert.ToSingle(data[0]);
                yVal = Convert.ToSingle(data[1]);
                colorStr = data[4];
            } else {
                widthval = ConvenVal(gui.Q<TextField>("SizeX").text);
                heightval = ConvenVal(gui.Q<TextField>("SizeY").text);
                xVal = ConvenVal(gui.Q<TextField>("PositionX").text);
                yVal = ConvenVal(gui.Q<TextField>("PositionY").text);
            }

            var canvas = rt.GetComponentInParent<Canvas>();
            //获取父节点
            RectTransform root = canvas.rootCanvas.GetComponent<RectTransform>();

            float screenHeight = root.rect.height;
            float screenWeight = root.rect.width;

            var parent = rt.parent;
            rt.SetParent(root);
            Undo.RecordObject(rt, "SetRoot");

            if (setColor.value) {
                var Image = rt.GetComponent<Image>();
                if (Image != null)
                    Image.color = ColorUtility.TryParseHtmlString(colorStr, out var color) ? color : Color.white;

                var text = rt.GetComponent<Text>();
                if (text != null)
                    text.color = ColorUtility.TryParseHtmlString(colorStr, out var color) ? color : Color.white;

                var TMPText = rt.GetComponent<TMPro.TextMeshProUGUI>();
                if (TMPText != null)
                    TMPText.color = ColorUtility.TryParseHtmlString(colorStr, out var color) ? color : Color.white;
            }

            // var pivot = rt.pivot;
            rt.pivot = Vector2.one * 0.5f;

            switch (mode) {
                case Mode.All:
                    if (leftUp.value) {
                        rt.anchorMin = new Vector2(0f, 1f);
                        rt.anchorMax = new Vector2(0f, 1f);
                        rt.anchoredPosition = new Vector2(
                            xVal + widthval * 0.5f,
                            - yVal - heightval * 0.5f);
                    } else {
                        rt.anchorMin = new Vector2(0.5f, 0.5f);
                        rt.anchorMax = new Vector2(0.5f, 0.5f);
                        rt.anchoredPosition = new Vector2(
                            xVal + widthval * 0.5f - screenWeight * 0.5f,
                            screenHeight - yVal - heightval * 0.5f - screenHeight * 0.5f);
                    }
                    rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, widthval);
                    rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, heightval);
                    break;
                case Mode.Pos:
                    if (leftUp.value) {
                        rt.anchorMin = new Vector2(0f, 1f);
                        rt.anchorMax = new Vector2(0f, 1f);
                        rt.anchoredPosition = new Vector2(
                            xVal + widthval * 0.5f,
                            - yVal - heightval * 0.5f);
                    } else {
                        rt.anchorMin = new Vector2(0.5f, 0.5f);
                        rt.anchorMax = new Vector2(0.5f, 0.5f);
                        rt.anchoredPosition = new Vector2(
                            xVal + widthval * 0.5f - screenWeight * 0.5f,
                            screenHeight - yVal - heightval * 0.5f - screenHeight * 0.5f);
                    }
                    break;
                case Mode.Size:
                    rt.sizeDelta = new Vector2(widthval, heightval);
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }
            // rt.pivot = pivot;
            rt.SetParent(parent, true);
            Undo.RecordObject(rt, "SetParent");

            EditorUtility.SetDirty(rt);
        }

        //如果字符串尾部有 px 则去掉
        private float ConvenVal(string str) {
            str = str.Trim().ToLower();
            if (str.EndsWith("px")) str = str.Substring(0, str.Length - 2);
            return float.Parse(str);
        }

        private void OnSelectionChange() {
            Repaint();
        }
    }
}