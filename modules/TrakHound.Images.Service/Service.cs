// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using ImageMagick;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using TrakHound.Clients;
using TrakHound.Services;
using TrakHound.Volumes;

namespace TrakHound.Images.Service
{
    public class Service : TrakHoundService
    {
        private const string _basePath = "/.images/files";
        private const string _original = "original";
        private const string _messageQueue = "/.images/work-queue";

        private ITrakHoundConsumer<byte[]> _consumer;


        public Service(ITrakHoundServiceConfiguration configuration, ITrakHoundClient client, ITrakHoundVolume volume) : base(configuration, client, volume) { }


        protected override async Task OnStartAsync()
        {
            Log(TrakHoundLogLevel.Information, "TrakHound.Images.Service Service STARTED.");

            _consumer = await Client.Entities.SubscribeMessageQueueContent(_messageQueue);
            if (_consumer != null)
            {
                _consumer.Received += ConsumerReceived;
            }
            else
            {
                Log(TrakHoundLogLevel.Error, $"Error Getting MessageQueue Consumer : {_messageQueue}");
            }
        }

        protected override void OnStop()
        {
            if (_consumer != null) _consumer.Dispose();

            Log(TrakHoundLogLevel.Information, "TrakHound.Images.Service Service STOPPED.");
        }

        private async void ConsumerReceived(object sender, byte[] messageBytes)
        {
            Log(TrakHoundLogLevel.Debug, "Message Recieved");

            var imageId = StringFunctions.GetUtf8String(messageBytes);
            if (imageId != null)
            {
                Log(TrakHoundLogLevel.Information, $"Image Process Request : {imageId}");

                var basePath = TrakHoundPath.Combine(_basePath, imageId);
                var originalPath = TrakHoundPath.Combine(basePath, _original);
                var originalStream = await Client.Entities.GetBlobStream(originalPath);
                if (originalStream != null)
                {
                    // High Quality
                    var outputPath = TrakHoundPath.Combine(basePath, "high");
                    var outputStream = await ProcessImage(imageId, "high", originalStream, 500);
                    if (outputStream != null) await Client.Entities.PublishBlob(outputPath, outputStream, "image/jpeg", async: true);
                    Log(TrakHoundLogLevel.Debug, $"Image Processing : {imageId} : high");

                    // Low Quality
                    outputPath = TrakHoundPath.Combine(basePath, "low");
                    outputStream = await ProcessImage(imageId, "low", originalStream, 500, 15);
                    if (outputStream != null) await Client.Entities.PublishBlob(outputPath, outputStream, "image/jpeg", async: true);
                    Log(TrakHoundLogLevel.Debug, $"Image Processing : {imageId} : low");

                    // Thumbnail
                    outputPath = TrakHoundPath.Combine(basePath, "thumbnail");
                    outputStream = await ProcessImage(imageId, "thumbnail", originalStream, 50, 50, true);
                    if (outputStream != null) await Client.Entities.PublishBlob(outputPath, outputStream, "image/jpeg", async: true);
                    Log(TrakHoundLogLevel.Debug, $"Image Processing : {imageId} : thumbnail");
                }
            }
        }

        public async Task<Stream> ProcessImage(
           string imageId,
           string tag,
           Stream stream,
           uint size = 0,
           uint quality = 0,
           bool crop = false
           )
        {
            try
            {
                var stpw = new Stopwatch();
                stpw.Start();

                if (stream.Position > 0) stream.Seek(0, SeekOrigin.Begin);

                using var img = new MagickImage();
                await img.ReadAsync(stream);

                if (size > 0)
                {
                    var resize = new MagickGeometry(size);
                    resize.IgnoreAspectRatio = false;

                    img.Format = MagickFormat.Jpg;
                    img.Settings.Compression = CompressionMethod.JPEG;

                    if (img.Height > img.Width) img.Resize(size, size * 2);
                    else img.Resize(size * 2, size);

                    if (crop)
                    {
                        img.Crop(resize, Gravity.Center);
                    }
                }

                if (quality > 0)
                {
                    img.Quality = quality;
                }

                var outputStream = new MemoryStream();
                await img.WriteAsync(outputStream);
                if (outputStream.Position > 0) outputStream.Seek(0, SeekOrigin.Begin);

                stpw.Stop();
                Log(TrakHoundLogLevel.Debug, $"Image Processed : {imageId} : {tag} : {stpw.ElapsedMilliseconds}ms");

                return outputStream;
            }
            catch (Exception ex)
            {
                Log(TrakHoundLogLevel.Error, $"Image Processing Error : {imageId} : {tag} : {ex.Message}");
            }

            return null;
        }
    }
}
