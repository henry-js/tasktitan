namespace henryjs.Nuke.Components;

public interface IHasMinVer : IHasMainProject
{
    [MinVer]
    MinVer MinVer => TryGetValue(() => MinVer);
    Func<MinVerSettings, MinVerSettings> CustomMinVerSettings => null;
}
