using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using To_Do_List.ViewModels;

namespace To_Do_List.Models
{
    public class TaskItem : BaseViewModel
    { 
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? Deadline { get; set; } 
        public PriorityLevel Priority { get; set; }
        public int CategoryId { get; set; }

        public Brush DeadlineColor
        {
            get
            {
                if (!Deadline.HasValue)
                    return Brushes.White; 
                var timeRemaining = Deadline.Value - DateTime.Now;
                if (timeRemaining > TimeSpan.FromDays(1))
                    return Brushes.Green; 
                else if (timeRemaining <= TimeSpan.FromDays(1) && timeRemaining > TimeSpan.Zero)
                    return Brushes.Yellow;
                else
                    return Brushes.Red; 
            }
        }
    }
}
