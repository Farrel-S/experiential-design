using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // Safe even if you don’t use TMP

public class BookCarousel : MonoBehaviour
{
    [Header("Books (Image Components)")]
    public List<Image> books;

    [Header("Anchors (UI RectTransforms)")]
    public RectTransform leftAnchor;
    public RectTransform centerAnchor;
    public RectTransform rightAnchor;

    [Header("Scale")]
    public Vector3 centerScale = Vector3.one;
    public Vector3 sideScale = new Vector3(0.8f, 0.8f, 1f);

    [Header("Color")]
    public Color centerColor = Color.white;
    public Color sideColor = new Color(0.6f, 0.6f, 0.6f, 1f);

    [Header("Title Text Color")]
    public Color centerTextColor = Color.white;
    public Color sideTextColor = new Color(0.7f, 0.7f, 0.7f, 1f);

    [Header("Animation")]
    public float moveSpeed = 10f;

    private int centerIndex = 0;
    private Vector2 swipeStart;

    void Start()
    {
        UpdateCarouselInstant();
    }

    void Update()
    {
        AnimateCarousel();
        HandleSwipe();
    }

    void AnimateCarousel()
    {
        for (int i = 0; i < books.Count; i++)
        {
            Image book = books[i];

            RectTransform targetAnchor;
            Vector3 targetScale;
            Color targetColor;
            Color targetTextColor;

            if (i == centerIndex)
            {
                targetAnchor = centerAnchor;
                targetScale = centerScale;
                targetColor = centerColor;
                targetTextColor = centerTextColor;

                book.transform.SetAsLastSibling();
            }
            else if (i == GetLeftIndex())
            {
                targetAnchor = leftAnchor;
                targetScale = sideScale;
                targetColor = sideColor;
                targetTextColor = sideTextColor;
            }
            else
            {
                targetAnchor = rightAnchor;
                targetScale = sideScale;
                targetColor = sideColor;
                targetTextColor = sideTextColor;
            }

            book.rectTransform.position =
                Vector3.Lerp(book.rectTransform.position, targetAnchor.position, Time.deltaTime * moveSpeed);

            book.rectTransform.localScale =
                Vector3.Lerp(book.rectTransform.localScale, targetScale, Time.deltaTime * moveSpeed);

            book.color =
                Color.Lerp(book.color, targetColor, Time.deltaTime * moveSpeed);

            // ---- TITLE TEXT (TMP or Legacy) ----
            TMP_Text tmp = book.GetComponentInChildren<TMP_Text>();
            if (tmp != null)
            {
                tmp.color = Color.Lerp(tmp.color, targetTextColor, Time.deltaTime * moveSpeed);
            }
            else
            {
                Text txt = book.GetComponentInChildren<Text>();
                if (txt != null)
                {
                    txt.color = Color.Lerp(txt.color, targetTextColor, Time.deltaTime * moveSpeed);
                }
            }
        }
    }

    void UpdateCarouselInstant()
    {
        for (int i = 0; i < books.Count; i++)
        {
            Image book = books[i];

            if (i == centerIndex)
            {
                ApplyInstant(book, centerAnchor, centerScale, centerColor, centerTextColor);
                book.transform.SetAsLastSibling();
            }
            else if (i == GetLeftIndex())
            {
                ApplyInstant(book, leftAnchor, sideScale, sideColor, sideTextColor);
            }
            else
            {
                ApplyInstant(book, rightAnchor, sideScale, sideColor, sideTextColor);
            }
        }
    }

    void ApplyInstant(Image book, RectTransform anchor, Vector3 scale, Color color, Color textColor)
    {
        book.rectTransform.position = anchor.position;
        book.rectTransform.localScale = scale;
        book.color = color;

        TMP_Text tmp = book.GetComponentInChildren<TMP_Text>();
        if (tmp != null)
        {
            tmp.color = textColor;
        }
        else
        {
            Text txt = book.GetComponentInChildren<Text>();
            if (txt != null)
            {
                txt.color = textColor;
            }
        }
    }

    int GetLeftIndex()
    {
        return (centerIndex - 1 + books.Count) % books.Count;
    }

    int GetRightIndex()
    {
        return (centerIndex + 1) % books.Count;
    }

    public void SwipeLeft()
    {
        centerIndex = GetRightIndex();
    }

    public void SwipeRight()
    {
        centerIndex = GetLeftIndex();
    }

    void HandleSwipe()
    {
        if (Input.GetMouseButtonDown(0))
            swipeStart = Input.mousePosition;

        if (Input.GetMouseButtonUp(0))
        {
            float deltaX = Input.mousePosition.x - swipeStart.x;

            if (deltaX > 60f)
                SwipeRight();
            else if (deltaX < -60f)
                SwipeLeft();
        }
    }
}
