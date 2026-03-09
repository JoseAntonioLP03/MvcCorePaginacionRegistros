using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MvcCorePaginacionRegistros.Data;
using MvcCorePaginacionRegistros.Models;
using System.Data;
using System.Diagnostics.Metrics;


#region VISTAS Y PROCEDURES
//create view V_DEPARTAMENTOS_INDIVIDUAL 
//as
//	select CAST( 
//	ROW_NUMBER() over (order by DEPT_NO) as int) as POSICION, DEPT_NO, DNOMBRE, LOC  from DEPT
//go



//EMP
//create view V_GRUPO_EMPLEADOS 
//as
//	select CAST( 
//	ROW_NUMBER() over (order by APELLIDO) as int) as POSICION, EMP_NO, APELLIDO, OFICIO, SALARIO, DEPT_NO  from EMP
//go

//create procedure SP_GRUPO_EMPLEADOS (@posicion int)
//as
//	select EMP_NO, APELLIDO, OFICIO, SALARIO, DEPT_NO from V_GRUPO_EMPLEADOS
//	where POSICION >= @posicion and POSICION < (@posicion + 3)
//go




//alter PROCEDURE SP_GRUPO_EMPLEADOS_OFICIO
//    (@POSICION INT, @OFICIO NVARCHAR(50), @REGISTROS int out)
//AS
//BEGIN
//    SELECT @REGISTROS = COUNT(EMP_NO) from EMP
//        where OFICIO = @OFICIO
//    SELECT 
//        POSICION,
//        EMP_NO,
//        APELLIDO,
//        OFICIO,
//        SALARIO,
//        DEPT_NO
//    FROM (
//        SELECT 
//            CAST(ROW_NUMBER() OVER (ORDER BY APELLIDO) AS INT) AS POSICION,
//            EMP_NO,
//            APELLIDO,
//            OFICIO,
//            SALARIO,
//            DEPT_NO
//        FROM EMP      
//        WHERE OFICIO = @OFICIO
//    ) AS QUERY 
//    WHERE QUERY.POSICION BETWEEN @POSICION AND (@POSICION + 2);
//END
//GO



//ALTER PROCEDURE SP_GRUPO_EMPLEADOS_DEPT
//    (@POSICION INT, @DEPT_NO NVARCHAR(50), @REGISTROS int out)
//AS
//BEGIN
//    SELECT @REGISTROS = COUNT(EMP_NO) from EMP
//        where DEPT_NO = @DEPT_NO
//    SELECT 
//        POSICION,
//        EMP_NO,
//        APELLIDO,
//        OFICIO,
//        SALARIO,
//        DEPT_NO
//    FROM (
//        SELECT 
//            CAST(ROW_NUMBER() OVER (ORDER BY APELLIDO) AS INT) AS POSICION,
//            EMP_NO,
//            APELLIDO,
//            OFICIO,
//            SALARIO,
//            DEPT_NO
//        FROM EMP      
//        WHERE DEPT_NO = @DEPT_NO
//    ) AS QUERY 
//    WHERE QUERY.POSICION = @POSICION ;
//END
//GO



#endregion

namespace MvcCorePaginacionRegistros.Repositories
{
    public class RepositoryHospital
    {
        private HospitalContext context;

        public RepositoryHospital(HospitalContext context)
        {
            this.context = context;
        }

        public async Task<List<Departamento>> GetDepartamentosAsync()
        {
            var consulta = from datos in this.context.Departamentos select datos;
            return await consulta.ToListAsync();
        }
        public async Task<Departamento> GetDepartamentoDetailsAsync(int id)
        {
            var consulta = from datos in this.context.Departamentos 
                           where datos.IdDepartamento == id 
                           select datos;
            return await consulta.FirstOrDefaultAsync();
        }

        public async Task<int> GetEmpleadosXDepartamentoCount(int id)
        {
            return await this.context.Empleados.Where(z => z.IdDepartamento == id).CountAsync();
        }

