/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using System;
using System.Reflection;
using UnityEngine.UIElements;

namespace InfinityCode.uContext.UnityTypes
{
    public static class IMGUIContainerRef
    {
        private static FieldInfo _onGUIHandlerField;

        private static FieldInfo onGUIHandlerField
        {
            get
            {
                if (_onGUIHandlerField == null) _onGUIHandlerField = type.GetField("m_OnGUIHandler", Reflection.InstanceLookup);
                return _onGUIHandlerField;
            }
        }

        public static Type type
        {
            get { return typeof(IMGUIContainer); }
        }

        public static Action GetGUIHandler(IMGUIContainer container)
        {
            return (Action)onGUIHandlerField.GetValue(container);
        }

        public static void SetGUIHandler(IMGUIContainer container, Action handler)
        {
            onGUIHandlerField.SetValue(container, handler);
        }
    }
}