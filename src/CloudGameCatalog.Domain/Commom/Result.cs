namespace CloudGameCatalog.Domain.Commom;

public class Result
{
    public IReadOnlyList<Error> Errors { get; } = [];

    protected Result(List<Error> errors)
    {
        Errors = errors;
    }

    public static Result Success() => new([]);

    public static Result Failure(List<Error> errors) => new(errors);

    public bool IsSuccess => Errors.Count == 0;
}

public sealed class Result<T> : Result where T : notnull
{
    public T? Data { get; } = default;

    private Result(T value) : base([])
    {
        Data = value;
    }

    private Result(List<Error> errors) : base(errors)
    { }

    public static Result<T> Success(T data) => new(data);

    public static new Result<T> Failure(List<Error> errors) => new(errors);
}
