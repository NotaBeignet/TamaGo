/// Credit BinaryX 
/// Sourced from - http://forum.unity3d.com/threads/scripts-useful-4-6-scripts-collection.264161/page-2#post-1945602

using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

[RequireComponent(typeof(ScrollRect))]
[AddComponentMenu("UI/My Scroll Snap")]
public class ScrollSnap : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IScrollSnap
{
    // needed because of reversed behaviour of axis Y compared to X
    // (positions of children lower in children list in horizontal directions grows when in vertical it gets smaller)
    public enum ScrollDirection
    {
        Horizontal,
        Vertical
    }

    ExtendCanvas m_canvasParent;

    ScrollRect m_scrollRect;

    RectTransform m_scrollRectTransform;

    protected Transform m_listContainerTransform;

    int m_pagesCount;

    int m_currentPage = -1;

    // anchor points to lerp to to see child on certain indexes
    Vector3[] m_pageAnchorPositions;

    int m_lerpTarget;

    bool m_isLerping;

    bool m_needToSentSelectEvent;

    float m_listContainerSize;

    RectTransform m_listContainerRectTransform;

    Vector2 m_listContainerOldSize;

    float m_itemSize;

    int m_itemsCount = 0;

    // drag related
    bool m_isDragging = true;

    [Tooltip("Button to go to the next page. (optional)")]
    [SerializeField]
    Button m_nextButton;

    [Tooltip("Button to go to the previous page. (optional)")]
    [SerializeField]
    Button m_prevButton;

    [Tooltip("Number of items visible in one page of scroll frame.")]
    [RangeAttribute(1, 100)]
    [SerializeField]
    int m_itemsVisibleAtOnce = 1;

    [Tooltip("Sets minimum width of list items to 1/itemsVisibleAtOnce.")]
    [SerializeField]
    bool m_utoLayoutItems = true;

    [Tooltip("If you wish to update scrollbar numberOfSteps to number of active children on list.")]
    [SerializeField]
    bool m_linkScrolbarSteps = false;

    [Tooltip("If you wish to update scrollrect sensitivity to size of list element.")]
    [SerializeField]
    bool m_linkScrolrectScrollSensitivity = false;

    [SerializeField]
    float m_speedLerp = 12.5f;

    [SerializeField]
    ScrollDirection m_direction = ScrollDirection.Horizontal;

    [SerializeField]
    float m_thresholdVelocity;

    public Action<ScrollSnapItem> Select { get; set; }

    public Action<ScrollSnapItem> UnSelect { get; set; }
    public Transform ListContainerTransform 
    {
        get 
        {
            if(m_listContainerTransform == null)
            {
                m_listContainerTransform = ScrollRect.content;
            }

            return m_listContainerTransform;
        }

        protected set => m_listContainerTransform = value; 
    }

    public ScrollRect ScrollRect
    {
        get
        {
            if (m_scrollRect == null)
            {
                m_scrollRect = gameObject.GetComponent<ScrollRect>();
            }

            return m_scrollRect;
        }

        protected set => m_scrollRect = value;
    }

    bool m_haveBeenDrag;

    LayoutGroup m_layoutGroup;

    ScrollSnapItem[] m_items;

    float start;
    [SerializeField]
    float m_timeBeforeSelect = 0.5f;


    // Use this for initialization
    protected virtual void Awake()
    {
        m_isLerping = false;

        m_canvasParent = GetComponentInParent<ExtendCanvas>();
        m_scrollRectTransform = gameObject.GetComponent<RectTransform>();
        m_listContainerRectTransform = ListContainerTransform.GetComponent<RectTransform>();
        m_layoutGroup = ListContainerTransform.GetComponent<LayoutGroup>();

        //todo should be adapt as hozitonzal
        if (m_direction == ScrollDirection.Horizontal)
        {
            m_layoutGroup.padding.left = (int)((HorizontalLayoutGroup)m_layoutGroup).spacing / 2;
            m_layoutGroup.padding.right = (int)((HorizontalLayoutGroup)m_layoutGroup).spacing / 2;
        }

        //_rectTransform = _listContainerTransform.gameObject.GetComponent<RectTransform>();
        UpdateListItemsSize();
        UpdateListItemPositions();

        if (m_nextButton)
        {
            m_nextButton.GetComponent<Button>().onClick.AddListener(() =>
            {
                NextScreen();
            });
        }

        if (m_prevButton)
        {
            m_prevButton.GetComponent<Button>().onClick.AddListener(() =>
            {
                PreviousScreen();
            });
        }

        if (ScrollRect.horizontalScrollbar != null && ScrollRect.horizontal)
        {
            ScrollSnapScrollbarHelper hscroll = ScrollRect.horizontalScrollbar.gameObject.AddComponent<ScrollSnapScrollbarHelper>();
            hscroll.ss = this;
        }

        if (ScrollRect.verticalScrollbar != null && ScrollRect.vertical)
        {
            ScrollSnapScrollbarHelper vscroll = ScrollRect.verticalScrollbar.gameObject.AddComponent<ScrollSnapScrollbarHelper>();
            vscroll.ss = this;
        }
    }

