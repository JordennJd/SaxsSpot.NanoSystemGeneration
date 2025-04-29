using FluentValidation;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.GenerationParameters;

namespace SaxsSpot.NanoSystemGeneration.Engine.Validation;

public class ParticleGenerationParametersValidator : AbstractValidator<ParticleGenerationParameters>
{
    public ParticleGenerationParametersValidator()
    {
        RuleFor(parameters => parameters)
            .Must(parameters => parameters.GlobalSize is not null || parameters.NumericalConcentration is not null)
            .WithMessage("Global size or Numerical Concentration must be specified");
        
        RuleFor(parameters => parameters.NumericalConcentration)
            .GreaterThanOrEqualTo(0).When(parameters => parameters.NumericalConcentration is not null)
            .WithMessage("Numerical Concentration must be greater than or equal to 0");
        
        // RuleFor(parameters => parameters.NumericalConcentration)
        //     .LessThanOrEqualTo(0.4f).When(parameters => parameters.NumericalConcentration is not null)
        //     .WithMessage("Numerical Concentration must be less than 0.4");
        
        RuleFor(parameters => parameters.GlobalSize)
            .GreaterThanOrEqualTo(0).When(parameters => parameters.GlobalSize is not null)
            .WithMessage("Global size must be greater than or equal to 0");
        
        RuleFor(parameters => parameters.Count).GreaterThanOrEqualTo(0)
            .WithMessage("Count must be greater than or equal to 0");
        
        RuleFor(parameters => parameters.K).GreaterThanOrEqualTo(0)
            .WithMessage("K must be greater than or equal to 0");
        
        RuleFor(parameters => parameters.Theta).GreaterThanOrEqualTo(0)
            .WithMessage("K must be greater than or equal to 0");
        
        RuleFor(parameters => parameters)
            .Must(parameters => parameters.MinSize <= parameters.MaxSize)
            .WithMessage("Min size must be less than max size");
    }
}