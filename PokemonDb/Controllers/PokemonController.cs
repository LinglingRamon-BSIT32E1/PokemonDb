using Microsoft.AspNetCore.Mvc;
using PokemonDb.Models;
using System.Runtime.InteropServices.JavaScript;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace PokemonDb.Controllers
{
    public class PokemonController : Controller
    {
        private readonly HttpClient _httpClient;

        public PokemonController() 
        { 
            _httpClient = new HttpClient { BaseAddress = new System.Uri("https://pokeapi.co/api/v2/") };
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            var response = await _httpClient.GetAsync($"pokemon?offset={(page - 1) * 20}&limit=20");
            var content = await response.Content.ReadAsStringAsync();
            var pokemonList = JObject.Parse(content)["results"]
                                      .Select(p => new Pokemon { Name = p["name"].ToString() })
                                      .ToList();
            return View(pokemonList);
        }

        public async Task<IActionResult> Details(string name)
        {
            var response = await _httpClient.GetAsync($"pokemon/{name}");
            var content = await response.Content.ReadAsStringAsync();
            var json = JObject.Parse(content);

            var pokemon = new Pokemon
            {
                Name = json["name"].ToString(),
                Height = json["height"].ToObject<int>(),
                Weight = json["weight"].ToObject<int>(),
                Types = json["types"].Select(t => t["type"]["name"].ToString()).ToList(),
                Moves = json["moves"].Select(m => m["move"]["name"].ToString()).ToList(),
                Abilities = json["abilities"].Select(a => a["ability"]["name"].ToString()).ToList()
            };

            return View(pokemon);
        }
    }
}
