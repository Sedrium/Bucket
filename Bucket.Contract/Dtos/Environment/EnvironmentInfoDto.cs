namespace Bucket.Contract.Dtos.Environment;

public sealed record EnvironmentInfoDto(
    string EnvironmentName,
    bool IsProduction,
    string ApiVersion,
    string? FrontendVersion);
