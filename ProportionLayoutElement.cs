using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Element component for proportion layout.
/// Defines the weight share on the primary axis for this UI element.
/// </summary>
[RequireComponent(typeof(RectTransform))]
[ExecuteInEditMode]
public class ProportionLayoutElement : MonoBehaviour
{
    [Tooltip("Weight share on the primary axis. For example, if two elements have weights 1 and 3, they will take 1/4 and 3/4 of the available space.")]
    [Min(0)]
    public float proportion = 1f;

    [Tooltip("When enabled, clamp this element's primary-axis size to a pixel range.")]
    public bool enablePixelLimit = false;

    [Tooltip("Minimum allowed pixels on the primary axis.")]
    [Min(0)]
    public float minPixels = 0f;

    [Tooltip("Maximum allowed pixels on the primary axis.")]
    [Min(0)]
    public float maxPixels = 0f;

    protected void OnEnable()
    {
        // Notify parent layout when the component is enabled
        if (transform.parent)
        {
            LayoutRebuilder.MarkLayoutForRebuild(transform.parent as RectTransform);
        }
    }

    protected void OnDisable()
    {
        // Notify parent layout when the component is disabled
        if (transform.parent)
        {
            LayoutRebuilder.MarkLayoutForRebuild(transform.parent as RectTransform);
        }
    }

    protected void OnValidate()
    {
        // Notify parent layout when values change in the Inspector
        if (transform.parent)
        {
            LayoutRebuilder.MarkLayoutForRebuild(transform.parent as RectTransform);
        }

        if (minPixels < 0f)
        {
            minPixels = 0f;
        }

        if (maxPixels < 0f)
        {
            maxPixels = 0f;
        }

        if (maxPixels < minPixels)
        {
            maxPixels = minPixels;
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(ProportionLayoutElement))]
public class ProportionLayoutElementEditor : Editor
{
    private SerializedProperty m_Proportion;
    private SerializedProperty m_EnablePixelLimit;
    private SerializedProperty m_MinPixels;
    private SerializedProperty m_MaxPixels;

    private void OnEnable()
    {
        m_Proportion = serializedObject.FindProperty("proportion");
        m_EnablePixelLimit = serializedObject.FindProperty("enablePixelLimit");
        m_MinPixels = serializedObject.FindProperty("minPixels");
        m_MaxPixels = serializedObject.FindProperty("maxPixels");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(m_Proportion);
        EditorGUILayout.PropertyField(m_EnablePixelLimit);

        if (m_EnablePixelLimit.boolValue)
        {
            EditorGUILayout.PropertyField(m_MinPixels);
            EditorGUILayout.PropertyField(m_MaxPixels);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
