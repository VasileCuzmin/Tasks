using NBB.Correlation;
using NBB.Messaging.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;
using Tasks.Api.Services;

namespace Tasks.Api.Decorators
{
    public class MessageBusPublisherDecorator : IMessageBusPublisher
    {
        private readonly IMessageBusPublisher _inner;
        private readonly IUserService _userService;

        public MessageBusPublisherDecorator(IMessageBusPublisher inner, IUserService userService)
        {
            _inner = inner;
            _userService = userService;
        }

        public Task PublishAsync<T>(T message, MessagingPublisherOptions options = null, CancellationToken cancellationToken = default)
        {
            options ??= MessagingPublisherOptions.Default;

             void NewCustomizer(MessagingEnvelope outgoingEnvelope)
            {
                var correlationId = CorrelationManager.GetCorrelationId();
                if (correlationId.HasValue)
                    outgoingEnvelope.SetHeader(NBB.Messaging.Abstractions.MessagingHeaders.CorrelationId, correlationId.ToString());

                outgoingEnvelope.SetHeader(MessagingHeaders.UserId, _userService.GetUserId() ?? string.Empty);
                outgoingEnvelope.SetHeader(MessagingHeaders.Language, _userService.GetLanguage() ?? string.Empty);

                options.EnvelopeCustomizer?.Invoke(outgoingEnvelope);
            }

            return _inner.PublishAsync(message, options with { EnvelopeCustomizer = NewCustomizer }, cancellationToken);
        }
    }

    public static class MessagingHeaders
    {
        /// <summary>
        /// Sub claim from Identity
        /// </summary>
        public const string UserId = "UserId";

        /// <summary>
        /// Selected language in browser
        /// </summary>
        public const string Language = "Language";
    }
}