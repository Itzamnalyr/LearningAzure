using System;

namespace SamLearnsAzure.Web.Models
{
    public class ErrorViewModel
    {
        public ErrorViewModel(string requestId)
        {
            RequestId = requestId;
        }

        public string RequestId { get; set; }

        public bool ShowRequestId
        {
            get
            {
                return !string.IsNullOrEmpty(RequestId);
            }
        }
    }
}