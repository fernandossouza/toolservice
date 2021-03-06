using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace toolservice.Model {
    public class ToolType {
        public int toolTypeId { get; set; }

        [Required]
        [MaxLength (50)]
        public string name { get; set; }

        [MaxLength (100)]
        public string description { get; set; }

        [Column ("thingGroupIds", TypeName = "integer[]")]
        public int[] thingGroupIds { get; set; }

        [Required]
        public string status { get; set; }

        [NotMapped]
        public IList<ThingGroup> thingGroups { get; set; }
        public string unitOfMeasurement { get; set; }
        public double lifeCycle { get; set; }

    }
}