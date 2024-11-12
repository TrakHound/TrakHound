// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Drivers.FileSystem
{
    public class Blobs : TrakHoundDriver,
        IBlobReadDriver,
        IBlobPublishDriver,
        IBlobDeleteDriver
    {
        private const string TypeId = "FileSystem";
        private const string DefaultDirectory = "files";

        private readonly ITrakHoundDriverConfiguration _configuration;
        private readonly string _directory;


        public override bool IsAvailable => true;


        public Blobs(ITrakHoundDriverConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;

            if (configuration != null)
            {
                var path = configuration.GetParameter("directory");
                if (string.IsNullOrEmpty(path)) path = DefaultDirectory;

                if (!string.IsNullOrEmpty(path))
                {
                    if (!Path.IsPathRooted(path)) path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
                }

                _directory = Path.Combine(path, configuration.Id);


                if (!Directory.Exists(_directory))
                {
                    try
                    {
                        Directory.CreateDirectory(_directory);
                    }
                    catch { }
                }
            }
        }


        public async Task<TrakHoundResponse<Stream>> Read(string blobId)
        {
            var stpw = System.Diagnostics.Stopwatch.StartNew();
            var results = new List<TrakHoundResult<Stream>>();

            if (!string.IsNullOrEmpty(blobId))
            {
                try
                {
                    var filePath = Path.Combine(_directory, blobId);
                    if (File.Exists(filePath))
                    {
                        var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

                        results.Add(new TrakHoundResult<Stream>(_configuration.Id, blobId, TrakHoundResultType.Ok, stream));
                    }
                    else
                    {
                        results.Add(new TrakHoundResult<Stream>(_configuration.Id, blobId, TrakHoundResultType.NotFound));
                    }
                }
                catch (Exception ex)
                {
                    results.Add(new TrakHoundResult<Stream>(_configuration.Id, blobId, TrakHoundResultType.InternalError));
                }
            }
            else
            {
                results.Add(new TrakHoundResult<Stream>(_configuration.Id, blobId, TrakHoundResultType.BadRequest));
            }

            stpw.Stop();

            return new TrakHoundResponse<Stream>(results, stpw.ElapsedTicks);
        }

        public async Task<TrakHoundResponse<bool>> Publish(string blobId, Stream content)
        {
            var stpw = System.Diagnostics.Stopwatch.StartNew();
            var results = new List<TrakHoundResult<bool>>();

            if (!string.IsNullOrEmpty(blobId) && content != null)
            {
                try
                {
                    if (!Directory.Exists(_directory)) Directory.CreateDirectory(_directory);

                    var filePath = Path.Combine(_directory, blobId);
                    if (File.Exists(filePath)) File.Delete(filePath);

                    using (var fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        if (content.CanSeek) content.Seek(0, SeekOrigin.Begin);
                        await content.CopyToAsync(fileStream);
                        await fileStream.FlushAsync();
                    }

                    results.Add(new TrakHoundResult<bool>(_configuration.Id, blobId, TrakHoundResultType.Ok, true));
                }
                catch (Exception ex)
                {
                    results.Add(new TrakHoundResult<bool>(_configuration.Id, null, TrakHoundResultType.InternalError));
                }
            }
            else
            {
                results.Add(new TrakHoundResult<bool>(_configuration.Id, null, TrakHoundResultType.BadRequest));
            }

            stpw.Stop();

            return new TrakHoundResponse<bool>(results, stpw.ElapsedTicks);
        }

        public async Task<TrakHoundResponse<bool>> Delete(string blobId)
        {
            var stpw = System.Diagnostics.Stopwatch.StartNew();
            var results = new List<TrakHoundResult<bool>>();

            if (!string.IsNullOrEmpty(blobId))
            {
                try
                {
                    if (!Directory.Exists(_directory)) Directory.CreateDirectory(_directory);

                    var filePath = Path.Combine(_directory, blobId);
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);

                        results.Add(new TrakHoundResult<bool>(_configuration.Id, blobId, TrakHoundResultType.Ok, true));
                    }
                    else
                    {
                        results.Add(new TrakHoundResult<bool>(_configuration.Id, null, TrakHoundResultType.InternalError));
                    }
                }
                catch (Exception ex)
                {
                    results.Add(new TrakHoundResult<bool>(_configuration.Id, null, TrakHoundResultType.InternalError));
                }
            }
            else
            {
                results.Add(new TrakHoundResult<bool>(_configuration.Id, null, TrakHoundResultType.BadRequest));
            }

            stpw.Stop();

            return new TrakHoundResponse<bool>(results, stpw.ElapsedTicks);
        }
    }
}
