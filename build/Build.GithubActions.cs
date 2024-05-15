using Nuke.Common.CI.GitHubActions;

[GitHubActions(
    "continuous",
    GitHubActionsImage.UbuntuLatest,
    On = [GitHubActionsTrigger.Push],
    InvokedTargets = [nameof(Compile)],
    AutoGenerate = true,
    FetchDepth = 0)]
partial class Build
{

}
