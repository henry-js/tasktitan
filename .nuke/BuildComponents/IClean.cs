namespace henryjs.Nuke.BuildComponents;

public interface IClean : IHasSolution
{
    Target Clean => _ => _
        .Executes(() =>
        {
            Solution.CleanSolution(BuildProjectDirectory);
        });
}
