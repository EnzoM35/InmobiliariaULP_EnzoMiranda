using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Laboratorio_II___Proyecto_Inmobiliaria_EnzoMiranda.Models;

namespace Laboratorio_II___Proyecto_Inmobiliaria_EnzoMiranda.Controllers
{
    [Authorize]
    public class InmueblesController : Controller
    {
        private readonly IRepositorioInmueble _repoInmueble;
        private readonly IRepositorioPropietario _repoPropietario;
        private readonly IRepositorioTipoInmueble _repoTipoInmueble;
        private readonly IWebHostEnvironment _env;

        public InmueblesController(
            IRepositorioInmueble repoInmueble,
            IRepositorioPropietario repoPropietario,
            IRepositorioTipoInmueble repoTipoInmueble,
            IWebHostEnvironment env)
        {
            _repoInmueble = repoInmueble;
            _repoPropietario = repoPropietario;
            _repoTipoInmueble = repoTipoInmueble;
            _env = env;
        }

        private void CargarSelects()
        {
            ViewBag.Propietarios = _repoPropietario.ObtenerTodos();
            ViewBag.TiposInmueble = _repoTipoInmueble.ObtenerTodos();
        }

        // GET: Inmuebles
        public ActionResult Index()
        {
            var lista = _repoInmueble.ObtenerTodos();
            return View(lista);
        }

        // GET: Inmuebles/Details/5
        public ActionResult Details(int id)
        {
            var inmueble = _repoInmueble.ObtenerPorId(id);
            if (inmueble == null)
            {
                TempData["Error"] = "El inmueble no existe.";
                return RedirectToAction(nameof(Index));
            }
            return View(inmueble);
        }

        // GET: Inmuebles/Create
        public ActionResult Create()
        {
            CargarSelects();
            return View(new Inmueble { Disponible = true, PorcentajeReserva = 10.00m });
        }

        // POST: Inmuebles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Inmueble inmueble, IFormFile? archivoPortada, IList<IFormFile>? fotos)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Guardar foto de portada si se seleccionó
                    if (archivoPortada != null && archivoPortada.Length > 0)
                    {
                        inmueble.Portada = await GuardarArchivoEnDisco(archivoPortada);
                    }

                    int id = _repoInmueble.Alta(inmueble);

                    if (id > 0)
                    {
                        // Si se subió portada, registrarla también en la tabla ImagenesInmueble
                        if (!string.IsNullOrEmpty(inmueble.Portada))
                        {
                            _repoInmueble.AltaImagen(new ImagenInmueble
                            {
                                IdInmueble = id,
                                Url = inmueble.Portada,
                                EsPortada = true
                            });
                        }

                        // Subir fotos adicionales si vienen
                        if (fotos != null && fotos.Count > 0)
                        {
                            foreach (var f in fotos)
                            {
                                if (f.Length > 0)
                                {
                                    string url = await GuardarArchivoEnDisco(f);
                                    _repoInmueble.AltaImagen(new ImagenInmueble
                                    {
                                        IdInmueble = id,
                                        Url = url,
                                        EsPortada = string.IsNullOrEmpty(inmueble.Portada)
                                    });
                                }
                            }
                        }

                        TempData["Mensaje"] = "Inmueble registrado exitosamente.";
                        return RedirectToAction(nameof(Details), new { id });
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al crear inmueble: " + ex.Message);
            }

            CargarSelects();
            return View(inmueble);
        }

        // GET: Inmuebles/Edit/5
        public ActionResult Edit(int id)
        {
            var inmueble = _repoInmueble.ObtenerPorId(id);
            if (inmueble == null)
            {
                TempData["Error"] = "El inmueble no existe.";
                return RedirectToAction(nameof(Index));
            }
            CargarSelects();
            return View(inmueble);
        }