    public virtual void Reset()
    {
        Utils.DestroyChildsImmediate(ListContainerTransform);
        ForceUpdate();
    }

    public void UpdateListItemsSize()
    {
        float currentSize = 0;
        if (m_direction == ScrollSnap.ScrollDirection.Horizontal)
        {
            m_itemSize = (m_scrollRectTransform.rect.width - m_itemsVisibleAtOnce * ((HorizontalLayoutGroup)m_layoutGroup).spacing) / m_itemsVisibleAtOnce;
            currentSize = (m_listContainerRectTransform.rect.width - (m_itemsCount - 1)  * ((HorizontalLayoutGroup)m_layoutGroup).spacing - m_layoutGroup.padding.left - m_layoutGroup.padding.right)  / m_itemsCount;
        }
        else
        {
            //todo should be adapt as hozitonzal
            m_itemSize = m_scrollRectTransform.rect.height / m_itemsVisibleAtOnce;
            currentSize = m_listContainerRectTransform.rect.height / m_itemsCount;
        }

        if (m_linkScrolrectScrollSensitivity)
        {
            ScrollRect.scrollSensitivity = m_itemSize;
        }

        if (m_utoLayoutItems && currentSize != m_itemSize && m_itemsCount > 0)
        {
            if (m_direction == ScrollDirection.Horizontal)
            {
                foreach (Transform tr in ListContainerTransform)
                {
                    GameObject child = tr.gameObject;
                    if (child.activeInHierarchy)
                    {
                        LayoutElement childLayout = child.GetComponent<LayoutElement>();

                        if (childLayout == null)
                        {
                            childLayout = child.AddComponent<LayoutElement>();
                        }

                        childLayout.minWidth = m_itemSize;
                    }
                }
            }
            else
            {
                foreach (Transform tr in ListContainerTransform)
                {
                    GameObject child = tr.gameObject;
                    if (child.activeInHierarchy)
                    {
                        LayoutElement childLayout = child.GetComponent<LayoutElement>();

                        if (childLayout == null)
                        {
                            childLayout = child.AddComponent<LayoutElement>();
                        }

                        childLayout.minHeight = m_itemSize;
                    }
                }
            }
        }
    }

