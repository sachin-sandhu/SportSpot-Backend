﻿using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Rest_Emulator.Services
{
    public class SearchModeService : IModeService
    {
        public bool Success { get; set; }
        private string? _response;
        public string Response { get => _response ?? throw new InvalidDataException("Response can not be null!"); set => _response = value; }

        public IActionResult GetResult()
        {
            if (Success)
            {
                JsonNode jobj = JsonSerializer.Deserialize<JsonNode>(Response) ?? throw new InvalidOperationException("Response is not a JSON!");
                OkObjectResult result = new(jobj);
                result.ContentTypes.Add("application/json");
                return result;
            }
            else
            {
                return new NotFoundObjectResult(Response);
            }
        }
    }
}
