using System.ComponentModel.DataAnnotations;

namespace Shared.Models.Patient;
public class Patient
{
    [Key]
    public string SSN { get; set; }
    public string Mail { get; set; }
    public string Name { get; set; }
}
