using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.XR.ARCoreExtensions;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class AppContoller : MonoBehaviour
{
    public GameObject HostedPointPrefab;
    public GameObject ResolvedPointPrefab;
    public ARAnchorManager AnchorManager;
    public ARRaycastManager RaycastManager;
    public InputField InputField;
    public Text OutputText;
    public Text cloudAnchorId;
    public GameObject panel;

    private enum AppMode
    {
        // Wait for user to tap screen to begin hosting a point.
        TouchToHostCloudReferencePoint,

        // Poll hosted point state until it is ready to use.
        WaitingForHostedReferencePoint,

        // Wait for user to tap screen to begin resolving the point.
        TouchToResolveCloudReferencePoint,

        // Poll resolving point state until it is ready to use.
        WaitingForResolvedReferencePoint,
    }

    private AppMode m_AppMode = AppMode.TouchToHostCloudReferencePoint;
    private ARCloudAnchor m_CloudReferencePoint;
    private string m_CloudReferenceId;

    // Start is called before the first frame update
    void Start()
    {
        InputField.onEndEdit.AddListener(OnInputEndEdit);
        panel.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_AppMode == AppMode.TouchToHostCloudReferencePoint)
        {
            OutputText.text = m_AppMode.ToString();

            if (Input.touchCount >= 1
                && Input.GetTouch(0).phase == TouchPhase.Began
                && !EventSystem.current.IsPointerOverGameObject(
                        Input.GetTouch(0).fingerId))
            {
                List<ARRaycastHit> hitResults = new List<ARRaycastHit>();
                RaycastManager.Raycast(Input.GetTouch(0).position, hitResults);
                if (hitResults.Count > 0)
                {
                    Pose pose = hitResults[0].pose;

                    // dokundurulan yerde referans noktası oluşturuyor.
                    ARAnchor referencePoint =
                        AnchorManager.AddAnchor(hitResults[0].pose);

                    // bulut referans noktasını oluşturuyor.
                    m_CloudReferencePoint =
                         AnchorManager.HostCloudAnchor(referencePoint);
                    if (m_CloudReferencePoint == null)
                    {
                        OutputText.text = "Create Failed!";
                        return;
                    }

                    // Wait for the reference point to be ready.
                    m_AppMode = AppMode.WaitingForHostedReferencePoint;
                }
            }
        }
        else if (m_AppMode == AppMode.WaitingForHostedReferencePoint)
        {
            OutputText.text = m_AppMode.ToString();

            CloudAnchorState cloudReferenceState =
                m_CloudReferencePoint.cloudAnchorState;
            OutputText.text += " - " + cloudReferenceState.ToString();

            if (cloudReferenceState == CloudAnchorState.Success)
            {
                GameObject cloudAnchor = Instantiate(
                                             HostedPointPrefab,
                                             Vector3.zero,
                                             Quaternion.identity);
                cloudAnchor.transform.SetParent(
                    m_CloudReferencePoint.transform, false);
                panel.gameObject.SetActive(true);
                m_CloudReferenceId = m_CloudReferencePoint.cloudAnchorId;
                cloudAnchorId.text = m_CloudReferenceId;
                m_CloudReferencePoint = null;
                m_AppMode = AppMode.TouchToHostCloudReferencePoint;
            }
        }

    }
    private void OnInputEndEdit(string text)
    {
        m_CloudReferenceId = string.Empty;

        m_CloudReferencePoint =
            AnchorManager.ResolveCloudAnchorId(text);
        if (m_CloudReferencePoint == null)
        {
            OutputText.text = "Resolve Failed!";
            m_AppMode = AppMode.TouchToHostCloudReferencePoint;
            return;
        }

        // Wait for the reference point to be ready.
        m_AppMode = AppMode.WaitingForResolvedReferencePoint;
    }
}
