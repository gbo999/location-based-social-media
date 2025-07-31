/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using System;
using InfinityCode.uPano.Directions;
using InfinityCode.uPano.Editors.Utils;
using InfinityCode.uPano.HotSpots;
using InfinityCode.uPano.InteractiveElements;
using InfinityCode.uPano.Renderers.Base;
using UnityEditor;
using UnityEngine;

namespace InfinityCode.uPano.Editors.VisualEditors.InteractiveElements
{
    public class InteractiveElementNode
    {
        public enum ElementType
        {
            hotSpot,
            direction
        }

        public InteractiveElement element;
        public Rect rect;
        internal bool used;
        private VisualInteractiveElementEditor editor;
        private bool isDrag = false;
        private bool isSelected = false;
        public HotSpot hotSpot;
        public Direction direction;

        public ElementType type;

        internal Action<InteractiveElementNode> OnSelect;

        public InteractiveElementNode(InteractiveElement element, VisualInteractiveElementEditor editor)
        {
            this.element = element;
            this.editor = editor;

            hotSpot = element as HotSpot;
            direction = element as Direction;

            type = hotSpot != null ? ElementType.hotSpot : ElementType.direction;
        }

        public void Deselect()
        {
            isSelected = false;
        }

        public void Dispose()
        {
            element = null;
            editor = null;
        }

        public void Draw(ISingleTexturePanoRenderer r)
        {
            float pan, tilt;
            element.GetPanTilt(out pan, out tilt);
            Vector2 uv = r.GetUV(pan, tilt);
            uv.x = editor.viewRect.width * uv.x + editor.viewRect.position.x;
            uv.y = (1 - uv.y) * editor.viewRect.height + editor.viewRect.position.y;
            rect = new Rect(uv.x - 16, uv.y - 16, 32, 32);

            GUIContent content = new GUIContent(element.title);
            Vector2 size = GUI.skin.label.CalcSize(content);

            Rect labelRect = new Rect(rect.center.x - size.x / 2, rect.y - size.y - 5, size.x, size.y);

            GUIStyle titleStyle = GUI.skin.label;

            if (hotSpot != null && hotSpot.prefab == null) titleStyle = editor.missedPrefabStyle;

            GUI.Label(labelRect, element.title, titleStyle);

            if (editor.hotSpotIcon != null) GUI.DrawTexture(rect, isSelected? editor.hotSpotSelectedIcon: editor.hotSpotIcon);
            else GUI.Box(rect, "", GUI.skin.box);

            used = true;
        }

        private void ProcessContextMenu(Event e)
        {
            GenericMenu genericMenu = new GenericMenu();
            genericMenu.AddItem(new GUIContent("Remove"), false, Remove);
            genericMenu.ShowAsContext();
        }

        public void ProcessEvents(Event e)
        {
            if (e.type == EventType.MouseDown)
            {
                if (rect.Contains(e.mousePosition))
                {
                    if (e.button == 0)
                    {
                        Select();
                        isDrag = true;
                        e.Use();
                    }
                    else if (e.button == 1)
                    {
                        ProcessContextMenu(e);
                        e.Use();
                    }
                }
            }
            else if (e.type == EventType.MouseDrag)
            {
                if (isDrag)
                {
                    Vector2 uv = editor.GetUV(e);
                    float pan, tilt;
                    if ((editor.panoRenderer as ISingleTexturePanoRenderer).GetPanTiltByUV(uv, out pan, out tilt))
                    {
                        element.SetPanTilt(pan, tilt);
                        InteractiveElementSettings.Redraw();
                        EditorUtils.SetDirty(VisualInteractiveElementEditor.hotSpotManager);
                    }
                    GUI.changed = true;
                    e.Use();
                }
            }
            else if (e.type == EventType.MouseUp)
            {
                isDrag = false;
            }
        }

        public void Remove()
        {
            if (hotSpot != null)
            {
                VisualInteractiveElementEditor.hotSpotManager.Remove(hotSpot);
                EditorUtils.SetDirty(VisualInteractiveElementEditor.hotSpotManager);
            }
            else if (direction != null)
            {
                VisualInteractiveElementEditor.directionManager.Remove(direction);
                EditorUtils.SetDirty(VisualInteractiveElementEditor.directionManager);
            }

            Dispose();
        }

        internal void Select()
        {
            isSelected = true;
            OnSelect(this);
        }
    }
}