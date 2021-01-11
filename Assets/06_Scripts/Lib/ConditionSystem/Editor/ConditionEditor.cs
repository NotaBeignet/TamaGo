using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Linq;
using System;

[CustomPropertyDrawer(typeof(Condition))]
public class ConditionEditor : EditorCommon
{
    protected void GenerateStyle(Rect a_position)
    {
        Color color = GUI.color;

        Color faded = new Color(0, 0, 0);
        faded.a = 0.05f;
        GUI.color = faded;
        GUI.Box(a_position, "");

        GUI.color = new Color(0.95f,0.95f,0.95f);
        Rect bottomline = new Rect(a_position.x, a_position.y + a_position.height, a_position.width, 2);
        GUI.Box(bottomline, "");

        GUI.color = color;
    }


    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        base.OnGUI(position, property, label);

        GenerateStyle(position);
        label = EditorGUI.BeginProperty(position, label, property);

        int originalIndent = EditorGUI.indentLevel;

        // Draw label
        GUIStyle labelStyle = new GUIStyle(EditorStyles.foldout);
        labelStyle.fontStyle = FontStyle.Bold;

        if (GenerateFoldout(property, label, true, labelStyle))
        {
            CreatePropertyField(property, "m_name");

            SerializedProperty isList = CreatePropertyField(property, "m_isList");

            CreatePropertyField(property, "m_isNot");


            if (isList.boolValue)
            {
                CreatePropertyField(property, "m_isOr");
                CreatePropertyField(property, "m_conditions");
            }
            else
            {
                DisplayCondition(position, property, label);
            }
        }

        EditorGUI.indentLevel = originalIndent;

        EditorGUI.EndProperty();

    }




    private void DisplayCondition(Rect a_position, SerializedProperty a_property, GUIContent a_label)
    {

        List<string> allConditionCommandTypeStrings = new List<string>();
        List<System.Type> allConditionCommandType = typeof(ConditionCommand).Assembly.GetTypes().Where(type => type.IsSubclassOf(typeof(ConditionCommand))).ToList();

        foreach (System.Type type in allConditionCommandType)
        {
            allConditionCommandTypeStrings.Add(type.Name);
        }


        //Create PopUp To choice Class
        EditorGUI.BeginChangeCheck();

        int newIndex = 0;

        SerializedProperty commandProperty = CreatePopUp(a_property, "m_conditionCommand", allConditionCommandTypeStrings, (o) => { return (o.objectReferenceValue != null ? o.objectReferenceValue.GetType().Name : null);} , out newIndex);

        if (EditorGUI.EndChangeCheck())
        {
            commandProperty.objectReferenceValue = ScriptableObject.CreateInstance(allConditionCommandType[newIndex]);
        }


        //If Class choosen,display fields and methods popUp
        if (commandProperty.objectReferenceValue != null)
        {
            EditorGUI.indentLevel += 2;

            SerializedObject commandObject = new UnityEditor.SerializedObject(commandProperty.objectReferenceValue);
            GeneratePropetiesFields(commandObject);

            List<string> methodsNames = new List<string>();
            MethodInfo[] methodInfos = allConditionCommandType[newIndex].GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            for (int j = 0; j < methodInfos.Length; ++j)
            {
                methodsNames.Add(methodInfos[j].Name);
            }

            EditorGUI.BeginChangeCheck();

            SerializedProperty methodProperty = CreatePopUp(commandObject, "m_method", methodsNames, (o) => { return o.stringValue; }, out newIndex);

            if (EditorGUI.EndChangeCheck())
            {
                 methodProperty.stringValue = methodsNames[newIndex];
                 commandObject.ApplyModifiedProperties();
            }



            //Parametters of the method
            if (newIndex >= 0)
            {
                EditorGUI.indentLevel+=2;

                MethodInfo method = methodInfos[newIndex];

                for (int i = 0; i < method.GetParameters().Length; i++)
                {
                    ParameterInfo parametter = method.GetParameters()[i];

                    SerializedProperty paramProperty = commandObject.GetIterator();

                    while (paramProperty.Next(true))
                    {
                        if (paramProperty.name == parametter.Name)
                        {
                            CreatePropertyField(paramProperty);
                            break;
                        }
                    }
                }
            }

            commandObject.ApplyModifiedProperties();

        }




    }


    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
      
        float totalHeight = 0;
        SerializedProperty isList = property.FindPropertyRelative("m_isList");


        //to handle the label
        totalHeight += EditorGUI.GetPropertyHeight(SerializedPropertyType.String, label) + EditorGUIUtility.standardVerticalSpacing; 


        if (property.isExpanded)
        {

            totalHeight += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("m_name"), label, true) + EditorGUIUtility.standardVerticalSpacing;       
            totalHeight += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("m_isList"), label, true) + EditorGUIUtility.standardVerticalSpacing;
            totalHeight += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("m_isNot"), label, true) + EditorGUIUtility.standardVerticalSpacing;

            if (!isList.boolValue)
            {
                UnityEngine.Object command = property.FindPropertyRelative("m_conditionCommand").objectReferenceValue;

                //it's condition command & script
                totalHeight += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("m_conditionCommand"), label, true) + EditorGUIUtility.standardVerticalSpacing; 

                if (command != null)
                {

                    SerializedObject commandObject = new UnityEditor.SerializedObject(command as ConditionCommand);

                    SerializedProperty propertyIterator = commandObject.GetIterator();

                    while (propertyIterator.NextVisible(true))
                    {
                        totalHeight += EditorGUI.GetPropertyHeight(propertyIterator, label, true) + EditorGUIUtility.standardVerticalSpacing;
                    }


                    SerializedProperty methodProperty = commandObject.FindProperty("m_method");

                    totalHeight += EditorGUI.GetPropertyHeight(methodProperty, label, true) + EditorGUIUtility.standardVerticalSpacing;

                    MethodInfo methodInfo = (command as ConditionCommand).GetType().GetMethod(methodProperty.stringValue);
                    if (methodInfo != null)
                    {
                        for (int i = 0; i < methodInfo.GetParameters().Length; i++)
                        {
                            ParameterInfo parametter = methodInfo.GetParameters()[i];

                            propertyIterator = commandObject.GetIterator();

                            while (propertyIterator.Next(true))
                            {
                                if (propertyIterator.name == parametter.Name)
                                {
                                    totalHeight += EditorGUI.GetPropertyHeight(propertyIterator, label, true) + EditorGUIUtility.standardVerticalSpacing;
                                    break;
                                }

                            }
                        }
                    }


                }
            }
            else
            {
                totalHeight += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("m_isOr"), label, true) + EditorGUIUtility.standardVerticalSpacing;
                totalHeight += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("m_conditions"), label, true) + EditorGUIUtility.standardVerticalSpacing;
            }


            totalHeight += EditorGUIUtility.standardVerticalSpacing * 3;
        }

        return totalHeight;
    }




}
