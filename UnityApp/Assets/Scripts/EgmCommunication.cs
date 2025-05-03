using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* EGM */
using Abb.Egm;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Google.Protobuf;
using System.ComponentModel;
using System;
using TMPro;

public class EgmCommunication : MonoBehaviour
{
    /* UDP port where EGM communication should happen (specified in RobotStudio) */
    public static int port = 6511;
    /* UDP client used to send messages from computer to robot */
    private UdpClient server = null;
    /* Endpoint used to store the network address of the ABB robot.
     * Make sure your robot is available on your local network. The easiest option
     * is to connect your computer to the management port of the robot controller
     * using a network cable. */
    private IPEndPoint robotAddress;
    /* Variable used to count the number of messages sent */
    private uint sequenceNumber = 0;

    /* Robot joints values (in degrees) */
    /* If you are using a robot with 7 degrees of freedom (e.g., YuMi), 
       please adapt this code. */
    private static double j1, j2, j3, j4, j5, j6;
    /* Current state of EGM communication (disconnected, connected or running) */
    private string egmState = "Undefined";

    /* This worker creates a secondary thread that listens to every message
     * sent by the robot over the network. */
    private BackgroundWorker worker;

    public Slider j1Slider, j2Slider, j3Slider, j4Slider, j5Slider, j6Slider;
    public TextMeshProUGUI egmStateText;
    private List<double[]> waypoints = new List<double[]>();
    public main_ui_control uiControl; // Assign this in Unity Inspector




    /* (Unity) Start is called before the first frame update */
    void Start()
    {
        /* Initializes EGM connection with robot */
        CreateConnection();

        j1Slider.onValueChanged.AddListener(delegate { SendJointsMessageToRobot(j1Slider.value, j2, j3, j4, j5, j6); });
        j2Slider.onValueChanged.AddListener(delegate { SendJointsMessageToRobot(j1, j2Slider.value, j3, j4, j5, j6); });
        j3Slider.onValueChanged.AddListener(delegate { SendJointsMessageToRobot(j1, j2, j3Slider.value, j4, j5, j6); });
        j4Slider.onValueChanged.AddListener(delegate { SendJointsMessageToRobot(j1, j2, j3, j4Slider.value, j5, j6); });
        j5Slider.onValueChanged.AddListener(delegate { SendJointsMessageToRobot(j1, j2, j3, j4, j5Slider.value, j6); });
        j6Slider.onValueChanged.AddListener(delegate { SendJointsMessageToRobot(j1, j2, j3, j4, j5, j6Slider.value); });

        UpdateSlidersWithJointValues();
        
        // Initially hide all arrows
        CreateJointDiscs();
    }

    


    


   



    /* (Unity) Update is called once per frame */
    void Update()
    {
        egmStateText.text = "EGM State: " + egmState;
        if (server.Available > 0) // Check if new data is available
        {
            UpdateSlidersWithJointValues();
        }
        
    }

    /* (Unity) OnApplicationQuit is called when the program is closed */
    void OnApplicationQuit()
    {
        worker.CancelAsync(); /* Destroys secondary thread */
    }

    


