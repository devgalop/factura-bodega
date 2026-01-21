using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace devgalop.facturabodega.webapi.Common;

public interface IQuery{}

public interface IQueryHandler<TQuery, TResult> where TQuery : IQuery
{
    /// <summary>
    /// Maneja la consulta y devuelve el resultado
    /// </summary>
    /// <param name="query">Query</param>
    /// <returns></returns>
    Task<TResult> HandleAsync(TQuery query);
}