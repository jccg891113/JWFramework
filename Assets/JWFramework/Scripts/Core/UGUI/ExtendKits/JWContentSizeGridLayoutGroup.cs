using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class JWContentSizeGridLayoutGroup : GridLayoutGroup, ILayoutSelfController
{
	public enum FitMode
	{
		Unconstrained,
		PreferredSize
	}

	[SerializeField] 
	protected FitMode m_HorizontalFit = FitMode.Unconstrained;

	public FitMode horizontalFit {
		get { return m_HorizontalFit; }
		set {
			if (!m_HorizontalFit.Equals (value)) {
				m_HorizontalFit = value;
				ContentSize ();
			}
		}
	}

	[SerializeField] 
	protected FitMode m_VerticalFit = FitMode.Unconstrained;

	public FitMode verticalFit {
		get { return m_VerticalFit; }
		set {
			if (!m_VerticalFit.Equals (value)) {
				m_VerticalFit = value;
				ContentSize ();
			}
		}
	}

	public override void CalculateLayoutInputHorizontal ()
	{
		rectChildren.Clear ();
		List<Component> toIgnoreList = new List<Component> ();
		for (int i = 0; i < rectTransform.childCount; i++) {
			var rect = rectTransform.GetChild (i) as RectTransform;
//			if (rect == null || !rect.gameObject.activeInHierarchy)
//				continue;
			if (rect == null)
				continue;

			rect.GetComponents (typeof(ILayoutIgnorer), toIgnoreList);

			if (toIgnoreList.Count == 0) {
				rectChildren.Add (rect);
				continue;
			}

			for (int j = 0; j < toIgnoreList.Count; j++) {
				var ignorer = (ILayoutIgnorer)toIgnoreList [j];
				if (!ignorer.ignoreLayout) {
					rectChildren.Add (rect);
					break;
				}
			}
		}
		m_Tracker.Clear ();
		
		int minColumns = 0;
		int preferredColumns = 0;
		if (m_Constraint == Constraint.FixedColumnCount) {
			minColumns = preferredColumns = m_ConstraintCount;
		} else if (m_Constraint == Constraint.FixedRowCount) {
			minColumns = preferredColumns = Mathf.CeilToInt (rectChildren.Count / (float)m_ConstraintCount - 0.001f);
		} else {
			minColumns = 1;
			preferredColumns = Mathf.CeilToInt (Mathf.Sqrt (rectChildren.Count));
		}

		SetLayoutInputForAxis (
			padding.horizontal + (cellSize.x + spacing.x) * minColumns - spacing.x,
			padding.horizontal + (cellSize.x + spacing.x) * preferredColumns - spacing.x,
			-1, 0);
		ContentSize ();
	}

	public override void CalculateLayoutInputVertical ()
	{
		base.CalculateLayoutInputVertical ();
		ContentSize ();
	}

	protected override void OnRectTransformDimensionsChange ()
	{
		base.OnRectTransformDimensionsChange ();
		ContentSize ();
	}

	public override void SetLayoutHorizontal ()
	{
		base.SetLayoutHorizontal ();
		ContentSizeHorizontal ();
	}

	public override void SetLayoutVertical ()
	{
		base.SetLayoutVertical ();
		ContentSizeVertical ();
	}

	private void ContentSize ()
	{
		ContentSizeHorizontal ();
		ContentSizeVertical ();
	}

	private void ContentSizeHorizontal ()
	{
		if (horizontalFit == FitMode.PreferredSize) {
			rectTransform.SetSizeWithCurrentAnchors (RectTransform.Axis.Horizontal, GetTotalPreferredSize (0));
		}
	}

	private void ContentSizeVertical ()
	{
		if (verticalFit == FitMode.PreferredSize) {
			rectTransform.SetSizeWithCurrentAnchors (RectTransform.Axis.Vertical, GetTotalPreferredSize (1));
		}
	}

	private ScrollRect _scrollRect;

	protected ScrollRect scrollRect {
		get {
			if (_scrollRect == null) {
				_scrollRect = GetComponentInParent<ScrollRect> ();
			}
			return _scrollRect;
		}
	}

	private RectTransform _mask;

	protected RectTransform mask {
		get {
			if (_mask == null) {
				var _maskScript = GetComponentInParent<Mask> ();
				if (_maskScript != null) {
					_mask = _maskScript.rectTransform;
				}
			}
			return _mask;
		}
	}

	protected override void OnEnable ()
	{
		base.OnEnable ();
		if (scrollRect != null) {
			scrollRect.onValueChanged.AddListener (OnScrollChange);
		}
	}

	protected override void OnDisable ()
	{
		base.OnDisable ();
		if (scrollRect != null) {
			scrollRect.onValueChanged.RemoveListener (OnScrollChange);
		}
	}

	private void OnScrollChange (Vector2 delta)
	{
		if (scrollRect != null && mask != null) {
			float laft = mask.localPosition.x - mask.pivot.x * mask.rect.size.x - cellSize.x;
			float right = laft + mask.rect.size.x + cellSize.x * 2;
			float top = mask.localPosition.y + (1 - mask.pivot.y) * mask.rect.size.y + cellSize.y;
			float bottom = top - mask.rect.size.y - cellSize.y * 2;
			
			var matrix = mask.parent.worldToLocalMatrix;
			foreach (var item in rectChildren) {
				Vector3 itemLocalPosition = matrix.MultiplyPoint3x4 (item.position);
				item.gameObject.SetActive (itemLocalPosition.x >= laft && itemLocalPosition.x <= right && itemLocalPosition.y >= bottom && itemLocalPosition.y <= top);
			}
		}
	}
}
