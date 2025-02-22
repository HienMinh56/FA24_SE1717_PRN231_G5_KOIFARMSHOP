﻿using System;
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
        public async Task<IBusinessResult> PostConsignment([FromBody] CreateConsignmentRequest consignment)
        {
            return await _consignmentService.SaveConsignment(consignment);
        }

        // PUT: api/Consignments
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut]
        public async Task<IBusinessResult> PutConsignment([FromBody] UpdateConsignmentRequest consignment)
        {
            return await _consignmentService.SaveConsignment(consignment);
        }

        [HttpDelete("{consignmentId}")]
        public async Task<IBusinessResult> DeleteConsignment(string consignmentId)
        {
            return await _consignmentService.DeleteById(consignmentId);
        }
    }
}
