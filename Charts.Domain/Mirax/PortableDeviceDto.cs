using Charts.Api.Domain.Mirax.CopyModels;
using Mirax.AvisAcceptanceApp.Models.Entity.ModBusDevice.PortableDeviceModel;
using Mirax.AvisAcceptanceApp.Share.Types;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;


namespace Charts.Api.Domain.Mirax
{
    public class PortableDeviceDto
    {
      
        public Guid Id { get; set; }
         
        public string FactoryNumber { get; set; } 
         
        public string? Name { get; set; }
        public string? ComPortName { get; set; }

        public Guid TechnicalRunToStartId { get; set; } 

    }
}
