using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Services.ModelResult;
using Services.Services.Interfaces;
using Services.Utiltities;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Services.Services
{
    public class AzureFileServices: IFileServices
    {
        private readonly IServiceConfig _serviceConfig;
        private readonly Lazy<BlobContainerClient> _containerClient;
        public AzureFileServices(IServiceConfig serviceConfig)
        {
            _serviceConfig = serviceConfig;
            _containerClient = new Lazy<BlobContainerClient>(() => new BlobContainerClient(_serviceConfig.AzureBlobConnectionString, _serviceConfig.AzureBlobContainer));
        }

        public async Task<bool> DeleteAsync(string blobFile)
        {
            if (string.IsNullOrEmpty(blobFile))
            {
                return true;
            }

            try
            {
                var blobFileUri = blobFile.Replace(_containerClient.Value.Uri.ToString() + "/", string.Empty);
                BlobClient blob = _containerClient.Value.GetBlobClient(blobFileUri);
                if (await blob.ExistsAsync())
                {
                    await blob.DeleteAsync();
                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DeleteAsync(ProductImage productImage)
        {
            var tasks = new List<Task<bool>>();
            tasks.Add(DeleteAsync(productImage.Url));
            tasks.Add(DeleteAsync(productImage.UrlPreview));
            var result = await Task.WhenAll(tasks);
            return result.All(_ => _);
        }

        public async Task<List<ImageUploadResult>> UploadAsync(Guid uniqueId, List<IFormFile> formFiles)
        {
            var result = new List<ImageUploadResult>();
            if(formFiles == null)
            {
                return result;
            }

            foreach (var formFile in formFiles)
            {
                var blobFileName = $"{uniqueId}_{formFile.FileName}";
                BlobClient blob = _containerClient.Value.GetBlobClient(blobFileName);
                var stream = new MemoryStream();
                formFile.CopyTo(stream);
                stream.Position = 0;
                await blob.UploadAsync(stream, true);
                var resultItem = new ImageUploadResult
                {
                    OriginalUri = blob.Uri.OriginalString
                };

                if (formFile.ContentType.Contains("image"))
                {
                    resultItem.SecondaryUri = await Upload2ndFileAsync(_containerClient.Value, formFile, $"2nd_{uniqueId}_{formFile.FileName}");
                }

                result.Add(resultItem);
            }

            return result;
        }

        private async Task<string> Upload2ndFileAsync(BlobContainerClient container, IFormFile formFile, string blobFileName)
        {
            BlobClient blob = container.GetBlobClient(blobFileName);
            using (var memoryStream = new MemoryStream())
            {
                await formFile.CopyToAsync(memoryStream);
                using (var image = Image.FromStream(memoryStream))
                {
                    var stream = ImageHelper.GetSmallImageBytes(image);
                    await blob.UploadAsync(stream, true);
                    return blob.Uri.OriginalString;
                }
            }
        }

        public async Task<ImageUploadResult> UploadAsync(string prefix, IFormFile formFile)
        {
            var blobFileName = $"{prefix}_{formFile.FileName}";
            BlobClient blob = _containerClient.Value.GetBlobClient(blobFileName);
            var stream = new MemoryStream();
            formFile.CopyTo(stream);
            stream.Position = 0;
            await blob.UploadAsync(stream, true);
            var resultItem = new ImageUploadResult
            {
                OriginalUri = blob.Uri.OriginalString
            };

            return resultItem;
        }
    }
}
