using System.Collections.Generic;

namespace PCG.GOAL.Common.Models
{
    public class BehaviorInfo
    {
        public string Name { get; set; }
        public string Description;
        public string DataCollectionMethod { get; set; }
        public string IepGoal { get; set; }
        public List<BehaviorObjective> BehaviorObjectives { get; set; }
    }
}