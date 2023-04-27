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
    public class LocallyFileServices: IFileServices
    {
        private readonly IServiceConfig _serviceConfig;
        public LocallyFileServices(IServiceConfig serviceConfig)
        {
            _serviceConfig = serviceConfig;
        }

        public async Task<bool> DeleteAsync(string blobFile)
        {
            if (string.IsNullOrEmpty(blobFile))
            {
                return true;
            }

            try
            {
                var path = $"{_serviceConfig.HostWebRootPath}/{blobFile}";
                if (File.Exists(path))
                {
                    File.Delete(path);
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
                if (formFile.Length > 0)
                {
                    var filePath = $"{_serviceConfig.HostWebRootPath}/{_serviceConfig.LmbFiles}/{uniqueId}_{Guid.NewGuid()}_{formFile.FileName}";
                    using (var stream = File.Create(filePath))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                    var resultItem = new ImageUploadResult
                    {
                        OriginalUri = filePath.Replace(_serviceConfig.HostWebRootPath, string.Empty)
                    };
                    if (formFile.ContentType.Contains("image"))
                    {
                        var secondImageUrl = await Upload2ndFileAsync(formFile, $"{_serviceConfig.HostWebRootPath}/{_serviceConfig.LmbFiles}/2nd/{uniqueId}_{Guid.NewGuid()}_{formFile.FileName}");
                        resultItem.SecondaryUri = secondImageUrl.Replace(_serviceConfig.HostWebRootPath, string.Empty);
                    }
                    result.Add(resultItem);
                }
            }

            return result;
        }

        private async Task<string> Upload2ndFileAsync(IFormFile formFile, string filePath)
        {
            using (var memoryStream = new MemoryStream())
            {
                await formFile.CopyToAsync(memoryStream);
                using (var image = Image.FromStream(memoryStream))
                {
                    using (var stream = File.OpenWrite(filePath))
                    {
                        var streamSmall = ImageHelper.GetSmallImageBytes(image);
                        await streamSmall.CopyToAsync(stream);
                        return filePath;
                    }
                }
            }
        }

        public async Task<ImageUploadResult> UploadAsync(string prefix, IFormFile formFile)
        {
            var filePath = $"{_serviceConfig.HostWebRootPath}/{_serviceConfig.LmbFiles}/{prefix}_{Guid.NewGuid()}_{formFile.FileName}";
            using (var stream = File.Create(filePath))
            {
                await formFile.CopyToAsync(stream);
            }
            var resultItem = new ImageUploadResult
            {
                OriginalUri = filePath.Replace(_serviceConfig.HostWebRootPath, string.Empty)
            };

            return resultItem;

        }
    }
}
