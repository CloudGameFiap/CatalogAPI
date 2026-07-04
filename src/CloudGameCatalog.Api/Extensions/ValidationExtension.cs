using CloudGameCatalog.Api.Filters;

namespace CloudGameCatalog.Api.Extensions
{
    public static class ValidationExtension
    {
        public static RouteHandlerBuilder WithValidation<T>(this RouteHandlerBuilder builder) where T : class
        {
            return builder.AddEndpointFilter<ValidationFilter<T>>()
                .ProducesValidationProblem();
        }
    }
}
