
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Iridescent.LightGroupControl
{

#if UNITY_EDITOR
    using UnityEditor;

    public sealed class NonEditableAttribute : PropertyAttribute
    {
    }

    [CustomPropertyDrawer(typeof(NonEditableAttribute))]
    public sealed class NonEditableAttributeDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = true;
        }
    }
#endif

    [Serializable]
    public class LightValue
    {
        [NonEditable] public string lightName;
        public Color color = Color.white;
        public float intensity = 1f;
        public float bounceIntensity = 1f;
        public float range = 10f;

        public LightValue()
        {
            Reset();
        }

        public void Reset()
        {
            color = Color.clear;
            intensity = 0f;
            bounceIntensity = 9f;
            range = 0f;
        }
    }

    public class LightGroup : MonoBehaviour
    {

        public List<Light> lights = new List<Light>();

        // public Dictionary<Light,LightValue> lightValueDict = new Dictionary<Light, LightValue>();
        public List<LightValue> lightValues = new List<LightValue>();
        // {
        //     get
        //     {
        //         return lightValueDict.Values.ToList();
        //     }
        // }

        // Start is called before the first frame update
        void Start()
        {

        }

        private void OnValidate()
        {
            // lightValues.Clear();
            // lightValueDict.Clear();
            if (lightValues.Count > lights.Count)
            {
                lightValues.RemoveRange(lights.Count, lightValues.Count - lights.Count);
            }
            else if (lightValues.Count < lights.Count)
            {
                for (int i = lightValues.Count; i < lights.Count; i++)
                {
                    lightValues.Add(new LightValue());
                }
            }

            var lightIndex = 0;
            foreach (var l in lights)
            {
                lightValues[lightIndex].lightName = l.gameObject.name;
                lightIndex++;
            }
        }

        [ContextMenu("Search in children")]
        public void SearchInChildren()
        {
            lights = GetComponentsInChildren<Light>().ToList();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}