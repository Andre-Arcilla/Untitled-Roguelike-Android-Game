using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TownBtnSetupChecker : MonoBehaviour
{
    void Start()
    {
        CheckCollider();
        CheckEventSystem();
        CheckCameraRaycaster();
        CheckInputModule();
    }

    private void CheckCollider()
    {
        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            Debug.LogWarning($"{name}: No Collider found. Adding BoxCollider automatically.");
            gameObject.AddComponent<BoxCollider>();
        }
    }

    private void CheckEventSystem()
    {
        if (EventSystem.current == null)
        {
            Debug.LogWarning("No EventSystem found in the scene. Please add one via GameObject > UI > Event System.");
        }
    }

    private void CheckCameraRaycaster()
    {
        Camera mainCam = Camera.main;
        if (mainCam == null)
        {
            Debug.LogWarning("No main camera found. Make sure you have a camera tagged as 'MainCamera'.");
            return;
        }

        if (mainCam.GetComponent<UnityEngine.EventSystems.PhysicsRaycaster>() == null)
        {
            Debug.LogWarning($"Main Camera '{mainCam.name}' has no PhysicsRaycaster. Adding one automatically.");
            mainCam.gameObject.AddComponent<UnityEngine.EventSystems.PhysicsRaycaster>();
        }
    }

    private void CheckInputModule()
    {
        EventSystem es = EventSystem.current;
        if (es == null) return;

        var inputModule = es.GetComponent<InputSystemUIInputModule>();
        if (inputModule == null)
        {
            Debug.LogWarning("EventSystem does not have an InputSystemUIInputModule. Make sure to use the new Input System UI Input Module instead of StandaloneInputModule.");
        }
    }
}
