namespace Content.Server.Touched;

[RegisterComponent]
public sealed partial class TouchedComponent : Component
{
    [DataField("touched")]
    public bool Touched { get; set; } = default!;
}
