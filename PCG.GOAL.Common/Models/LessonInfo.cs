using System.Collections.Generic;

namespace PCG.GOAL.Common.Models
{
    public class LessonInfo
    {
        public string LessonName { get; set; }
        public string LessonCategory { get; set; }
        public string IsCore { get; set; }
        public string LessonSubCategory { get; set; }
        public string IepGoal { get; set; }
        public string CommonStandard { get; set; }
        public string CommonStandardDescription { get; set; }
        public string CommonStandardRationale { get; set; }
        public List<LessonObjective> LessonObjectives { get; set; }
    }
}