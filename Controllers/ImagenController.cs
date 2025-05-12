using Microsoft.AspNetCore.Mvc;
using InmobiliariaAlcaraz.Models;

namespace InmobiliariaAlcaraz.Controllers
{
    public class ImagenController : Controller
    {
        private readonly RepositorioImagen repoImagen;
        private readonly IWebHostEnvironment env;

        public ImagenController(IWebHostEnvironment env, IConfiguration cfg)
        {
            repoImagen = new RepositorioImagen(cfg);
            this.env = env;
        }

        [HttpGet]
        public IActionResult Imagenes(int id)
        {
            ViewBag.IdInmueble = id;
            return View();
        }

        
        [HttpGet]
        public IActionResult ObtenerPorInmueble(int id)
        {
            var imgs = repoImagen.ObtenerPorInmueble(id);
            return Json(imgs);
        }

        
        [HttpPost]
        public async Task<IActionResult> SubirImagen(int idInmueble, IFormFile archivo)
        {
            try
            {
                if (archivo != null && archivo.Length > 0)
                {
                    var uploads = Path.Combine(env.WebRootPath, "uploads");
                    if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);

                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(archivo.FileName)}";
                    var filePath = Path.Combine(uploads, fileName);
                    await using var stream = new FileStream(filePath, FileMode.Create);
                    await archivo.CopyToAsync(stream);

                   
                    repoImagen.Alta(new Imagen
                    {
                        Url = "/uploads/" + fileName,
                        IdInmueble = idInmueble
                    });
                }
                return Ok();
            }
            catch (Exception ex)
            {
             
                return StatusCode(500, ex.Message);
            }
        }

      
        [HttpPost]
        public IActionResult EliminarImagen(int id)
        {
            try
            {
                var img = repoImagen.ObtenerPorId(id);
                if (img != null)
                {
                    var path = Path.Combine(env.WebRootPath, img.Url.TrimStart('/'));
                    if (System.IO.File.Exists(path))
                        System.IO.File.Delete(path);

                    repoImagen.Baja(id);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
