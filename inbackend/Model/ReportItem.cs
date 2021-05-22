using System.Collections.Generic;
using System.Linq;

namespace Model
{
  public class ReportItem
  {
    public string type { get; set; }
    public object value { get; set; }
  }

  public class FullReport
  {
    public double alphanumericPerc
    {
      get
      {
        var count = fullReport.Where(f => f.type == "alphanumeric").Count();
        return (double)count / fullReport.Count * 100;
      }
    }
    public double numericPerc
    {
      get
      {
        var count = fullReport.Where(f => f.type == "numeric").Count();
        return (double)count / fullReport.Count * 100;
      }
    }
    public double floatPerc
    {
      get
      {
        var count = fullReport.Where(f => f.type == "float").Count();
        return (double)count / fullReport.Count * 100;
      }
    }
    public List<ReportItem> fullReport { get; set; }
  }
}