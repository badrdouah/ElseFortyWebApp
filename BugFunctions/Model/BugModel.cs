using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BugReportArchive.Model
{
	public class BugModel
	{
          public string id { get; set; } 
          public string name { get; set; }
          public string email { get; set; }
          public string software { get; set; }
          public string subject { get; set; }
          public string content { get; set; }
          public string resolution { get; set; }
          public string status { get; set; }
          public DateTime resolutionTime { get; set; }
          public DateTime creationTime { get; set; }
    }
}

