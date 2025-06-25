using Robust.Shared.Configuration;

namespace Content.Server._AS;

[CVarDefs]
public sealed class AuroraCVars
{
    /// <summary>
    /// How often station staff wages are paid.
    /// </summary>
    public static readonly CVarDef<int> StationPayDelay =
        CVarDef.Create(
            "station_pay.delay",
            3600,
            CVar.SERVERONLY,
            "how often station staff wages are paid"
        );

    /// <summary>
    /// How long until suit sensors for dead players are automatically toggled on, following their death.
    /// </summary>
    public static readonly CVarDef<int> SuitSensorDeathActivationDelay =
        CVarDef.Create(
            "suit_sensors.death_activation_delay",
            600,
            CVar.SERVERONLY,
            "how long before dead player's suit sensors are toggled, in seconds"
        );
}
