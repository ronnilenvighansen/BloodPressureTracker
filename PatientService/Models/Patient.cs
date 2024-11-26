using System.ComponentModel.DataAnnotations;

namespace PatientService.Models;
public class Patient
{
    [Key]
    public string SSN { get; set; }
    public string Mail { get; set; }
    public string Name { get; set; }
}
