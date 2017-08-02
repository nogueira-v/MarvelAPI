using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Security.Cryptography;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;


namespace MarvelAPI.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            Personagem personagem;

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                string ts = DateTime.Now.Ticks.ToString();
                string publicKey = "PUBLICKEY";
                string privateKey = "PRIVATEKEY";
                string hash = GerarHash(ts, publicKey, privateKey);
//                string teste = $"http://gateway.marvel.com/v1/public/" +
//    $"characters?ts={ts}&apikey={publicKey}&hash={hash}&" +
//$"name={Uri.EscapeUriString("Captain America")}";


                HttpResponseMessage response = client.GetAsync($"http://gateway.marvel.com/v1/public/" +
                                                               $"characters?ts={ts}&apikey={publicKey}&hash={hash}&" +
                                                               $"name={Uri.EscapeUriString("Wolverine")}").Result;

                response.EnsureSuccessStatusCode();

                string conteudo = response.Content.ReadAsStringAsync().Result;

                dynamic resultado = JsonConvert.DeserializeObject(conteudo);

                personagem = new Personagem();

                personagem.Nome = resultado.data.results[0].name;
                personagem.Descricao = resultado.data.results[0].description;
                personagem.UrlImagem = resultado.data.results[0].thumbnail.path + "." +
                                       resultado.data.results[0].thumbnail.extension;
                personagem.UrlWiki = resultado.data.results[0].urls[1].url;

            }


            return View(personagem);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }

        private string GerarHash(string ts, string publicKey, string privateKey)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(ts + privateKey + publicKey);
            var gerador = MD5.Create();
            byte[] byteshash = gerador.ComputeHash(bytes);
            return BitConverter.ToString(byteshash).ToLower().Replace("-", string.Empty);
        }
    }
}
