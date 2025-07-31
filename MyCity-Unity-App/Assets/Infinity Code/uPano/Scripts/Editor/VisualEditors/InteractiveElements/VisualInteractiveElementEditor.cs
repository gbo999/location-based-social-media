/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using System.Collections.Generic;
using System.Linq;
using InfinityCode.uPano.Directions;
using InfinityCode.uPano.Editors.Utils;
using InfinityCode.uPano.Editors.VisualEditors.Tours;
using InfinityCode.uPano.HotSpots;
using InfinityCode.uPano.InteractiveElements;
using InfinityCode.uPano.Renderers;
using InfinityCode.uPano.Renderers.Base;
using InfinityCode.uPano.Tours;
using UnityEditor;
using UnityEngine;

namespace InfinityCode.uPano.Editors.VisualEditors.InteractiveElements
{
    public class VisualInteractiveElementEditor : EditorWindow
    {
        internal static InteractiveElementNode activeNode;
        internal static DirectionManager directionManager;
        internal static HotSpotManager hotSpotManager;
        internal static SerializedObject serializedDirectionManager;
        internal static SerializedProperty serializedDirectionItems;
        internal static SerializedObject serializedHotSpotManager;
        internal static SerializedProperty serializedHotSpotItems;
        internal static VisualInteractiveElementEditor wnd;

        private float scale = 1;
        internal Rect viewRect;
        private Vector2 rectPosition;

        internal PanoRenderer panoRenderer;

        private Dictionary<InteractiveElement, InteractiveElementNode> nodes;
        internal Texture hotSpotIcon;
        internal Texture hotSpotSelectedIcon;

        public GUIStyle _missedPrefabStyle;
        private bool isTextureDrag;
        private TourItem connectionTarget;
        private bool isConnectionMode;
        private bool isReturnMode;
        private TourItem connectionFrom;

        public GUIStyle missedPrefabStyle
        {
            get
            {
                if (_missedPrefabStyle == null)
                {
                    _missedPrefabStyle = new GUIStyle(GUI.skin.label);
                    _missedPrefabStyle.normal.textColor = Color.red;
                }

                return _missedPrefabStyle;
            }
        }

        private void CreateDirectionFromContextMenu(Vector2 mousePosition)
        {
            ISingleTexturePanoRenderer r = panoRenderer as ISingleTexturePanoRenderer;
            Vector2 uv = new Vector2(
                (mousePosition.x - viewRect.x) / viewRect.width,
                1 - (mousePosition.y - viewRect.y) / viewRect.height);
            float pan, tilt;
            r.GetPanTiltByUV(uv, out pan, out tilt);
            Direction direction = directionManager.Create(pan, null);
            InteractiveElementNode node = new InteractiveElementNode(direction, this);
            nodes.Add(direction, node);
            node.OnSelect += OnSelectNode;
            node.Select();
            TrySetConnection(direction);
            EditorUtils.SetDirty(directionManager);
        }

        private void CreateHotSpotFromContextMenu(Vector2 mousePosition)
        {
            ISingleTexturePanoRenderer r = panoRenderer as ISingleTexturePanoRenderer;
            Vector2 uv = new Vector2(
                (mousePosition.x - viewRect.x) / viewRect.width,
                1 - (mousePosition.y - viewRect.y) / viewRect.height);
            float pan, tilt;
            r.GetPanTiltByUV(uv, out pan, out tilt);
            HotSpot hotSpot = hotSpotManager.Create(pan, tilt, null);
            InteractiveElementNode node = new InteractiveElementNode(hotSpot, this);
            nodes.Add(hotSpot, node);
            node.OnSelect += OnSelectNode;
            node.Select();
            TrySetConnection(hotSpot);
            EditorUtils.SetDirty(hotSpotManager);
        }

        private void DrawElements()
        {
            ISingleTexturePanoRenderer r = panoRenderer as ISingleTexturePanoRenderer;

            if (nodes == null) nodes = new Dictionary<InteractiveElement, InteractiveElementNode>();

            foreach (var pair in nodes) pair.Value.used = false;

            foreach (HotSpot spot in hotSpotManager) 
            {
                InteractiveElementNode node;
                if (!nodes.TryGetValue(spot, out node))
                {
                    node = new InteractiveElementNode(spot, this);
                    node.OnSelect += OnSelectNode;
                    nodes.Add(spot, node);
                }

                node.Draw(r);
            }

            foreach (Direction direction in directionManager)
            {
                InteractiveElementNode node;
                if (!nodes.TryGetValue(direction, out node))
                {
                    node = new InteractiveElementNode(direction, this);
                    node.OnSelect += OnSelectNode;
                    nodes.Add(direction, node);
                }

                node.Draw(r);
            }

            foreach (var n in nodes.Where(n => !n.Value.used).ToArray())
            {
                if (n.Value == activeNode)
                {
                    activeNode = null;
                    InteractiveElementSettings.Redraw();
                }
                n.Value.Dispose();
                nodes.Remove(n.Key);
            }
        }

