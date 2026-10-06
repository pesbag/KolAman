using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CommandCenter.Models;

public class AlertModel
{
    [Required(AllowEmptyStrings = false)]
    [JsonPropertyName("alert_id")]
    public string AlertId { get; set; } = string.Empty;
    [JsonPropertyName("source")]
    [RegularExpression("^(aman|mossad|pikud-haoref|shabak)$")]
    [Required]
    public string Source { get; set; } = string.Empty;
    [Required(AllowEmptyStrings = false)]
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;
    [Required(AllowEmptyStrings = false)]
    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;
    [JsonPropertyName("priority")]
    [Required(AllowEmptyStrings =false)]
    public string Priority { get; set; } = string.Empty;
    [Required(AllowEmptyStrings =false)]
    [JsonPropertyName("classification")]
    [RegularExpression("^(UNCLASSIFIED|RESTRICTED|SECRET|TOP_SECRET)^")]
    public string Classification { get; set; } = string.Empty;
    [Required]
    [Range(-90,90)]
    [JsonPropertyName("lat")]
    public double Lat { get; set; }
    [Required]
    [JsonPropertyName("lon")]
    [Range(-180,180)]
    public double Lon { get; set; }
    [Required]
    [JsonPropertyName("timestamp")]
    public DateTime TimeStamp { get; set; }
    [JsonPropertyName("status")]
    [Required(AllowEmptyStrings =false)]
    public string Status { get; set; } = "WAITING";
}
