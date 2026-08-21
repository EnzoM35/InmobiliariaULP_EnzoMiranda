using Microsoft.AspNetCore.Mvc;
using Laboratorio_II___Proyecto_Inmobiliaria_EnzoMiranda.Models;

namespace Laboratorio_II___Proyecto_Inmobiliaria_EnzoMiranda.Controllers
{
    public class InquilinosController : Controller
    {
        private readonly IRepositorioInquilino _repositorio;

        public InquilinosController(IRepositorioInquilino repositorio)
        {
            _repositorio = repositorio;
        }

        // GET: Inquilinos
        public ActionResult Index()
        {
            var lista = _repositorio.ObtenerTodos();
            return View(lista);
        }

        // GET: Inquilinos/Details/5
        public ActionResult Details(int id)
        {
            var inquilino = _repositorio.ObtenerPorId(id);
            if (inquilino == null)
            {
                return NotFound();
            }
            return View(inquilino);
        }

        // GET: Inquilinos/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Inquilinos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Inquilino inquilino)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _repositorio.Alta(inquilino);
                    return RedirectToAction(nameof(Index));
                }
                return View(inquilino);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(inquilino);
            }
        }

        // GET: Inquilinos/Edit/5
        public ActionResult Edit(int id)
        {
            var inquilino = _repositorio.ObtenerPorId(id);
            if (inquilino == null)
            {
                return NotFound();
            }
            return View(inquilino);
        }

        // POST: Inquilinos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Inquilino inquilino)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    inquilino.IdInquilino = id;
                    _repositorio.Modificacion(inquilino);
                    return RedirectToAction(nameof(Index));
                }
                return View(inquilino);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(inquilino);
            }
        }

        // GET: Inquilinos/Delete/5
        public ActionResult Delete(int id)
        {
            var inquilino = _repositorio.ObtenerPorId(id);
            if (inquilino == null)
            {
                return NotFound();
            }
            return View(inquilino);
        }

        // POST: Inquilinos/Delete/5
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
