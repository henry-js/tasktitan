using henryjs.Nuke.Extensions;

namespace henryjs.Nuke.Components;

public interface IClean : IHasSolution
{
    Target Clean => _ => _
        .Executes(() =>
        {
            Solution.CleanSolution(BuildProjectDirectory);
        });
}
