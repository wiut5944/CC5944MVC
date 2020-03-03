using CC5944MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace CC5944MVC.Controllers
{
    public class ProductsController : Controller
    {
        // GET: Products
        public ActionResult Index()
        {
            var products = Get();
            return View(products);
        }

        private List<Product> Get()
        {
            try
            {
                var resultList = new List<Product>();
                var client = new HttpClient();
                var getDataTask = client.GetAsync("https://localhost:44344/api/product").ContinueWith(response =>
                {
                    var result = response.Result;
                    if (result.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var readResult = result.Content.ReadAsAsync<List<Product>>();
                        readResult.Wait();
                        resultList = readResult.Result;
                    }
                });
                getDataTask.Wait();
                return resultList;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        // Post
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(Product product)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44344/api/product");
                var postProduct = client.PostAsJsonAsync<Product>("product", product);
                postProduct.Wait();
                var postResult = postProduct.Result;
                if (postResult.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }

                ModelState.AddModelError(string.Empty, "Server error");
            }
            return View(product);
        }

        public ActionResult Edit(int id)
        {
            Product product = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44344/api/");
                var responseTask = client.GetAsync("product/" + id.ToString());
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<Product>();
                    readTask.Wait();
                    product = readTask.Result;
                }
            }

            return View(product);
        }

        [HttpPost]
        public ActionResult Edit(Product product)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44344/api/product/" + product.Id);
                var putTask = client.PutAsJsonAsync<Product>("product", product);
                putTask.Wait();
                var result = putTask.Result;
                if (result.IsSuccessStatusCode)
                    return RedirectToAction("Index");
                return View(product);
            }
        }



        public ActionResult Delete(int id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44344/api/");
                var deleteTask = client.DeleteAsync("product/" + id.ToString());
                var result = deleteTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
            }

            return RedirectToAction("Index");
        }


    }
}