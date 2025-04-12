using FluentValidation;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.GenerationParameters;

namespace SaxsSpot.NanoSystemGeneration.Engine.Validation;

public class ParallelepipedGenerationParametersValidator : AbstractValidator<ParallelepipedGenerationParameters>
{
    public ParallelepipedGenerationParametersValidator()
    {
        Include(new ParticleGenerationParametersValidator());

        RuleFor(parameters => parameters.Epsilon).GreaterThanOrEqualTo(1)
            .WithMessage("Epsilon should be greater than or equal to 1.");
    }
}