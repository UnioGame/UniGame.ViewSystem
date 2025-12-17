namespace UniGame.ViewSystem.Inspector.Editor.Utilities
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;
    using UnityEngine;
    using UnityEditor;
    using Inspector;
    using UnityEngine.UIElements;

    /// <summary>
    /// Utility class for reflection operations on serialized properties
    /// </summary>
    public static class PropertyDrawerUtility
    {
        /// <summary>
        /// Gets the target object from a SerializedProperty
        /// </summary>
        public static object GetTargetObjectOfProperty(SerializedProperty prop)
        {
            if (prop == null) return null;

            string[] elements = prop.propertyPath.Split('.');
            object obj = prop.serializedObject.targetObject;
            int index = 0;

            while (index < elements.Length)
            {
                if (elements[index] == "Array")
                {
                    // Handle arrays
                    index++;
                    if (index < elements.Length && elements[index].StartsWith("data["))
                    {
                        int arrayIndex = int.Parse(elements[index].Substring(5, elements[index].Length - 6));
                        if (obj is IList list)
                        {
                            obj = list[arrayIndex];
                        }
                        index++;
                    }
                }
                else
                {
                    FieldInfo field = obj.GetType().GetField(elements[index], 
                        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    
                    if (field != null)
                    {
                        obj = field.GetValue(obj);
                    }
                    index++;
                }
            }

            return obj;
        }

        /// <summary>
        /// Gets a method from target object
        /// </summary>
        public static MethodInfo GetMethodFromObject(object target, string methodName)
        {
            if (target == null) return null;
            return target.GetType().GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        }

        /// <summary>
        /// Gets a field from target object
        /// </summary>
        public static FieldInfo GetFieldFromObject(object target, string fieldName)
        {
            if (target == null) return null;
            return target.GetType().GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        }

        /// <summary>
        /// Gets a property from target object
        /// </summary>
        public static PropertyInfo GetPropertyFromObject(object target, string propertyName)
        {
            if (target == null) return null;
            return target.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        }

        /// <summary>
        /// Tries to get a boolean value from condition (field, property, or method)
        /// </summary>
        public static bool TryGetConditionValue(object target, string conditionName, out bool result)
        {
            result = false;

            if (target == null) return false;

            // Try field
            FieldInfo field = GetFieldFromObject(target, conditionName);
            if (field != null && field.FieldType == typeof(bool))
            {
                result = (bool)field.GetValue(target);
                return true;
            }

            // Try property
            PropertyInfo property = GetPropertyFromObject(target, conditionName);
            if (property != null && property.PropertyType == typeof(bool))
            {
                result = (bool)property.GetValue(target);
                return true;
            }

            // Try method
            MethodInfo method = GetMethodFromObject(target, conditionName);
            if (method != null && method.ReturnType == typeof(bool) && method.GetParameters().Length == 0)
            {
                result = (bool)method.Invoke(target, null);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Loads a style sheet from a relative path
        /// </summary>
        public static StyleSheet LoadStyleSheet(string relativePath)
        {
            // Build path relative to the Inspector Editor folder
            string editorPath = "Packages/com.unigame.viewsystem/Inspector/Editor";
            string fullPath = $"{editorPath}/{relativePath}";
            return Resources.Load<StyleSheet>(fullPath);
        }
    }
}
