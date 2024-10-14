using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KoiFarmShop.Data.Models;
using KoiFarmShop.Service;
using KoiFarmShop.Service.Base;
using KoiFarmShop.Data.Request;

namespace KoiFarmShop.APIService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConsignmentsController : ControllerBase
    {
        private readonly IConsignmentService _consignmentService;

        public ConsignmentsController(IConsignmentService consignmentService)
        {
            _consignmentService = consignmentService;
        }

        // GET: api/Consignments
        [HttpGet]
        public async Task<IBusinessResult> GetConsignments()
        {
            return await _consignmentService.GetAll();
        }

        // GET: api/Consignments/5
        [HttpGet("{consignmentId}")]
        public async Task<IBusinessResult> GetConsignment(string consignmentId)
        {
            return await _consignmentService.GetById(consignmentId);
        }

        // POST: api/Consignments
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IBusinessResult> PostConsignment(CreateConsignmentRequest consignment)
        {
            return await _consignmentService.Create(consignment);
        }

        // PUT: api/Consignments/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{consignmentId}")]
        public async Task<IBusinessResult> PutConsignment(string consignmentId, int status)
        {
            return await _consignmentService.Update(consignmentId, status);
        }
    }
}
