using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// 比例布局控制组件。
/// 根据子对象上唯一的proportion值和自身的布局方向来动态调整其尺寸。
/// 支持反转布局顺序和设置元素间距。
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

    [Tooltip("布局方向：\nHorizontal - 根据proportion设置宽度，高度填满容器。\nVertical - 根据proportion设置高度，宽度填满容器。")]
    [SerializeField]
    private LayoutDirection m_Direction = LayoutDirection.Horizontal;
    public LayoutDirection Direction
    {
        get { return m_Direction; }
        set { SetProperty(ref m_Direction, value); }
    }

    [Tooltip("子元素在布局方向上的间距（像素值）。")]
    [SerializeField]
    private float m_Spacing = 0;
    public float Spacing
    {
        get { return m_Spacing; }
        set { SetProperty(ref m_Spacing, value); }
    }

    [Tooltip("勾选后，子元素的布局顺序将与它们在层级面板中的顺序相反。")]
    [SerializeField]
    private bool m_ReverseOrder = false;
    public bool ReverseOrder
    {
        get { return m_ReverseOrder; }
        set { SetProperty(ref m_ReverseOrder, value); }
    }

    // 缓存带有ProportionLayoutElement的子对象列表
    private readonly List<RectTransform> m_ValidChildren = new List<RectTransform>();

    public override void CalculateLayoutInputHorizontal()
    {
        base.CalculateLayoutInputHorizontal();
        UpdateValidChildren();
    }

    public override void CalculateLayoutInputVertical() { }

    public override void SetLayoutHorizontal()
    {
        SetLayout(0); // 0代表水平轴 (X)
    }

    public override void SetLayoutVertical()
    {
        SetLayout(1); // 1代表垂直轴 (Y)
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
        // 1. 计算总权重
        float totalProportion = 0;
        foreach (var child in m_ValidChildren)
        {
            var element = child.GetComponent<ProportionLayoutElement>();
            totalProportion += Mathf.Max(0, element.proportion);
        }

        if (totalProportion <= 0) return;

        // --- 新增逻辑：计算总间距 ---
        // 如果有N个元素，则有N-1个间距
        float totalSpacing = Mathf.Max(0, m_ValidChildren.Count - 1) * m_Spacing;

        // 2. 计算可用空间，需要减去内边距和总间距
        float availableSize = (axis == 0) ? rectTransform.rect.width - padding.horizontal : rectTransform.rect.height - padding.vertical;
        availableSize = Mathf.Max(0, availableSize - totalSpacing); // 确保可用空间不为负

        float startPos = (axis == 0) ? padding.left : padding.top;

        // 3. 遍历并设置每个子对象的尺寸和位置
        float currentPos = startPos;
        foreach (var child in m_ValidChildren)
        {
            var element = child.GetComponent<ProportionLayoutElement>();
            float proportion = Mathf.Max(0, element.proportion);

            float childSize = (proportion / totalProportion) * availableSize;
            SetChildAlongAxis(child, axis, currentPos, childSize);

            // --- 修改点：更新当前位置时，加上元素尺寸和间距 ---
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