        public async Task<ModelEmpleadosOficio> GetEmpleadosXDepartamentoAsyncOut(int id, int posicion)
        {
            string sql = "SP_GRUPO_EMPLEADOS_DEPT @posicion, @dept_no , @registros out";
            SqlParameter pamPos = new SqlParameter("@posicion", posicion);
            SqlParameter pamId = new SqlParameter("@dept_no", id);
            SqlParameter pamReg = new SqlParameter("@registros", 0);
            pamReg.DbType = DbType.Int32;
            pamReg.Direction = ParameterDirection.Output;
            var consulta = this.context.Empleados.FromSqlRaw(sql, pamPos, pamId, pamReg);
            List<Empleado> empleados = await consulta.ToListAsync();
            int registros = (int)pamReg.Value;
            return new ModelEmpleadosOficio { Empleados = empleados, NumeroRegistros = registros };
        }

        public async Task<int> GetNumeroRegistrosVistaDepartamentosAsync()
        {
            return await this.context.VistaDepartamentos.CountAsync();
        }

        public async Task<VistaDepartamento> GetVistaDepartamentoAsync(int posicion)
        {
            VistaDepartamento departamento = await this.context.VistaDepartamentos
                .Where(z => z.Posicion == posicion)
                .FirstOrDefaultAsync();
            return departamento;
        }

        public async Task<List<VistaDepartamento>> GetGrupoVistaDepartamentosAsync(int posicion)
        {
            var consulta = from datos in this.context.VistaDepartamentos
                           where datos.Posicion >= posicion
                           && datos.Posicion < (posicion + 2) 
                           select datos;
            return await consulta.ToListAsync();
        }

        public async Task<List<Departamento>> GetGrupoDepartamentosAsync(int posicion)
        {
            string sql = "SP_GRUPO_DEPARTAMENTOS @posicion";
            SqlParameter pamPosicion = new SqlParameter("@posicion", posicion);
            var consulta = this.context.Departamentos.FromSqlRaw(sql, pamPosicion);
            return await consulta.ToListAsync();
        }

        public async Task<int> GetNumeroRegistrosEmpleadosAsync()
        {
            return await this.context.Empleados.CountAsync();
        }

        public async Task<List<Empleado>> GetGrupoEmpleadosAsync(int posicion)
        {
            string sql = "SP_GRUPO_EMPLEADOS @posicion";
            SqlParameter pamPosicion = new SqlParameter("@posicion", posicion);
            var consulta = this.context.Empleados.FromSqlRaw(sql, pamPosicion);
            return await consulta.ToListAsync();
        }

        public async Task<int> GetEmpleadosOficioCountAsync(string oficio)
        {
            return await this.context.Empleados.Where(z => z.Oficio == oficio).CountAsync();
        }

        public async Task<List<Empleado>> GetGrupoEmpleadosOficioAsync(string oficio , int posicion)
        {
            string sql = "SP_GRUPO_EMPLEADOS_OFICIO @posicion, @oficio";
            SqlParameter pamPos = new SqlParameter("@posicion", posicion);
            SqlParameter pamOfi = new SqlParameter("@oficio", oficio);
            var consulta = this.context.Empleados.FromSqlRaw(sql, pamPos, pamOfi);
            return await consulta.ToListAsync();
        }
        public async Task<ModelEmpleadosOficio> GetGrupoEmpleadosOficioOutAsync(string oficio, int posicion)
        {
            string sql = "SP_GRUPO_EMPLEADOS_OFICIO @posicion, @oficio , @registros out";
            SqlParameter pamPos = new SqlParameter("@posicion", posicion);
            SqlParameter pamOfi = new SqlParameter("@oficio", oficio);
            SqlParameter pamReg = new SqlParameter("@registros", 0);
            pamReg.DbType = DbType.Int32;
            pamReg.Direction = ParameterDirection.Output;
            var consulta = this.context.Empleados.FromSqlRaw(sql, pamPos, pamOfi,pamReg);
            List<Empleado> empleados = await consulta.ToListAsync();
            int registros = (int)pamReg.Value;
            return new ModelEmpleadosOficio { Empleados = empleados, NumeroRegistros = registros };
        }


    }
}
