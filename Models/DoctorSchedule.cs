using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AppointmentBooking.Models;

public class DoctorSchedule
{
 
    public int ScheduleId { get; set; }

    [Required]
    public int DoctorId { get; set; }

    [Required]
    public DateTime ScheduleDate { get; set; }

    [Required]
    public TimeSpan StartTime { get; set; }

    [Required]
    public TimeSpan EndTime { get; set; }

    [Required]
    [Range(30, 60)]
    public int SlotDurationMinutes { get; set; }

    [Required]
    public DateTime CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

   
}