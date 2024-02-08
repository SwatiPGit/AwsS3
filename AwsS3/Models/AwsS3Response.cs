using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwsS3.Models;
public class AwsS3Response
{
    public int StatusCode { get; set; } = 200;
    public string Message { get; set; } = "";
}

