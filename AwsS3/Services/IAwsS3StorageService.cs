using AwsS3.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwsS3.Services;
public interface IAwsS3StorageService
{
    Task<List<AwsS3FileDetails>> GetAwsS3AllFilesAsync(AwsS3Object s3Obj, AwsCredentials awsCredentials);
    Task<AwsS3Response> DownloadAwsS3FileAsync(AwsS3Object AwsS3Obj, AwsCredentials awsCredentials, string key, string consumeFilePath, string apiFilePath);
    Task<AwsS3Response> SaveCroppedImage(IFormFile file, string filePath);
    Task<AwsS3Response> ResizeAwsS3AllFileAsync(AwsS3Object AwsS3Obj, AwsCredentials awsCredentials, string key, string apiFilePath);
}