        private void DrawNoPano()
        {
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.fontSize = 32;
            style.normal.textColor = Color.white;
            style.alignment = TextAnchor.MiddleCenter;
            EditorGUI.LabelField(new Rect(position.width / 2 - 150, position.height / 2 - 50, 300, 100), "No PanoRenderer", style);
        }

        private void DrawTexture()
        {
            Texture texture = null;
            if (panoRenderer is ISingleTexturePanoRenderer) texture = (panoRenderer as ISingleTexturePanoRenderer).texture;

            if (texture == null)
            {
                GUI.Box(new Rect(0, 0, position.width, position.height), GUIContent.none);
                return;
            }

            if (texture is Cubemap) texture = EditorUtils.GetCubemapTexture(texture as Cubemap);

            float ratio = texture.width / (float)texture.height;
            float screenRatio = position.width / position.height;

            float width = 0;
            float height = 0;
            float offsetX = 0;
            float offsetY = 0;

            if (screenRatio < ratio)
            {
                width = position.width;
                height = position.width / ratio;
                offsetY = (position.height - height) / 2;
            }
            else
            {
                height = position.height;
                width = position.height * ratio;
                offsetX = (position.width - width) / 2;
            }

            viewRect = new Rect(offsetX, offsetY, width, height);
            viewRect.position += rectPosition;
            viewRect.size *= scale;
            GUI.DrawTexture(viewRect, texture, ScaleMode.ScaleToFit);
        }

        private void DrawToolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            if (GUILayout.Button("Reset view", EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)))
            {
                scale = 1;
                rectPosition = Vector2.zero;
            }

            EditorGUILayout.Space();

