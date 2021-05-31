using System;
using System.Threading.Tasks;
using Packt.Ecommerce.DTO.Models;

namespace Packt.Ecommerce.Invoice.Contracts
{
    public interface IInvoiceService
    {
        Task<InvoiceDetailsViewModel> GetInvoiceByIdAsync(string invoiceId);

        Task<InvoiceDetailsViewModel> AddInvoiceAsync(InvoiceDetailsViewModel invoice);
    }
}
