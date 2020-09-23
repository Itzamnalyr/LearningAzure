using SamLearnsAzure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SamLearnsAzure.Web.Models
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class CdnTestViewModel
    {
        public CdnTestViewModel(string baseSetPartsImagesStorageURL, string baseSetPartsImagesCDNURL)
        {
            BaseSetPartsImagesStorageURL = baseSetPartsImagesStorageURL;
            BaseSetPartsImagesCDNURL = baseSetPartsImagesCDNURL;
        }

        public string BaseSetPartsImagesStorageURL { get; set; }
        public string BaseSetPartsImagesCDNURL { get; set; }
        
    }
}
