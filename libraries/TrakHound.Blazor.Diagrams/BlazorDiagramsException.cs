using System;

namespace TrakHound.Blazor.Diagrams;

public class BlazorDiagramsException : Exception
{
    public BlazorDiagramsException(string? message) : base(message)
    {
    }
}