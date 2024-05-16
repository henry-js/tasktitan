using Nuke.Common.CI.GitHubActions;

[GitHubActions(
    "continuous",
    GitHubActionsImage.UbuntuLatest,
    OnPushBranchesIgnore = ["main"],
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
[GitHubActions(
    "release",
    GitHubActionsImage.UbuntuLatest,
    OnPushBranches = ["main"],
    InvokedTargets = [nameof(Publish)],
    AutoGenerate = true,
    FetchDepth = 0, OnPushTags = ["v*.*"]
)]
partial class Build
{

}
