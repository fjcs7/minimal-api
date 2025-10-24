using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MinimalApi.Dominio.DTOs;

namespace MinimalApi.Dominio.Entidades;

public class Veiculo
{    
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; } = default;

    [Required]
    [StringLength(150)]
    public string Nome { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Marca { get; set; } = string.Empty;

    [Required]
    public int Ano { get; set; } = default;
}
    
    
