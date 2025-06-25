using Content.Server.GameTicking;
using Content.Server.Medical.SuitSensors;
using Content.Shared.Medical.SuitSensor;
using Content.Shared.Mind.Components;
using Content.Shared.Mobs;
using JetBrains.Annotations;
using Robust.Shared.Configuration;

namespace Content.Server._AS.AutoSensor;

internal sealed class ScheduledEntity(EntityUid uid, int when) : IComparable<ScheduledEntity>
{
    public EntityUid Uid { get; } = uid;
    public int When { get; } = when;

    public int CompareTo(ScheduledEntity? other)
    {
        if (other == null)
            return 1;
        if (other.Uid == Uid)
            return 0;
        var cmp = When.CompareTo(other.When);
        // we can never return 0 here because SortedSet.Contains uses SortedSet.FindNode which
        // considers a node as "contained" if its CompareTo equals 0 with any other node
        return cmp != 0 ? cmp : Uid.CompareTo(other.Uid);
    }

    public override bool Equals(object? obj)
    {
        return obj is ScheduledEntity sp && sp.Uid == Uid;
    }

    public override int GetHashCode()
    {
        return Uid.GetHashCode();
    }
}

[UsedImplicitly]
public sealed class AutoSensorSystem : EntitySystem
{
    [Dependency] private readonly IConfigurationManager _config = default!;
    [Dependency] private readonly GameTicker _gameTicker = default!;
    [Dependency] private readonly SuitSensorSystem _sensor = default!;

    private int _suitDelay = 6000;
    // TODO: same trick as StationPaySystem, this should probably be abstracted
    private readonly SortedSet<ScheduledEntity> _pendingSuits = [];

    public override void Initialize()
    {
        base.Initialize();

        Subs.CVar(_config, AuroraCVars.SuitSensorDeathActivationDelay, value => _suitDelay = value, true);

        SubscribeLocalEvent<MobStateChangedEvent>(OnMobState);
    }

    private void OnMobState(MobStateChangedEvent ev)
    {
        if (!TryComp<MindContainerComponent>(ev.Target, out _))
            return;

        if (ev.NewMobState == MobState.Dead)
        {
            var now = (int)_gameTicker.RoundDuration().TotalSeconds;
            _pendingSuits.Add(new ScheduledEntity(ev.Target, now + _suitDelay));
        }
        else
        {
            _pendingSuits.Remove(new ScheduledEntity(ev.Target, 0));
        }
    }

    public override void Update(float frameTime)
    {
        var now = (int)_gameTicker.RoundDuration().TotalSeconds;

        while (_pendingSuits.Count > 0)
        {
            var item = _pendingSuits.Min!;

            if (now < item.When)
                break;

            _pendingSuits.Remove(item);
            _sensor.SetAllSensors(item.Uid, SuitSensorMode.SensorCords);
        }
    }
}
