using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatalogMicroservice.RabbitMQ.Model
{
    internal class CommandDto
    {
        // ReturnQueue den der bliver svaret til
        public string ReturnQueue { get; set; }

        //Command get item id fx
        public Command Command {  get; set; }
    }
}
