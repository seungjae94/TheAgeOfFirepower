using Mathlife.ProjectL.Gameplay;
using UnityEditor;
using UnityEditor.DeviceSimulation;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Mathlife.ProjectL.Editor
{
    public class DeviceSimulatorHelper : DeviceSimulatorPlugin
    {
        private const string ToggleMenuName = "Utils/Display Auto Adaptation";
        private static bool _shouldAdapt = true;

        [MenuItem(ToggleMenuName)]
        internal static void ToggleAdapt()
        {
            _shouldAdapt = !_shouldAdapt;

            Menu.SetChecked(ToggleMenuName, _shouldAdapt);
        }

        public override string title => "Display Adapter";

        public override void OnCreate()
        {
            Menu.SetChecked(ToggleMenuName, _shouldAdapt);
            deviceSimulator.deviceChanged += OnDeviceChanged;
        }

        public override void OnDestroy()
        {
            deviceSimulator.deviceChanged -= OnDeviceChanged;
        }

        void OnDeviceChanged()
        {
            // 해상도 계산
            Resolution screenResolution = Screen.currentResolution;
            float screenAspectRatio = (float)screenResolution.width / (float)screenResolution.height;

            string deviceInfo = $"[DeviceSimulatorHelper]\n" +
                                $"해상도: {screenResolution.width}x{screenResolution.height}, 가로세로비: {screenAspectRatio}\n" +
                                $"안전 영역: {Screen.safeArea.width}x{Screen.safeArea.height}";
            Debug.Log(deviceInfo);

            if (_shouldAdapt == false)
                return;

            CameraViewportAdapter adapter = GameObject.Find("Display Adapter").GetComponent<CameraViewportAdapter>();
            adapter.AdaptDisplay(screenResolution.width, screenResolution.height);

            Debug.Log("[DeviceSimulatorHelper] Display adapted to the device screen.");
        }
    }
}