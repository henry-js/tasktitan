namespace TaskTitan.Lib.Services;

public record TTaskResult(bool Success, TTask? task, string? Messaqge = null);
