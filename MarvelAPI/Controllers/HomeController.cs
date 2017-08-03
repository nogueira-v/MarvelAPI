using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Security.Cryptography;
using Newtonsoft.Json;

namespace MarvelAPI.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            Personagem personagem;

            HttpClient client = new HttpClient();
            
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            string ts = DateTime.Now.Ticks.ToString();
            string publicKey = "PUBLICKEY";
            string privateKey = "PRIVATEKEY";
            string hash = GerarHash(ts, publicKey, privateKey);

            //VALIDAR SE O REQUEST ESTÁ SENDO MONTADO CORRETAMENTE
            //                string teste = $"http://gateway.marvel.com/v1/public/" +
            //    $"characters?ts={ts}&apikey={publicKey}&hash={hash}&" +
            //$"name={Uri.EscapeUriString("Captain America")}";


            HttpResponseMessage response = client.GetAsync($"http://gateway.marvel.com/v1/public/" +
                                                           $"characters?ts={ts}&apikey={publicKey}&hash={hash}&limit=100").Result;

            response.EnsureSuccessStatusCode();

            string conteudo = response.Content.ReadAsStringAsync().Result;

            dynamic resultado = JsonConvert.DeserializeObject(conteudo);
            
            //EXEMPLO DE COMO PEGAR OS DADOS DO JSON
            //personagem.Nome = resultado.data.results[1].name;
            //personagem.UrlImagem = resultado.data.results[1].thumbnail.path;

            List<Personagem> Personagens = new List<Personagem>();

            foreach (var item in resultado.data.results)
            {
                personagem = new Personagem()
                {
                    Nome = item.name,
                    Descricao = item.description,
                    UrlImagem = item.thumbnail.path + "." +
                item.thumbnail.extension,
                    UrlWiki = item.urls[1].url
                };

                    Personagens.Add(personagem);

                }


            ViewBag.Personagens = Personagens;
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
