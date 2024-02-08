using Amazon.S3;
using AwsS3.Models;
using AwsS3.Services;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using ThirdParty.Json.LitJson;

namespace AwsS3.Api.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class AwsS3ApiController : ControllerBase
{
    private readonly IAwsS3StorageService _storageService;
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _hostingEnvironment;

    public AwsS3ApiController(IAwsS3StorageService storageService, IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
    {
        _storageService = storageService;
        _configuration = configuration;
        _hostingEnvironment = hostingEnvironment;
    }

    private (AwsS3Object awsS3Obj, AwsCredentials awsCred) GetAwsDetails()
    {
        var s3Obj = new AwsS3Object()
        {
            BucketName = "abesto-images",
            Prefix = "profiles"
        };
        var cred = new AwsCredentials()
        {
            AwsKey = _configuration["AwsConfiguration:AWSAccessKey"],
            AwsSecretKey = _configuration["AwsConfiguration:AWSSecretKey"]
        };
        return (s3Obj,cred);
    }

    [HttpGet(Name = "GetAwsS3AllFiles")]
    public async Task<IActionResult> GetAwsS3AllFiles()
    {
        var result = new List<AwsS3FileDetails>();

       
        var awsDetails = GetAwsDetails();
       

        result = await _storageService.GetAwsS3AllFilesAsync(awsDetails.awsS3Obj,awsDetails.awsCred);
        
        Log.Information("Controller: AwsS3ApiController, Method: GetAwsS3AllFiles, Message: Amazon Aws S3 files fetched");
      
        return Ok(result);
    }

    [HttpGet(Name = "DownloadAwsS3AllFile")]
    public async Task<IActionResult> DownloadAwsS3AllFile(string key,string consumeFilePath)
    {
        var result = new AwsS3Response();

        string apiFilePath = _hostingEnvironment.WebRootPath + "\\" + "Cimages" + "\\" + key;

        var awsDetails = GetAwsDetails();


        result = await _storageService.DownloadAwsS3FileAsync(awsDetails.awsS3Obj, awsDetails.awsCred, key, consumeFilePath, apiFilePath);

        Log.Information("Controller: AwsS3ApiController, Method: DownloadAwsS3AllFile, Message: {result}", result);
  
        return Ok(result);
    }

    [HttpPost(Name = "SaveCroppedImage")]
    public async Task<IActionResult> SaveCroppedImage([FromForm] IFormFile file)
    {
        AwsS3Response result = new AwsS3Response();

        string filePath = _hostingEnvironment.WebRootPath + "\\" + "Cimages\\profiles";
        result = await _storageService.SaveCroppedImage(file,filePath);

        Log.Information("Controller: AwsS3ApiController, Method: SaveCroppedImage, Message: {result}", result);
        return Ok(result);
    }

    [HttpGet(Name = "ResizeAwsS3AllFile")]
    public async Task<IActionResult> ResizeAwsS3AllFile(string key)
    {
        var result = new AwsS3Response();

        string apiFilePath = _hostingEnvironment.WebRootPath + "\\" + "Rimages";

        var awsDetails = GetAwsDetails();


        result = await _storageService.ResizeAwsS3AllFileAsync(awsDetails.awsS3Obj, awsDetails.awsCred, key, apiFilePath);

        Log.Information("Controller: AwsS3ApiController, Method: ResizeAwsS3AllFileAsync, Message: {result}", result);

        return Ok(result);
    }
}