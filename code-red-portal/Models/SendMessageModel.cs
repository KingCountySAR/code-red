using System.ComponentModel.DataAnnotations;

namespace Kcsar.Paging.Web.Models
{
  public class SendMessageModel
  {
    [Required]
    public string Message { get; set; }
  }
}
