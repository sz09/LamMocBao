using Microsoft.AspNetCore.Http;
using Services.ModelResult;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Services.Interfaces
{
    public interface IFileServices
    {
        Task<List<ImageUploadResult>> UploadAsync(Guid uniqueId, List<IFormFile> formFiles);
        Task<ImageUploadResult> UploadAsync(string prefix, IFormFile formFile);
        Task<bool> DeleteAsync(string blobFile);
        Task<bool> DeleteAsync(ProductImage productImage);
    }

}