    public void CreateConnection()
    {
        if (server != null) // Prevent multiple bindings
        {
            server.Close();
            server = null;
        }

        IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6511); // Bind to the same port as in RobotStudio
        server = new UdpClient(localEndPoint); // This will listen for RobotStudio's EGM messages
    }



    private void UpdateSlidersWithJointValues()
    {
        /* Receives the messages sent by the robot in as a byte array */
        var bytes = server.Receive(ref robotAddress);

        if (bytes != null)
        {
            /* De-serializes the byte array using the EGM protocol */
            EgmRobot message = EgmRobot.Parser.ParseFrom(bytes);

            ParseJointValuesFromMessage(message);
        }

        j1Slider.value = (float)j1;
        j2Slider.value = (float)j2;
        j3Slider.value = (float)j3;
        j4Slider.value = (float)j4;
        j5Slider.value = (float)j5;
        j6Slider.value = (float)j6;
    }

    private void ParseJointValuesFromMessage(EgmRobot message)
    {
        /* Parse the current robot position and EGM state from message
            received from robot and update the related variables */

        /* Checks if header is valid */
        if (message.Header.HasSeqno && message.Header.HasTm)
        {
            j1 = message.FeedBack.Joints.Joints[0];
            j2 = message.FeedBack.Joints.Joints[1];
            j3 = message.FeedBack.Joints.Joints[2];
            j4 = message.FeedBack.Joints.Joints[3];
            j5 = message.FeedBack.Joints.Joints[4];
            j6 = message.FeedBack.Joints.Joints[5];
            egmState = message.MciState.State.ToString();

            // Update diagnostic panel
            double[] jointValues = { j1, j2, j3, j4, j5, j6 };
            uiControl.UpdateDiagnosticPanel(jointValues);
        }
        else
        {
            Debug.Log("The message received from robot is invalid.");
        }
    }

    private void SendJointsMessageToRobot(double j1, double j2, double j3, double j4, double j5, double j6)
    {
        /* Send message containing new positions to robot in EGM format.
         * This is the primary method used to move the robot in cartesian coordinates. */

        /* Warning: If you are planning to manipulate an ABB robot with Hololens, this implementation
         * will not work. Hololens runs under Universal Windows Platform (UWP), which at the present
         * moment does not work with UdpClient class. DatagramSocket should be used instead. */

        using (MemoryStream memoryStream = new MemoryStream())
        {
            EgmSensor message = new EgmSensor();
            /* Prepare a new message in EGM format */
            CreateJointsMessage(message, j1, j2, j3, j4, j5, j6);

            message.WriteTo(memoryStream);

            /* Send the message as a byte array over the network to the robot */
            int bytesSent = server.Send(memoryStream.ToArray(), (int)memoryStream.Length, robotAddress);

            if (bytesSent < 0)
            {
                Debug.Log("No message was sent to robot.");
            }
        }
    }

    private void CreateJointsMessage(EgmSensor message, double j1, double j2, double j3, double j4, double j5, double j6)
    {
        /* Create a message in EGM format specifying a new joint configuration 
           for the ABB robot. The message contains a header with general
           information and a body with the planned joint configuration.

           Notice that in order for this code to work, your robot must be running a EGM client 
           in RAPID containing EGMActJoint and EGMRunJoint methods.

           See one example here: https://github.com/vcuse/egm-for-abb-robots/blob/main/EGMJointCommunication.modx */

        EgmHeader hdr = new EgmHeader();
        hdr.Seqno = sequenceNumber++;
        hdr.Tm = (uint) DateTime.Now.Ticks;
        hdr.Mtype = EgmHeader.Types.MessageType.MsgtypeCorrection;

        message.Header = hdr;
        EgmPlanned plannedTrajectory = new EgmPlanned();
        EgmJoints jointsConfiguration = new EgmJoints();

        jointsConfiguration.Joints.Add(j1);
        jointsConfiguration.Joints.Add(j2);
        jointsConfiguration.Joints.Add(j3);
        jointsConfiguration.Joints.Add(j4);
        jointsConfiguration.Joints.Add(j5);
        jointsConfiguration.Joints.Add(j6);

        plannedTrajectory.Joints = jointsConfiguration;
        message.Planned = plannedTrajectory;
    }

    public static double GetJointPosition(int jointIndex)
    {
        switch (jointIndex)
        {
            case 0: return j1;
            case 1: return j2;
            case 2: return j3;
            case 3: return j4;
            case 4: return j5;
            case 5: return j6;
            default:
                Debug.LogError("Invalid joint index.");
                return 0.0;
        }
    }

    public GameObject waypointPrefab; // Assign this in Unity Inspector

    public Transform endEffectorTransform; // Assign the robot's end effector (e.g., a GameObject representing the tool) in Unity Inspector

    public void AddWaypoint()
    {
        double[] currentWaypoint = { j1, j2, j3, j4, j5, j6 };
        waypoints.Add(currentWaypoint);

        // Use the end effector's position as the waypoint position
        Vector3 waypointPosition = endEffectorTransform.position;

        // Instantiate a visual marker for the waypoint
        GameObject waypointInstance = Instantiate(waypointPrefab, waypointPosition, Quaternion.identity);
        waypointInstance.tag = "Waypoint";

        // Update line renderer positions
        UpdateLineRenderer();

        Debug.Log("Waypoint added at position: " + waypointPosition);
    }



    private void UpdateLineRenderer()
    {
        if (waypoints.Count < 2)
        {
            // No lines to draw if there are fewer than 2 waypoints
            lineRenderer.positionCount = 0;
            return;
        }

        // Set line renderer position count
        lineRenderer.positionCount = waypoints.Count;

        for (int i = 0; i < waypoints.Count; i++)
        {
            // Use instantiated waypoint markers' positions for line rendering
            GameObject[] existingWaypoints = GameObject.FindGameObjectsWithTag("Waypoint");
            if (existingWaypoints.Length > i)
            {
                lineRenderer.SetPosition(i, existingWaypoints[i].transform.position);
            }
        }
    }




    public void PlayWaypoints()
    {
        if (waypoints.Count == 0)
        {
            Debug.LogWarning("No waypoints to play.");
            return;
        }

        // Add current position as starting point if needed
        if (!IsAtWaypoint(waypoints[0]))
        {
            AddCurrentPositionAsStartPoint();
        }

        StartCoroutine(PlayWaypointsCoroutine());
    }

    private bool IsAtWaypoint(double[] waypoint)
    {
        return Mathf.Approximately((float)j1, (float)waypoint[0]) &&
            Mathf.Approximately((float)j2, (float)waypoint[1]) &&
            Mathf.Approximately((float)j3, (float)waypoint[2]) &&
            Mathf.Approximately((float)j4, (float)waypoint[3]) &&
            Mathf.Approximately((float)j5, (float)waypoint[4]) &&
            Mathf.Approximately((float)j6, (float)waypoint[5]);
    }

    private void AddCurrentPositionAsStartPoint()
    {
        double[] currentPosition = { j1, j2, j3, j4, j5, j6 };
        waypoints.Insert(0, currentPosition);
        Debug.Log("Added current position as start point.");
    }


    public void ClearWaypoints()
    {
        StopAllCoroutines(); // Stop playback

        // Find and destroy all objects tagged as "Waypoint"
        GameObject[] existingWaypoints = GameObject.FindGameObjectsWithTag("Waypoint");
        foreach (GameObject wp in existingWaypoints)
        {
            Destroy(wp);
        }

        // Clear the list of waypoints
        waypoints.Clear();

        // Reset line renderer
        lineRenderer.positionCount = 0;

        Debug.Log("All waypoints cleared.");
    }



    private IEnumerator PlayWaypointsCoroutine()
    {
        if (waypoints.Count == 0)
        {
            Debug.LogWarning("No waypoints to play.");
            yield break;
        }

        for (int i = 0; i < waypoints.Count - 1; i++)
        {
            double[] startPoint = waypoints[i];
            double[] endPoint = waypoints[i + 1];

            float duration = 2f; // Time to move between waypoints
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;

                // Interpolate joint values
                double[] interpolatedJoints = new double[startPoint.Length];
                for (int j = 0; j < startPoint.Length; j++)
                {
                    interpolatedJoints[j] = Mathf.Lerp(
                        (float)startPoint[j], 
                        (float)endPoint[j], 
                        elapsedTime / duration
                    );
                }

                // Send interpolated joint values to the robot
                SendJointsMessageToRobot(
                    interpolatedJoints[0], 
                    interpolatedJoints[1], 
                    interpolatedJoints[2], 
                    interpolatedJoints[3], 
                    interpolatedJoints[4], 
                    interpolatedJoints[5]
                );

                yield return null; // Wait for the next frame
            }
        }

        Debug.Log("Finished playing all waypoints.");
    }


    public void ExportWaypointsToRAPID()
    {
        if (waypoints.Count == 0)
        {
            Debug.LogWarning("No waypoints available for export.");
            return;
        }

        // Define RAPID script header
        string rapidScript = "MODULE Module1\n";
        rapidScript += "VAR jointtarget waypoint;\n";
        rapidScript += "PROC Main()\n";

        // Add waypoints as RAPID commands
        for (int i = 0; i < waypoints.Count; i++)
        {
            double[] wp = waypoints[i];
            rapidScript += $"  waypoint := [[{wp[0]:F2},{wp[1]:F2},{wp[2]:F2},{wp[3]:F2},{wp[4]:F2},{wp[5]:F2}], [9E9,9E9,9E9,9E9,9E9,9E9]];\n";
            rapidScript += $"  MoveAbsJ waypoint, v100, fine, tool0;\n";
        }

        // Close RAPID script
        rapidScript += "ENDPROC\n";
        rapidScript += "ENDMODULE";

        // Save to file
        string filePath = Application.dataPath + "/Waypoints.mod"; // Save in Assets folder
        File.WriteAllText(filePath, rapidScript);

        Debug.Log($"RAPID script exported to {filePath}");
    }

    public LineRenderer lineRenderer; // Assign this in Unity Inspector
    
    public Material highlightMaterial; // Assign this in Unity Inspector
    public Material defaultMaterial; // Assign the default material for joints
    public GameObject joint1, joint2, joint3, joint4, joint5, joint6; // Assign these in Unity Inspector

    private Coroutine resetHighlightCoroutine;

    private void HighlightJoint(GameObject joint)
    {
        // Cancel any pending resets
        if (resetHighlightCoroutine != null)
        {
            StopCoroutine(resetHighlightCoroutine);
        }

        ResetAllJoints();

        Renderer renderer = joint.GetComponentInChildren<Renderer>();
        if (renderer != null)
        {
            Material[] materials = renderer.materials;
            for (int i = 0; i < materials.Length; i++)
            {
                if (materials[i].name.Contains("Mat4"))
                {
                    materials[i] = highlightMaterial;
                }
            }
            renderer.materials = materials;
        }

        // Optional: Auto-reset after delay (e.g., 2 seconds)
        resetHighlightCoroutine = StartCoroutine(ResetHighlightAfterDelay(10f));
    }



    private IEnumerator ResetHighlightAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Reset all joints to their default material
        ResetAllJoints();
        resetHighlightCoroutine = null;
    }


    public void ResetAllJoints()
    {
        // Reset each joint's material to the default material
        ResetJointMaterial(joint1);
        ResetJointMaterial(joint2);
        ResetJointMaterial(joint3);
        ResetJointMaterial(joint4);
        ResetJointMaterial(joint5);
        ResetJointMaterial(joint6);
    }

    private void ResetJointMaterial(GameObject joint)
    {
        Renderer renderer = joint.GetComponentInChildren<Renderer>();
        if (renderer != null)
        {
            Material[] materials = renderer.materials;
            for (int i = 0; i < materials.Length; i++)
            {
                // Check for BOTH original and highlighted materials
                if (materials[i].name.Contains("Mat4") || 
                    materials[i].name.Contains(highlightMaterial.name))
                {
                    materials[i] = defaultMaterial;
                }
            }
            renderer.materials = materials;
        }
    }

    public void ResetAllHighlights()
    {
        // Stop any ongoing delayed reset coroutine
        if (resetHighlightCoroutine != null)
        {
            StopCoroutine(resetHighlightCoroutine);
            resetHighlightCoroutine = null;
        }
        
        // Immediately reset all joints
        ResetAllJoints();
    }



    public void HighlightJoint1()
    {
        HighlightJoint(joint1);
    }

    public void HighlightJoint2()
    {
        HighlightJoint(joint2);
    }

    public void HighlightJoint3()
    {
        HighlightJoint(joint3);
    }

    public void HighlightJoint4()
    {
        HighlightJoint(joint4);
    }

    public void HighlightJoint5()
    {
        HighlightJoint(joint5);
    }

    public void HighlightJoint6()
    {
        HighlightJoint(joint6);
    }

    private double[] homePosition = { 0, 0, 0, 0, 0, 0 }; // Default joint values for home position

    public void ResetToHomePosition()
    {
        // Start a coroutine to smoothly move the robot to its home position
        StartCoroutine(SmoothResetToHome(2f)); // 2 seconds duration for smooth transition
    }

    private IEnumerator SmoothResetToHome(float duration)
    {
        // Get the current joint values
        double[] currentJointValues = { j1, j2, j3, j4, j5, j6 };

        // Target joint values (home position)
        double[] targetJointValues = homePosition;

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            // Interpolate between current and target joint values
            double[] interpolatedValues = new double[currentJointValues.Length];
            for (int i = 0; i < currentJointValues.Length; i++)
            {
                interpolatedValues[i] = Mathf.Lerp(
                    (float)currentJointValues[i],
                    (float)targetJointValues[i],
                    elapsedTime / duration
                );
            }

            // Send interpolated joint values to the robot
            SendJointsMessageToRobot(
                interpolatedValues[0],
                interpolatedValues[1],
                interpolatedValues[2],
                interpolatedValues[3],
                interpolatedValues[4],
                interpolatedValues[5]
            );

            yield return null; // Wait until the next frame
        }

        // Ensure final position matches exactly
        SendJointsMessageToRobot(
            targetJointValues[0],
            targetJointValues[1],
            targetJointValues[2],
            targetJointValues[3],
            targetJointValues[4],
            targetJointValues[5]
        );

        Debug.Log("Robot reset to home position.");
    }

    public GameObject[] joints; // Assign each joint GameObject in the Inspector
    public Material discMaterial; // Material for the rotation discs
    public float discRadius = 0.5f; // Radius of the rotation discs
    private GameObject[] jointDiscVisuals; // Array to store disc visualizations


    private void CreateJointDiscs()
    {
        jointDiscVisuals = new GameObject[joints.Length];

        for (int i = 0; i < joints.Length; i++)
        {
            if (joints[i] == null) continue;

            // Create a parent GameObject for disc visualization
            GameObject discParent = new GameObject($"Joint{i + 1}_Disc");

            // Parent it locally to the joint
            discParent.transform.SetParent(joints[i].transform, false);

            // Align it with the joint's local pivot
            discParent.transform.localPosition = Vector3.zero;

            // Set rotation based on the joint's axis
            discParent.transform.localRotation = GetDiscRotation(i);

            // Determine rotation range text dynamically (example values)
            string rangeText = GetRotationRangeText(i);

            // Calculate radius dynamically based on joint size
            float dynamicRadius = CalculateDiscRadius(joints[i]);

            // Create the rotation disc with range text
            CreateRotationDisc(discParent.transform, dynamicRadius, GetDiscColor(i), rangeText);

            // Store reference for toggling visibility later
            jointDiscVisuals[i] = discParent;

            // Initially hide all discs
            discParent.SetActive(false);
        }
    }

    private string GetRotationRangeText(int jointIndex)
    {
        switch (jointIndex)
        {
            case 0: return "-165° / +165°"; // Example for Joint 1 (Z-axis)
            case 1: return "-110° / +110°";   // Example for Joint 2 (Y-axis)
            case 2: return "-110° / +70°";   // Example for Joint 3 (Y-axis)
            case 3: return "-160° / +160°"; // Example for Joint 4 (X-axis)
            case 4: return "-120° / +120°";   // Example for Joint 5 (Y-axis)
            case 5: return "-400° / +400°"; // Example for Joint 6 (Z-axis)
            default: return "N/A";
        }
    }


    private float CalculateDiscRadius(GameObject joint)
    {
        // Calculate radius based on joint's bounding box or scale
        Renderer renderer = joint.GetComponent<Renderer>();
        if (renderer != null)
        {
            Bounds bounds = renderer.bounds;
            return Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z) * 300f; // Scale factor for visual clarity
        }
        else
        {
            // Fallback if no renderer is available
            return discRadius; // Default radius
        }
    }

    private Quaternion GetDiscRotation(int jointIndex)
    {
        // Return the correct rotation for each joint's axis
        switch (jointIndex)
        {
            case 0: return Quaternion.Euler(0, 0, 0);   // Joint 1 (Z-axis)
            case 1: return Quaternion.Euler(90, 0, 0);  // Joint 2 (Y-axis)
            case 2: return Quaternion.Euler(90, 0, 0);  // Joint 3 (Y-axis)
            case 3: return Quaternion.Euler(0, 90, 0);  // Joint 4 (X-axis)
            case 4: return Quaternion.Euler(90, 0, 0);  // Joint 5 (Y-axis)
            case 5: return Quaternion.Euler(0, 90, 0);  // Joint 6 (Z-axis)
            default: return Quaternion.identity;       // Default fallback
        }
    }

    private Color GetDiscColor(int jointIndex)
    {
        // Muted pastel colors with 50% opacity
        switch (jointIndex)
        {
            case 0: return new Color(0.4f, 0.6f, 0.8f, 0.8f);   // Soft blue
            case 1: return new Color(0.5f, 0.7f, 0.5f, 0.8f);   // Sage green
            case 2: return new Color(0.8f, 0.8f, 0.4f, 0.8f);   // Mustard yellow
            case 3: return new Color(0.8f, 0.4f, 0.4f, 0.8f);   // Dusty red
            case 4: return new Color(0.7f, 0.4f, 0.7f, 0.8f);   // Muted purple
            case 5: return new Color(0.4f, 0.7f, 0.7f, 0.8f);   // Soft teal
            default: return new Color(0.9f, 0.9f, 0.9f, 0.8f);  // Off-white
        }
    }

    private void CreateRotationDisc(Transform parent, float radius, Color color, string rangeText)
    {
        // Create the disc object
        GameObject discObj = new GameObject("RotationDisc");
        discObj.transform.SetParent(parent, false);
        discObj.transform.localPosition = Vector3.zero;

        // Add LineRenderer for the disc
        LineRenderer lineRenderer = discObj.AddComponent<LineRenderer>();
        lineRenderer.useWorldSpace = false; // Use local space
        lineRenderer.material = discMaterial;
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
        lineRenderer.startWidth = 0.02f;
        lineRenderer.endWidth = 0.02f;

        int segments = 64; // Number of segments in the circle
        float angleStep = 360f / segments;

        lineRenderer.positionCount = segments + 1; // Include closing segment

        for (int i = 0; i <= segments; i++)
        {
            float angle = i * angleStep;
            float x = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;
            float y = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
            lineRenderer.SetPosition(i, new Vector3(x, y, 0)); // Circle lies in XY plane
        }

        // Add TextMeshPro for displaying range text
        GameObject textObj = new GameObject("RangeText");
        textObj.transform.SetParent(parent, false);
        
        // Position the text slightly above the center of the disc
        textObj.transform.localPosition = new Vector3(-radius + 0.2f, radius + 0.1f, -75f); 
        textObj.transform.localRotation = Quaternion.identity;

        // Configure TextMeshPro
        TextMeshPro textMeshPro = textObj.AddComponent<TextMeshPro>();
        textMeshPro.text = rangeText;
        textMeshPro.fontSize = 250;
        textMeshPro.alignment = TextAlignmentOptions.Left; // Left-align text
        textMeshPro.color = Color.yellow; // Set font color to black

        // Add the FaceCamera component to ensure the text faces the camera
        textObj.AddComponent<FaceCamera>();
    }


    public void ToggleJointDiscs(int jointIndex)
    {
        if (jointIndex >= 0 && jointIndex < jointDiscVisuals.Length && jointDiscVisuals[jointIndex] != null)
        {
            bool isActive = jointDiscVisuals[jointIndex].activeSelf;
            jointDiscVisuals[jointIndex].SetActive(!isActive);
        }
    }

}