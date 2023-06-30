using FC.Codeflix.Catalog.Domain.Enum;
using FC.Codeflix.Catalog.EndToEndTests.Api.Video.Common;
using FC.Codeflix.Catalog.Infra.Messaging.DTOs;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FC.Codeflix.Catalog.EndToEndTests.Worker;

[Collection(nameof(VideoBaseFixture))]
public class VideoEncodedEventConsumerTest : IDisposable
{
    private readonly VideoBaseFixture _fixture;

    public VideoEncodedEventConsumerTest(VideoBaseFixture fixture)
        => _fixture = fixture;

    [Fact(DisplayName = nameof(EncodingSucceededEventReceived))]
    [Trait("End2End/Worker", "Video Encoded - Event Handler")]
    public async Task EncodingSucceededEventReceived()
    {
        var exampleVideos = _fixture.GetVideoCollection(5);
        await _fixture.VideoPersistence.InsertList(exampleVideos);
        var video = exampleVideos[2];
        var encodedFilePath = _fixture.GetValidMediaPath();
        var exampleEvent = new VideoEncodedMessageDTO
        {
            Video = new VideoEncodedMetadataDTO
            {
                EncodedVideoFolder = encodedFilePath,
                FilePath = video.Media!.FilePath,
                ResourceId = video.Id.ToString()
            }
        };

        _fixture.PublishMessageToRabbitMQ(exampleEvent);

        await Task.Delay(800);
        var videoFromDB = await _fixture.VideoPersistence.GetById(video.Id);
        videoFromDB.Should().NotBeNull();
        videoFromDB!.Media!.Status.Should().Be(MediaStatus.Completed);
        videoFromDB!.Media!.FilePath.Should().Be(encodedFilePath);
        (object? @event, uint count) = _fixture.ReadMessageFromRabbitMQ<object>();
        @event.Should().BeNull();
        count.Should().Be(0);   
    }

    [Fact(DisplayName = nameof(EncodingFailedEventReceived))]
    [Trait("End2End/Worker", "Video Encoded - Event Handler")]
    public async Task EncodingFailedEventReceived()
    {
        var exampleVideos = _fixture.GetVideoCollection(5);
        await _fixture.VideoPersistence.InsertList(exampleVideos);
        var video = exampleVideos[2];
        var encodedFilePath = _fixture.GetValidMediaPath();
        var exampleEvent = new VideoEncodedMessageDTO
        {
            Message = new VideoEncodedMetadataDTO
            {
                FilePath = video.Media!.FilePath,
                ResourceId = video.Id.ToString()
            },
            Error = "There was an error on processing the video."
        };

        _fixture.PublishMessageToRabbitMQ(exampleEvent);

        await Task.Delay(800);
        var videoFromDB = await _fixture.VideoPersistence.GetById(video.Id);
        videoFromDB.Should().NotBeNull();
        videoFromDB!.Media!.Status.Should().Be(MediaStatus.Error);
        videoFromDB!.Media!.FilePath.Should().BeNull();
        (object? @event, uint count) = _fixture.ReadMessageFromRabbitMQ<object>();
        @event.Should().BeNull();
        count.Should().Be(0);
    }

    [Fact(DisplayName = nameof(InvalidMessageEventReceived))]
    [Trait("End2End/Worker", "Video Encoded - Event Handler")]
    public async Task InvalidMessageEventReceived()
    {
        var exampleVideos = _fixture.GetVideoCollection(5);
        await _fixture.VideoPersistence.InsertList(exampleVideos);
        var encodedFilePath = _fixture.GetValidMediaPath();
        var exampleEvent = new VideoEncodedMessageDTO
        {
            Message = new VideoEncodedMetadataDTO
            {
                FilePath = _fixture.GetValidMediaPath(),
                ResourceId = Guid.NewGuid().ToString()
            },
            Error = "There was an error on processing the video."
        };

        _fixture.PublishMessageToRabbitMQ(exampleEvent);

        await Task.Delay(800);
        (object? @event, uint count) = _fixture.ReadMessageFromRabbitMQ<object>();
        @event.Should().BeNull();
        count.Should().Be(0);
    }

    public void Dispose()
    {
        _fixture.CleanPersistence();
        _fixture.PurgeRabbitMQQueues();
    }
}
