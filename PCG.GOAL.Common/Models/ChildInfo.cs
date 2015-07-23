using System.Collections.Generic;

namespace PCG.GOAL.Common.Models
{
    public class ChildInfo
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DateOfBirth { get; set; }
        public string Grade { get; set; }
        public string Gender { get; set; }
        public string[] EducationalClassification { get; set; }
        public string LanguageAbility { get; set; }
        public string StateTestNumber { get; set; }
        public List<LessonInfo> Lessons { get; set; }
        public List<BehaviorInfo> Behaviors { get; set; }
        public string AreaOfConcern { get; set; }
    }
}