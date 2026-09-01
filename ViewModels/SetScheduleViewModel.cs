using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppointmentBooking.ViewModels
{
    public class SetScheduleViewModel
    {

        public DateTime ScheduleFrom { get; set; }

        public DateTime ScheduleTo { get; set; }

        public List<string> Days { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public int SlotDurationMinutes { get; set; }

        public List<BreakViewModel> Breaks { get; set; }

    }
}
