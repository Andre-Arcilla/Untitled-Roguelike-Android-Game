using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (mainCamera == null)
            mainCamera = Camera.main;

        ApplyCameraToCanvases();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    [SerializeField] private Camera mainCamera;
    [SerializeField] private List<Canvas> targetCanvases = new List<Canvas>();

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        ApplyCameraToCanvases();
    }

    private void ApplyCameraToCanvases()
    {
        foreach (Canvas canvas in targetCanvases)
        {
            if (canvas != null && canvas.renderMode == RenderMode.ScreenSpaceCamera)
            {
                canvas.worldCamera = mainCamera;
            }
        }
    }

    public void RegisterCanvas(Canvas canvas)
    {
        if (!targetCanvases.Contains(canvas))
        {
            targetCanvases.Add(canvas);
            if (canvas.renderMode == RenderMode.ScreenSpaceCamera && mainCamera != null)
                canvas.worldCamera = mainCamera;
        }
    }

    public void SetMainCamera(Camera camera)
    {
        mainCamera = camera;
        ApplyCameraToCanvases();
    }

    public Camera GetMainCamera()
    {
        return mainCamera;
    }
}
