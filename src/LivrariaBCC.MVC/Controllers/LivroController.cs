using LivrariaBCC.Entity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace LivrariaBCC.MVC.Controllers
{
    public class LivroController : Controller
    {
        const string baseUriWebApi = "https://localhost:44308";

        private HttpResponseMessage WebApiRequest(string method, Dictionary<string, string> parameters = null, string rote = "")
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(baseUriWebApi);
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                string routeTemplate = string.IsNullOrEmpty(rote) ? "api/livro" : rote;

                switch (method.ToUpper())
                {
                    case "GET":

                        if (parameters != null && parameters.Count > 0)
                            routeTemplate += BuildQuerystring(parameters);
                        return httpClient.GetAsync(routeTemplate).Result;

                        break;

                    case "DELETE":

                        routeTemplate += BuildQuerystring(parameters);
                        return httpClient.DeleteAsync(routeTemplate).Result;

                        break;
                }
            }

            return null;
        }

        private HttpResponseMessage WebApiRequest(string method, Livro livro)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(baseUriWebApi);
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                string routeTemplate = "api/livro";

                var myContent = JsonConvert.SerializeObject(livro);
                var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

                switch (method.ToUpper())
                {
                    case "POST":

                        return httpClient.PostAsync(routeTemplate, byteContent).Result;

                        break;

                    case "PUT":

                        return httpClient.PutAsync(routeTemplate, byteContent).Result;

                        break;
                }
            }

            return null;
        }

        private string BuildQuerystring(Dictionary<string, string> parameters)
        {
            List<string> paramList = new List<string>();
            foreach (var parameter in parameters)
            {
                paramList.Add(parameter.Key + "=" + parameter.Value);
            }
            return "?" + string.Join("&", paramList);
        }


        public ActionResult Index(string sortOrder = "", string searchString = "")
        {
            try
            {
                ViewBag.IsbnSortParm = String.IsNullOrEmpty(sortOrder) ? "isbn_desc" : "";
                ViewBag.AutorSortParm = sortOrder == "autor" ? "autor_desc" : "autor";
                ViewBag.NomeSortParm = sortOrder == "nome" ? "nome_desc" : "nome";
                ViewBag.PrecoSortParm = sortOrder == "preco" ? "preco_desc" : "preco";
                ViewBag.DataSortParm = sortOrder == "data" ? "data_desc" : "data";
                ViewBag.ImagemSortParm = sortOrder == "imagem" ? "imagem_desc" : "imagem";

                IEnumerable<Livro> livros = new List<Livro>();
                var response = WebApiRequest("GET");
                if (response.IsSuccessStatusCode)
                    livros = response.Content.ReadAsAsync<List<Livro>>().Result;
                else
                    ViewBag.result = "Error";

                // buscar
                if (!String.IsNullOrEmpty(searchString))
                {
                    livros = livros.Where(s => s.ISBN.Contains(searchString)
                                           || s.Autor.Contains(searchString)
                                           || s.Nome.Contains(searchString)
                                           || s.Preco.Equals(searchString)
                                           || s.DataPublicacao.Equals(searchString)
                                           || s.ImagemCapa.Contains(searchString)
                                           );
                }

                // ordenar
                switch (sortOrder)
                {
                    case "isbn_desc":
                        livros = livros.OrderByDescending(s => s.ISBN);
                        break;
                    case "autor":
                        livros = livros.OrderBy(s => s.Autor);
                        break;
                    case "autor_desc":
                        livros = livros.OrderByDescending(s => s.Autor);
                        break;
                    case "nome":
                        livros = livros.OrderBy(s => s.Nome);
                        break;
                    case "nome_desc":
                        livros = livros.OrderByDescending(s => s.Nome);
                        break;
                    case "preco":
                        livros = livros.OrderBy(s => s.Preco);
                        break;
                    case "preco_desc":
                        livros = livros.OrderByDescending(s => s.Preco);
                        break;
                    case "data":
                        livros = livros.OrderBy(s => s.DataPublicacao);
                        break;
                    case "data_desc":
                        livros = livros.OrderByDescending(s => s.DataPublicacao);
                        break;
                    case "imagem":
                        livros = livros.OrderBy(s => s.ImagemCapa);
                        break;
                    case "imagem_desc":
                        livros = livros.OrderByDescending(s => s.ImagemCapa);
                        break;
                    default:
                        livros = livros.OrderBy(s => s.ISBN);
                        break;
                }
                
                return View(livros.ToList());
            }
            catch
            {
                ModelState.AddModelError("", "Problemas ocorrem.");
            }
            return View();
        }

        public ActionResult Editar(int? id = null)
        {
            var livro = new Livro();
            if (id != null)
            {
                var query = new Dictionary<string, string>();
                query["id"] = id.ToString();

                var response = WebApiRequest("GET", query);
                if (response.IsSuccessStatusCode)
                {
                    livro = response.Content.ReadAsAsync<Livro>().Result;
                }
                else
                {
                    ViewBag.result = "Error";
                }
            }

            return View(livro);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(Livro livro)
        {
            if (ModelState.IsValid)
            {
                // buca ISBN unico
                var query = new Dictionary<string, string>();
                query["isbn"] = livro.ISBN;

                var responseGet = WebApiRequest("GET", query, "api/LivroSearch");
                if (responseGet.IsSuccessStatusCode)
                {
                    var livrosGet = responseGet.Content.ReadAsAsync<List<Livro>>().Result;
                    if (livrosGet.Count > 1 || (livrosGet.Count == 1 && livrosGet[0].Id != livro.Id))
                    {
                        ViewBag.result = "Já existe livro com o mesmo ISBN";
                    }
                    else
                    {
                        var file = GetUploadFile();
                        if (!string.IsNullOrEmpty(file))
                            livro.ImagemCapa = file;

                        HttpResponseMessage response;
                        if (livro.Id == 0) // insert
                            response = WebApiRequest("PUT", livro);
                        else // update
                            response = WebApiRequest("POST", livro);

                        if (response.IsSuccessStatusCode)
                            return RedirectToAction("Index");
                        else
                            ViewBag.result = "Error";
                    }
                }
            }

            return View(livro);
        }

        public ActionResult Excluir(int? id = null)
        {
            if (id != null)
            {
                var query = new Dictionary<string, string>();
                query["id"] = id.ToString();

                var response = WebApiRequest("DELETE", query);
                if (response.IsSuccessStatusCode)
                {
                    ViewBag.result = "Livro excluído com sucesso";
                    return RedirectToAction("Index");
                }
                else
                    ViewBag.result = "Error";
            }
            return View("Index");
        }


        private string GetUploadFile()
        {
            string nomeArquivo = string.Empty;
            int arquivosSalvos = 0;
            for (int i = 0; i < Request.Files.Count; i++)
            {
                HttpPostedFileBase arquivo = Request.Files[i];

                //só permitir imagens
                string[] formats = new string[] { ".jpg", ".png", ".gif", ".jpeg" };
                if (formats.Any(item => arquivo.FileName.EndsWith(item, StringComparison.OrdinalIgnoreCase)))
                {
                    //Salva o arquivo
                    if (arquivo.ContentLength > 0)
                    {
                        var uploadPath = Server.MapPath("~/Content/Uploads");
                        nomeArquivo = Path.GetFileName(arquivo.FileName);
                        var caminhoArquivo = Path.Combine(@uploadPath, nomeArquivo);

                        arquivo.SaveAs(caminhoArquivo);
                        arquivosSalvos++;
                    }
                }
            }
            return nomeArquivo;
        }

        public ActionResult Buscar(string isbn = "", string autor = "", string nome = "", double preco = 0, DateTime? dataPublicacao = null, string imagemCapa = "")
        {
            List<Livro> livros = new List<Livro>();
            var query = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(isbn))
                query["isbn"] = isbn;
            if (!string.IsNullOrEmpty(nome))
                query["nome"] = nome;
            if (preco > 0)
                query["preco"] = preco.ToString();
            if (dataPublicacao != null)
                query["dataPublicacao"] = dataPublicacao.ToString();
            if (!string.IsNullOrEmpty(imagemCapa))
                query["imagemCapa"] = imagemCapa;
            
            var responseGet = WebApiRequest("GET", query, "api/LivroSearch");
            if (responseGet.IsSuccessStatusCode)
            {
                livros = responseGet.Content.ReadAsAsync<List<Livro>>().Result;
            }

            return View("Index", livros);
        }

    }
}