    public void UpdateListItemPositions()
    {
        //if size changed, actualize positions of childs
        if (!m_listContainerRectTransform.rect.size.Equals(m_listContainerOldSize))
        {
            m_listContainerOldSize.Set(m_listContainerRectTransform.rect.size.x, m_listContainerRectTransform.rect.size.y);

            // checking how many children of list are active
            int activeCount = 0;

            foreach (Transform tr in ListContainerTransform)
            {
                if (tr.gameObject.activeInHierarchy)
                {
                    activeCount++;
                }
            }

            m_itemsCount = activeCount;

            m_pagesCount = Mathf.Max(activeCount - m_itemsVisibleAtOnce + 1, 1);

            // if anything changed since last check reinitialize anchors list
            Array.Resize(ref m_pageAnchorPositions, m_pagesCount);

            if (activeCount > 0)
            {
                if (m_direction == ScrollDirection.Horizontal)
                {
                    // setting at 0 to get origin position
                    ScrollRect.horizontalNormalizedPosition = 0;

                    m_listContainerSize = (m_itemSize + ((HorizontalLayoutGroup)m_layoutGroup).spacing) * m_pagesCount; 

                    //create item local position
                    for (int i = 0; i < m_pagesCount; i++)
                    {
                        m_pageAnchorPositions[i] = new Vector3(
                            //todo should be adapt as hozitonzal
                            ListContainerTransform.localPosition.x - (m_listContainerSize / m_pagesCount) * i,
                            ListContainerTransform.localPosition.y,
                            ListContainerTransform.localPosition.z
                        );
                    }
                }
                else
                {
                    //Debug.Log ("-------------looking for list spanning range----------------");
                    // looking for list spanning range
                    ScrollRect.verticalNormalizedPosition = 1;
                 //   m_listContainerMinPosition = m_listContainerTransform.localPosition.y;
                    ScrollRect.verticalNormalizedPosition = 0;
                 //   m_listContainerMaxPosition = m_listContainerTransform.localPosition.y;

               //     m_listContainerSize = m_listContainerMaxPosition - m_listContainerMinPosition;

                  /*  for (var i = 0; i < m_pagesCount; i++)
                    {
                        m_pageAnchorPositions[i] = new Vector3(
                            m_listContainerTransform.localPosition.x,
                            m_listContainerMinPosition + m_itemSize * i,
                            m_listContainerTransform.localPosition.z
                        );
                    }*/
                }

                UpdateScrollbar(m_linkScrolbarSteps);
              //  m_currentPage = Mathf.Min(m_currentPage, m_pagesCount);
               // ResetPage();

                ChangePageInstant(m_currentPage);
            }
        }

    }

    public void ResetPage()
    {
        if (m_direction == ScrollDirection.Horizontal)
        {
            ScrollRect.horizontalNormalizedPosition = m_pagesCount > 1 ? (float)m_currentPage / (float)(m_pagesCount - 1) : 0;
        }
        else
        {
            ScrollRect.verticalNormalizedPosition = m_pagesCount > 1 ? (float)(m_pagesCount - m_currentPage - 1) / (float)(m_pagesCount - 1) : 0;
        }
    }

    private void UpdateScrollbar(bool a_linkSteps)
    {
        if (a_linkSteps)
        {
            if (m_direction == ScrollDirection.Horizontal)
            {
                if (ScrollRect.horizontalScrollbar != null)
                {
                    ScrollRect.horizontalScrollbar.numberOfSteps = m_pagesCount;
                }
            }
            else
            {
                if (ScrollRect.verticalScrollbar != null)
                {
                    ScrollRect.verticalScrollbar.numberOfSteps = m_pagesCount;
                }
            }
        }
        else
        {
            if (m_direction == ScrollDirection.Horizontal)
            {
                if (ScrollRect.horizontalScrollbar != null)
                {
                    ScrollRect.horizontalScrollbar.numberOfSteps = 0;
                }
            }
            else
            {
                if (ScrollRect.verticalScrollbar != null)
                {
                    ScrollRect.verticalScrollbar.numberOfSteps = 0;
                }
            }
        }
    }


    public void ForceUpdate()
    {
     /*   if(m_currentPage != -1)
        {*/
            Canvas.ForceUpdateCanvases();
       // }
        m_currentPage = -1;
        UpdateListItemsSize();
        UpdateListItemPositions();
    }

    void LateUpdate()
    {
        if (!m_canvasParent.IsEnabled())
        {
            return;
        }

        UpdateListItemsSize();
        UpdateListItemPositions();


        if(VisiblePage() != m_currentPage)
        {
            PageChanged(VisiblePage());
            start = Time.time;
            m_needToSentSelectEvent = true;
        }

        //should handle vertical (y velocity)
        if (m_haveBeenDrag && Mathf.Abs(ScrollRect.velocity.x) < m_thresholdVelocity)
        {
            m_haveBeenDrag = false;
            m_isLerping = true;
            m_lerpTarget = VisiblePage();
        }

        //if we are not lerping nor in velocity mode
        if(m_needToSentSelectEvent && ((!m_isLerping && Time.time - start > m_timeBeforeSelect) || m_isLerping && m_lerpTarget == VisiblePage()) && !m_haveBeenDrag)
        {
            m_needToSentSelectEvent = false;
            if (Select != null)
            {
                Select(ListContainerTransform.GetChild(m_currentPage).GetComponent<ScrollSnapItem>());
            }
        }

        if (m_isLerping)
        {
            UpdateScrollbar(false);

            ListContainerTransform.localPosition = Vector3.Lerp(ListContainerTransform.localPosition, m_pageAnchorPositions[m_lerpTarget], m_speedLerp * Time.deltaTime);

            if (Vector3.Distance(ListContainerTransform.localPosition, m_pageAnchorPositions[m_lerpTarget]) < 0.001f)
            {
                ListContainerTransform.localPosition = m_pageAnchorPositions[m_lerpTarget];
                m_isLerping = false;
                UpdateScrollbar(m_linkScrolbarSteps);
            }
        }
    }

