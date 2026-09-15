using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Laboratorio_II___Proyecto_Inmobiliaria_EnzoMiranda.Models;

namespace Laboratorio_II___Proyecto_Inmobiliaria_EnzoMiranda.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly IRepositorioUsuario _repoUsuario;
        private readonly IWebHostEnvironment _env;

        public UsuariosController(IRepositorioUsuario repoUsuario, IWebHostEnvironment env)
        {
            _repoUsuario = repoUsuario;
            _env = env;
        }

        // ==========================================
        // LOGIN / LOGOUT
        // ==========================================
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl)
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var usuario = _repoUsuario.ObtenerPorEmail(model.Email);
            if (usuario == null || !PasswordHasher.Verify(model.Password, usuario.PasswordHash))
            {
                ModelState.AddModelError("", "Correo o contraseña incorrectos.");
                return View(model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuario.Email),
                new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()),
                new Claim(ClaimTypes.Role, usuario.Rol),
                new Claim("FullName", $"{usuario.Nombre} {usuario.Apellido}"),
                new Claim("AvatarUrl", usuario.AvatarUrl ?? "/img/default-avatar.png")
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = model.Recordarme,
                ExpiresUtc = model.Recordarme ? DateTimeOffset.UtcNow.AddDays(7) : null
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Usuarios");
        }

        // ==========================================
        // PERFIL (Cualquier usuario autenticado)
        // ==========================================
        [Authorize]
        public IActionResult Perfil()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(idClaim, out int idUsuario))
            {
                var usuario = _repoUsuario.ObtenerPorId(idUsuario);
                if (usuario != null)
                {
                    var vm = new PerfilViewModel
                    {
                        IdUsuario = usuario.IdUsuario,
                        Nombre = usuario.Nombre,
                        Apellido = usuario.Apellido,
                        Email = usuario.Email,
                        Rol = usuario.Rol,
                        AvatarUrl = usuario.AvatarUrl
                    };
                    return View(vm);
                }
            }
            return NotFound();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Perfil(PerfilViewModel vm)
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(idClaim, out int idUsuario) || idUsuario != vm.IdUsuario)
            {
                return Forbid();
            }

            if (!ModelState.IsValid)
                return View(vm);

            try
            {
                string? avatarPath = vm.AvatarUrl;

                // Subir nuevo archivo de avatar si fue provisto
                if (vm.AvatarFile != null && vm.AvatarFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "avatars");
                    Directory.CreateDirectory(uploadsFolder);
                    string fileName = $"avatar_{idUsuario}_{Guid.NewGuid()}{Path.GetExtension(vm.AvatarFile.FileName)}";
                    string filePath = Path.Combine(uploadsFolder, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await vm.AvatarFile.CopyToAsync(stream);
                    }
                    avatarPath = $"/uploads/avatars/{fileName}";
                }

                _repoUsuario.ActualizarPerfil(vm.IdUsuario, vm.Nombre, vm.Apellido, vm.Email, avatarPath);

                if (!string.IsNullOrWhiteSpace(vm.NuevaPassword))
                {
                    string hash = PasswordHasher.Hash(vm.NuevaPassword);
                    _repoUsuario.CambiarPassword(vm.IdUsuario, hash);
                }

                // Refrescar cookies con los nuevos datos
                var usuarioActualizado = _repoUsuario.ObtenerPorId(vm.IdUsuario);
                if (usuarioActualizado != null)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, usuarioActualizado.Email),
                        new Claim(ClaimTypes.NameIdentifier, usuarioActualizado.IdUsuario.ToString()),
                        new Claim(ClaimTypes.Role, usuarioActualizado.Rol),
                        new Claim("FullName", $"{usuarioActualizado.Nombre} {usuarioActualizado.Apellido}"),
                        new Claim("AvatarUrl", usuarioActualizado.AvatarUrl ?? "/img/default-avatar.png")
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity));
                }

                TempData["MensajeExito"] = "Perfil actualizado correctamente.";
                return RedirectToAction(nameof(Perfil));
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al actualizar perfil: " + ex.Message;
                return View(vm);
            }
        }

        // ==========================================
        // GESTIÓN DE USUARIOS (Solo Administrador)
        // ==========================================
        [Authorize(Policy = "Administrador")]
        public IActionResult Index(string? filtro, int pagina = 1)
        {
            var resultado = _repoUsuario.ObtenerPaginado(filtro, pagina, 5);
            return View(resultado);
        }

        [Authorize(Policy = "Administrador")]
        public IActionResult Details(int id)
        {
            var u = _repoUsuario.ObtenerPorId(id);
            if (u == null) return NotFound();
            return View(u);
        }

        [Authorize(Policy = "Administrador")]
        public IActionResult Create()
        {
            return View(new Usuario());
        }

        [Authorize(Policy = "Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Usuario usuario)
        {
            if (string.IsNullOrWhiteSpace(usuario.Password))
            {
                ModelState.AddModelError("Password", "La contraseña es obligatoria.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    usuario.PasswordHash = PasswordHasher.Hash(usuario.Password!);
                    _repoUsuario.Alta(usuario);
                    TempData["MensajeExito"] = "Usuario creado correctamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (MySqlConnector.MySqlException ex) when (ex.Number == 1062)
                {
                    ModelState.AddModelError("Email", "Ya existe un usuario registrado con este correo.");
                }
            }
            return View(usuario);
        }

        [Authorize(Policy = "Administrador")]
        public IActionResult Edit(int id)
        {
            var u = _repoUsuario.ObtenerPorId(id);
            if (u == null) return NotFound();
            return View(u);
        }

        [Authorize(Policy = "Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    usuario.IdUsuario = id;
                    _repoUsuario.Modificacion(usuario);

                    if (!string.IsNullOrWhiteSpace(usuario.Password))
                    {
                        string hash = PasswordHasher.Hash(usuario.Password);
                        _repoUsuario.CambiarPassword(id, hash);
                    }

                    TempData["MensajeExito"] = "Usuario actualizado correctamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (MySqlConnector.MySqlException ex) when (ex.Number == 1062)
                {
                    ModelState.AddModelError("Email", "Ya existe un usuario con este correo.");
                }
            }
            return View(usuario);
        }

        [Authorize(Policy = "Administrador")]
        public IActionResult Delete(int id)
        {
            var u = _repoUsuario.ObtenerPorId(id);
            if (u == null) return NotFound();
            return View(u);
        }

        [Authorize(Policy = "Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id, IFormCollection collection)
        {
            _repoUsuario.Baja(id);
            TempData["MensajeExito"] = "Usuario eliminado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [AllowAnonymous]
        public IActionResult Restringido()
        {
            return View();
        }
    }
}