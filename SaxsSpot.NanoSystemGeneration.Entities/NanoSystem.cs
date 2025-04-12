using System.ComponentModel.DataAnnotations.Schema;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.Enums;

namespace SaxsSpot.NanoSystemGeneration.Entities;

[Table("nano_system")]
public class NanoSystem
{
    [Column("id")]
    public long Id { get; set; }
    
    [Column("series_id")]
    public long SeriesId { get; set; }
    
    [Column("object_id")]
    public Guid ObjectId { get; set; }
    
    [Column("particle_count")]
    public int ParticleCount { get; set; }

    [Column("current_particle_count")]
    public int CurrentParticleCount { get; set; }
    
    [Column("particle_kind")]
    public ParticleKind ParticleKind { get; set; }
    
    [Column("global_size")]
    public float GlobalSize { get; set; }

    [Column("numerical_concentration")]
    public float NumericalConcentration { get; set; }
    
    [Column("max_particle_size")]
    public float MaxParticleSize { get; set; }
    
    [Column("min_particle_size")]
    public float MinParticleSize { get; set; }
    
    [Column("excess")]
    public float Excess { get; set; }
    
    [Column("k")]
    public float K { get; set; }
    
    [Column("theta")]
    public float Theta { get; set; }
    
    [Column("current_numerical_concentration")]
    public float CurrentNumericalConcentration { get; set; }
    
    [Column("current_global_size")]
    public float CurrentGlobalSize { get; set; }
    
    [Column("generation_time")]
    public DateTime GenerationTime { get; set; }
    
    [Column("input_date")]
    public DateTime InputDate { get; set; }
    
    [Column("user_id")]
    public long UserId { get; set; }
}