using Rest_Emulator.Dtos;
using Rest_Emulator.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Integration_Test.V1.Libs
{
    public class RestEmulatorLib
    {
        private readonly HttpClient _client;

        public RestEmulatorLib(string baseUri)
        {
            _client = new()
            {
                BaseAddress = new Uri(baseUri)
            };
        }
 
        public async Task SetMode(ModeType mode, bool success, string content)
        {
            SetModeDto modeDto = new()
            {
                Mode = mode,
                Success = success,
                Response = content
            };

            HttpResponseMessage response =  await _client.PostAsJsonAsync("mode", modeDto);
            response.EnsureSuccessStatusCode();
        }
    }
}
