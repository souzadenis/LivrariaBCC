using LivrariaBCC.API.Models;
using LivrariaBCC.Entity;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace LivrariaBCC.API.Controllers
{
    public class LivroController : ApiController
    {
        
        private Livro CreateEntity(MySqlDataReader reader)
        {
            Livro livro = null;
            if (!reader.IsClosed && reader.HasRows)
            {
                livro = new Livro();
                livro.Id = Convert.ToInt32(reader["Id"]);
                livro.ISBN = reader["ISBN"].ToString();
                livro.Autor = reader["Autor"].ToString();
                livro.Nome = reader["Nome"].ToString();
                livro.Preco = Convert.ToDouble(reader["Preco"]);
                livro.DataPublicacao = Convert.ToDateTime(reader["DataPublicacao"]);
                livro.ImagemCapa = reader["ImagemCapa"].ToString();
            }
            return livro;
        }

        public IEnumerable<Livro> GetAll()
        {
            List<Livro> livros = new List<Livro>();
            using (var command = DAOHelper.CreateCommand("Livro_SelectAll"))
            using (MySqlDataReader reader = command.ExecuteReader())
                while (reader.Read())
                    livros.Add(CreateEntity(reader));

            return livros;
        }

        [HttpGet]
        [Route("api/LivroSearch")]
        public IEnumerable<Livro> Search(string isbn = "", string autor = "", string nome = "", double preco = 0, DateTime? dataPublicacao = null, string imagemCapa = "")
        {
            List<Livro> livros = new List<Livro>();
            using (var command = DAOHelper.CreateCommand("Livro_Search"))
            {
                command.Parameters.AddWithValue("$ISBN", string.IsNullOrEmpty(isbn) ? DBNull.Value : (object)isbn);
                command.Parameters.AddWithValue("$Autor", string.IsNullOrEmpty(autor) ? DBNull.Value : (object)autor);
                command.Parameters.AddWithValue("$Nome", string.IsNullOrEmpty(nome) ? DBNull.Value : (object)nome);
                command.Parameters.AddWithValue("$Preco", preco == 0 ? DBNull.Value : (object)preco);
                command.Parameters.AddWithValue("$DataPublicacao", dataPublicacao == null ? DBNull.Value : (object)dataPublicacao);
                command.Parameters.AddWithValue("$ImagemCapa", string.IsNullOrEmpty(imagemCapa) ? DBNull.Value : (object)imagemCapa);

                using (MySqlDataReader reader = command.ExecuteReader())
                    while (reader.Read())
                        livros.Add(CreateEntity(reader));
            }
            return livros;
        }

        public IHttpActionResult GetById(int id)
        {
            Livro livro = null;
            using (var command = DAOHelper.CreateCommand("Livro_SelectById"))
            {
                command.Parameters.AddWithValue("$Id", id);
                using (MySqlDataReader reader = command.ExecuteReader())
                    if (reader.Read())
                        livro = CreateEntity(reader);
            }

            if (livro == null)
                return NotFound();

            return Ok(livro);
        }

        public IHttpActionResult Put(Livro livro)
        {
            using (var command = DAOHelper.CreateCommand("Livro_Insert"))
            {
                command.Parameters.AddWithValue("$Id", livro.Id).Direction = System.Data.ParameterDirection.InputOutput;
                command.Parameters.AddWithValue("$ISBN", livro.ISBN);
                command.Parameters.AddWithValue("$Autor", livro.Autor);
                command.Parameters.AddWithValue("$Nome", livro.Nome);
                command.Parameters.AddWithValue("$Preco", livro.Preco);
                command.Parameters.AddWithValue("$DataPublicacao", livro.DataPublicacao);
                command.Parameters.AddWithValue("$ImagemCapa", livro.ImagemCapa);

                command.ExecuteNonQuery();
                livro.Id = (int)command.Parameters["$Id"].Value;
            }

            if (livro.Id > 0)
                return Ok(livro);
            else
                return BadRequest("Não foi possível inserir o livro");
        }

        public IHttpActionResult Post(Livro livro)
        {
            int result = 0;
            using (var command = DAOHelper.CreateCommand("Livro_Update"))
            {
                command.Parameters.AddWithValue("$Id", livro.Id).Direction = System.Data.ParameterDirection.InputOutput;
                command.Parameters.AddWithValue("$ISBN", livro.ISBN);
                command.Parameters.AddWithValue("$Autor", livro.Autor);
                command.Parameters.AddWithValue("$Nome", livro.Nome);
                command.Parameters.AddWithValue("$Preco", livro.Preco);
                command.Parameters.AddWithValue("$DataPublicacao", livro.DataPublicacao);
                command.Parameters.AddWithValue("$ImagemCapa", livro.ImagemCapa);

                result = command.ExecuteNonQuery();
                livro.Id = (int)command.Parameters["$Id"].Value;
            }

            if (result > 0)
                return Ok(livro);
            else
                return BadRequest("Não foi possível atualizar o livro");
        }

        public IHttpActionResult Delete(int id)
        {
            int result = 0;
            using (var command = DAOHelper.CreateCommand("Livro_Delete"))
            {
                command.Parameters.AddWithValue("$Id", id);
                result = command.ExecuteNonQuery();
            }

            if (result > 0)
                return Ok();
            else
                return BadRequest("Não foi possível deleter o livro de Id: " + id.ToString());
        }

    }
}
