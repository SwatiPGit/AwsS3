using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwsS3.Models;
public class AwsS3Object
{
    public string BucketName { get; set; } = null;
    public string Prefix { get; set; } = null;
}

