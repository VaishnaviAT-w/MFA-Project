using MFA.Enum;
using System.Text.Json.Serialization;

namespace MFA.Enum
{
    public class ResultResponse
    {
        public ResponseModel Result {   get; set; }
    }
    public enum ResponseModel
    {
        Success = 1,
        Failed = 2,
        NotFound = 3,
        AlreadyExists = 4,
        Expired = 5
    }


    //public enum MfaType
    //{
    //    SMS = 1,
    //    Email = 2
    //}

}


