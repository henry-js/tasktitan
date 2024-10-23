// using Nuke.Common.CI.GitHubActions;

// [GitHubActions(
//     "continuous",
//     GitHubActionsImage.WindowsLatest,
//     OnPushBranchesIgnore = ["main"],
//     OnPullRequestExcludePaths = ["feat/*", "build", "build/*", "bug/*"],
//     InvokedTargets = [nameof(Compile)],
//     AutoGenerate = true,
//     FetchDepth = 0)]
// [GitHubActions(
//     "test",
//     GitHubActionsImage.WindowsLatest,
//     On = [GitHubActionsTrigger.PullRequest],
//     InvokedTargets = [nameof(Test)],
//     AutoGenerate = true,
//     FetchDepth = 0)]
// [GitHubActions(
//     "release",
//     GitHubActionsImage.WindowsLatest,
//     // OnPushBranches = ["main"],
//     InvokedTargets = [nameof(Publish)],
//     AutoGenerate = true,
//     FetchDepth = 0, OnPushTags = ["v*.*"]
// )]
// partial class Build
// {

// }