            EditorGUILayout.EndHorizontal();
        }

        public Vector2 GetUV(Event e)
        {
            return new Vector2(
                (e.mousePosition.x - viewRect.x) / viewRect.width,
                1 - (e.mousePosition.y - viewRect.y) / viewRect.height);
        }

        private void InitPanoRenderer()
        {
            serializedHotSpotManager = null;
            serializedHotSpotItems = null;

            if (Selection.activeGameObject == null)
            {
                panoRenderer = null;
                return;
            }

            panoRenderer = Selection.activeGameObject.GetComponent<PanoRenderer>();

            if (panoRenderer == null) return;
            if (!(panoRenderer is ISingleTexturePanoRenderer))
            {
                panoRenderer = null;
                return;
            }

            hotSpotManager = panoRenderer.GetComponent<HotSpotManager>();
            if (hotSpotManager == null) hotSpotManager = panoRenderer.gameObject.AddComponent<HotSpotManager>();

            serializedHotSpotManager = new SerializedObject(hotSpotManager);
            serializedHotSpotItems = serializedHotSpotManager.FindProperty("_items");

            directionManager = panoRenderer.GetComponent<DirectionManager>();
            if (directionManager == null) directionManager = panoRenderer.gameObject.AddComponent<DirectionManager>();

            serializedDirectionManager = new SerializedObject(directionManager);
            serializedDirectionItems = serializedDirectionManager.FindProperty("_items");
        }

        private void OnDrag(Vector2 delta)
        {
            rectPosition += delta;
            GUI.changed = true;
        }

        private void OnDestroy()
        {
            InteractiveElementSettings.CloseWindow();
        }

        private void OnEnable()
        {
            wnd = this;

            Selection.selectionChanged -= OnSelectionChanged;
            Selection.selectionChanged += OnSelectionChanged;

            InitPanoRenderer();

            string[] GUIDs = AssetDatabase.FindAssets("t:Texture hotspot-icon");
            if (GUIDs.Length > 0)
            {
                hotSpotIcon = AssetDatabase.LoadAssetAtPath<Texture>(AssetDatabase.GUIDToAssetPath(GUIDs[0]));
            }

            GUIDs = AssetDatabase.FindAssets("t:Texture hotspot-selected-icon");
            if (GUIDs.Length > 0)
            {
                hotSpotSelectedIcon = AssetDatabase.LoadAssetAtPath<Texture>(AssetDatabase.GUIDToAssetPath(GUIDs[0]));
            }
        }

        private void OnFocus()
        {
            OnEnable();
        }

        private void OnGUI()
        {
            if (panoRenderer == null)
            {
                DrawNoPano();
                return;
            }

            bool drawContent = panoRenderer != null && panoRenderer is ISingleTexturePanoRenderer;

            if (drawContent)
            {
                DrawTexture();
                DrawElements();
            }
            
            DrawToolbar();

            Rect rect = new Rect(position.width / 2 - 80, position.height - 120, 180, 30);

            if (isConnectionMode)
            {
                float windowCenter = position.width / 2;
                float headerWidth = 500;
                GUILayout.BeginArea(new Rect(windowCenter - headerWidth / 2, 25, headerWidth, 60), EditorStyles.helpBox);

                GUIStyle headerStyle = new GUIStyle(EditorStyles.whiteLargeLabel);
                headerStyle.alignment = TextAnchor.MiddleCenter;
                headerStyle.fontStyle = FontStyle.Bold;
                headerStyle.fontSize = 24;

                GUIStyle centerLabelStyle = new GUIStyle(EditorStyles.whiteLabel);
                centerLabelStyle.alignment = TextAnchor.MiddleCenter;

                GUILayout.Label("Connection mode", headerStyle);
                GUILayout.Label("Right-click in the place where you want to create a Hot Spot or Direction", centerLabelStyle);

                GUILayout.EndArea();

                if (GUI.Button(rect, "Cancel connection creation"))
                {
                    GetWindow<TourMaker>();
                    InteractiveElementSettings.CloseWindow();
                    isConnectionMode = false;
                }
            }

            if (isReturnMode && GUI.Button(rect, "Return to Tour Maker"))
            {
                GetWindow<TourMaker>();
                InteractiveElementSettings.CloseWindow();
                isReturnMode = false;
            }

            if (drawContent)
            {
                ProcessNodeEvents(Event.current);
                ProcessEvents(Event.current);
            }

            if (GUI.changed) Repaint();
        }

        private void OnScrollWheel(Event e)
        {
            float oldScale = scale;
            scale = Mathf.Clamp(scale * (1 - e.delta.y / 100), 0.1f, 10);

            float scaleDelta = scale - oldScale;
            if (Mathf.Abs(scaleDelta) < float.Epsilon) return;

            scaleDelta = scale / oldScale;

            Vector2 p1 = e.mousePosition - viewRect.position;
            p1.x /= viewRect.width;
            p1.y /= viewRect.height;

            Vector2 p2 = e.mousePosition - viewRect.position;
            p2.x /= viewRect.width * scaleDelta;
            p2.y /= viewRect.height * scaleDelta;

            Vector2 offset = p2 - p1;
            offset.x *= viewRect.width * scaleDelta;
            offset.y *= viewRect.height * scaleDelta;

            rectPosition += offset;

            GUI.changed = true;
        }

        private void OnSelectNode(InteractiveElementNode node)
        {
            if (activeNode == node) return;
            if (activeNode != null) activeNode.Deselect();
            activeNode = node;
            InteractiveElementSettings.OpenWindow();
            wnd.Focus();
            GUI.changed = true;
        }

        private void OnSelectionChanged()
        {
            InitPanoRenderer();
            Redraw();
        }

        [MenuItem(EditorUtils.MENU_PATH + "Interactive Element Editor", false, 2)]
        public static void OpenWindow()
        {
            wnd = GetWindow<VisualInteractiveElementEditor>("Interactive Element Editor");
            wnd.autoRepaintOnSceneChange = true;
            wnd.isReturnMode = false;
        }

        public static void OpenWindow(TourItem item1, TourItem item2)
        {
            OpenWindow();
            wnd.connectionFrom = item1;
            wnd.connectionTarget = item2;
            wnd.isConnectionMode = true;
        }

        private void ProcessContextMenu(Vector2 mousePosition)
        {
            GenericMenu genericMenu = new GenericMenu();
            genericMenu.AddItem(new GUIContent("Create Direction"), false, () => { CreateDirectionFromContextMenu(mousePosition); });
            genericMenu.AddItem(new GUIContent("Create HotSpot"), false, () => { CreateHotSpotFromContextMenu(mousePosition); });
            genericMenu.ShowAsContext();
        }

        private void ProcessEvents(Event e)
        {
            if (e.type == EventType.MouseDown)
            {
                if (e.button == 0)
                {
                    isTextureDrag = true;
                    if (activeNode != null) activeNode.Deselect();
                    activeNode = null;
                    InteractiveElementSettings.Redraw();
                }
                else if (e.button == 1)
                {
                    ProcessContextMenu(e.mousePosition);
                }
                else if (e.button == 2)
                {
                    isTextureDrag = true;
                }
            }
            else if (e.type == EventType.MouseDrag)
            {
                if ((e.button == 0 || e.button == 2) && isTextureDrag) OnDrag(e.delta);
            }
            else if (e.type == EventType.MouseUp)
            {
                isTextureDrag = false;
            }
            else if (e.type == EventType.ScrollWheel) OnScrollWheel(e);
            else if (e.type == EventType.KeyUp)
            {
                if (e.keyCode == KeyCode.Delete || e.keyCode == KeyCode.Backspace)
                {
                    if (activeNode != null)
                    {
                        activeNode.Remove();
                        activeNode = null;
                        InteractiveElementSettings.CloseWindow();
                        GUI.changed = true;
                    }
                }
            }
        }

        private void ProcessNodeEvents(Event e)
        {
            foreach (var node in nodes)
            {
                node.Value.ProcessEvents(e);
            }
        }

        internal static void Redraw()
        {
            if (wnd != null) wnd.Repaint();
        }

        private void TrySetConnection(InteractiveElement el)
        {
            if (!isConnectionMode) return;

            el.switchToPanorama = connectionTarget.gameObject;
            connectionFrom.UpdateOutLinks();
            isConnectionMode = false;
            isReturnMode = true;
            connectionFrom = null;
            connectionTarget = null;
        }
    }
}