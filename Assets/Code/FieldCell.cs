using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldCell : MonoBehaviour
{
    private RectTransform _cellTransform;
    private Vector2 _fieldposition;
    public Vector2 FieldPosition
    {
        get
        {
            return  _fieldposition;
        }
    }

    public Vector2 Position
    {
        private set
        {
            _cellTransform.anchoredPosition = value;
        }

        get
        {
            return _cellTransform.anchoredPosition;
        }
    }

    public RectTransform CellTransform
    {
        get
        {
            return _cellTransform;
        } 
            
    }

    public bool IsEmpty
    {
        get
        {
            return _cellTransform.childCount == 0;
        }
    }

    public Dot Dot 
    {
        get
        {   
            if(!IsEmpty)
            {
              return _cellTransform.GetChild(0).GetComponent<Dot>();
            }
            return null;
        }
    }

    public void SetPosition(Vector2 position, Vector2 fieldPosition)
    {
        _fieldposition = fieldPosition;
        _cellTransform = GetComponent<RectTransform>();
        Position = position;
    }
}
