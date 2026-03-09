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

        public async Task<IActionResult> DepartamentoDetails(int? posicion , int id)
        {
            Departamento dept = await this.repo.GetDepartamentoDetailsAsync(id);
            ViewData["Departamento"] = dept;
            
            if (posicion == null)
            {
                posicion = 1;
            }
            int numRegistros = await this.repo.GetEmpleadosXDepartamentoCount(id);
            //PRIMERO = 1
            //ULTIMO = 4
            //ANTERIOR = POSICION -1
            //SIGUIENTE = POSICION +1
            int siguiente = posicion.Value + 1;
            if (siguiente > numRegistros)
            {
                siguiente = 1;
            }
            int anterior = posicion.Value - 1;
            if (anterior < 1)
            {
                anterior = numRegistros;
            }
            ViewData["POSICION"] = posicion.Value;
            ViewData["SIGUIENTE"] = siguiente;
            ViewData["ANTERIOR"] = anterior;
            ModelEmpleadosOficio model = await this.repo.GetEmpleadosXDepartamentoAsyncOut(id, posicion.Value);
            ViewData["ULTIMO"] = model.NumeroRegistros;
            ViewData["DEPT_NO"] = id;
            return View(model.Empleados);
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

        public async Task<IActionResult> GrupoDepartamentos(int? posicion)
        {
            if (posicion == null)
            {
                posicion = 1;
            }

            int numeroRegistros = await this.repo.GetNumeroRegistrosVistaDepartamentosAsync();

            ViewData["NUMEROREGISTROS"] = numeroRegistros;

            List<Departamento> departamentos = await this.repo.GetGrupoDepartamentosAsync(posicion.Value);
            return View(departamentos);
        }

        public async Task<IActionResult> GrupoEmpleados(int? posicion)
        {
            if (posicion == null)
            {
                posicion = 1;
            }

            int numeroRegistros = await this.repo.GetNumeroRegistrosEmpleadosAsync();

            ViewData["NUMEROREGISTROS"] = numeroRegistros;

            List<Empleado> empleados = await this.repo.GetGrupoEmpleadosAsync(posicion.Value);
            return View(empleados);
        }

        public async Task<IActionResult> EmpleadosOficio(int? posicion,string oficio)
        {
            if(posicion == null)
            {
                posicion = 1;
                return View();
            }
            else
            {
                List<Empleado> empleados = await this.repo.GetGrupoEmpleadosOficioAsync(oficio, posicion.Value);
                int registros = await this.repo.GetEmpleadosOficioCountAsync(oficio);
                ViewData["REGISTROS"] = registros;
                ViewData["OFICIO"] = oficio;
                return View(empleados);
            }

        }

        [HttpPost]
        public async Task<IActionResult> EmpleadosOficio(string oficio)
        {
            List<Empleado> empleados = await this.repo.GetGrupoEmpleadosOficioAsync(oficio, 1);
            int registros = await this.repo.GetEmpleadosOficioCountAsync(oficio);
            ViewData["REGISTROS"] = registros;
            ViewData["OFICIO"] = oficio;
            return View(empleados);
        }

        public IActionResult Index()
        {
            return View();
        }


        public async Task<IActionResult> EmpleadosOficioOut(int? posicion, string oficio)
        {
            if (posicion == null)
            {
                posicion = 1;
                return View();
            }
            else
            {
                ModelEmpleadosOficio model = await this.repo.GetGrupoEmpleadosOficioOutAsync(oficio, posicion.Value);
                ViewData["REGISTROS"] = model.NumeroRegistros;
                ViewData["OFICIO"] = oficio;
                return View(model.Empleados);
            }

        }

        [HttpPost]
        public async Task<IActionResult> EmpleadosOficioOut(string oficio)
        {
            ModelEmpleadosOficio model = await this.repo.GetGrupoEmpleadosOficioOutAsync(oficio, 1);
            ViewData["REGISTROS"] = model.NumeroRegistros;
            ViewData["OFICIO"] = oficio;
            return View(model.Empleados);
        }
    }
}
