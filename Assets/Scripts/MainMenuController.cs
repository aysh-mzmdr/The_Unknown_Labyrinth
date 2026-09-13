using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainMenuController : MonoBehaviour
{
    public GameObject menuRoot;
    public RectTransform zombiePreviewRect;
    public Color normalColor = new Color(0.35f, 0.22f, 0.1f);
    public Color highlightColor = new Color(0.85f, 0.55f, 0.15f);
    public Color normalTextColor = new Color(0.86f, 0.67f, 0.28f);
    public Color highlightTextColor = new Color(0.12f, 0.07f, 0.03f);
    public float zombieGap = 25f;
    public MonoBehaviour playerController;
    public GameObject menuCameraObject;
    public GameObject clownPreviewCameraObject;
    public GameObject defaultSelected;
    public AudioSource musicSource;
    public AudioClip gameBgmClip;

    private MenuOptionHighlighter currentHighlighted;

    void Start()
    {
        if (defaultSelected != null && EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(defaultSelected);
        }
    }

    public void SetHighlighted(MenuOptionHighlighter option)
    {
        if (currentHighlighted == option) return;

        if (currentHighlighted != null)
        {
            Image prevBg = currentHighlighted.GetComponent<Image>();
            if (prevBg != null) prevBg.color = normalColor;
            Text prevText = currentHighlighted.GetComponentInChildren<Text>();
            if (prevText != null) prevText.color = normalTextColor;
        }

        currentHighlighted = option;

        Image bg = option.GetComponent<Image>();
        if (bg != null) bg.color = highlightColor;
        Text text = option.GetComponentInChildren<Text>();
        if (text != null) text.color = highlightTextColor;

        if (zombiePreviewRect != null)
        {
            RectTransform optionRect = option.GetComponent<RectTransform>();
            zombiePreviewRect.gameObject.SetActive(true);
            Vector2 targetAnchoredPos = optionRect.anchoredPosition + new Vector2(optionRect.rect.width * 0.5f + zombieGap, 0f);
            zombiePreviewRect.anchoredPosition = targetAnchoredPos;
        }
    }

    public void StartGame()
    {
        if (menuRoot != null) menuRoot.SetActive(false);
        if (menuCameraObject != null) menuCameraObject.SetActive(false);
        if (clownPreviewCameraObject != null) clownPreviewCameraObject.SetActive(false);

        if (playerController != null) playerController.enabled = true;
        if (musicSource != null)
        {
            musicSource.Stop();
            if (gameBgmClip != null)
            {
                musicSource.clip = gameBgmClip;
                musicSource.loop = true;
                musicSource.Play();
            }
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
