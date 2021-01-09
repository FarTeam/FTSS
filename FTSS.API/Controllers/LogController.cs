﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace FTSS.API.Controllers
{
    public class complex
    {
        public string message { get; set; }
    }


    [Route("/api/[controller]/[action]")]
    public class LogController : Controller
    {
        private readonly Logic.Log.ILog _logger;

        public LogController(Logic.Log.ILog logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public IActionResult Add([FromBody] Models.Database.Log data)
        {
            _logger.Add(data.MSG);
            return Ok(data);
        }

    }
}