        // POST: Inmuebles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, Inmueble inmueble, IFormFile? archivoPortada)
        {
            if (id != inmueble.IdInmueble)
            {
                return NotFound();
            }

            try
            {
                if (ModelState.IsValid)
                {
                    if (archivoPortada != null && archivoPortada.Length > 0)
                    {
                        string nuevaUrl = await GuardarArchivoEnDisco(archivoPortada);
                        inmueble.Portada = nuevaUrl;

                        _repoInmueble.AltaImagen(new ImagenInmueble
                        {
                            IdInmueble = inmueble.IdInmueble,
                            Url = nuevaUrl,
                            EsPortada = true
                        });
                    }

                    _repoInmueble.Modificacion(inmueble);
                    TempData["Mensaje"] = "Inmueble actualizado correctamente.";
                    return RedirectToAction(nameof(Details), new { id = inmueble.IdInmueble });
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al modificar inmueble: " + ex.Message);
            }

            CargarSelects();
            return View(inmueble);
        }

        // GET: Inmuebles/Delete/5 (Solo Administrador)
        [Authorize(Roles = "Administrador")]
        public ActionResult Delete(int id)
        {
            var inmueble = _repoInmueble.ObtenerPorId(id);
            if (inmueble == null)
            {
                TempData["Error"] = "El inmueble no existe.";
                return RedirectToAction(nameof(Index));
            }
            return View(inmueble);
        }

        // POST: Inmuebles/Delete/5 (Solo Administrador)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                _repoInmueble.Baja(id);
                TempData["Mensaje"] = "Inmueble dado de baja correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al eliminar inmueble: " + ex.Message;
                return View(_repoInmueble.ObtenerPorId(id));
            }
        }

        // ==========================================
        // MÓDULO 4: GESTIÓN DE GALERÍA DE IMÁGENES
        // ==========================================
        public ActionResult Galeria(int id)
        {
            var inmueble = _repoInmueble.ObtenerPorId(id);
            if (inmueble == null)
            {
                TempData["Error"] = "El inmueble no existe.";
                return RedirectToAction(nameof(Index));
            }

            inmueble.Imagenes = _repoInmueble.ObtenerImagenesPorInmueble(id);
            return View(inmueble);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SubirFotos(int idInmueble, IList<IFormFile> fotos)
        {
            if (fotos == null || fotos.Count == 0)
            {
                TempData["Error"] = "Debe seleccionar al menos una foto para subir.";
                return RedirectToAction(nameof(Galeria), new { id = idInmueble });
            }

            var inmueble = _repoInmueble.ObtenerPorId(idInmueble);
            if (inmueble == null)
            {
                TempData["Error"] = "El inmueble no existe.";
                return RedirectToAction(nameof(Index));
            }

            int subidas = 0;
            bool noTienePortada = string.IsNullOrEmpty(inmueble.Portada) || inmueble.Imagenes.Count == 0;

            foreach (var foto in fotos)
            {
                if (foto.Length > 0)
                {
                    var extension = Path.GetExtension(foto.FileName).ToLower();
                    if (extension == ".jpg" || extension == ".jpeg" || extension == ".png" || extension == ".webp")
                    {
                        string url = await GuardarArchivoEnDisco(foto);
                        bool esPortada = noTienePortada && subidas == 0;

                        _repoInmueble.AltaImagen(new ImagenInmueble
                        {
                            IdInmueble = idInmueble,
                            Url = url,
                            EsPortada = esPortada
                        });
                        subidas++;
                    }
                }
            }

            TempData["Mensaje"] = $"{subidas} imagen(es) agregada(s) exitosamente a la galería.";
            return RedirectToAction(nameof(Galeria), new { id = idInmueble });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EstablecerPortada(int idInmueble, int idImagen)
        {
            int res = _repoInmueble.EstablecerPortada(idInmueble, idImagen);
            if (res > 0)
            {
                TempData["Mensaje"] = "Foto de portada actualizada exitosamente.";
            }
            else
            {
                TempData["Error"] = "No se pudo actualizar la foto de portada.";
            }
            return RedirectToAction(nameof(Galeria), new { id = idInmueble });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EliminarFoto(int idInmueble, int idImagen)
        {
            var img = _repoInmueble.ObtenerImagenPorId(idImagen);
            if (img != null)
            {
                // Eliminar archivo físico de disco si existe
                if (!string.IsNullOrEmpty(img.Url) && img.Url.StartsWith("/uploads/"))
                {
                    try
                    {
                        string rutaFisica = Path.Combine(_env.WebRootPath, img.Url.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                        if (System.IO.File.Exists(rutaFisica))
                        {
                            System.IO.File.Delete(rutaFisica);
                        }
                    }
                    catch
                    {
                        // No interrumpir si el archivo estaba en uso o no se pudo borrar
                    }
                }

                _repoInmueble.EliminarImagen(idImagen);
                TempData["Mensaje"] = "Imagen eliminada de la galería.";
            }
            else
            {
                TempData["Error"] = "La imagen solicitada no existe.";
            }

            return RedirectToAction(nameof(Galeria), new { id = idInmueble });
        }

        private async Task<string> GuardarArchivoEnDisco(IFormFile archivo)
        {
            string carpetaDestino = Path.Combine(_env.WebRootPath, "uploads", "inmuebles");
            if (!Directory.Exists(carpetaDestino))
            {
                Directory.CreateDirectory(carpetaDestino);
            }

            string extension = Path.GetExtension(archivo.FileName).ToLower();
            string nombreUnico = $"{Guid.NewGuid()}{extension}";
            string rutaCompleta = Path.Combine(carpetaDestino, nombreUnico);

            using (var stream = new FileStream(rutaCompleta, FileMode.Create))
            {
                await archivo.CopyToAsync(stream);
            }

            return $"/uploads/inmuebles/{nombreUnico}";
        }
    }
}
