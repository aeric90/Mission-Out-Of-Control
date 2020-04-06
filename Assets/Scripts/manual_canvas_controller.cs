using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class manual_canvas_controller : MonoBehaviour
{
    public static manual_canvas_controller manualCanvasInstance;

    public GameObject loadingPanel;
    public GameObject manualPanel;

    public TMPro.TextMeshProUGUI loadingText;
    public TMPro.TextMeshProUGUI manualCode;

    public Button continueButton;

    private bool manualsReady = false;

    private void Awake()
    {
        if (manualCanvasInstance == null) { manualCanvasInstance = this; }
    }

    void Start()
    {
        StartCoroutine(WaitForManual());
    }

    IEnumerator WaitForManual()
    {
        while(!manualsReady)
        {
            loadingText.text = "please wait";

            for(int i = 1; i <= 10; i++)
            {
                loadingText.text += ".";
                yield return new WaitForSeconds(0.5f);
            }

            yield return null;
        }

        HideLoadingPanel();
        ShowManualPanel();

        continueButton.interactable = true;
    }

    public void HideLoadingPanel()
    {
        loadingPanel.SetActive(false);
    }

    public void ShowManualPanel()
    {
        manualPanel.SetActive(true);
    }

    public void UpdateManualCode(string manualCode)
    {
        this.manualCode.text = manualCode.ToString();
    }

    public void SetManualsReady()
    {
        manualsReady = true;
    }
}
