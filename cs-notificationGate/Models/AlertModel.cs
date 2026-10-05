using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace cs_notificationGate.Models;

public class AlertModel
{
    [Required(AllowEmptyStrings =false)]
    [JsonPropertyName("alert_id")]
    public string AlertId { get; set; } = string.Empty;
    [JsonPropertyName("source")]
    [RegularExpression("^(aman|mossad|pikud-haoref|shabak)$")]
    [Required]
    public string Source { get; set; } = string.Empty;
    [Required(AllowEmptyStrings =false)]
    [JsonPropertyName("title")]
    public string Title { get; set; }= string.Empty;
    [Required(AllowEmptyStrings =false)]
    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public string Classification { get; set; } = string.Empty;
    public double Lat { get; set; }
    public double Lon { get; set; }
    public DateTime TimeStamp { get; set; }
    public string Status { get; set; }= string.Empty;
}
