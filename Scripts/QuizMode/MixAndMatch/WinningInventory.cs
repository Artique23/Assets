using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class WinningInventory : MonoBehaviour
{
    public int CurrentRound => currentRound;
    [Header("Game Setup")]
    public InventorySlot[] winningSlots;
    public TextMeshProUGUI[] slotLabels;
    public List<GameObject> roadSignPrefabs;
    public Transform roadSignsContainer;
    public Button nextButton;
    public Button resetButton;

    [Header("Game Settings")]
    public int totalRounds;

    [Header("Animation Settings")]
    public float buttonBreatheDuration = 1.5f;
    public float buttonBreathScale = 1.1f;
    public float signFadeDuration = 0.5f;
    public float signReplaceDelay = 0.2f; // Delay between fade-out and instantiation

    [Header("References")]
    public GameObject quizPanel;

    private List<GameObject> spawnedSigns = new();
    private List<string> currentRoundSignNames = new();
    private List<string> usedSignNames = new();
    private List<Tween> activeFadeTweens = new();

    private int currentRound = 0;
    private bool roundComplete = false;
    private Tween buttonBreatheTween;
    private bool isQuizPanelActive = false;

    void Start()
    {
        nextButton.gameObject.SetActive(false);
        nextButton.onClick.AddListener(StartNextRound);
        resetButton.onClick.AddListener(ResetCurrentRound);
        StartNextRound();
    }

    void Update()
    {
        if (!roundComplete && AreAllSlotsCorrect())
        {
            roundComplete = true;
            OnRoundComplete();
        }
    }

    void OnDestroy()
    {
        if (buttonBreatheTween != null) buttonBreatheTween.Kill();
    }

    public void StartNextRound()
    {
        if (buttonBreatheTween != null) buttonBreatheTween.Kill();

        roundComplete = false;
        currentRoundSignNames.Clear();
        ReturnSignsToContainer();

        if (nextButton != null) nextButton.gameObject.SetActive(false);

        // Fade out current signs first, then delay, then spawn new
        List<DraggableItem> signs = new();
        foreach (Transform child in roadSignsContainer)
        {
            DraggableItem item = child.GetComponentInChildren<DraggableItem>();
            if (item != null) signs.Add(item);
        }

        FadeOutSigns(signs, () =>
        {
            // Delay after fade-out
            DOVirtual.DelayedCall(signReplaceDelay, () =>
            {
                SelectRandomSigns();
                PopulateInventoryWithCurrentSigns();

                // Fade in new signs
            DOVirtual.DelayedCall(0.05f, () =>
            {
                List<DraggableItem> newSigns = new();
                foreach (Transform child in roadSignsContainer)
                {
                    DraggableItem item = child.GetComponentInChildren<DraggableItem>();
                    if (item != null) newSigns.Add(item);
                }
                FadeInSigns(newSigns);
            });

                for (int i = 0; i < winningSlots.Length && i < currentRoundSignNames.Count; i++)
                {
                    if (winningSlots[i] != null)
                        winningSlots[i].expectedSignName = currentRoundSignNames[i];

                    if (slotLabels != null && i < slotLabels.Length && slotLabels[i] != null)
                        slotLabels[i].text = currentRoundSignNames[i];
                }

                currentRound++;
                Debug.Log($"Starting Round {currentRound}");
            });
        });
    }

    private void FadeOutSigns(List<DraggableItem> signs, TweenCallback onComplete)
    {
        foreach (Tween t in activeFadeTweens)
            if (t.IsActive()) t.Kill();
        activeFadeTweens.Clear();

        Sequence fadeSequence = DOTween.Sequence();
        foreach (DraggableItem sign in signs)
        {
            if (sign == null) continue;

            CanvasGroup canvasGroup = sign.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = sign.gameObject.AddComponent<CanvasGroup>();

            Tween fade = canvasGroup.DOFade(0, signFadeDuration);
            activeFadeTweens.Add(fade);
            fadeSequence.Join(fade);
        }
        fadeSequence.OnComplete(onComplete);
    }

public void FadeInCurrentSigns()
{
    List<DraggableItem> signs = new List<DraggableItem>();
    foreach (Transform child in roadSignsContainer)
    {
        DraggableItem item = child.GetComponentInChildren<DraggableItem>();
        if (item != null) signs.Add(item);
    }
    FadeInSigns(signs);
}

    public void FadeInSigns(List<DraggableItem> signs)
    {
        foreach (Tween t in activeFadeTweens)
            if (t.IsActive()) t.Kill();
        activeFadeTweens.Clear();

        Sequence fadeSequence = DOTween.Sequence();

        foreach (DraggableItem sign in signs)
        {
            if (sign == null) continue;

            CanvasGroup canvasGroup = sign.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = sign.gameObject.AddComponent<CanvasGroup>();

            // Force alpha to 0 before fading in
            canvasGroup.alpha = 0;

            Tween fade = canvasGroup
                .DOFade(1, signFadeDuration)
                .OnComplete(() => canvasGroup.alpha = 1); // Ensure it's set to 1

            activeFadeTweens.Add(fade);
            fadeSequence.Join(fade);
        }

        // Optional: slight delay before starting fade
        fadeSequence.PrependInterval(0.05f);
    }


    private void SelectRandomSigns()
    {
        if (usedSignNames.Count >= roadSignPrefabs.Count)
        {
            usedSignNames.Clear();
            Debug.Log("All signs used! Resetting usedSignNames.");
        }

        List<GameObject> availableSigns = new(roadSignPrefabs);
        availableSigns.RemoveAll(sign => usedSignNames.Contains(sign.name));

        int signsToSelect = Mathf.Min(winningSlots.Length, availableSigns.Count);
        currentRoundSignNames.Clear();

        for (int i = 0; i < signsToSelect; i++)
        {
            int index = Random.Range(0, availableSigns.Count);
            GameObject selected = availableSigns[index];
            string signName = selected.name;
            currentRoundSignNames.Add(signName);
            usedSignNames.Add(signName);
            availableSigns.RemoveAt(index);
        }
    }

    private void PopulateInventoryWithCurrentSigns()
    {
        // Kill all fade tweens before changing objects
        foreach (Tween t in activeFadeTweens)
            if (t.IsActive()) t.Kill();
        activeFadeTweens.Clear();

        HashSet<string> allSignNames = new(currentRoundSignNames);
        List<string> distractors = new();

        foreach (var prefab in roadSignPrefabs)
            if (!allSignNames.Contains(prefab.name)) distractors.Add(prefab.name);

        // Shuffle distractors
        for (int i = distractors.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (distractors[i], distractors[j]) = (distractors[j], distractors[i]);
        }

        foreach (var d in distractors)
        {
            if (allSignNames.Count >= 15) break;
            allSignNames.Add(d);
        }

        List<string> shuffledSigns = new(allSignNames);
        for (int i = shuffledSigns.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (shuffledSigns[i], shuffledSigns[j]) = (shuffledSigns[j], shuffledSigns[i]);
        }

        // Destroy old signs
        for (int i = 0; i < roadSignsContainer.childCount; i++)
        {
            Transform slot = roadSignsContainer.GetChild(i);
            if (slot.childCount > 0)
                Destroy(slot.GetChild(0).gameObject);
        }

        // Instantiate new signs
        for (int i = 0; i < roadSignsContainer.childCount && i < shuffledSigns.Count; i++)
        {
            Transform slot = roadSignsContainer.GetChild(i);
            string signName = shuffledSigns[i];

            GameObject prefab = roadSignPrefabs.Find(p => p.name == signName);
            if (prefab != null)
            {
            GameObject newSign = Instantiate(prefab, slot);
            newSign.name = signName;

            // Ensure CanvasGroup exists and alpha is 0 for fade-in
            CanvasGroup cg = newSign.GetComponent<CanvasGroup>();
            if (cg == null) cg = newSign.AddComponent<CanvasGroup>();
            cg.alpha = 0;

            var draggable = newSign.GetComponent<DraggableItem>();
            if (draggable != null)
                draggable.originalParent = slot;

            }
        }
    }

    private bool AreAllSlotsCorrect()
    {
        if (currentRoundSignNames.Count == 0) return false;

        for (int i = 0; i < winningSlots.Length && i < currentRoundSignNames.Count; i++)
        {
            var slot = winningSlots[i];
            if (slot == null || slot.transform.childCount == 0) return false;

            Transform child = slot.transform.GetChild(0);
            string name = child.name.Replace("(Clone)", "");
            if (name != currentRoundSignNames[i]) return false;
        }

        return true;
    }

    private void OnRoundComplete()
    {
        Debug.Log("Round Complete!");
        if (nextButton != null)
        {
            nextButton.gameObject.SetActive(true);
            StartButtonBreathingAnimation();
        }
    }

    private void StartButtonBreathingAnimation()
    {
        if (buttonBreatheTween != null) buttonBreatheTween.Kill();

        Vector3 originalScale = nextButton.transform.localScale;
        Vector3 targetScale = originalScale * buttonBreathScale;

        buttonBreatheTween = nextButton.transform
            .DOScale(targetScale, buttonBreatheDuration)
            .SetEase(Ease.InOutQuad)
            .SetLoops(-1, LoopType.Yoyo);
    }

    public void ResetCurrentRound()
    {
        ReturnSignsToContainer();
        roundComplete = false;

        if (nextButton != null)
        {
            if (buttonBreatheTween != null)
            {
                buttonBreatheTween.Kill();
                buttonBreatheTween = null;
            }
            nextButton.gameObject.SetActive(false);
        }
    }

    private void ReturnSignsToContainer()
    {
        foreach (Tween t in activeFadeTweens)
            if (t.IsActive()) t.Kill();
        activeFadeTweens.Clear();

        foreach (InventorySlot slot in winningSlots)
        {
            if (slot == null) continue;

            List<Transform> children = new();
            foreach (Transform child in slot.transform)
                children.Add(child);

            foreach (Transform child in children)
            {
                if (child == null) continue;

                DraggableItem draggable = child.GetComponent<DraggableItem>();
                if (draggable != null)
                    draggable.ReturnToOriginalSlot();
                else
                    child.SetParent(roadSignsContainer);
            }
        }
    }

    public void ResetUsedSigns()
    {
        usedSignNames.Clear();
    }

    public void SetQuizPanelActive(bool active)
    {
        isQuizPanelActive = active;
    }
    public void ShuffleRoadSigns()
{
    PopulateInventoryWithCurrentSigns(); // or your intended shuffle logic
}
}
