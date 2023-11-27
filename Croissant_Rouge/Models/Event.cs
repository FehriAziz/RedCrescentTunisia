#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Croissant_Rouge.Utility;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Croissant_Rouge.Models;




public class FuturDate : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if ((DateTime)value < DateTime.Now)
            return new ValidationResult("Date must be in the Futur");
        return ValidationResult.Success;
    }
}




public class Event
{
    // Donation Id
    [Key]
    public int EventId { get; set; }

    //! Foreign Key

    [Required]
    public int UserId { get; set; }

    // Title
    [Required(ErrorMessage = "Please enter your Title.")]
    [MinLength(3, ErrorMessage = "Please enter a valid Title .")]
    public string Title { get; set; }


    // Date
    [FuturDate]
    [Required(ErrorMessage = "Please enter your Date.")]
    public DateTime Date { get; set; }




    //Staff 
    [Required(ErrorMessage = "Please enter number of staff requested.")]
    public int Staff { get; set; }


    //Description
    [Required(ErrorMessage = "Please enter your Description.")]
    [MinLength(3, ErrorMessage = "Please enter a valid Description.")]
    public string Description { get; set; }



    //Place 
    [Required(ErrorMessage = "Please enter your Place.")]
    [MinLength(3, ErrorMessage = "Please enter a valid Place.")]
    public string Place { get; set; }


    //Picture
    [ValidateNever]
    public string? Picture { get; set; }


    // Created At
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // Updated At
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    // Add the navigation property Here

    public User? EventCreator { get; set; }

    public List<Participation> EventParticipations { get; set; } = new List<Participation>();
}

