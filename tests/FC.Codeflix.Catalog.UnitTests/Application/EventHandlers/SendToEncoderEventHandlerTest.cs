using FC.Codeflix.Catalog.Application.EventHandlers;
using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Domain.Events;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace FC.Codeflix.Catalog.UnitTests.Application.EventHandlers;
public class SendToEncoderEventHandlerTest
{
    [Fact(DisplayName = nameof(HandleAsync))]
    [Trait("Application", "EventHandlers")]
    public async Task HandleAsync()
    {
        var messageProducerMock = new Mock<IMessageProducer>();
        messageProducerMock
            .Setup(x => x.SendMessageAsync(
                It.IsAny<VideoUploadedEvent>(),
                It.IsAny<CancellationToken>()
             ))
            .Returns(Task.CompletedTask);
        var handler = new SendToEncoderEventHandler(messageProducerMock.Object);
        VideoUploadedEvent @event = new(Guid.NewGuid(), "medias/video.mp4");

        await handler.HandleAsync(@event, CancellationToken.None);

        messageProducerMock
            .Verify(x => x.SendMessageAsync(@event, CancellationToken.None),
                Times.Once);
    }
}
