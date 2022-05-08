﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WelcomeController : ControllerBase
    {
        private readonly ILogger<WelcomeController> _logger;

        public WelcomeController(ILogger<WelcomeController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<Welcome> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 1).Select(index => new Welcome
            {
                Date = DateTime.Now,
                Greeting = "Welcome to the page"
            })
            .ToArray();
        }
    }
}
