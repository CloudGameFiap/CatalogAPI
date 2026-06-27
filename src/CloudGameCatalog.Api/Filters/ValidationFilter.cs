using FluentValidation;

namespace CloudGameCatalog.Api.Filters
{
    public class ValidationFilter<T>(IValidator<T>? validator = null) : IEndpointFilter where T : class
    {
        private readonly IValidator<T>? _validator = validator;

        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            if(_validator is null)
            {
                return await next(context);
            }

            if (context.Arguments.FirstOrDefault(t => t is T) is not T argument)
            {
                return Results.BadRequest("The expected request body was missing or invalid.");
            }

            var validationResult = await _validator.ValidateAsync(argument);

            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            return await next(context);
        }
    }
}
