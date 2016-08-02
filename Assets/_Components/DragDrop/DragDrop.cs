﻿using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using DG.Tweening;

public class DragDrop : MonoBehaviour
{
	// DragDrop Group
	public GameObject dragDropContainer;
	public GameObject dragDropObject;

	// A certain distance to decide whether it is in the place or not  
	public Vector2 detectMargin;

	// Structure
	private RectTransform _dragDropRectTransform;
//	private Vector2 _dragDropRectSize;
	private RectTransform _objectRectTransform;
	private Vector2 _objectRectSize;

	// Drag Event Variables
	private int _originalOrder;
	private int _dragEndOrder;
	private GameObject _replacedObject;
	private Vector3 _realtimeDragPosition;
	private Vector3 _dragBeginPosition;
	private Vector3 _dragEndPosition;
	private float _spacingX;
	private float _spacingY;
	private float _row;
	private float _column;
	private Vector3 _dragDeltaBeginPosition;
	private Vector3 _dragDeltaPosition;

	void Awake ()
	{
		_dragDropRectTransform = GetComponent<RectTransform> ();
//		_dragDropRectSize = _dragDropRectTransform.sizeDelta;
		_objectRectTransform = transform.parent.gameObject.GetComponent<RectTransform> ();
		_objectRectSize = _objectRectTransform.sizeDelta;

		_spacingX = dragDropContainer.GetComponent<GridLayoutGroup> ().spacing.x;
		_spacingY = dragDropContainer.GetComponent<GridLayoutGroup> ().spacing.y;

		_row = dragDropContainer.GetComponent<DragDrop_Container> ().row;
		_column = dragDropContainer.GetComponent<DragDrop_Container> ().column;
	}

	void Start ()
	{
		
	}

	Vector3 GetDragEndPosition ()
	{
		return transform.localPosition;
	}

	int GetClosetInteger (float f, float limit)
	{
		if (f < 0)
		{
			return 0;
		}
		else if (f > (limit - 1))
		{
			return (int)limit - 1;
		}
		else if (f >= 0 && f - (int)f > 0.5f)
		{
			return (int)f + 1;
		}
		else
		{
			return (int)f;
		}
	}

	int GetObjectOrder ()
	{
		return dragDropObject.GetComponent<DragDrop_Object> ().objectOrder;
	}

	int GetDragEndOrder ()
	{
		// objPosX + dragEndX = objW / 2  + (objW + SpaceX) * x
		// objPosY + dragEndY = - objH / 2 - (objH + SpaceY) * y

		float x;
		float y;
		int order;

		// Get Drag Object's DragEnd Position
		_dragEndPosition = GetDragEndPosition ();

		// Get arrange order (x, y)
		x = (_objectRectTransform.localPosition.x + _dragEndPosition.x - _objectRectSize.x / 2) / (_objectRectSize.x + _spacingX);
		y = 0 - (_objectRectTransform.localPosition.y + _dragEndPosition.y + _objectRectSize.y / 2) / (_objectRectSize.y + _spacingY);

		Debug.Log (_dragEndPosition.x.ToString () + ", " + _dragEndPosition.y.ToString ());
		Debug.Log (x.ToString () + ", " + y.ToString ());

		x = GetClosetInteger (x, _row);
		y = GetClosetInteger (y, _column);

		Debug.Log (x.ToString () + ", " + y.ToString ());

		// order = column * x + y
		order = (int)x + (int)_column * (int)y;

		Debug.Log (order);

		return order;
	}

	public void OnDragBegin ()
	{
		// Save current Team Member Order
		_originalOrder = GetObjectOrder ();

		// Display in the top layer
		dragDropObject.transform.SetAsLastSibling ();

		_dragBeginPosition = _dragDropRectTransform.localPosition;
		_dragDeltaBeginPosition = Input.mousePosition;
	}

	public void OnDrag ()
	{
		_realtimeDragPosition = Input.mousePosition;

		_dragDeltaPosition = _realtimeDragPosition - _dragDeltaBeginPosition;

		_dragDropRectTransform.localPosition = _dragBeginPosition + new Vector3 (_dragDeltaPosition.x, _dragDeltaPosition.y, 0);
	}

	public void OnDragEnd ()
	{
		// Get Drag Object's DragEnd Order
		_dragEndOrder = GetDragEndOrder ();

		// Get Replaced Object by DragEnd Order
		_replacedObject = dragDropContainer.GetComponent<DragDrop_Container> ().GetObjectByOrder (_dragEndOrder);

		// Change _replacedMember's order to _originalOrder
		_replacedObject.GetComponent<DragDrop_Object> ().ChangeObjectOrder (_originalOrder);

		// Change DragMember (this Member) 's order to dragEndOrder
		dragDropObject.GetComponent<DragDrop_Object> ().ChangeObjectOrder (_dragEndOrder);

		// Change GameObject of _replaceMember and DragMember
		dragDropContainer.GetComponent<DragDrop_Container> ().ChangeObjectByOrder (_dragEndOrder, _originalOrder);

		// Change Position of _replacedMember and DragMember
		dragDropContainer.GetComponent<DragDrop_Container> ().ChangeObjectPosition (dragDropObject, _replacedObject);

	}
}
