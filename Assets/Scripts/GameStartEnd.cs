using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameStartEnd : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text text;
    [SerializeField] private Button button;

    private Image panelImage;
    private float duration = 3f;

    private void Awake()
    {
        canvas.enabled = true;
        button.onClick.AddListener(QuitGame);
        button.gameObject.SetActive(false);
        panelImage = panel.GetComponent<Image>();
        StartCoroutine(SlowFadePanel());
    }

    private IEnumerator SlowFadePanel()
    {
        float elapsed = 0f;
        float startAlpha = panelImage.color.a;
        Color newColor = panelImage.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            newColor.a = Mathf.Lerp(startAlpha, 0f, elapsed / duration);
            panelImage.color = newColor;
            yield return null;
        }

        newColor.a = 0f;
        panelImage.color = newColor;
        canvas.enabled = false;
    }

    public void GameOver()
    {
        canvas.enabled = true;
        panelImage.color = new Color(panelImage.color.r, panelImage.color.g, panelImage.color.b, 1f);
        button.gameObject.SetActive(true);
        
        text.text = "GAME OVER!";
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInput>().DeactivateInput();
    }

    private void QuitGame()
    {
        Debug.Log("Quitting game...");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
