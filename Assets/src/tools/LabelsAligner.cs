using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public enum AlignType
{
    Center, Left, Right
}

public class LabelsAligner : MonoBehaviour
{
    public int Gap;
    public AlignType Align;

    public void Horizontal()
    {
        var labels = GetComponentsInChildren<RectTransform>().Where(label => label.gameObject != gameObject).ToList();
        labels = labels.OrderBy(l => l.position.x).ToList();

        var sizes = labels.Select(label => LayoutUtility.GetPreferredWidth(label)).ToList();
        var totalWidth = sizes.Sum() + Gap * (labels.Count - 1);

        switch (Align)
        {
            case AlignType.Center:
                var xPos = -totalWidth * 0.5f;
                for (var i = 0; i < labels.Count; i++)
                {
                    xPos += sizes[i] * 0.5f;
                    var pos = labels[i].localPosition;
                    labels[i].localPosition = new Vector3(xPos, pos.y, pos.z);
                    xPos += sizes[i] * 0.5f + Gap;
                }
                break;

            case AlignType.Left:
                for (var i = 1; i < labels.Count; i++)
                {
                    var pos = labels[i].localPosition;
                    labels[i].localPosition = new Vector3(labels[i - 1].localPosition.x + sizes[i - 1] + Gap, pos.y, pos.z);
                }
                break;

            case AlignType.Right:
                for (var i = labels.Count - 2; i >= 0; i--)
                {
                    var pos = labels[i].localPosition;
                    labels[i].localPosition = new Vector3(labels[i + 1].localPosition.x - sizes[i + 1] - Gap, pos.y, pos.z);
                }
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
