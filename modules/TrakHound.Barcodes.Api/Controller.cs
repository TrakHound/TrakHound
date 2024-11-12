// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using BarcodeStandard;
using SkiaSharp;
using SkiaSharp.QrCode;
using TrakHound.Api;

namespace TrakHound.Barcodes
{
    public class Controller : TrakHoundApiController
    {
        [TrakHoundApiQuery]
        public async Task<TrakHoundApiResponse> GenerateBarcode(
                    [FromQuery] string value,
                    [FromQuery] string type = "Code39",
                    [FromQuery] int width = 200,
                    [FromQuery] int height = 100,
                    [FromQuery] string imageFormat = "Png",
                    [FromQuery] string foreground = "#000",
                    [FromQuery] string background = "#00000000",
                    [FromQuery] int quality = 100
                    )
        {
            if (!string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(type))
            {
                try
                {
                    var imageType = imageFormat.ConvertEnum<SKEncodedImageFormat>();

                    if (type.ToLower() == "qr")
                    {
                        using var generator = new QRCodeGenerator();

                        // Generate QrCode
                        var qr = generator.CreateQrCode(value, ECCLevel.L, quietZoneSize: 0);

                        // Render to canvas
                        var info = new SKImageInfo(width, height);
                        using var surface = SKSurface.Create(info);

                        var canvas = surface.Canvas;
                        canvas.Render(qr, info.Width, info.Height, SKColor.Empty, SKColor.Parse(foreground), SKColor.Parse(background));

                        // Output to Stream -> File
                        using var image = surface.Snapshot();
                        using var data = image.Encode(imageFormat.ConvertEnum<SKEncodedImageFormat>(), quality);
                        var bytes = data.ToArray();
                        return Ok(bytes, $"image/{imageFormat.ToLower()}");

                        //using var stream = File.OpenWrite(@"output/hoge.png");
                        //data.SaveTo(stream);


                        //var qrCode = new QrCode(codeString, new Vector2Slim(width, height), imageType);

                        //using var memoryStream = new MemoryStream();
                        //qrCode.GenerateImage(memoryStream);
                        //var bytes = memoryStream.ToArray();

                        //return Ok(bytes, $"image/{imageFormat.ToLower()}");
                    }
                    else
                    {
                        var barcodeType = type.ConvertEnum<BarcodeStandard.Type>();

                        var barcode = new Barcode();
                        var img = barcode.Encode(barcodeType, value, width, height);

                        using var stream = img.Encode(imageType, quality).AsStream();
                        using var memoryStream = new MemoryStream();
                        await stream.CopyToAsync(memoryStream);
                        var bytes = memoryStream.ToArray();

                        return Ok(bytes, $"image/{imageFormat.ToLower()}");
                    }
                }
                catch (Exception ex)
                {
                    return InternalError(ex.Message);
                }
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
