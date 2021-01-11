using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Linq;
using System;


public class EditorCommon : PropertyDrawer
{
    protected Rect m_globalPosition;
    protected Rect m_currentPosition;

    protected SerializedProperty CreatePropertyField( SerializedProperty a_property, string a_subProperty)
    {
        SerializedProperty subProperty = a_property.FindPropertyRelative(a_subProperty);

        return CreatePropertyField(subProperty);
    }


    protected SerializedProperty CreatePropertyField(SerializedProperty a_property)
    {
        EditorGUI.PropertyField(GetNextRect(a_property), a_property, true);

        return a_property;
    }


    protected SerializedProperty CreatePopUp(SerializedProperty a_property, string a_subProperty, List<string> a_choices, Func<SerializedProperty, string> a_stringGetter, out int out_newIndex)
    {
        SerializedProperty subProperty = a_property.FindPropertyRelative(a_subProperty);

        return CreatePopUp(subProperty, a_choices, a_stringGetter, out out_newIndex);
    }


    protected SerializedProperty CreatePopUp(SerializedObject a_object, string a_subProperty, List<string> a_choices, Func<SerializedProperty, string> a_stringGetter, out int out_newIndex)
    {
        SerializedProperty subProperty = a_object.FindProperty(a_subProperty);

        return CreatePopUp(subProperty, a_choices, a_stringGetter, out out_newIndex);
    }


    protected SerializedProperty CreatePopUp(SerializedProperty a_property, List<string> a_choices, Func<SerializedProperty, string> a_stringGetter, out int out_newIndex)
    {
        int currIndex = -1;
        if (a_property != null)
        {
            for (int i = 0; i < a_choices.Count; ++i)
            {
                if (a_choices[i] == a_stringGetter(a_property))
                {
                    currIndex = i;
                    break;
                }
            }
        }


        Rect rect = GetNextRect( a_property);
        out_newIndex = -1;
        out_newIndex = EditorGUI.Popup(rect, a_property.displayName, currIndex, a_choices.ToArray());


        return a_property;
    }

    

    protected Rect GetNextRect(SerializedProperty a_property)
    {
        return GetNextRect(EditorGUI.GetPropertyHeight(a_property/*, a_label*/, true));
    }


    protected Rect GetNextRect(SerializedPropertyType a_serializedPropetyType)
    {
        GUIContent label = new GUIContent();
        return GetNextRect( EditorGUI.GetPropertyHeight(a_serializedPropetyType, label));
    }

    private Rect GetNextRect(float a_propertyheight)
    {
        m_currentPosition.y += m_currentPosition.height + EditorGUIUtility.standardVerticalSpacing;
        m_currentPosition.height = a_propertyheight;
        
        return m_currentPosition;
    }

    protected void GeneratePropetiesFields(SerializedProperty a_property, string[] except)
    {
        SerializedProperty property = a_property.Copy();

        //used to prevent to go to further elements than the current one
        SerializedProperty propertyStop = a_property.Copy();

        //check if not root, because nextvisible(false) is not allowed on root
        SerializedProperty propertyRoot = a_property.Copy();
        propertyRoot.Reset();
  
        if(propertyRoot.propertyPath != propertyStop.propertyPath)
        {
            propertyStop.NextVisible(false);
        }

        bool shoulContinue = property.NextVisible(true);
        bool shoulGoInside = true;

        while (shoulContinue)
        {
            shoulContinue = propertyStop.propertyPath != property.propertyPath;
            shoulGoInside = !except.Contains(property.name);
            if (shoulContinue && shoulGoInside)
            {
                CreatePropertyField( property);
            }
            //don't need to enter ourself inside
            shoulContinue &= property.NextVisible(false);
        }
    }

    protected void GeneratePropetiesFields(SerializedObject a_serializedObj)
    {
        SerializedProperty property = a_serializedObj.GetIterator();
        property.isExpanded = true;
        GeneratePropetiesFields( property, new string[0]);

        a_serializedObj.ApplyModifiedProperties();
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        m_globalPosition = position;
        m_currentPosition = new Rect(position.x, position.y, position.width, 0);
    }


