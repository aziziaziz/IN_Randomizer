using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Model;

namespace inbackend.Controllers 
{
  [ApiController]
  [Route("apiv1/[action]")]
  public class RandomizerController : ControllerBase
  {
    Characters characters = new Characters();

    [HttpGet]
    public object GetRandom(string distribution)
    {
      string[] distList = null;
      int random = 0;
      if (!String.IsNullOrEmpty(distribution))
      {
        distList = distribution.Split(',');
        random = int.Parse(distList[new Random().Next(0, distList.Count())]);
      }
      else
        random = new Random().Next(3);

      switch (random)
      {
          case 0:
            return new ReportItem { type = "Alphanumeric", value = characters.GetAlphaNumeric };
          case 1:
            return new ReportItem { type = "Numeric", value = characters.GetNumeric };
          case 2:
            return new ReportItem { type = "Float", value = characters.GetFloat };
          default:
            return new ReportItem { type = "Alphanumeric", value = characters.GetAlphaNumeric };
      }
    }

    [HttpPost]
    public async Task<bool> GenerateReport()
    {
      try
      {
        var body = await new StreamReader(Request.Body, Encoding.UTF8).ReadToEndAsync();
    
        var dirPath = Path.Combine(Directory.GetCurrentDirectory(), "Report");
        var filePath = Path.Combine(dirPath, $"{DateTime.Now.ToString("ddMMyyyyHHmm")}.txt");

        if (!Directory.Exists(dirPath))
          Directory.CreateDirectory(dirPath);

        await System.IO.File.WriteAllTextAsync(filePath, body);

        return true;
      }
      catch (System.Exception)
      {
        return false;
      }
    }

    [HttpGet]
    public async Task<FullReport> GetReport()
    {
      var dirPath = Path.Combine(Directory.GetCurrentDirectory(), "Report");

      if (!Directory.Exists(dirPath))
        return null;
        
      var latestFile = new DirectoryInfo(dirPath).GetFiles().OrderByDescending(f => f.LastWriteTime).First();
      
      if (!System.IO.File.Exists(latestFile.FullName))
        return null;

      var rawReport = await System.IO.File.ReadAllTextAsync(latestFile.FullName);
      var allValues = rawReport.Split(',');

      var result = new FullReport();
      var reports = new List<ReportItem>();
      
      foreach(var a in allValues)
      {
        int intValue = 0;
        float floatValue = 0;

        var isInt = int.TryParse(a, out intValue);
        var isFloat = float.TryParse(a, out floatValue);

        if (isInt)
          reports.Add(new ReportItem { type = "numeric", value = a });
        else if (isFloat)
          reports.Add(new ReportItem { type = "float", value = a });
        else
          reports.Add(new ReportItem { type = "alphanumeric", value = a.Trim() });
      }

      result.fullReport = reports;

      return result;
    }
  }
}