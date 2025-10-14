using Mirax.AvisAcceptanceApp.Models.DTO.ClimateTest;
using Mirax.AvisAcceptanceApp.Share.Types;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mirax.AvisAcceptanceApp.Share.CopyModels
{
    public partial class CreatedClimateTest 
    {
        [Key]
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public ObservableCollection<DeviceContainer>? DeviceContainers { get; set; } = [];
        public ObservableCollection<SensorModificationWithGases>? SensorModifications { get; set; } = [];
        public ObservableCollection<ClimateTestGas>? ClimateTestGases { get; set; } = [];
        public ObservableCollection<ClimateTestTemperaturePoint>? TemperaturePoints { get; set; } = [];

        public Gas? NullGas { get; set; }

 
        public ClimateGasUseMode ClimateGasUseMode { get; set; }

        
    }
}
