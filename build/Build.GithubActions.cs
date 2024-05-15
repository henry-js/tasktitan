using Nuke.Common.CI.GitHubActions;

[GitHubActions(
    "continuous",
    GitHubActionsImage.UbuntuLatest,
    On = [GitHubActionsTrigger.Push],
    InvokedTargets = [nameof(Compile)],
    AutoGenerate = true,
    FetchDepth = 0)]
[GitHubActions(
    "test",
    GitHubActionsImage.UbuntuLatest,
    On = [GitHubActionsTrigger.PullRequest],
    InvokedTargets = [nameof(Test)],
    AutoGenerate = true,
    FetchDepth = 0)]
partial class Build
{

}