    protected bool GenerateFoldout(SerializedProperty a_property, GUIContent a_label, bool toggleOnLabelClick = true, GUIStyle style = null)
    {
        style  = style ?? EditorStyles.foldout;

        a_property.isExpanded = EditorGUI.Foldout(GetNextRect(SerializedPropertyType.String), a_property.isExpanded, a_property.displayName, toggleOnLabelClick, style);
        EditorGUI.indentLevel += 2;

        return a_property.isExpanded;
    }

    protected Rect ResizeSpritePosition(Rect a_position, Sprite a_sprite,  out Rect o_position)
    {
        Vector2 fullSize = new Vector2(a_sprite.texture.width, a_sprite.texture.height);
        Vector2 size = new Vector2(a_sprite.textureRect.width, a_sprite.textureRect.height);

        Rect coords = a_sprite.textureRect;
        coords.x /= fullSize.x;
        coords.width /= fullSize.x;
        coords.y /= fullSize.y;
        coords.height /= fullSize.y;

        Vector2 ratio;
        ratio.x = a_position.width / size.x;
        ratio.y = a_position.height / size.y;
        float minRatio = Mathf.Min(ratio.x, ratio.y);

        Vector2 center = a_position.center;
        a_position.width = size.x * minRatio;
        a_position.height = size.y * minRatio;
        a_position.center = center;

        o_position = a_position;

        return coords;
    }

    protected void DrawTexturePreview(Rect a_position, Sprite a_sprite)
    {
        Rect pos;
        Rect coords = ResizeSpritePosition(a_position, a_sprite, out pos);

        GUI.DrawTextureWithTexCoords(pos, a_sprite.texture, coords);
    }
}


[System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
public class PreviewSpriteAttribute : PropertyAttribute
{
    [SerializeField]
    private float m_heigth;


    [SerializeField]
    private bool m_displayField;

    public PreviewSpriteAttribute()
    {
        Heigth = 100;
        DisplayField = true;
    }

    public PreviewSpriteAttribute(float a_heigth, bool a_displayField = true)
    {
        if (a_heigth <= 0)
        {
            a_heigth = 100;
        }

        Heigth = a_heigth;

        DisplayField = a_displayField;
    }

    public float Heigth { get => m_heigth; private set => m_heigth = value; }
    public bool DisplayField { get => m_displayField; private set => m_displayField = value; }
}

[CustomPropertyDrawer(typeof(PreviewSpriteAttribute))]
public class PreviewSpriteDrawer : EditorCommon
{

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (property.propertyType == SerializedPropertyType.ObjectReference &&
            (property.objectReferenceValue as Sprite) != null)
        {
            PreviewSpriteAttribute previewSpriteAttribute = attribute as PreviewSpriteAttribute;

            return (previewSpriteAttribute.DisplayField ? EditorGUI.GetPropertyHeight(property, label, true) : 0) + previewSpriteAttribute.Heigth + 10;
        }
        return EditorGUI.GetPropertyHeight(property, label, true);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        PreviewSpriteAttribute previewSpriteAttribute = attribute as PreviewSpriteAttribute;
        //Draw the normal property field
        if (previewSpriteAttribute.DisplayField)
        {
            EditorGUI.PropertyField(position, property, label, true);
            position.y += EditorGUI.GetPropertyHeight(property, label, true) + 5;
        }

        if (property.propertyType == SerializedPropertyType.ObjectReference)
        {
            Sprite sprite = property.objectReferenceValue as Sprite;
            if (sprite != null)
            {
                position.height = previewSpriteAttribute.Heigth;

                //GUI.DrawTexture(position, sprite.texture, ScaleMode.ScaleToFit);
                DrawTexturePreview(position, sprite);
            }
        }
    }

}


