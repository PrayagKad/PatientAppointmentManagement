using System;
using System.Collections.Generic;

namespace AppointmentBooking.ViewModels
{
    public class BreakViewModel
    {
            public TimeSpan StartTime { get; set; }
            public TimeSpan EndTime { get; set; }
            public string Reason { get; set; }

    }
}