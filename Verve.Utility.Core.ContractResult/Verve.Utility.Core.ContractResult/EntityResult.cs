﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

using JetBrains.Annotations;

using Microsoft.Extensions.Logging;

namespace Verve.Utility.Core.ContractResult
{
    public class Result<TEntity> : Result
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="success"></param>
        /// <param name="reason"></param>
        /// <param name="userFriendlyMessage">A user friendly message to show end users about the result.</param>
        /// <param name="developerFriendlyMessage">A message that might be helpful for developers to find out about the result.</param>
        /// <param name="entity"></param>
        [UsedImplicitly]
        public Result(bool success, ReasonCode reason, string? userFriendlyMessage, string? developerFriendlyMessage, [CanBeNull][AllowNull] TEntity entity)
            : base(success, reason, userFriendlyMessage, developerFriendlyMessage)
        {
            if (success && entity == null && reason != ReasonCode.NoContent)
            {
                throw new ArgumentException($"Parameter {nameof(entity)} must have a value when {nameof(success)} is 'true' and {nameof(reason)} is not '{nameof(ReasonCode.NoContent)}'.");
            }

            if (success &&
                reason != ReasonCode.Accepted &&
                reason != ReasonCode.NoContent &&
                reason != ReasonCode.Success)
            {
                throw new ArgumentException(
                    $"Parameter '{nameof(success)}' is set to 'true' but the value of the '{reason}' is not one of the values: " +
                    $"'{nameof(ReasonCode.Accepted)}', '{nameof(ReasonCode.NoContent)}', '{nameof(ReasonCode.Success)}'.");
            }

            _entity = entity!;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="success"></param>
        /// <param name="reason"></param>
        /// <param name="userFriendlyMessage">A user friendly message to show end users about the result.</param>
        /// <param name="developerFriendlyMessage">A message that might be helpful for developers to find out about the result.</param>
        /// <param name="entity"></param>
        [UsedImplicitly]
        public Result(bool success, ReasonCode reason, string? userFriendlyMessage, string? developerFriendlyMessage)
            : this(success, reason, userFriendlyMessage, developerFriendlyMessage, default!)
        {
        }

        public Result(bool success,  TEntity entity, ReasonCode reasonCode)
        {
            Succeeded = success;
            _entity = entity;
            ReasonCode = reasonCode;
        }


        [UsedImplicitly]
        [JetBrains.Annotations.NotNull]
        public static Result<TEntity> Success(TEntity entity)
            => Success(entity, ReasonCode.Success);

        [UsedImplicitly]
        [JetBrains.Annotations.NotNull]
        public static Result<TEntity> Success(TEntity entity, ReasonCode reasonCode)
            => new Result<TEntity>(true, entity, reasonCode);

        public new static Result<TEntity> Failure(Exception exception)
            => Failure(exception.Message, exception.StackTrace ?? exception.Message, ReasonCode.InternalServerError);

        public new static Result<TEntity> Failure(string errorMessage)
            => Failure(errorMessage, ReasonCode.UnknownError, null);

        public new static Result<TEntity> Failure(string errorMessage, ReasonCode reasonCode)
            => Failure(errorMessage, reasonCode, null);

        public new static Result<TEntity> Failure(string errorMessage, ReasonCode reasonCode, Exception? exception)
            => Failure(errorMessage, errorMessage, reasonCode, exception);

        public new static Result<TEntity> Failure(string errorMessage, string detailErrorMessage, ReasonCode reasonCode, Exception? exception)
            => new Result<TEntity>(false, reasonCode, errorMessage, detailErrorMessage);


        public new static Result<TEntity> Failure(string errorMessage, string detailError, ReasonCode reasonCode)
            => Failure(errorMessage, reasonCode, null);

        public static Result<TEntity> NoContent()
            => new Result<TEntity>(true, ReasonCode.NoContent, null, null);


        [UsedImplicitly]
        public static Result<TEntity> From([AllowNull] Result? other)
        {
            if (other == null)
            {
                return new Result<TEntity>(false, ReasonCode.UnknownError, "An error occurred.", "Other result was null.");
            }

            var otherContent = default(TEntity)!;
            var typeOfOther = other.GetType();

            if (other.Succeeded)
            {
                if (typeOfOther.IsGenericType && (typeOfOther.GenericTypeArguments[0].IsSubclassOf(typeof(TEntity))
                                                    || typeOfOther.GenericTypeArguments[0] == typeof(TEntity)))
                {
                    var property = typeOfOther.GetProperty(nameof(Entity));
                    otherContent = (TEntity)property?.GetValue(other)!;
                }
                else
                {
                    throw new ArgumentException($"Type of '{nameof(other)}.{nameof(Entity)}' is not assignable to '{typeof(TEntity)}'.");
                }
            }

            return new Result<TEntity>(other.Succeeded, other.ReasonCode, other.ErrorMessage, other.DetailErrorMessage, otherContent);
        }

        [UsedImplicitly]
        public static Result<TEntity> FailedFromOtherFailed(Result other)
        {
            if (other == null)
            {
                return new Result<TEntity>(false, ReasonCode.UnknownError, "An error occurred.", "Other result was null.");
            }
            return new Result<TEntity>(other.Succeeded, other.ReasonCode, other.ErrorMessage, other.DetailErrorMessage, default!);
        }

        [UsedImplicitly]
        public static async Task<Result<TEntity>> CheckResultAndExecuteNextAsync(Result other, Func<Task<Result<TEntity>>> next)
        {
            if (other.Failed)
            {
                return FailedFromOtherFailed(other);
            }

            return await next.Invoke();
        }

        public static Result<TEntity> CheckOtherEntityResultAndConvertToResult<TOtherEntity>(Result<TOtherEntity> otherEntityResult, Func<TOtherEntity, TEntity> converterFunc)
        {
            if (otherEntityResult.Failed)
            {
                return Result<TEntity>.FailedFromOtherFailed(otherEntityResult);
            }

            return Result<TEntity>.Success(converterFunc.Invoke(otherEntityResult.Entity));
        }

        public static async Task<Result<TEntity>> CheckOtherEntityResultAndConvertToResultAsync<TOtherEntity>(Result<TOtherEntity> otherEntityResult, Func<TOtherEntity, Task<TEntity>> converterFunc)
        {
            if (otherEntityResult.Failed)
            {
                return Result<TEntity>.FailedFromOtherFailed(otherEntityResult);
            }

            return Result<TEntity>.Success(await converterFunc.Invoke(otherEntityResult.Entity));
        }

        [UsedImplicitly]
        public static async Task<Result<TEntity>> CheckResultAndExecuteNextAsync(Result other, ILogger logger, Func<Task<Result<TEntity>>> next)
        {
            if (other.Failed)
            {
                logger.LogWarning(other.Exception, "Result failed, '{ErrorMessage}', '{DetailsError}', '{ReasonCode}'", other.ErrorMessage, other.DetailErrorMessage, other.ReasonCode);
                return FailedFromOtherFailed(other);
            }

            return await next.Invoke();
        }

        private TEntity _entity;

        /// <summary>
        /// Gets or sets content of the result.
        /// </summary>
        public TEntity Entity
        {
            get
            {
                if (typeof(TEntity).IsValueType)
                {
                    return _entity;
                }

                return _entity ?? TryCreate()!;
            }


            protected set => _entity = value;
        }

        private TEntity TryCreate(TEntity entity = default!)
        {
            var type = typeof(TEntity);

            return @type.IsClass && !@type.IsAbstract && @type.GetConstructor(Type.EmptyTypes) != null
                ? (TEntity)@type.GetConstructor(Type.EmptyTypes)!.Invoke(Type.EmptyTypes)
                : entity;
        }
    }
}