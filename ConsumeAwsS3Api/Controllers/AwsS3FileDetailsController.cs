using ConsumeAwsS3Api.Models;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Serilog;
using SkiaSharp;
using System.Drawing;

namespace ConsumeAwsS3Api.Controllers;

public class AwsS3FileDetailsController : Controller
{
    Uri api = new Uri("http://localhost:5116/api");
    private readonly HttpClient _httpClient;

    private readonly IWebHostEnvironment _hostingEnvironment;
    public AwsS3FileDetailsController(IWebHostEnvironment hostingEnvironment)
    {
        _httpClient = new HttpClient();
        _httpClient.BaseAddress = api;
        _hostingEnvironment = hostingEnvironment;
    }

    [HttpGet]
    public IActionResult GetAwsS3AllFiles()
    {
        List<AwsS3FileDetailsViewModel> fileDetails = new List<AwsS3FileDetailsViewModel>();
     
            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "/AwsS3Api/GetAwsS3AllFiles").Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                fileDetails = JsonConvert.DeserializeObject<List<AwsS3FileDetailsViewModel>>(data);
            }
            else
            {
                throw new Exception();
            }
    
        return View(fileDetails);
    }

    [HttpGet]
    public IActionResult DownloadAwsS3AllFile(string key, long size, string fileName)
    {
        AwsS3FileDetailsViewModel result = new AwsS3FileDetailsViewModel();
        string consumeFilePath = _hostingEnvironment.WebRootPath + "\\" + "images" + "\\" + key;

        HttpResponseMessage response = _httpClient.GetAsync(string.Format(_httpClient.BaseAddress + "/AwsS3Api/DownloadAwsS3AllFile/?key={0}&consumeFilePath={1}", key, consumeFilePath)).Result;
        if (response.IsSuccessStatusCode)
            {
                result.Response = "File downloaded successfully.";
                result.Key = ".." + @"\images\" + key.Replace("/", @"\");
                result.FileName = fileName;
            }
            else
            {
                throw new Exception();
            } 
            return View(result);
    }

    [HttpPost]
    [DisableRequestSizeLimit]
    public async Task<IActionResult> SaveCroppedImage()
    {
        AwsS3FileDetailsViewModel CroppedFileDetailsinputs = new AwsS3FileDetailsViewModel();
        AwsS3FileDetailsViewModel result = new AwsS3FileDetailsViewModel();
        var file = Request.Form.Files.First();
        var bytes = file.OpenReadStream().ReadByte();
        string fileLocation = Request.Form["UploadLocation"].ToString();
            string Newfilename = Request.Form["FileInitials"].ToString();

            using var requestContent = new MultipartFormDataContent();
            using var filestream = file.OpenReadStream();
            requestContent.Add(new StreamContent(filestream),"file",Newfilename);
            
            HttpResponseMessage response= _httpClient.PostAsync(_httpClient.BaseAddress + "/AwsS3Api/SaveCroppedImage", requestContent).Result;

            if (response.IsSuccessStatusCode)
            {
                result.Response = "File saved successfully.";
                result.FileName = Newfilename;
    
            }
            else
            {
                throw new Exception();
            }

        return Json(result);


        //string fullpath = hoststr + fileLocation;
        ////string fullpath = hoststr + fileLocation + $@"\{Newfilename}";
        //Image image = Image.FromStream(file.OpenReadStream(), true, true);
        //image.Save(fullpath);


        // To Save IFormFile
        //using (FileStream fs = System.IO.File.Create(fullpath))
        //{
        //    file.CopyTo(fs);
        //    fs.Flush();
        //    fs.Close();
        //}


    }

    [HttpGet]
    public IActionResult ResizeAwsS3AllFile(string key, string fileName)
    {
        AwsS3FileDetailsViewModel result = new AwsS3FileDetailsViewModel();

        HttpResponseMessage response = _httpClient.GetAsync(string.Format(_httpClient.BaseAddress + "/AwsS3Api/ResizeAwsS3AllFile/?key={0}", key)).Result;
        if (response.IsSuccessStatusCode)
        {
            result.Response = "File resized successfully.";
            result.Key = ".." + @"\images\" + key.Replace("/", @"\");
            result.FileName = fileName;
        }
        else
        {
            throw new Exception();
        }
        return View(result);
    }

}