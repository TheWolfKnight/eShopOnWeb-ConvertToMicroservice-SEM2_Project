using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatalogMicroservice.RabbitMQ.Model;

namespace CatalogMicroservice.RabbitMQ.Interfaces;
public interface ICatalogSender
{
    Task<string> CreateAsync(Command command, string claimKey);
    Task<string> GetAsync(Command command, string claimKey);
    Task<string> GetAllAsync(Command command, string claimKey);
    Task<string> UpdateAsync(Command command, string claimKey);
    Task<string> DeleteAsync(Command command, string claimKey);
}
