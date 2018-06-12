using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace toolservice.Model
{
    public class StateTransitionHistory
    {
        [JsonIgnore]
        public int stateTransitionHistoryId { get; set; }
        [NotMapped]
        public Tool tool { get; set; }
        public int toolId { get; set; }
        public bool justificationNeeded { get; set; }
        public Justification justification { get; set; }
        public string previousState { get; set; }
        public string nextState { get; set; }
        public long timeStampTicks { get; set; }
        public double previoustLife { get; set; }
        public string username { get; set; }

    }
}
