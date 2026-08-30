using Microsoft.AspNetCore.Mvc;
using Laboratorio_II___Proyecto_Inmobiliaria_EnzoMiranda.Models;

namespace Laboratorio_II___Proyecto_Inmobiliaria_EnzoMiranda.Controllers
{
    public class InmueblesController : Controller
    {
        private readonly IRepositorioInmueble _repoInmueble;
        private readonly IRepositorioPropietario _repoPropietario;
        private readonly IRepositorioTipoInmueble _repoTipoInmueble;

        public InmueblesController(
            IRepositorioInmueble repoInmueble,
            IRepositorioPropietario repoPropietario,
            IRepositorioTipoInmueble repoTipoInmueble)
        {
            _repoInmueble = repoInmueble;
            _repoPropietario = repoPropietario;
            _repoTipoInmueble = repoTipoInmueble;
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
                return NotFound();
            }
            return View(inmueble);
        }

        // GET: Inmuebles/Create
        public ActionResult Create()
        {
            CargarSelects();
            return View();
        }

        // POST: Inmuebles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Inmueble inmueble)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _repoInmueble.Alta(inmueble);
                    return RedirectToAction(nameof(Index));
                }
                CargarSelects();
                return View(inmueble);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                CargarSelects();
                return View(inmueble);
            }
        }

        // GET: Inmuebles/Edit/5
        public ActionResult Edit(int id)
        {
            var inmueble = _repoInmueble.ObtenerPorId(id);
            if (inmueble == null)
            {
                return NotFound();
            }
            CargarSelects();
            return View(inmueble);
        }

        // POST: Inmuebles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Inmueble inmueble)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    inmueble.IdInmueble = id;
                    _repoInmueble.Modificacion(inmueble);
                    return RedirectToAction(nameof(Index));
                }
                CargarSelects();
                return View(inmueble);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                CargarSelects();
                return View(inmueble);
            }
        }

        // GET: Inmuebles/Delete/5
        public ActionResult Delete(int id)
        {
            var inmueble = _repoInmueble.ObtenerPorId(id);
            if (inmueble == null)
            {
                return NotFound();
            }
            return View(inmueble);
        }

        // POST: Inmuebles/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                _repoInmueble.Baja(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(_repoInmueble.ObtenerPorId(id));
            }
        }
    }
}
