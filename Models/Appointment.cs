using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AppointmentBooking.Models;


public class Appointment
{

    public int AppointmentId { get; set; }

    [Required]
    public int SlotId { get; set; }

    [Required]
    public int PatientId { get; set; }

    [Required]
    [StringLength(20)]
    public string Status { get; set; }

    [Required]
    public DateTime BookedDate { get; set; }

    public DateTime? ConfirmedDate { get; set; }

    public DateTime? CancelledDate { get; set; }

    public DateTime? CompletedDate { get; set; }

}