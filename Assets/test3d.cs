using UnityEngine;
using System.Collections;

public class test3d : MonoBehaviour
{
	public void OnClick ()
	{
		Debug.Log ("OnClick");
	}

	public void OnHover (bool isOver)
	{
		Debug.Log ("OnHover:" + isOver);
	}

	public void OnPress (bool isDown)
	{
		Debug.Log ("OnPress:" + isDown);
	}

	public void OnSelect (bool selected)
	{
		Debug.Log ("OnSelect:" + selected);
	}

	public void OnDoubleClick ()
	{
		Debug.Log ("OnDoubleClick");
	}

	public void OnDragStart ()
	{
		Debug.Log ("OnDragStart");
	}

	public void OnDrag (Vector2 delta)
	{
		Debug.Log ("OnDrag:" + delta);
	}

	public void OnDragOver (GameObject draggedObject)
	{
		Debug.Log ("OnDragOver:" + draggedObject.name);
	}

	public void OnDragOut (GameObject draggedObject)
	{
		Debug.Log ("OnDragOut:" + draggedObject.name);
	}

	public void OnDragEnd ()
	{
		Debug.Log ("OnDragEnd");
	}

	public void OnTooltip (bool show)
	{
		Debug.Log ("OnTooltip:" + show);
	}
}
