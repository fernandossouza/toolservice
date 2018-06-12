using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using toolservice.Validation;

namespace toolservice.Model {
    public class Tool {
        public int toolId { get; set; }

        [MaxLength (50)]
        public string name { get; set; }

        [MaxLength (100)]
        public string description { get; set; }

        [MaxLength (100)]
        public string serialNumber { get; set; }

        [MaxLength (100)]
        public string code { get; set; }

        [MaxLength (100)]
        public string codeClient { get; set; }
        public int? position { get; set; }
        public double currentLife { get; set; }

        [Required]
        [ToolTypeValidation]
        public int? typeId { get; set; }

        [NotMapped]
        public string typeName { get; set; }

        [Required]
        public string status { get; set; }
        public int? currentThingId { get; set; }

        [NotMapped]
        public Thing currentThing { get; set; }
        public List<ToolInformation> informations { get; set; }

        [NotMapped]
        public string unitOfMeasurement { get; set; }

        [NotMapped]
        public double lifeCycle { get; set; }

        public string username { get; set; }

    }
}