using System.Runtime.Serialization;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace Spotify.Identity.Exeptions;

public class FailedCreatedException(ProblemDetails problemDetails) : Exception(problemDetails.Title)
{
    public ProblemDetails ProblemDetails { get; } = problemDetails;
}