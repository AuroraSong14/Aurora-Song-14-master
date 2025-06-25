using Content.Server.StationEvents.Events;

namespace Content.Server._AS.StationEvents.Events;

[RegisterComponent, Access(typeof(MeteorSwarmSystem))]
public sealed partial class ValidMeteorSwarmComponent : Component;
