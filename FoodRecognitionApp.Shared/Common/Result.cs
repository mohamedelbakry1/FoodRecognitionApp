using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Shared.Common
{
    public class Result
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public IReadOnlyList<Error> Errors { get; } = default!;

        protected Result(bool isSuccess, IReadOnlyList<Error> errors)
        {
            IsSuccess = isSuccess;
            Errors = errors;
        }

        public static Result Ok() => new(true, Array.Empty<Error>());
        public static Result Fail(Error error) => new(false, new[] { error });
        public static Result Fail(IReadOnlyList<Error> errors) => new(false, errors);

        public static implicit operator Result(Error error) => Fail(error);
        public static implicit operator Result(List<Error> errors) => Fail(errors);
    }

    public class Result<TValue> : Result
    {
        private readonly TValue _value;
        public TValue Value => IsSuccess ? _value : throw new InvalidOperationException("Cannot access Value of a failed result.");

        private Result(TValue value) : base(true, Array.Empty<Error>())
        {
            _value = value;
        }

        private Result(Error error) : base(false, new[] { error })
        {
            _value = default!;
        }

        private Result(IReadOnlyList<Error> errors) : base(false, errors)
        {
            _value = default!;
        }


        public static Result<TValue> Ok(TValue value) => new Result<TValue>(value);
        public new static Result<TValue> Fail(Error error) => new Result<TValue>(error);
        public new static Result<TValue> Fail(IReadOnlyList<Error> errors) => new Result<TValue>(errors);

        public static implicit operator Result<TValue>(TValue value) => Ok(value);
        public static implicit operator Result<TValue>(Error error) => Fail(error);
        public static implicit operator Result<TValue>(List<Error> errors) => Fail(errors);
    }
}
