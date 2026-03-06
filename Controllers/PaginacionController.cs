using Microsoft.AspNetCore.Mvc;
using MvcCorePaginacionRegistros.Models;
using MvcCorePaginacionRegistros.Repositories;

namespace MvcCorePaginacionRegistros.Controllers
{
    public class PaginacionController : Controller
    {
        private RepositoryHospital repo;
        public PaginacionController(RepositoryHospital repo)
        {
            this.repo = repo;
        }

        public async Task<IActionResult> RegistroVistaDepartamento(int? posicion)
        {
            if(posicion == null)
            {
                posicion = 1;
            }
            int numRegistros = await this.repo.GetNumeroRegistrosVistaDepartamentosAsync();
            //PRIMERO = 1
            //ULTIMO = 4
            //ANTERIOR = POSICION -1
            //SIGUIENTE = POSICION +1
            int siguiente = posicion.Value + 1;
            if(siguiente > numRegistros)
            {
                siguiente = 1;
            }
            int anterior = posicion.Value - 1;
            if(anterior < 1)
            {
                anterior = numRegistros;
            }
            ViewData["ULTIMO"] = numRegistros;
            ViewData["SIGUIENTE"] = siguiente;
            ViewData["ANTERIOR"] = anterior;
            VistaDepartamento departamento = await this.repo.GetVistaDepartamentoAsync(posicion.Value);
            return View(departamento);

        }

        public async Task<IActionResult> GrupoVistaDepartamentos(int? posicion)
        {
            if (posicion == null)
            {
                posicion = 1;
            }

            int numeroRegistros = await this.repo.GetNumeroRegistrosVistaDepartamentosAsync();

            ViewData["NUMEROREGISTROS"] = numeroRegistros;

            List<VistaDepartamento> departamentos = await this.repo.GetGrupoVistaDepartamentosAsync(posicion.Value);
            return View(departamentos);
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
