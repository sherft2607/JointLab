#if ENABLE_VR || UNITY_GAMECORE || PACKAGE_DOCS_GENERATION
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.XR;
using UnityEngine.Scripting;

namespace UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation
{
    /// <summary>
    /// An input device representing a simulated XR head mounted display.
    /// </summary>
    [InputControlLayout(stateType = typeof(XRSimulatedHMDState), isGenericTypeOfDevice = false, displayName = "XR Simulated HMD", updateBeforeRender = true)]
    [Preserve]
    public class XRSimulatedHMD : XRHMD
    {
        /// <inheritdoc />
        protected override unsafe long ExecuteCommand(InputDeviceCommand* commandPtr)
        {
            return XRSimulatorUtility.TryExecuteCommand(commandPtr, out var result)
                ? result
                : base.ExecuteCommand(commandPtr);
        }
    }
}
#endif
