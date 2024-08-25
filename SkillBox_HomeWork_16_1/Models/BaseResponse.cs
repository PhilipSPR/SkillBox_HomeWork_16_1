using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBox_HomeWork_16_1.Models
{
    public class BaseResponse
    {
        public bool Succes { get; set; }
        public string Message { get; set; }

        public BaseResponse()
        {
            
        }
        public BaseResponse(string message)
        {
            Succes = true;
            Message = message;
        }

        public BaseResponse(bool status ,string message)
        {
            Succes = status;
            Message = message;
        }

    }
}
