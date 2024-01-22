using UnityEngine;
using UnityEngine.AdaptivePerformance;

public class APController : MonoBehaviour
{
#if UNITY_EDITOR
    public static APController instance;

    private IAdaptivePerformance apHolder = null;

    private void Awake()
    {
        if (instance != null)
            Destroy(instance);
        instance = this;

        Application.targetFrameRate = 60;

        apHolder = Holder.Instance;
        if (!apHolder.Active)
            return;

        var ctrlConfig = apHolder.DevicePerformanceControl;
        apHolder.DevicePerformanceControl.CpuLevel = ctrlConfig.MaxCpuPerformanceLevel;
        apHolder.DevicePerformanceControl.GpuLevel = ctrlConfig.MaxGpuPerformanceLevel;

        apHolder.DevicePerformanceControl.AutomaticPerformanceControl = true;
        apHolder.ThermalStatus.ThermalEvent += ThermalEvent;
    }

    private void ThermalEvent(ThermalMetrics tm)
    {
        switch (tm.WarningLevel)
        {
            case UnityEngine.AdaptivePerformance.WarningLevel.NoWarning:
                QualitySettings.lodBias = 1.0f;
                break;
            case UnityEngine.AdaptivePerformance.WarningLevel.ThrottlingImminent:
                if (tm.TemperatureLevel > 0.75f)
                    QualitySettings.lodBias = 0.75f;
                break;
            case UnityEngine.AdaptivePerformance.WarningLevel.Throttling:
                QualitySettings.lodBias = 0.5f;
                break;
        }
    }
#endif
}