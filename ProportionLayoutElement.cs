using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 比例布局的元素组件。
/// 用于定义此UI元素在布局主轴方向上所占的权重/份数。
/// </summary>
[RequireComponent(typeof(RectTransform))]
[ExecuteInEditMode]
public class ProportionLayoutElement : MonoBehaviour
{
    [Tooltip("在布局主轴方向上的占比权重/份数。例如，如果两个元素的占比分别为1和3，它们将分别占据父容器可用空间的1/4和3/4。")]
    [Min(0)]
    public float proportion = 1f;

    protected void OnEnable()
    {
        // 当组件启用时，通知父布局更新
        if (transform.parent)
        {
            LayoutRebuilder.MarkLayoutForRebuild(transform.parent as RectTransform);
        }
    }

    protected void OnDisable()
    {
        // 当组件禁用时，也通知父布局更新
        if (transform.parent)
        {
            LayoutRebuilder.MarkLayoutForRebuild(transform.parent as RectTransform);
        }
    }

    protected void OnValidate()
    {
        // 当在Inspector中修改值时，通知父布局更新
        if (transform.parent)
        {
            LayoutRebuilder.MarkLayoutForRebuild(transform.parent as RectTransform);
        }
    }
}