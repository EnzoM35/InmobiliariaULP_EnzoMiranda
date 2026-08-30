using Microsoft.AspNetCore.Mvc;
using Laboratorio_II___Proyecto_Inmobiliaria_EnzoMiranda.Models;

namespace Laboratorio_II___Proyecto_Inmobiliaria_EnzoMiranda.Controllers
{
    public class TiposInmuebleController : Controller
    {
        private readonly IRepositorioTipoInmueble _repositorio;

        public TiposInmuebleController(IRepositorioTipoInmueble repositorio)
        {
            _repositorio = repositorio;
        }

        // GET: TiposInmueble
        public ActionResult Index()
        {
            var lista = _repositorio.ObtenerTodos();
            return View(lista);
        }

        // GET: TiposInmueble/Details/5
        public ActionResult Details(int id)
        {
            var tipo = _repositorio.ObtenerPorId(id);
            if (tipo == null)
            {
                return NotFound();
            }
            return View(tipo);
        }

        // GET: TiposInmueble/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TiposInmueble/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TipoInmueble tipo)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _repositorio.Alta(tipo);
                    return RedirectToAction(nameof(Index));
                }
                return View(tipo);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(tipo);
            }
        }

        // GET: TiposInmueble/Edit/5
        public ActionResult Edit(int id)
        {
            var tipo = _repositorio.ObtenerPorId(id);
            if (tipo == null)
            {
                return NotFound();
            }
            return View(tipo);
        }

        // POST: TiposInmueble/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, TipoInmueble tipo)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    tipo.IdTipoInmueble = id;
                    _repositorio.Modificacion(tipo);
                    return RedirectToAction(nameof(Index));
                }
                return View(tipo);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(tipo);
            }
        }

        // GET: TiposInmueble/Delete/5
        public ActionResult Delete(int id)
        {
            var tipo = _repositorio.ObtenerPorId(id);
            if (tipo == null)
            {
                return NotFound();
            }
            return View(tipo);
        }

        // POST: TiposInmueble/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                _repositorio.Baja(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(_repositorio.ObtenerPorId(id));
            }
        }
    }
}
