using FluentValidation;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.GenerationParameters;

namespace SaxsSpot.NanoSystemGeneration.Engine.Validation;

public class SphereGenerationParametersValidator : AbstractValidator<SphereGenerationParameters>
{
    public SphereGenerationParametersValidator()
    {
        Include(new ParticleGenerationParametersValidator());
    }
}