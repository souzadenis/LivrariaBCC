using System;
using System.Web.Mvc;
using LivrariaBCC.Entity;
using LivrariaBCC.MVC.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LivrariaBCC.Tests.Controllers
{
    [TestClass]
    public class LivroControllerTest
    {
        [TestMethod]
        public void Index()
        {
            var controller = new LivroController();
            ViewResult result = controller.Index() as ViewResult;
            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Editar()
        {
            var controller = new LivroController();
            ViewResult result = controller.Editar() as ViewResult;
            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Editar(int id)
        {
            LivroController controller = new LivroController();
            ViewResult result = controller.Editar(id) as ViewResult;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Buscar()
        {
            LivroController controller = new LivroController();
            ViewResult result = controller.Buscar(autor: "Eddie") as ViewResult;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Excluir()
        {
            LivroController controller = new LivroController();
            ViewResult result = controller.Excluir(id: 7) as ViewResult;
            Assert.IsNotNull(result);
        }
    }
}
