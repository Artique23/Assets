using System.Collections;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CutsceneScript : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private RawImage videoDisplay;
    [SerializeField] private Button actionButton; // Skip/continue button
    [SerializeField] private TextMeshProUGUI buttonText; // Text component of the button
    [SerializeField] private CanvasGroup containerCanvasGroup;
    [SerializeField] private CanvasGroup buttonCanvasGroup;

    [Header("Timing Settings")]
    [SerializeField] private float initialDelay = 1.0f;        // Delay before video starts
    [SerializeField] private float buttonShowDelay = 3.0f;     // When button appears after video starts
    [SerializeField] private float transitionDuration = 0.5f;  // How long fade transitions take
    
    [Header("Auto-Fade Settings")]
    [SerializeField] private bool useAutoFade = true;          // Whether to automatically fade out the cutscene
    [SerializeField] private float autoFadeTime = 10.0f;       // How many seconds before auto-fading the cutscene
    [SerializeField] private bool countFromVideoStart = true;  // If true, counts from video start. If false, counts from scene load

    [Header("Timer Info (Read-Only)")]
    [SerializeField] private float currentTime = 0f; // Current timer in seconds
    [SerializeField] private float videoDuration = 0f; // Total video duration in seconds
    [SerializeField] private float timeUntilFade = 0f; // Time remaining until auto-fade
    [SerializeField] private string timerStatus = "Not Started"; // Current status

    [Header("Events")]
    [SerializeField] public UnityEvent onCutsceneComplete;    // Called when cutscene is exited

    private bool hasSkipped = false;
    private bool videoFinished = false;
    private Coroutine autoFadeCoroutine;
    private float videoStartTime = 0f;

    private void Awake()
    {
        // Ensure we have all required components
        if (videoPlayer == null)
            videoPlayer = GetComponentInChildren<VideoPlayer>();
            
        if (videoDisplay == null)
            videoDisplay = GetComponentInChildren<RawImage>();
            
        if (containerCanvasGroup == null)
            containerCanvasGroup = GetComponent<CanvasGroup>();
            
        // Add CanvasGroup if missing
        if (containerCanvasGroup == null)
            containerCanvasGroup = gameObject.AddComponent<CanvasGroup>();

        // Setup button if it exists
        if (actionButton != null)
        {
            // Set initial button text
            if (buttonText != null)
                buttonText.text = "Skip";
                
            // Get or add CanvasGroup to button
            if (buttonCanvasGroup == null)
                buttonCanvasGroup = actionButton.GetComponent<CanvasGroup>();
                
            if (buttonCanvasGroup == null)
                buttonCanvasGroup = actionButton.gameObject.AddComponent<CanvasGroup>();
                
            buttonCanvasGroup.alpha = 0f; // Start invisible
            actionButton.interactable = false;
            
            // Set button listener
            actionButton.onClick.AddListener(OnButtonClick);
        }
        
        // Set up video end event
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached += OnVideoFinished;
            videoPlayer.prepareCompleted += OnVideoPrepared;
        }
        
        // Start auto fade timer if not counting from video start
        if (useAutoFade && !countFromVideoStart)
        {
            timerStatus = "Counting from scene start";
            autoFadeCoroutine = StartCoroutine(AutoFadeAfterDelay(autoFadeTime));
        }
    }

    private void OnVideoPrepared(VideoPlayer vp)
    {
        videoDuration = (float)vp.length;
        Debug.Log($"Video prepared. Duration: {videoDuration} seconds");
    }

    private void Start()
    {
        // Start with everything visible but video paused
        containerCanvasGroup.alpha = 1f;
        currentTime = 0f;
        
        if (videoPlayer != null)
            videoPlayer.Pause();
            
        // Start the delayed sequence
        StartCoroutine(PlayCutsceneSequence());
    }

    private void Update()
    {
        // Update timer info for inspector
        if (videoPlayer != null && videoPlayer.isPlaying)
        {
            currentTime = (float)videoPlayer.time;
            
            if (useAutoFade && countFromVideoStart)
            {
                timeUntilFade = Mathf.Max(0, autoFadeTime - (currentTime - videoStartTime));
            }
        }
        else if (useAutoFade && !countFromVideoStart && !hasSkipped)
        {
            currentTime = Time.time;
            timeUntilFade = Mathf.Max(0, autoFadeTime - Time.time);
        }
    }

    private IEnumerator PlayCutsceneSequence()
    {
        timerStatus = "Waiting for initial delay";
        // Initial delay before starting video
        yield return new WaitForSeconds(initialDelay);
        
        if (hasSkipped) yield break;
        
        // Start playing the video
        if (videoPlayer != null)
        {
            videoPlayer.Play();
            videoStartTime = Time.time;
            timerStatus = "Video playing";
            Debug.Log("Video started playing at time: " + videoStartTime);
            
            // Start auto fade timer if counting from video start
            if (useAutoFade && countFromVideoStart)
            {
                timerStatus = "Counting from video start";
                autoFadeCoroutine = StartCoroutine(AutoFadeAfterDelay(autoFadeTime));
            }
            
            // Wait before showing the button (if it exists)
            if (actionButton != null)
            {
                yield return new WaitForSeconds(buttonShowDelay);
                
                if (hasSkipped) yield break;
                
                // Fade in the button
                if (buttonCanvasGroup != null)
                {
                    buttonCanvasGroup.DOFade(1f, transitionDuration)
                        .SetEase(Ease.InOutQuad)
                        .OnComplete(() => {
                            actionButton.interactable = true;
                            Debug.Log("Action button is now interactable");
                        });
                }
            }
        }
    }

    private IEnumerator AutoFadeAfterDelay(float delay)
    {
        float startTime = Time.time;
        Debug.Log($"Auto-fade will trigger in {delay} seconds (at time: {startTime + delay})");
        
        // Update timer until fade occurs
        while (Time.time < startTime + delay && !hasSkipped)
        {
            timeUntilFade = (startTime + delay) - Time.time;
            yield return null;
        }
        
        if (!hasSkipped)
        {
            timerStatus = "Auto-fade triggered";
            Debug.Log("Auto-fade timer complete, fading out cutscene");
            FadeOutCutscene();
        }
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        videoFinished = true;
        timerStatus = "Video finished";
        
        // Make sure button is visible (if it exists)
        if (actionButton != null && buttonCanvasGroup != null && buttonCanvasGroup.alpha < 1f)
        {
            buttonCanvasGroup.DOFade(1f, transitionDuration)
                .SetEase(Ease.InOutQuad);
            actionButton.interactable = true;
        }
        
        Debug.Log("Video finished");
    }

    public void OnButtonClick()
    {
        if (hasSkipped) return;
        timerStatus = "Button clicked";
        FadeOutCutscene();
    }

    private void FadeOutCutscene()
    {
        if (hasSkipped) return;
        hasSkipped = true;
        timerStatus = "Fading out";
        
        // Cancel any auto-fade coroutine
        if (autoFadeCoroutine != null)
        {
            StopCoroutine(autoFadeCoroutine);
            autoFadeCoroutine = null;
        }
        
        // Stop video playback
        if (videoPlayer != null && videoPlayer.isPlaying)
            videoPlayer.Stop();
            
        // Fade out the entire cutscene container
        containerCanvasGroup.DOFade(0f, transitionDuration)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() => {
                timerStatus = "Completed";
                // Invoke any completion events
                onCutsceneComplete?.Invoke();
                
                // Hide the container entirely when done
                gameObject.SetActive(false);
            });
            
        Debug.Log("Cutscene " + (videoFinished ? "completed" : "skipped"));
    }

    private void OnDisable()
    {
        // Clean up DOTween animations
        DOTween.Kill(containerCanvasGroup);
        if (buttonCanvasGroup != null)
            DOTween.Kill(buttonCanvasGroup);
        
        // Remove listeners
        if (actionButton != null)
            actionButton.onClick.RemoveListener(OnButtonClick);
            
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoFinished;
            videoPlayer.prepareCompleted -= OnVideoPrepared;
        }
    }
}

#if UNITY_EDITOR
// Custom attribute to make fields read-only in the inspector
public class ReadOnlyAttribute : PropertyAttribute { }

    // Custom property drawer for the ReadOnly attribute
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Save previous GUI state
            GUI.enabled = false;
            
            // Draw property with disabled GUI
            EditorGUI.PropertyField(position, property, label, true);
            
            // Restore GUI state
            GUI.enabled = true;
        }
    }
    #endif