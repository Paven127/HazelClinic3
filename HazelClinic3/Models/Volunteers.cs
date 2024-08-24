﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HazelClinic3.Models
{
    public class Volunteer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int VolunteerId { get; set; }

        [Required(ErrorMessage = "Field is required")]
        [Display(Name = "Enter your full name:")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Field is required")]
        [Display(Name = "Enter your surname:")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "Field is required")]
        [Display(Name = "Enter your email address:")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Field is required")]
        [Display(Name = "Enter your cellphone number:")]
        [MinLength(10)]
        [StringLength(10)]
        public string CellNo { get; set; }

        [Required(ErrorMessage = "Field is required")]
        [Display(Name = "Enter emergency contact's full name:")]
        public string EmergencyContactName { get; set; }

        [Required(ErrorMessage = "Field is required")]
        [Display(Name = "Enter emergency contact's cellphone number:")]
        [MinLength(10)]
        [StringLength(10)]
        [NotSameAsCellNo(ErrorMessage = "Emergency contact number cannot be the same as your cell number.")]
        public string EmergencyContactCellNo { get; set; }

        [Display(Name = "Enter any relevant experience or skills:")]
        public string Experience { get; set; }

        [Required(ErrorMessage = "Select availability")]
        [Display(Name = "Select your availability:")]
        public string Availability { get; set; }

        [Required(ErrorMessage = "Select volunteer type")]
        [Display(Name = "Select the type of volunteer work you are interested in:")]
        public string VolunteerType { get; set; }

        public string Status { get; set; }


        public class NotSameAsCellNoAttribute : ValidationAttribute
        {
            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                var volunteer = (Volunteer)validationContext.ObjectInstance;

                if (value != null && value.ToString() == volunteer.CellNo)
                {
                    return new ValidationResult(ErrorMessage);
                }

                return ValidationResult.Success;
            }
        }
    }
}