    //Function for switching screens with buttons
    public void NextScreen()
    {
        UpdateListItemPositions();

        if (VisiblePage() < m_pagesCount - 1)
        {
            m_isLerping = true;
            m_lerpTarget = VisiblePage() + 1;
        }
    }

    //Function for switching screens with buttons
    public void PreviousScreen()
    {
        UpdateListItemPositions();

        if (VisiblePage() > 0)
        {
            m_isLerping = true;
            m_lerpTarget = VisiblePage() - 1;
        }
    }

    //returns the current screen that the is seeing
    public int VisiblePage()
    {
        float pos = 0;
        float page = 0;

        if (m_direction == ScrollDirection.Horizontal)
        {
            pos = m_pageAnchorPositions[0].x - ListContainerTransform.localPosition.x;
            pos = Mathf.Clamp(pos, 0, m_listContainerSize);
            page = pos / (m_listContainerSize / m_pagesCount);
        }
        else
        {
           // pos = m_listContainerTransform.localPosition.y - m_listContainerMinPosition;
            pos = Mathf.Clamp(pos, 0, m_listContainerSize);
            page = pos / (m_listContainerSize / m_pagesCount);
        }

        return Mathf.Clamp(Mathf.RoundToInt(page), 0, m_pagesCount - 1);
    }

    /// <summary>
    /// Added to provide a uniform interface for the ScrollBarHelper
    /// </summary>
    public void SetLerp(bool value)
    {
        m_isLerping = value;
    }

    public void ChangePage(int a_page)
    {
        a_page = Mathf.Clamp(a_page, 0, m_pagesCount - 1);
        m_isLerping = true;

        m_lerpTarget = a_page;
    }

    public void ChangePageInstant(int a_page)
    {
        a_page = Mathf.Clamp(a_page, 0, m_pagesCount - 1);

        m_isLerping = true;
        m_lerpTarget = a_page;
        ListContainerTransform.localPosition = m_pageAnchorPositions[a_page];
    }


    //changes the bullets on the bottom of the page - pagination
    private void PageChanged(int currentPage)
    {

        if (currentPage != m_currentPage)
        {
            if (UnSelect != null && ListContainerTransform.childCount > m_currentPage && m_currentPage >= 0)
            {
                UnSelect(ListContainerTransform.GetChild(m_currentPage).GetComponent<ScrollSnapItem>());
            }

            if (m_itemsVisibleAtOnce == 1)
            {
                if (ListContainerTransform.childCount > m_currentPage && m_currentPage >= 0)
                {
                    ListContainerTransform.GetChild(m_currentPage).GetComponent<ScrollSnapItem>().SetIsSelected(false);
                }

                if (ListContainerTransform.childCount > currentPage && currentPage >= 0)
                {
                    ListContainerTransform.GetChild(currentPage).GetComponent<ScrollSnapItem>().SetIsSelected(true);
                }
            }
        }

        m_currentPage = currentPage;

        if (m_nextButton)
        {
            m_nextButton.interactable = currentPage < m_pagesCount - 1;
        }

        if (m_prevButton)
        {
            m_prevButton.interactable = currentPage > 0;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        m_isDragging = true;
        m_haveBeenDrag = false;

        UpdateScrollbar(false);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        m_isDragging = false;

        m_haveBeenDrag = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        m_isLerping = false;

        if (!m_isDragging)
        {
            OnBeginDrag(eventData);
        }
    }

    public void StartScreenChange() { }
}