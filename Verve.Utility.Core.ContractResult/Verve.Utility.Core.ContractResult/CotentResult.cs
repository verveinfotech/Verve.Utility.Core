using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace Verve.Utility.Core.ContractResult
{
    [Obsolete("Use EntitytResult")]
    public class ContentResult<T> : Result
    {
        public static ContentResult<T> Instance { get; } = new ContentResult<T>();

        /// <summary>
        /// 
        /// </summary>
        private ContentResult()
        {
            _content = default!;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="success"></param>
        /// <param name="reason"></param>
        /// <param name="userFriendlyMessage">A user friendly message to show end users about the result.</param>
        /// <param name="developerFriendlyMessage">A message that might be helpful for developers to find out about the result.</param>
        /// <param name="content"></param>
        [JetBrains.Annotations.UsedImplicitly]
        public ContentResult( bool success, ReasonCode reason, string? userFriendlyMessage, string? developerFriendlyMessage,
             T content = default! )
            : base( success, reason, userFriendlyMessage, developerFriendlyMessage )
        {
            if ( success && content == null && reason != ReasonCode.NoContent )
            {
                throw new ArgumentException(
                    $"Parameter {nameof( content )} must have a value when {nameof( success )} is 'true' and {nameof( reason )} is not '{nameof( ReasonCode.NoContent )}'." );
            }

            if ( success &&
                reason != ReasonCode.Accepted &&
                reason != ReasonCode.NoContent &&
                reason != ReasonCode.Success )
            {
                throw new ArgumentException(
                    $"Parameter '{nameof( success )}' is set to 'true' but the value of the '{reason}' is not one of the values: " +
                    $"'{nameof( ReasonCode.Accepted )}', '{nameof( ReasonCode.NoContent )}', '{nameof( ReasonCode.Success )}'." );
            }

            _content = content ?? CreateNew();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="success"></param>
        /// <param name="reason"></param>
        /// <param name="userFriendlyMessage">A user friendly message to show end users about the result.</param>
        /// <param name="developerFriendlyMessage">A message that might be helpful for developers to find out about the result.</param>
        /// <param name="entity"></param>
        [JetBrains.Annotations.UsedImplicitly]
        public ContentResult( bool success, ReasonCode reason, string? userFriendlyMessage, string? developerFriendlyMessage )
            : this( success, reason, userFriendlyMessage, developerFriendlyMessage, default! )
        {
        }

        public ContentResult( bool success, T content, ReasonCode reasonCode )
        {
            Succeeded = success;
                        _content = content;
            
            ReasonCode = reasonCode;
        }

        [JetBrains.Annotations.UsedImplicitly]
        [JetBrains.Annotations.NotNull]
        public static ContentResult<T> Success( T entity )
            => Success( entity, ReasonCode.Success );

        [JetBrains.Annotations.UsedImplicitly]
        [JetBrains.Annotations.NotNull]
        public static ContentResult<T> Success( T content, ReasonCode reasonCode )
            => new ContentResult<T>( true, content, reasonCode );

        public new static ContentResult<T> Failure( Exception exception )
            => Failure( exception.Message, exception.StackTrace ?? exception.Message, ReasonCode.InternalServerError );

        public new static ContentResult<T> Failure( string errorMessage )
            => Failure( errorMessage, ReasonCode.UnknownError, null );

        public new static ContentResult<T> Failure( string errorMessage, ReasonCode reasonCode )
            => Failure( errorMessage, reasonCode, null );

        public new static ContentResult<T> Failure( string errorMessage, ReasonCode reasonCode, Exception? exception )
            => Failure( errorMessage, errorMessage, reasonCode, exception );

        public new static ContentResult<T> Failure( string errorMessage, string detailErrorMessage, ReasonCode reasonCode,
            Exception? exception )
        {
            var result = Instance;
            result.Succeeded = false;
            result.DetailErrorMessage = detailErrorMessage;
            result.ErrorMessage = errorMessage;
            result.Exception = exception;
            result.ReasonCode = reasonCode;
            return result;
        }

        public new static ContentResult<T> Failure( string errorMessage, string detailError, ReasonCode reasonCode )
            => Failure( errorMessage, reasonCode, null );

        public static ContentResult<T> FailedFromOtherFailed( Result other )
        {
            return new ContentResult<T>( false, other.ReasonCode, other.ErrorMessage, other.DetailErrorMessage )
            {
                Exception = other.Exception
            };
        }
              
        private T _content;

        public T Content
        {
            [return: NotNull]
            get
            {
                if ( typeof( T ).IsValueType && _content == null )
                {
                    return Activator.CreateInstance<T>();
                }

                return _content ??= CreateNew();
            }
            protected set => _content = value;
        }

        [return: NotNull]
        private T CreateNew()
        {
            var @type = typeof(T);

            if ( @type.IsClass && !@type.IsAbstract && @type.GetConstructor( Type.EmptyTypes ) != null )
            {
                return ( T )@type.GetConstructor( Type.EmptyTypes )!.Invoke( Type.EmptyTypes );
            }

            return default!;
        }
    }
}

