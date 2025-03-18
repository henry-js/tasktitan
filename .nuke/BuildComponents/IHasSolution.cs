namespace henryjs.Nuke.BuildComponents;

public interface IHasSolution : INukeBuild
{
    [Required]
    [Solution(SuppressBuildProjectCheck = true)]
    Solution Solution => TryGetValue(() => Solution);

}
