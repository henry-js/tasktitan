namespace henryjs.Nuke.Components;

public interface IHasSolution : INukeBuild
{
    [Required]
    [Solution(SuppressBuildProjectCheck = true)]
    Solution Solution => TryGetValue(() => Solution);

}
