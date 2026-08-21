using Microsoft.AspNetCore.Mvc;
using Laboratorio_II___Proyecto_Inmobiliaria_EnzoMiranda.Models;

namespace Laboratorio_II___Proyecto_Inmobiliaria_EnzoMiranda.Controllers
{
    public class PropietariosController : Controller
    {
        private readonly IRepositorioPropietario _repositorio;

        public PropietariosController(IRepositorioPropietario repositorio)
        {
            _repositorio = repositorio;
        }

        // GET: Propietarios
        public ActionResult Index()
        {
            var lista = _repositorio.ObtenerTodos();
            return View(lista);
        }

        // GET: Propietarios/Details/5
        public ActionResult Details(int id)
        {
            var propietario = _repositorio.ObtenerPorId(id);
            if (propietario == null)
            {
                return NotFound();
            }
            return View(propietario);
        }

        // GET: Propietarios/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Propietarios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Propietario propietario)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _repositorio.Alta(propietario);
                    return RedirectToAction(nameof(Index));
                }
                return View(propietario);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(propietario);
            }
        }

        // GET: Propietarios/Edit/5
        public ActionResult Edit(int id)
        {
            var propietario = _repositorio.ObtenerPorId(id);
            if (propietario == null)
            {
                return NotFound();
            }
            return View(propietario);
        }

        // POST: Propietarios/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Propietario propietario)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    propietario.IdPropietario = id;
                    _repositorio.Modificacion(propietario);
                    return RedirectToAction(nameof(Index));
                }
                return View(propietario);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(propietario);
            }
        }

        // GET: Propietarios/Delete/5
        public ActionResult Delete(int id)
        {
            var propietario = _repositorio.ObtenerPorId(id);
            if (propietario == null)
            {
                return NotFound();
            }
            return View(propietario);
        }

        // POST: Propietarios/Delete/5
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
