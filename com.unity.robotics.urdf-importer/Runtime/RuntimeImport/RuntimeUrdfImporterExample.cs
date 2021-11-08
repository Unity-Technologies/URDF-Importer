using System.Collections;
using System.Collections.Generic;
using Unity.Robotics.UrdfImporter;
using Unity.Robotics.UrdfImporter.Control;
using UnityEngine;

/// <summary>
/// Example component for using the runtime urdf import funcionality.
/// To use, attach to a gameobject and use the GUI to load a URDF.
///
/// Notes:
/// 
/// - This component is for demonstration and testing only
///   and is not intended to be used as is in a final product.
/// 
/// - The runtime import feature is currently only functional in builds created using
///   Mono scripting backend and will not fully work in standalone builds created with
///   IL2CPP due to the dependency to Assimpnet plugin for loading collada meshes.
///   However URDF files that only use STL format for both visual and collision meshes
///   can still be imported in runtime in standalone IL2CPP builds.  
/// </summary>
public class RuntimeUrdfImporterExample : MonoBehaviour
{
    public string urdfFilepath;
    public bool setImmovableLink = true;
    public bool useVHACD = false; // this is not yet fully functional in runtime.
    public bool showProgress = false; // this is not stable in runtime.
    public bool clearOnLoad = true;

    public string immovableLinkName = "base_link";
    // The values below are tested to work with the niryo_one URDF:
    private float controllerStiffness = 10000;
    private float controllerDamping = 100;
    private float controllerForceLimit = 1000;
    private float controllerSpeed = 30;
    private float controllerAcceleration = 10;
    private GameObject currentRobot = null;
    private bool isLoading = false;

    private IEnumerator LoadUrdf()
    {
        isLoading = true;
        if (string.IsNullOrEmpty(urdfFilepath))
        {
            isLoading = false;
            yield break;
        }

        // clear the existing robot to avoid collision
        if (clearOnLoad && currentRobot != null)
        {
            currentRobot.SetActive(false);
            Destroy(currentRobot);
        }
        yield return null;

        ImportSettings settings = new ImportSettings
        {
            chosenAxis = ImportSettings.axisType.yAxis,
            convexMethod = useVHACD ? ImportSettings.convexDecomposer.vHACD : ImportSettings.convexDecomposer.unity,
        };

        GameObject robotObject = null;
        if (showProgress) 
        {
            IEnumerator<GameObject> createRobot = UrdfRobotExtensions.Create(urdfFilepath, settings, showProgress, true);
            yield return createRobot;
            robotObject = createRobot.Current;
        }
        else
        {
            robotObject = UrdfRobotExtensions.CreateRuntime(urdfFilepath, settings);
        }

        if (robotObject != null && robotObject.transform != null) 
        {
            robotObject.transform.SetParent(transform);
            SetControllerParameters(robotObject);
            Debug.Log("Successfully Loaded URDF" + robotObject.name);
        }
        currentRobot = robotObject;
        isLoading = false;
    }

    void SetControllerParameters(GameObject robot)
    {
        if (setImmovableLink) 
        {
            Transform baseNode = robot.transform.FirstChildByQuery(x => x.name == immovableLinkName);
            if (baseNode && baseNode.TryGetComponent<ArticulationBody>(out ArticulationBody baseNodeAB)) 
            {
                baseNodeAB.immovable = true;
            }
        }

        if (robot.TryGetComponent<Controller>(out Controller controller))
        {
            controller.stiffness = controllerStiffness;
            controller.damping = controllerDamping;
            controller.forceLimit = controllerForceLimit;
            controller.speed = controllerSpeed;
            controller.acceleration = controllerAcceleration;
            controller.enabled = true;
        }
    }

    void OnGUI()
    {
        urdfFilepath = GUI.TextField(new Rect(10, 50, 500, 25), urdfFilepath);
        setImmovableLink = GUI.Toggle(new Rect(10, 75, 200, 25), setImmovableLink, "set immovable link");
        if (setImmovableLink)
        {
            immovableLinkName = GUI.TextField(new Rect(220, 75, 200, 25), immovableLinkName);
        }

        useVHACD = GUI.Toggle(new Rect(10, 100, 200, 25), useVHACD, "Use vHACD");
        showProgress = GUI.Toggle(new Rect(10, 125, 200, 25), showProgress, "Show Progress (experimental)");
        if (!isLoading && GUI.Button(new Rect(530, 50, 150, 25), "Load UDRF File"))
        {
            StartCoroutine(LoadUrdf());
        }
    }    
}