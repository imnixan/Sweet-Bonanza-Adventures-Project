using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dot : MonoBehaviour
{
    private const float timeSpeed = .05f;
    private Image image;
    public Vector2 _fieldCoords;
    private RectTransform dotTransform;
    private CircleCollider2D collider;
    private bool _movingDown;
    public Color color;
    public Sprite Sprite
    {
        get { return image.sprite; }
        private set { image.sprite = value; }
    }

    public bool IsMovingDown
    {
        get { return _movingDown; }
    }
    public RectTransform Cell
    {
        set
        {
            dotTransform.SetParent(value);
            StartCoroutine(MoveToPosition());
            _movingDown = true;
            FieldCoords = value.GetComponent<FieldCell>().FieldPosition;
        }
    }

    public Vector2 FieldCoords
    {
        get { return _fieldCoords; }
        private set { _fieldCoords = value; }
    }

    public Vector2 GlobalCoords
    {
        get { return dotTransform.position; }
    }

    public void CreateDot(float size, Vector2 fieldCoords, Sprite pic, Color color)
    {
        image = GetComponent<Image>();
        Sprite = pic;
        collider = GetComponent<CircleCollider2D>();
        collider.radius = size / 2;
        dotTransform = GetComponent<RectTransform>();
        dotTransform.sizeDelta = new Vector2(size, size);
        FieldCoords = fieldCoords;
        this.color = color;
        StartCoroutine(MoveToPosition());
    }

    public void CreateDot(
        float size,
        Sprite pic,
        Vector2 fieldCoords,
        Transform parent,
        Color color
    )
    {
        image = GetComponent<Image>();
        collider = GetComponent<CircleCollider2D>();
        collider.radius = size / 2;
        dotTransform = GetComponent<RectTransform>();
        dotTransform.sizeDelta = new Vector2(size, size);
        dotTransform.SetParent(parent);
        Sprite = pic;
        this.color = color;
        FieldCoords = fieldCoords;
        StartCoroutine(MoveToPosition());
    }

    public void CreateDot()
    {
        image = GetComponent<Image>();
        dotTransform = GetComponent<RectTransform>();
    }

    IEnumerator MoveToPosition()
    {
        float speed = Mathf.Abs(dotTransform.anchoredPosition.y) * timeSpeed;
        while (Mathf.Abs(dotTransform.anchoredPosition.y) > 0)
        {
            dotTransform.anchoredPosition = Vector2.MoveTowards(
                dotTransform.anchoredPosition,
                Vector2.zero,
                speed
            );
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }
        _movingDown = false;
    }

    public void PlayConnectAnim()
    {
        StartCoroutine(ConnectAnim());
    }

    IEnumerator ConnectAnim()
    {
        float defSize = dotTransform.sizeDelta.x;
        float animSize = defSize * 1.1f;
        float speed = animSize / defSize;
        while (dotTransform.sizeDelta.x < animSize)
        {
            dotTransform.sizeDelta = Vector2.MoveTowards(
                dotTransform.sizeDelta,
                new Vector2(animSize, animSize),
                speed
            );
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }
        while (dotTransform.sizeDelta.x > defSize)
        {
            dotTransform.sizeDelta = Vector2.MoveTowards(
                dotTransform.sizeDelta,
                new Vector2(defSize, defSize),
                speed
            );
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }
    }
}
