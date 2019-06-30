using LivrariaBCC.Entity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace LivrariaBCC.MVC.Controllers
{
    public class LivroController : Controller
    {
        private HttpResponseMessage WebApiRequest(string method, Dictionary<string, string> parameters = null)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri("https://localhost:44308");
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                string routeTemplate = "api/livro";

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
                httpClient.BaseAddress = new Uri("https://localhost:44308");
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


        public ActionResult Index()
        {
            var response = WebApiRequest("GET");
            if (response.IsSuccessStatusCode)
                ViewBag.result = response.Content.ReadAsAsync<List<Livro>>().Result;
            else
                ViewBag.result = "Error";

            return View();
        }

        public ActionResult Editar(int? id = null)
        {
            if (id != null)
            {
                var query = new Dictionary<string, string>();
                query["id"] = id.ToString();

                var response = WebApiRequest("GET", query);
                if (response.IsSuccessStatusCode)
                {
                    return View(response.Content.ReadAsAsync<Livro>().Result);
                }
                else
                {
                    ViewBag.result = "Error";
                }
            }

            return View();
        }

        [ValidateAntiForgeryToken]
        public ActionResult Salvar(Livro livro)
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

            return RedirectToAction("Edit", livro);
        }

        public ActionResult Excluir(int? id = null)
        {
            if (id != null)
            {
                var query = new Dictionary<string, string>();
                query["id"] = id.ToString();

                var response = WebApiRequest("DELETE", query);
                if (response.IsSuccessStatusCode)
                    ViewBag.result = "Livro excluído com sucesso";
                else
                    ViewBag.result = "Error";
            }
            return RedirectToAction("Index");
        }


        private string GetUploadFile()
        {
            string caminhoArquivo = string.Empty;
            int arquivosSalvos = 0;
            for (int i = 0; i < Request.Files.Count; i++)
            {
                HttpPostedFileBase arquivo = Request.Files[i];

                //Suas validações ......

                //Salva o arquivo
                if (arquivo.ContentLength > 0)
                {
                    var uploadPath = Server.MapPath("~/Content/Uploads");
                    caminhoArquivo = Path.Combine(@uploadPath,
                    Path.GetFileName(arquivo.FileName));

                    arquivo.SaveAs(caminhoArquivo);
                    arquivosSalvos++;
                }
            }
            return caminhoArquivo;
        }
    }
}