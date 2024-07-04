using System.Diagnostics.CodeAnalysis;

namespace WebApi;

[ExcludeFromCodeCoverage]
public class ApplicationSettings 
{
    public bool UseMigration { get; set; } = false;
}