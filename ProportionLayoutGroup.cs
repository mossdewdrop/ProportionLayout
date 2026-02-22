using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Proportion layout controller component.
/// Dynamically sizes children based on their proportion values and layout direction.
/// Supports reversing order and spacing between elements.
/// </summary>
[ExecuteInEditMode]
[RequireComponent(typeof(RectTransform))]
public class ProportionLayoutGroup : LayoutGroup
{
    public enum LayoutDirection
    {
        Horizontal,
        Vertical
    }

    [Tooltip("Layout direction:\nHorizontal - width by proportion, height fills the container.\nVertical - height by proportion, width fills the container.")]
    [SerializeField]
    private LayoutDirection m_Direction = LayoutDirection.Horizontal;
    public LayoutDirection Direction
    {
        get { return m_Direction; }
        set { SetProperty(ref m_Direction, value); }
    }

    [Tooltip("Pixel spacing between children on the primary axis.")]
    [SerializeField]
    private float m_Spacing = 0;
    public float Spacing
    {
        get { return m_Spacing; }
        set { SetProperty(ref m_Spacing, value); }
    }

    [Tooltip("When enabled, child layout order is reversed relative to hierarchy order.")]
    [SerializeField]
    private bool m_ReverseOrder = false;
    public bool ReverseOrder
    {
        get { return m_ReverseOrder; }
        set { SetProperty(ref m_ReverseOrder, value); }
    }

    // Cached list of children that have ProportionLayoutElement
    private readonly List<RectTransform> m_ValidChildren = new List<RectTransform>();

    public override void CalculateLayoutInputHorizontal()
    {
        base.CalculateLayoutInputHorizontal();
        UpdateValidChildren();
    }

    public override void CalculateLayoutInputVertical() { }

    public override void SetLayoutHorizontal()
    {
        SetLayout(0); // 0 represents the horizontal axis (X)
    }

    public override void SetLayoutVertical()
    {
        SetLayout(1); // 1 represents the vertical axis (Y)
    }

    private void UpdateValidChildren()
    {
        m_ValidChildren.Clear();
        for (int i = 0; i < rectChildren.Count; i++)
        {
            var child = rectChildren[i];
            if (child != null && child.gameObject.activeInHierarchy && child.GetComponent<ProportionLayoutElement>() != null)
            {
                m_ValidChildren.Add(child);
            }
        }

        if (m_ReverseOrder)
        {
            m_ValidChildren.Reverse();
        }
    }

    private void SetLayout(int axis)
    {
        if (m_ValidChildren.Count == 0) return;

        bool isHorizontal = (m_Direction == LayoutDirection.Horizontal);

        if ((axis == 0 && isHorizontal) || (axis == 1 && !isHorizontal))
        {
            LayoutPrimaryAxis(axis);
        }
        else
        {
            LayoutSecondaryAxis(axis);
        }
    }

    private void LayoutPrimaryAxis(int axis)
    {
        float totalProportion = 0;
        foreach (var child in m_ValidChildren)
        {
            var element = child.GetComponent<ProportionLayoutElement>();
            totalProportion += Mathf.Max(0, element.proportion);
        }

        if (totalProportion <= 0) return;

        float totalSpacing = Mathf.Max(0, m_ValidChildren.Count - 1) * m_Spacing;

        float availableSize = (axis == 0) ? rectTransform.rect.width - padding.horizontal : rectTransform.rect.height - padding.vertical;
        availableSize = Mathf.Max(0, availableSize - totalSpacing);

        float startPos = (axis == 0) ? padding.left : padding.top;

        int childCount = m_ValidChildren.Count;
        float[] childSizes = new float[childCount];
        float[] unconstrainedProportions = new float[childCount];

        float constrainedSizeSum = 0;
        float unconstrainedTotalProportion = 0;

        for (int i = 0; i < childCount; i++)
        {
            var child = m_ValidChildren[i];
            var element = child.GetComponent<ProportionLayoutElement>();
            float proportion = Mathf.Max(0, element.proportion);
            float initialSize = (proportion / totalProportion) * availableSize;

            if (element.enablePixelLimit)
            {
                float minPixels = Mathf.Max(0, element.minPixels);
                float maxPixels = Mathf.Max(minPixels, element.maxPixels);
                float clampedSize = Mathf.Clamp(initialSize, minPixels, maxPixels);
                childSizes[i] = clampedSize;
                constrainedSizeSum += clampedSize;
            }
            else
            {
                unconstrainedProportions[i] = proportion;
                unconstrainedTotalProportion += proportion;
            }
        }

        float remainingSize = Mathf.Max(0, availableSize - constrainedSizeSum);

        if (unconstrainedTotalProportion > 0)
        {
            for (int i = 0; i < childCount; i++)
            {
                if (unconstrainedProportions[i] > 0)
                {
                    childSizes[i] = (unconstrainedProportions[i] / unconstrainedTotalProportion) * remainingSize;
                }
            }
        }

        float currentPos = startPos;
        for (int i = 0; i < childCount; i++)
        {
            var child = m_ValidChildren[i];
            float childSize = childSizes[i];
            SetChildAlongAxis(child, axis, currentPos, childSize);
            currentPos += childSize + m_Spacing;
        }
    }

    private void LayoutSecondaryAxis(int axis)
    {
        float availableSize = (axis == 0) ? rectTransform.rect.width - padding.horizontal : rectTransform.rect.height - padding.vertical;
        float startPos = (axis == 0) ? padding.left : padding.top;

        foreach (var child in m_ValidChildren)
        {
            SetChildAlongAxis(child, axis, startPos, availableSize);
        }
    }
}
