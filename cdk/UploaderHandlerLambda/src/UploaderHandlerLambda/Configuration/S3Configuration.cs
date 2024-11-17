using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace UploaderHandlerLambda.Configuration;

[ExcludeFromCodeCoverage]
public record S3Configuration
{
    public const string SectionName = "S3Configuration";
    
    [Required]
    public required string NormalizedFramesBucketName { get; set; }
}